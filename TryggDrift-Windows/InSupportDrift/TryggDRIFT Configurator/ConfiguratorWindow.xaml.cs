using InSupport.Drift.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace TryggDRIFT_Configurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ConfiguratorWindow : Window
    {
        private readonly CustomServiceInstaller serviceInstaller;
        bool canInstallService = true;

        bool serviceWasRunningOnStartup = false;

        private readonly HttpClient httpClient = new HttpClient();
        private const string _BACKEND_URL = "/backend/api.php";
        private const string _GETPACKAGE_REQ = "?action=getPackages";
        private const string _DOWNLOADPACKAGE_REQ = "?action=downloadPackage&package={0}";
        internal const string _SERVICE_PATH = "TryggDrift Service";
        internal const string _SERVICE_NAME = "TryggDrift";
        internal const string _SERVICE_EXE = "TryggDrift.exe";

        List<TryggDRIFTPackage> packages;
        TryggDRIFTInstallCache installCache;

        PluginConfigCache pluginConfigCache;

        private string getPackagesUrl => new Uri(new Uri(serverUrlTxtBox.Text), _BACKEND_URL + _GETPACKAGE_REQ).OriginalString;
        private string downloadPackageUrl(int id) => new Uri(new Uri(serverUrlTxtBox.Text), _BACKEND_URL + string.Format(_DOWNLOADPACKAGE_REQ, id)).OriginalString;

        public ConfiguratorWindow()
        {
            InitializeComponent();
            serviceInstaller = new CustomServiceInstaller(_SERVICE_NAME, Path.Combine(App.MyDir, _SERVICE_PATH, _SERVICE_EXE));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            installCache = TryggDRIFTInstallCache.OpenOrCreate();
            serverUrlTxtBox.Text = installCache.PackageSource;

            packages = new List<TryggDRIFTPackage>();
            refreshBtn_Click(sender, e);

            RefreshServiceStatus();
        }

        private void RefreshPluginConfigCheck()
        {
            try
            {
                if (File.Exists(PluginConfigCache._FILENAME))
                {
                    pluginConfigCache = PluginConfigCache.FromDefaultFilePath();

                    if (pluginConfigCache.PluginConfigs != null)
                        foreach (var package in packages)
                        {
                            package.NotConfigured = package.Status != TryggDRIFTPackageStatus.NotInstalled && pluginConfigCache.PluginConfigs.FirstOrDefault(pc => pc.PluginID == package.ID) == null;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private async Task<List<TryggDRIFTPackage>> GetTryggDRIFTPackages()
        {
            try
            {
                var getPackagesResponse = await httpClient.GetAsync(getPackagesUrl);
                var getPackagesResponseText = await getPackagesResponse.Content.ReadAsStringAsync();
                var packages = JsonConvert.DeserializeObject<List<TryggDRIFTPackage>>(getPackagesResponseText);

                if (packages == null)
                    throw new NullReferenceException("No packages found");
                return packages;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to fetch packages from the server.\r\n\r\n" + ex.Message, "Error");
            }
            return new List<TryggDRIFTPackage>();
        }

        private async void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            var newPackages = await GetTryggDRIFTPackages();
            foreach (var newPackage in newPackages)
            {
                var installedPackage = installCache.Packages.FirstOrDefault(p => p.ID == newPackage.ID);
                if (installedPackage != null) // Already installed
                {
                    newPackage.IsChecked = true;
                    newPackage.UnzippedPath = installedPackage.Path;
                    newPackage.InstalledVersion = installedPackage.Version;
                    try
                    {
                        if (new Version(newPackage.Version).CompareTo(new Version(installedPackage.Version)) > 0)
                        {
                            newPackage.UpdateAvailable = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to parse the version string for package " + newPackage.Name + "\r\n\r\n" + ex.Message);
                    }
                    newPackage.Status = TryggDRIFTPackageStatus.Installed;
                }
                else if (newPackage.Mandatory)
                    newPackage.IsChecked = true;
                else
                    foreach (var oldPackage in packages)
                    {
                        if (newPackage.ID == oldPackage.ID)
                        {
                            newPackage.IsChecked = oldPackage.IsChecked;
                        }
                    }
            }

            foreach (var installedPackage in installCache.Packages) // Local packages
            {
                if (newPackages.Any(p => p.ID == installedPackage.ID) == false)
                {
                    newPackages.Add(new TryggDRIFTPackage()
                    {
                        ID = installedPackage.ID,
                        Name = installedPackage.Name,
                        Status = TryggDRIFTPackageStatus.Installed,
                        InstalledVersion = installedPackage.Version,
                        IsChecked = true,
                        Version = "N/A",
                        UpdateAvailable = false,
                        UnzippedPath = installedPackage.Path,
                        LocalPackage = true
                    });
                }
            }

            packages = newPackages;
            packagesListView.ItemsSource = packages;
            CollectionViewSource.GetDefaultView(packagesListView.ItemsSource).Refresh();
            RefreshPluginConfigCheck();

        }

        private async void installBtn_Click(object sender, RoutedEventArgs e)
        {
            var packagesToDownload = packages.Where(package => package.IsChecked && package.Status != TryggDRIFTPackageStatus.Installed);
            var packagesToUninstall = packages.Where(package => !package.Mandatory && !package.IsChecked && package.Status == TryggDRIFTPackageStatus.Installed);
            var packagesToUpdate = packages.Where(package => package.IsChecked && package.DoUpdate && package.UpdateAvailable);

            if (serviceInstaller.IsRunning() &&
                MessageBox.Show("The TryggDrift service is currently running and must be terminated to proceed with the installation. Do you want to continue?", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    if (!serviceInstaller.StopService())
                    {
                        throw new Exception("Unable to stop the service!");
                    }
                }
                catch
                {
                    MessageBox.Show("Error while trying to stop the service");
                    return;
                }
            }

            if (packagesToDownload.Count() == 0 && packagesToUninstall.Count() == 0 && packagesToUpdate.Count() == 0)
            {
                MessageBox.Show("There's nothing to modify!", "No changes");
                return;
            }

            if (MessageBox.Show("Are you sure you want to modify the installation?" +
                (packagesToDownload.Count() > 0 ? "\r\n\r\nPackages to install: " + packagesToDownload.Count() : "") +
                (packagesToUninstall.Count() > 0 ? "\r\n\r\nPackages to remove: " + packagesToUninstall.Count() : "") +
                (packagesToUpdate.Count() > 0 ? "\r\n\r\nPackages to update: " + packagesToUpdate.Count() : ""), "Are you sure?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            foreach (var package in packagesToUpdate)
            {
                package.IsUpdating = true;
            }

            // Download package to temp
            foreach (var package in packagesToDownload.Concat(packagesToUpdate))
            {
                package.Status = TryggDRIFTPackageStatus.Downloading;
                await DownloadPackage(package);
                package.Status = TryggDRIFTPackageStatus.Downloaded;

                if (package.Checksum != await CalculateMD5Async(package.DownloadedTempFile) &&
                    MessageBox.Show("Failed to verify the package \"" + package.Name + "\". Do you want to install anyway?", "Verification", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    package.Status = TryggDRIFTPackageStatus.Failed;
                    File.Delete(package.DownloadedTempFile);
                    package.DownloadedTempFile = null;
                }
            }

            // Uninstalling
            foreach (var package in packagesToUninstall.Concat(packagesToUpdate))
            {
                var installedPackageFromCache = installCache.Packages.FirstOrDefault(ip => ip.ID == package.ID);
                if (installedPackageFromCache != null)
                {
                    if (package.Name.ToLower() == "tryggdrift" &&
                        !package.IsUpdating &&
                        serviceInstaller.IsInstalled()) // Uninstall the service also
                    {
                        serviceInstaller.UninstallService();
                    }

                    try
                    {
                        if (package.Name.ToLower() == "tryggdrift" && package.Mandatory)
                        {
                            foreach (string file in Directory.EnumerateFiles(installedPackageFromCache.Path))
                            {
                                if (package.IsUpdating && file == "config.cfg")
                                    continue;


                                int fileDelTries = 0;
                                bool fileDeleted = false;
                                while (!fileDeleted && fileDelTries < 5)
                                {
                                    try
                                    {
                                        File.Delete(file);
                                        fileDeleted = true;
                                    }
                                    catch
                                    {
                                        fileDelTries++;
                                        Thread.Sleep(500);
                                    }
                                }
                                if (!fileDeleted)
                                {
                                    MessageBox.Show($"Unable to uninstall the package {package.Name}");
                                }

                            }
                        }
                        else
                        {
                            int dirDelTries = 0;
                            bool dirDeleted = false;
                            while (!dirDeleted && dirDelTries < 5)
                            {
                                try
                                {
                                    Directory.Delete(installedPackageFromCache.Path, recursive: true);
                                    dirDeleted = true;
                                }
                                catch
                                {
                                    dirDelTries++;
                                    Thread.Sleep(500);
                                }
                            }
                            if (!dirDeleted)
                            {
                                MessageBox.Show($"Unable to uninstall the package {package.Name}");
                            }
                        }

                        if (!package.IsUpdating && File.Exists(PluginConfigCache._FILENAME))
                        {
                            pluginConfigCache = PluginConfigCache.FromDefaultFilePath();
                            var config = pluginConfigCache.PluginConfigs.RemoveAll(pc => pc.PluginID == package.ID);
                            pluginConfigCache.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error when trying to uninstall " + package.Name + "\r\n\r\n" + ex.Message);
                    }

                    if (!package.IsUpdating)
                        installCache.Packages.Remove(installedPackageFromCache);

                    package.Status = TryggDRIFTPackageStatus.NotInstalled;
                }
            }

            //packages.RemoveAll(p => p.Status == TryggDRIFTPackageStatus.NotInstalled && p.LocalPackage); // Remove local packages

            // Unzip package to service folder
            foreach (var package in packages.Where(package => package.DownloadedTempFile != null && (package.Status == TryggDRIFTPackageStatus.Downloaded || package.IsUpdating)))
            {
                package.Status = TryggDRIFTPackageStatus.Extracting;

                if (await Unzip(package))
                {
                    package.Status = TryggDRIFTPackageStatus.Installed;

                    var existingInstall = installCache.Packages.FirstOrDefault(p => p.ID == package.ID);
                    if (existingInstall != null) // Updated
                    {
                        existingInstall.Version = package.Version;
                        package.InstalledVersion = package.Version;
                        package.UpdateAvailable = false;
                        package.DoUpdate = false;
                        package.IsUpdating = false;
                    }
                    else
                    {
                        installCache.Packages.Add(TryggDRIFTInstalledPackage.FromDownloadPackage(package));
                    }

                    File.Delete(package.DownloadedTempFile);
                    package.DownloadedTempFile = null;
                }
                else
                    package.Status = TryggDRIFTPackageStatus.Failed;
            }

            installCache.Save();
            RefreshServiceStatus();
            RefreshPluginConfigCheck();
            refreshBtn_Click(sender, e);
        }

        static async Task<string> CalculateMD5Async(string filename)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)) // true means use IO async operations
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = await stream.ReadAsync(buffer, 0, 4096);
                        if (bytesRead > 0)
                        {
                            md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                        }
                    } while (bytesRead > 0);

                    md5.TransformFinalBlock(buffer, 0, 0);
                    return BitConverter.ToString(md5.Hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private async Task<string> DownloadPackage(TryggDRIFTPackage package)
        {
            var url = downloadPackageUrl(package.ID);
            var tempFilePath = Path.GetTempFileName();

            var downloadPackageResponse = await httpClient.GetAsync(url);
            if (downloadPackageResponse.IsSuccessStatusCode)
            {

                var contentStream = await downloadPackageResponse.Content.ReadAsStreamAsync();
                using (var fs = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    contentStream.CopyTo(fs);
                }
                package.DownloadedTempFile = tempFilePath;
                contentStream.Close();
                return tempFilePath;
            }
            package.Status = TryggDRIFTPackageStatus.Failed;
            throw new Exception();
        }

        private async Task<bool> Unzip(TryggDRIFTPackage downloadedPackage)
        {
            string pluginPath = Path.Combine(App.MyDir, _SERVICE_PATH, "Plugins");

            string packageDst = downloadedPackage.Mandatory ? Path.Combine(App.MyDir, _SERVICE_PATH) : Path.Combine(pluginPath, downloadedPackage.Name.RemoveInvalidFilenameChars());

            Directory.CreateDirectory(packageDst);

            if (string.IsNullOrEmpty(downloadedPackage.DownloadedTempFile) || !File.Exists(downloadedPackage.DownloadedTempFile))
            {
                throw new InvalidOperationException("Package not downloaded.");
            }

            using (var fileStream = new FileStream(downloadedPackage.DownloadedTempFile, FileMode.Open, FileAccess.Read))
            using (var zip = new ZipArchive(fileStream))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    string fileDestinationPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(packageDst, entry.FullName));

                    if (System.IO.Path.GetFileName(fileDestinationPath).Length == 0) // It's a directory
                    {
                        if (entry.Length != 0)
                            throw new IOException("Directory entry with data.");

                        Directory.CreateDirectory(fileDestinationPath);
                    }
                    else // It's a file
                    {
                        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileDestinationPath));

                        using (Stream entryStream = entry.Open())
                        using (FileStream entryFileStream = new FileStream(fileDestinationPath, FileMode.Create))
                        {
                            await entryStream.CopyToAsync(entryFileStream);
                        }
                    }
                }
            }
            downloadedPackage.UnzippedPath = packageDst;
            return true;
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var package = (sender as Button).DataContext as TryggDRIFTPackage;
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    Arguments = string.Format("\"-plugin\" \"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\"", package.ID, package.UnzippedPath, this.Left + this.Width / 2, this.Top + this.Height / 2, Debugger.IsAttached),
                    UseShellExecute = false
                }
            };
            process.Start();

            process.WaitForExit();
            this.Activate();
            CompileConfigFile();
            RefreshPluginConfigCheck();
        }

        void CompileConfigFile()
        {
            if (File.Exists(PluginConfigCache._FILENAME))
            {
                var configCache = JsonConvert.DeserializeObject<PluginConfigCache>(File.ReadAllText(PluginConfigCache._FILENAME));
                if (configCache != null)
                {
                    List<string> lines = new List<string>();
                    foreach (var configDict in configCache.PluginConfigs.Select(pc => pc.Config))
                    {
                        foreach (var kvp in configDict)
                        {
                            lines.Add(kvp.Key + "=" + kvp.Value);
                        }
                    }
                    if (lines.Count > 0)
                    {
                        File.WriteAllLines(Path.Combine(_SERVICE_PATH, "config.cfg"), lines);
                    }
                }
                else
                {
                    throw new NullReferenceException("Config is null?");
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (serviceWasRunningOnStartup &&
                serviceInstaller.IsInstalled() &&
                !serviceInstaller.IsRunning() &&
                MessageBox.Show("The service is currently stopped. Do you want to start it?", "TryggDrift service", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    serviceInstaller.StartService();
                }
                catch { }
            }
        }

        bool firstRefresh = true;
        private void RefreshServiceStatus()
        {
            if (installCache.Packages.Any(package => package.Name.ToLower() == "tryggdrift") == false)
            {
                canInstallService = false;
                serviceStatusLabel.Content = "Package not installed";
                serviceStatusLabel.Foreground = Brushes.Gray;
                stopServiceBtn.IsEnabled = false;
                startServiceBtn.IsEnabled = false;
                installServiceBtn.IsEnabled = false;

            }
            else if (!File.Exists(serviceInstaller.ExePath))
            {

            }
            else if (serviceInstaller.IsInstalled())
            {
                installServiceBtn.IsEnabled = true;

                canInstallService = false;
                installServiceBtn.Content = "Uninstall service";
                if (serviceInstaller.IsRunning())
                {
                    if (firstRefresh)
                        serviceWasRunningOnStartup = true;

                    stopServiceBtn.IsEnabled = true;
                    startServiceBtn.IsEnabled = false;
                    serviceStatusLabel.Content = "Installed, running";
                    serviceStatusLabel.Foreground = Brushes.Green;
                }
                else
                {
                    startServiceBtn.IsEnabled = true;
                    stopServiceBtn.IsEnabled = false;
                    serviceStatusLabel.Content = "Installed, not running";
                    serviceStatusLabel.Foreground = Brushes.Orange;
                }
            }
            else
            {
                installServiceBtn.IsEnabled = true;
                installServiceBtn.Content = "Install service";
                stopServiceBtn.IsEnabled = false;
                startServiceBtn.IsEnabled = false;
                canInstallService = true;
                serviceStatusLabel.Content = "Not installed";
                serviceStatusLabel.Foreground = Brushes.Red;
            }
            firstRefresh = false;
        }

        private void installServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (canInstallService)
                {
                    serviceInstaller.CustomUserAccount = MessageBox.Show("Install service as default user?", "?", MessageBoxButton.YesNo) == MessageBoxResult.No;

                    if (serviceInstaller.InstallService(out Exception ex))
                    {
                        MessageBox.Show("Service installed!", "Success!");
                        RefreshServiceStatus();

                    }
                    else if (ex != null)
                    {
                        MessageBox.Show("Error installing service\r\n\r\n" + ex.Message);
                    }
                }
                else if (MessageBox.Show("Are you sure you want to uninstall the service?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    serviceInstaller.UninstallService();
                    RefreshServiceStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error modifying service\r\n\r\n" + ex.Message);
            }
        }

        private void startServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (serviceInstaller.StartService())
                {
                    RefreshServiceStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting service\r\n\r\n" + ex.Message);
            }
        }

        private void stopServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (serviceInstaller.StopService())
                {
                    RefreshServiceStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping service\r\n\r\n" + ex.Message);
            }
        }

        private void scanPluginsBtn_Click(object sender, RoutedEventArgs e)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    Arguments = "-scan",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            installCache = TryggDRIFTInstallCache.OpenOrCreate();
            refreshBtn_Click(sender, e);
        }

        private void serverUrlTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (installCache != null)
                installCache.PackageSource = (sender as TextBox).Text;
        }

        private void logBtn_Click(object sender, RoutedEventArgs e)
        {
            var logViewer = new LogViewerWindow();
            logViewer.Closing += (s, ee) =>
            {
                logBtn.IsEnabled = true;
            };

            logBtn.IsEnabled = false;
            logViewer.Show();
        }
    }

    public static class Extensions
    {
        public static string RemoveInvalidFilenameChars(this string fileName)
        {
            Array.ForEach(Path.GetInvalidFileNameChars(), c => fileName = fileName.Replace(c.ToString(), String.Empty));
            return fileName;
        }
    }

    public class TryggDRIFTInstallCache
    {
        [JsonIgnore]
        public const string FileName = "packages.json";

        public string PackageSource { get; set; } = "https://drift.tryggconnect.se";

        public List<TryggDRIFTInstalledPackage> Packages { get; set; }

        public TryggDRIFTInstallCache()
        {
            Packages = new List<TryggDRIFTInstalledPackage>();
        }

        public void Save()
        {
            File.WriteAllText(Path.Combine(App.MyDir, FileName), JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static TryggDRIFTInstallCache OpenOrCreate()
        {
            if (File.Exists(Path.Combine(App.MyDir, FileName)))
                return FromFile();
            else return new TryggDRIFTInstallCache();
        }

        public static TryggDRIFTInstallCache FromFile() => JsonConvert.DeserializeObject<TryggDRIFTInstallCache>(File.ReadAllText(Path.Combine(App.MyDir, FileName)));
    }

    public class TryggDRIFTInstalledPackage
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }

        public static TryggDRIFTInstalledPackage FromDownloadPackage(TryggDRIFTPackage package)
        {
            return new TryggDRIFTInstalledPackage()
            {
                ID = package.ID,
                Name = package.Name,
                Version = package.Version,
                Path = package.UnzippedPath
            };
        }
    }

    public enum TryggDRIFTPackageStatus
    {
        NotInstalled,
        Installed,
        Downloading,
        Downloaded,
        Extracting,
        Extracted,
        Updating,
        Failed
    }

    public class TryggDRIFTPackage : INotifyPropertyChanged
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("checksum")]
        public string Checksum { get; set; }


        [JsonIgnore]
        public bool LocalPackage { get; set; }

        private string installedVersion = "N/A";
        public string InstalledVersion
        {
            get => installedVersion;
            set
            {
                installedVersion = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("mandatory")]
        public bool Mandatory { get; set; }

        public bool NotMandatory => !Mandatory;

        public bool IsChecked { get; set; }

        private bool updateAvailable;
        public bool UpdateAvailable
        {
            get => updateAvailable;
            set
            {
                updateAvailable = value;
                OnPropertyChanged();
            }
        }

        private bool notConfigured;
        public bool NotConfigured
        {
            get => notConfigured;
            set
            {
                notConfigured = value;
                OnPropertyChanged();
            }
        }

        private bool doUpdate;
        public bool DoUpdate
        {
            get => doUpdate;
            set
            {
                doUpdate = value;
                OnPropertyChanged();
            }
        }
        public bool IsUpdating { get; set; }

        private TryggDRIFTPackageStatus status = TryggDRIFTPackageStatus.NotInstalled;
        public TryggDRIFTPackageStatus Status
        {
            get => status;
            set
            {
                status = value;
                CanConfigure = status == TryggDRIFTPackageStatus.Installed;
                OnPropertyChanged();
            }
        }

        private bool canConfigure = false;
        public bool CanConfigure
        {
            get => canConfigure;
            set
            {
                canConfigure = value;
                OnPropertyChanged();
            }
        }


        public string DownloadedTempFile { get; set; }
        public string UnzippedPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString() => string.Format("{0} ({1})", Name, Version);
    }
}
