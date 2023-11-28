
using System;
using System.Collections.Generic;
using System.Windows;

namespace InSupport_HostMonitor_Service
{
    /// <summary>
    /// Interaction logic for TryggDRIFTServiceConfigWpf.xaml
    /// </summary>
    public partial class TryggDRIFTServiceConfigWpf : InSupport.Drift.Plugins.Wpf.BasePluginConfigWpf
    {
        public TryggDRIFTServiceConfigWpf()
        {
            InitializeComponent();
            nameTxtBox.Text = Environment.MachineName;
        }

        public override bool VerifySettings()
        {
            if (nameTxtBox.Text == string.Empty)
            {
                MessageBox.Show("Enter information in Name");
                return false;
            }
            if (serverUrlTxtBox.Text == string.Empty)
            {
                MessageBox.Show("Enter information in TryggDRIFT Url");
                return false;
            }

            if (int.TryParse(updateIntervalTxtBox.Text, out _) == false)
            {
                MessageBox.Show("Failed to parse update interval, make sure it's a number without decimals.");
                return false;
            }

            if (int.TryParse(monitorSerializationTimeoutTxtBox.Text, out _) == false)
            {
                MessageBox.Show("Failed to parse monitor timeout value, make sure it's a number without decimals.");
                return false;
            }

            return true;
        }

        public override Dictionary<string, string> GetSettings()
        {
            return new Dictionary<string, string>()
            {
                { "Name", nameTxtBox.Text },
                { "DriftUrl", serverUrlTxtBox.Text },
                {"Interval", updateIntervalTxtBox.Text },
                {"AddCode", addcodeTxtBox.Text },
                { InSupport.Drift.HostMonitor.MonitorSerializationTimeoutSettingsKey, monitorSerializationTimeoutTxtBox.Text }
            };
        }

        public override void LoadSettings(Dictionary<string, string> config)
        {
            if (config.ContainsKey("Name"))
                nameTxtBox.Text = config["Name"];

            if (config.ContainsKey("DriftUrl"))
                serverUrlTxtBox.Text = config["DriftUrl"];

            if (config.ContainsKey("AddCode"))
                addcodeTxtBox.Text = config["AddCode"];

            if (config.ContainsKey("Interval"))
                updateIntervalTxtBox.Text = config["Interval"];

            if (config.ContainsKey(InSupport.Drift.HostMonitor.MonitorSerializationTimeoutSettingsKey))
                monitorSerializationTimeoutTxtBox.Text = config[InSupport.Drift.HostMonitor.MonitorSerializationTimeoutSettingsKey];
        }
    }
}
