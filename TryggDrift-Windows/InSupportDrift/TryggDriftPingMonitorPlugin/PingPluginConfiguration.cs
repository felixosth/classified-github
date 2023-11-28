using InSupport.Drift.Plugins;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TryggDriftPingMonitorPlugin
{
    public partial class PingPluginConfiguration : BasePluginConfig
    {
        public PingPluginConfiguration()
        {
            InitializeComponent();
        }

        public override bool ValidateForm()
        {
            //if(addressBox.Lines.Length < 1)
            //{
            //    if (MessageBox.Show("Are you sure you want to continue without any settings?", "Configurator", MessageBoxButtons.YesNo) == DialogResult.No)
            //    {
            //        return false;
            //    }
            //}

            foreach (PingItemUserControl pingItemControl in pingItemsPanel.Controls)
            {
                if (pingItemControl == null)
                    continue;

                var pingItem = pingItemControl.GetPingItem();

                if (pingItem.IP == "")
                {
                    MessageBox.Show("Fill in IP", "Validation error");
                    return false;
                }
            }

            return true;
        }

        public override string[] GetSettings()
        {
            List<PingItem> pingItems = new List<PingItem>();
            foreach (PingItemUserControl pingItemControl in pingItemsPanel.Controls)
            {
                if (pingItemControl == null)
                    continue;

                pingItems.Add(pingItemControl.GetPingItem());
            }

            return new string[]
            {
                "PingCount=" + pingCountNum.Value,
                "PingItems=" + JsonConvert.SerializeObject(pingItems)
            };
        }

        private void addPingItemBtn_Click(object sender, EventArgs e)
        {
            var ping = new PingItemUserControl();
            ping.deleteMeBtn.Click += (s, ev) =>
            {
                pingItemsPanel.Controls.Remove((s as Button).Parent as PingItemUserControl);
            };
            pingItemsPanel.Controls.Add(ping);
        }
    }
}
