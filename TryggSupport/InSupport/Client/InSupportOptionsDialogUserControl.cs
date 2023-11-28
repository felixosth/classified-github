using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoOS.Platform.Client;
using System.IO;

namespace InSupport.Client
{
    /// <summary>
    /// This UserControl is created by the PluginDefinition and placed on the Smart Client's options dialog when the user selects the options icon.<br/>
    /// The UserControl will be added to the owning parent UserControl and docking set to Fill.
    /// </summary>
    public partial class InSupportOptionsDialogUserControl : OptionsDialogUserControl
    {
        public InSupportOptionsDialogUserControl()
        {
            InitializeComponent();
        }

        public override void Init()
        {
        }

        public override void Close()
        {
            Properties.Settings.Default.NTPServer = ntpServerTextBox.Text;
            Properties.Settings.Default.Save();

        }

        private void InSupportOptionsDialogUserControl_Load(object sender, EventArgs e)
        {
            ntpServerTextBox.Text = Properties.Settings.Default.NTPServer;
            checkBox1.Checked = Properties.Settings.Default.Debug;
            checkBox2.Checked = Properties.Settings.Default.ControlPanelEnabled;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Debug = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ControlPanelEnabled = checkBox2.Checked;
        }

        private void ntpServerTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
