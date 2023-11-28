using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Admin;

namespace OpenALPR_Milestone.Admin
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

        private void Button1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Warning!\r\n\r\nAre you sure you want to delete the plugin configuration?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Configuration.Instance.DeleteItemConfiguration(OpenALPR_MilestoneDefinition.OpenALPR_MilestonePluginId, 
                    Configuration.Instance.GetItem(OpenALPR_MilestoneDefinition.OpenALPR_MilestoneConfigItemId, OpenALPR_MilestoneDefinition.OpenALPR_MilestoneKind));
                MessageBox.Show("Configuration deleted.", "Nice");
            }
        }
    }
}
