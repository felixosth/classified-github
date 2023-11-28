using InSupport.Drift.Plugins;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AxisPeopleCounterPlugin
{
    public partial class PeopleCounterConfig : BasePluginConfig
    {
        public PeopleCounterConfig()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        public override string[] GetSettings()
        {
            var counters = new List<object>();

            foreach (CounterUserControl control in flowLayoutPanel1.Controls)
            {
                counters.Add(new
                {
                    control.IP,
                    control.Username,
                    control.EncryptedPassword,
                    Name = control.CounterName
                });
            }

            var countersStr = JsonConvert.SerializeObject(counters);
            return new[] { $"{PeopleCounterMonitor._settingsKey}={countersStr}" };
        }

        public override bool ValidateForm()
        {
            foreach (CounterUserControl control in flowLayoutPanel1.Controls)
            {
                if (string.IsNullOrEmpty(control.IP) || string.IsNullOrEmpty(control.Username) || string.IsNullOrEmpty(control.EncryptedPassword))
                    return false;
            }

            return true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var control = new CounterUserControl();
            control.OnRemove += (s, ea) =>
            {
                flowLayoutPanel1.Controls.Remove(s as Control);
            };
            flowLayoutPanel1.Controls.Add(control);
        }
    }
}
