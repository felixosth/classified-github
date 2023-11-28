using InSupport.Drift.Plugins;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PlanetSwitchPlugin
{
    public partial class PlanetSwitchPluginCfgCtrl : BasePluginConfig
    {
        List<PlanetSwitchItem> pSwitchItems = new List<PlanetSwitchItem>();

        public PlanetSwitchPluginCfgCtrl()
        {
            InitializeComponent();
        }

        public override bool ValidateForm()
        {
            Cursor.Current = Cursors.WaitCursor;
            bool validation = true;
            foreach (var pSwitch in pSwitchItems)
            {
                if (!new PlanetSwitchLib.PlanetSwitch(pSwitch.IP).Login(pSwitch.Username, pSwitch.Password))
                {
                    pSwitch.BackColor = Color.Red;
                    validation = false;
                }
                else
                    pSwitch.BackColor = Color.Green;

                this.Refresh();
            }

            Cursor.Current = Cursors.Default;

            return validation;
        }

        public override string[] GetSettings()
        {
            List<PlanetSwitch> planetSwitches = new List<PlanetSwitch>();

            foreach (var pSwitch in pSwitchItems)
            {
                planetSwitches.Add(new PlanetSwitch(pSwitch.Username, pSwitch.Password, new PlanetSwitchLib.PlanetSwitch(pSwitch.IP)));
            }

            return new string[]
            {
                "PlanetSwitches=" + JsonConvert.SerializeObject(planetSwitches)
            };
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            PlanetSwitchItem item = new PlanetSwitchItem();
            item.OnCloseBtn += (s, earg) =>
            {
                pSwitchItems.Remove(item);
                flowLayoutPanel1.Controls.Remove(item);
            };
            pSwitchItems.Add(item);
            flowLayoutPanel1.Controls.Add(item);
        }
    }
}
