using InSupport.Drift.Plugins.Wpf;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace TryggDriftStoragePlugin
{
    /// <summary>
    /// Interaction logic for StoragePluginConfigWpf.xaml
    /// </summary>
    public partial class StoragePluginConfigWpf : BasePluginConfigWpf
    {
        internal const string _CFG_KEY = "Drives";
        ObservableCollection<PrettyDrive> prettyDrives;
        public StoragePluginConfigWpf()
        {
            InitializeComponent();

            var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType != DriveType.CDRom);
            prettyDrives = new ObservableCollection<PrettyDrive>(PrettyDrive.FromDriveInfo(drives));

            drivesListBox.ItemsSource = prettyDrives;
        }

        public override Dictionary<string, string> GetSettings()
        {
            return new Dictionary<string, string>()
            {
                {
                    _CFG_KEY,
                    JsonConvert.SerializeObject(
                        new StoragePluginCfg()
                        {
                            Drives = prettyDrives.Where(pd => pd.Checked).Select(pd => pd.Drive).ToArray()
                        }
                    )
                }
            };
        }

        public override void LoadSettings(Dictionary<string, string> config)
        {
            if (config.ContainsKey(_CFG_KEY))
            {
                try
                {
                    var myCfg = JsonConvert.DeserializeObject<StoragePluginCfg>(config[_CFG_KEY]);

                    foreach (var prettyDrive in prettyDrives)
                    {
                        var localDrive = myCfg.Drives.FirstOrDefault(d => d == prettyDrive.Drive);
                        if (localDrive != null)
                            prettyDrive.Checked = true;
                        else
                            prettyDrive.Checked = false;
                    }
                }
                catch
                {
                    MessageBox.Show("Error trying to load the existing config");
                }
            }
        }

        public override bool VerifySettings()
        {
            return true;
        }
    }

    public class StoragePluginCfg
    {
        public string[] Drives { get; set; }
    }

    public class PrettyDrive
    {
        public string Display { get; set; }
        public string Drive { get; set; }
        public bool Checked { get; set; }

        internal static IEnumerable<PrettyDrive> FromDriveInfo(IEnumerable<DriveInfo> drives)
        {
            List<PrettyDrive> prettyDrives = new List<PrettyDrive>();
            foreach (var drive in drives)
            {
                prettyDrives.Add(new PrettyDrive()
                {
                    Display = $"{drive.Name} " + (drive.VolumeLabel != "" ? $"({drive.VolumeLabel}) " : "") + $"[{GetSize(drive.TotalSize)}]",
                    Drive = drive.Name,
                    Checked = true
                });
            }
            return prettyDrives;
        }

        private static string GetSize(long size)
        {
            string postfix = "Bytes";
            long result = size;
            if (size >= 1073741824)//more than 1 GB
            {
                result = size / 1073741824;
                postfix = "GB";
            }
            else if (size >= 1048576)//more that 1 MB
            {
                result = size / 1048576;
                postfix = "MB";
            }
            else if (size >= 1024)//more that 1 KB
            {
                result = size / 1024;
                postfix = "KB";
            }

            return result.ToString("F1") + " " + postfix;
        }
    }
}
