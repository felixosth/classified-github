using InSupport.Drift.Plugins.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace LSIRaidMonitor
{
    /// <summary>
    /// Interaction logic for LSIRaidMonitorInstallControlWpf.xaml
    /// </summary>
    public partial class LSIRaidMonitorInstallControlWpf : BasePluginConfigWpf
    {
        internal const string StorCLIPathKey = "StorCLIPath";
        internal const string DefaultStorCLIPath = @"C:\Program Files (x86)\MegaRAID Storage Manager\StorCLI64.exe";

        private string _storCliPath;
        private string StorCLIPath
        {
            get
            {
                return _storCliPath;
            }
            set
            {
                _storCliPath = value;

                storCliPathTxtBox.Text = _storCliPath;
            }
        }

        public LSIRaidMonitorInstallControlWpf()
        {
            InitializeComponent();

            storCliPathTxtBox.Text = StorCLIPath;
        }

        public override Dictionary<string, string> GetSettings()
        {
            return new Dictionary<string, string>()
            {
                { "StorCLIPath", StorCLIPath }
            };
        }

        public override void LoadSettings(Dictionary<string, string> config)
        {
            if (config.ContainsKey(StorCLIPathKey))
            {
                StorCLIPath = config[StorCLIPathKey];
            }
            else
            {
                StorCLIPath = DefaultStorCLIPath;
            }
        }

        public override bool VerifySettings()
        {
            if (!File.Exists(StorCLIPath))
            {
                MessageBox.Show("StorCLI file not found.");
                return false;
            }
            return true;
        }

        private void browseBtn_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new OpenFileDialog();
            openFile.Filter = "StorCLI, StorCLI64|StorCLI.exe;StorCLI64.exe";
            openFile.InitialDirectory = File.Exists(StorCLIPath) ? @"C:\Program Files (x86)\MegaRAID Storage Manager\" : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            if (openFile.ShowDialog() == true)
            {
                StorCLIPath = openFile.FileName;
            }
        }
    }
}
