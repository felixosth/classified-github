using InSupport.Drift.Plugins;
using System.Data;
using System.IO;
using System.Linq;

namespace TryggDriftStoragePlugin
{
    public partial class StoragePluginConfig : BasePluginConfig
    {
        public StoragePluginConfig()
        {
            InitializeComponent();

            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType != DriveType.CDRom))
            {
                drivesListBox.Items.Add(new Drive() { Letter = drive.Name.Replace(":\\", "") }, true);
            }
            drivesListBox.CheckOnClick = true;
        }

        public override bool ValidateForm()
        {
            return true;
        }

        public override string[] GetSettings()
        {
            return new string[]
                {
                    "Drives=" + string.Join(",", drivesListBox.CheckedItems.Cast<Drive>())
                };
        }
    }
    class Drive
    {
        public string Letter { get; set; }
        public override string ToString() => Letter;
    }
}
