using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Admin;

namespace RegSormlandMilestonePlugin.Admin
{
    public partial class HelpPage : ItemNodeUserControl
    {
        /// <summary>
        /// User control to display help page
        /// </summary>	
        public HelpPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Display information from or about the Item selected.
        /// </summary>
        /// <param name="item"></param>
        public override void Init(Item item)
        {

        }

        /// <summary>
        /// Close any session and release any resources used.
        /// </summary>
        public override void Close()
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var configItem = Configuration.Instance.GetItem(RegSormlandMilestonePluginDefinition.ConfigItemGuid, RegSormlandMilestonePluginDefinition.RegSormlandMilestonePluginKind);
            if (configItem == null)
            {
                MessageBox.Show("There's no configuration to purge!");
                return;
            }

            if (MessageBox.Show("Are you sure you want to purge the configuration?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Configuration.Instance.DeleteItemConfiguration(RegSormlandMilestonePluginDefinition.RegSormlandMilestonePluginPluginId, configItem);
                MessageBox.Show("Configuration deleted.");
            }
        }
    }
}
