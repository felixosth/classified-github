using InSupport.Drift.Plugins;
using InSupport.Drift.Plugins.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;

namespace TryggDriftPingMonitorPlugin
{
    /// <summary>
    /// Interaction logic for PingPluginConfigurationWpf.xaml
    /// </summary>
    [Serializable]
    public partial class PingPluginConfigurationWpf : BasePluginConfigWpf
    {
        internal const string _PingItemsCfgKey = "PingItems";
        internal const string _PingCountCfgKey = "PingCount";

        public PingPluginConfigurationWpf()
        {
            InitializeComponent();
        }

        public override Dictionary<string, string> GetSettings()
        {
            var pingItems = new List<PingItem>();
            foreach (PingItemUserControlWpf pingItemUsrCtrl in pingItemsStackPanel.Children)
            {
                pingItems.Add(new PingItem()
                {
                    IP = pingItemUsrCtrl.ipTxtBox.Text,
                    Type = pingItemUsrCtrl.typeTxtBox.Text,
                    Label = pingItemUsrCtrl.labelTxtBox.Text
                });
            }

            return new Dictionary<string, string>()
            {
                { _PingItemsCfgKey, JsonConvert.SerializeObject(pingItems) },
                { _PingCountCfgKey, int.Parse(pingCountTxtBox.Text).ToString() }
            };
        }

        public override void LoadSettings(Dictionary<string, string> config)
        {
            if (config.ContainsKey(_PingItemsCfgKey))
            {
                try
                {
                    var existingPingItems = JsonConvert.DeserializeObject<PingItem[]>(config[_PingItemsCfgKey]);

                    foreach (var pingItem in existingPingItems)
                    {
                        var pingUserCtrl = new PingItemUserControlWpf();
                        pingUserCtrl.OnDelete += PingUserCtrl_OnDelete;
                        pingUserCtrl.labelTxtBox.Text = pingItem.Label;
                        pingUserCtrl.ipTxtBox.Text = pingItem.IP;
                        pingUserCtrl.typeTxtBox.Text = pingItem.Type;
                        pingItemsStackPanel.Children.Add(pingUserCtrl);
                    }
                }
                catch
                {

                }

            }
            if (config.ContainsKey(_PingCountCfgKey))
            {
                pingCountTxtBox.Text = config[_PingCountCfgKey];
            }
        }

        private void PingUserCtrl_OnDelete(object sender, EventArgs e)
        {
            pingItemsStackPanel.Children.Remove(sender as PingItemUserControlWpf);
        }

        public override bool VerifySettings()
        {
            foreach (PingItemUserControlWpf pingItemUsrCtrl in pingItemsStackPanel.Children)
            {
                if (pingItemUsrCtrl.ipTxtBox.Text == string.Empty)
                {
                    MessageBox.Show("Missing IP information");
                    pingItemUsrCtrl.ipTxtBox.Focus();
                    return false;
                }
                else if (pingItemUsrCtrl.typeTxtBox.Text == string.Empty)
                {
                    MessageBox.Show("Missing Type information");
                    pingItemUsrCtrl.typeTxtBox.Focus();
                    return false;

                }
                else if (pingItemUsrCtrl.labelTxtBox.Text == string.Empty)
                {
                    MessageBox.Show("Missing Label information");
                    pingItemUsrCtrl.labelTxtBox.Focus();
                    return false;
                }
            }

            if (!int.TryParse(pingCountTxtBox.Text, out _))
            {
                MessageBox.Show("Failed to parse number in Ping Count");
                pingCountTxtBox.Focus();
                return false;
            }
            return true;
        }

        private void addPingItemBtn_Click(object sender, RoutedEventArgs e)
        {
            var pingUserCtrl = new PingItemUserControlWpf();
            pingUserCtrl.OnDelete += PingUserCtrl_OnDelete;
            pingItemsStackPanel.Children.Add(pingUserCtrl);
        }
    }

    public class PingPluginConfig
    {
        public string Name { get; set; }
    }
}
