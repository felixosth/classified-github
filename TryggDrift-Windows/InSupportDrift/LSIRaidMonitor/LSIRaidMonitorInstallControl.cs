using InSupport.Drift.Plugins;
using System;
using System.IO;
using System.Windows.Forms;

namespace LSIRaidMonitor
{
    public partial class LSIRaidMonitorInstallControl : BasePluginConfig
    {
        const string _defaultPath = @"C:\Program Files (x86)\MegaRAID Storage Manager\StorCLI64.exe";

        public LSIRaidMonitorInstallControl()
        {
            InitializeComponent();

            if (File.Exists(_defaultPath))
            {
                storCliPathTxtBox.Text = _defaultPath;
            }
        }

        public override string[] GetSettings()
        {
            return new string[]
            {
                "StorCLIPath=" + storCliPathTxtBox.Text
            };
        }

        public override bool ValidateForm()
        {
            return !string.IsNullOrEmpty(storCliPathTxtBox.Text) && File.Exists(storCliPathTxtBox.Text);
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                storCliPathTxtBox.Text = openFileDialog1.FileName;
                MessageBox.Show("The account running the service must have administrator access to fetch information from MegaRaid. Make sure to assign the service to a admin account after installation.", "Information");
            }
        }
    }
}
