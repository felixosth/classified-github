using DigitalRevision.DataSource;
using DigitalRevision.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalRevision
{
    internal class DataSourceManager
    {
        internal ObservableCollection<DataSourceBase> DataSources { get; private set; } = new ObservableCollection<DataSourceBase>();

        internal DataSourceManager()
        {
            SetupDataSources();
        }

        private void SetupDataSources()
        {
            // Add all our datasources here
            //DataSources.Add(new TestDataSource());
            DataSources.Add(new EventViewerDataSource());
            DataSources.Add(new SystemInfoDataSource());
            DataSources.Add(new TryggDRIFTAlarmDataSource());
            DataSources.Add(new DatabaseBackupDataSource());
            DataSources.Add(new MilestoneDataSource());
            DataSources.Add(new DiskDataSource());
            DataSources = new ObservableCollection<DataSourceBase>(DataSources.OrderBy(source => source.Name));
        }

        internal async Task<string> CollectAllData(string zipFilePath)
        {
            // Create a temporary directory for our data
            var temporaryDirectory = Helper.GetTemporaryDirectory();

            // Loop through our data sources and let them collect
            List<Task> tasks = new List<Task>();
            foreach (var dataSource in DataSources.Where(ds => ds.IsEnabled))
            {
                var dataSourceDestinationFolder = Path.Combine(temporaryDirectory, dataSource.Name);
                Directory.CreateDirectory(dataSourceDestinationFolder);

                tasks.Add(Task.Run(async () => await dataSource.CollectData(dataSourceDestinationFolder)));
            }
            await Task.WhenAll(tasks.ToArray());

            if (zipFilePath != null) // If zipeFilePath is null we're in debug
            {
                // Create our zipfile
                await CreateZipFile(temporaryDirectory, zipFilePath);

                // Delete our temporary files
                Directory.Delete(temporaryDirectory, recursive: true);
                return zipFilePath;
            }
            return temporaryDirectory;
        }

        /// <summary>
        /// Creates a zipfile and includes all subfolders and files.
        /// </summary>
        /// <param name="folder">Source folder</param>
        /// <param name="savePath">Destination file</param>
        /// <returns></returns>
        private async Task CreateZipFile(string folder, string savePath)
        {

            // List all our files
            var files = new DirectoryInfo(folder).GetFiles("*", SearchOption.AllDirectories);

            // Write our zipfile to disk
            using (var zipFileStream = new FileStream(savePath, FileMode.Create))
            {
                using (var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var relPath = file.FullName.Replace(folder + "\\", "");
                        var zipEntry = archive.CreateEntry(relPath);

                        using (var fileStream = new FileStream(file.FullName, FileMode.Open))
                        using (var entryStream = zipEntry.Open())
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }
            }
        }

        public static string DefaultZipPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Digital revision {DateTime.Now.ToString().Replace(':', '_')}.zip");
    }
}
