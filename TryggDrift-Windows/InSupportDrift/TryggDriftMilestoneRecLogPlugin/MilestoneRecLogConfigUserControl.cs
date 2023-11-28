using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace InSupport.Drift.Plugins
{
    public partial class MilestoneRecLogConfigUserControl : BasePluginConfig
    {

        private string[] StorageFolders => storagesTextBox.Lines.Where(l => !string.IsNullOrEmpty(l) && !string.IsNullOrWhiteSpace(l)).ToArray();

        public MilestoneRecLogConfigUserControl()
        {
            InitializeComponent();

            deviceHandlingLogTxtBox.Text = @"C:\ProgramData\Milestone\XProtect Recording Server\Logs\DeviceHandling.log";
        }

        public override bool ValidateForm()
        {
            foreach (var storage in StorageFolders)
            {
                if (!Directory.Exists(storage))
                {
                    MessageBox.Show("\"" + storage + "\" is not a valid directory.", "Error");
                    return false;
                }

                var archiveConfigPath = Path.Combine(storage, "config.xml");
                var archiveCachePath = Path.Combine(storage, "archives_cache.xml");
                if (File.Exists(archiveConfigPath) && File.Exists(archiveCachePath))
                {

                    //var config = ArchiveConfig.Deserialize(archiveConfigPath);

                    //if(config.archive != null && config.archive.link != null)
                    //{
                    //    MessageBox.Show("The folder at \"" + storage + "\" is a archive to another storage. Only designate 1st level storages (like Live disks)", "Error");
                    //    return false;
                    //}
                }
                else
                {
                    MessageBox.Show("The folder at \"" + storage + "\" is not a valid storage.", "Error");
                    return false;
                }
            }

            return true;
        }

        public override string[] GetSettings()
        {
            return new string[]
            {
                "MilestoneRecLogThreshold=20",
                @"MilestoneRecLogFilePath=" + deviceHandlingLogTxtBox.Text,
                "MilestoneRecMediaDbs=" + JsonConvert.SerializeObject(StorageFolders)
            };
        }

        private void browseDeviceHandlingBtn_Click(object sender, EventArgs e)
        {
            using (var openFile = new OpenFileDialog())
            {
                openFile.Filter = "Device handling logfile|DeviceHandling.log";
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    deviceHandlingLogTxtBox.Text = openFile.FileName;
                }
            }
        }
    }
}
