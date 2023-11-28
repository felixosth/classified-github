using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.UI;

namespace TryggLarm.Admin
{
    /// <summary>
    /// This UserControl only contains a configuration of the Name for the Item.
    /// The methods and properties are used by the ItemManager, and can be changed as you see fit.
    /// </summary>
    public partial class TryggLarmUserControl : UserControl
    {
        const bool isDebug = true;

        internal event EventHandler ConfigurationChangedByUser;

        List<string> alarmList = new List<string>();

        public TryggLarmUserControl()
        {
            InitializeComponent();
        }

        internal String DisplayName
        {
            get { return nameBox.Text; }
            set { nameBox.Text = value; }
        }

        string test
        {
            get { return telBox.Text; }
            set { telBox.Text = value; }
        }

        /// <summary>
        /// Ensure that all user entries will call this method to enable the Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnUserChange(object sender, EventArgs e)
        {
            if (ConfigurationChangedByUser != null)
                ConfigurationChangedByUser(this, new EventArgs());
        }

        internal void FillContent(Item item)
        {
            nameBox.Text = item.Name;

            if(item.Properties.ContainsKey("tel"))
                telBox.Text = item.Properties["tel"];
            if (item.Properties.ContainsKey("email"))
                emailBox.Text = item.Properties["email"];

            if (item.Properties.ContainsKey("alarmList"))
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(item.Properties["alarmList"])))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    alarmList = (List<string>)bf.Deserialize(ms);
                }

            if (item.Properties.ContainsKey("sendToTel"))
                sendToTelChkBox.Checked = bool.Parse(item.Properties["sendToTel"]);
            if (item.Properties.ContainsKey("sendToEmail"))
                sendToEmailChkBox.Checked = bool.Parse(item.Properties["sendToEmail"]);
            if (item.Properties.ContainsKey("emailSubject"))
                emailSubjectBox.Text = item.Properties["emailSubject"];
            if (item.Properties.ContainsKey("emailBody"))
                emailBodyBox.Text = item.Properties["emailBody"];
            if (item.Properties.ContainsKey("smsFormat"))
                smsMessageBox.Text = item.Properties["smsFormat"];
            if (item.Properties.ContainsKey("alarmOffsetSec"))
                alarmOffsetNum.Value = decimal.Parse(item.Properties["alarmOffsetSec"]);
            if (item.Properties.ContainsKey("attachCamImg"))
                attachImageChkBox.Checked = bool.Parse(item.Properties["attachCamImg"]);
            RefreshListView();
        }

        internal void UpdateItem(Item item)
        {
            item.Name = DisplayName;
            item.Properties["tel"] = telBox.Text;
            item.Properties["email"] = emailBox.Text;

            item.Properties["sendToTel"] = sendToTelChkBox.Checked.ToString();
            item.Properties["sendToEmail"] = sendToEmailChkBox.Checked.ToString();

            item.Properties["smsFormat"] = smsMessageBox.Text;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, alarmList);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                item.Properties["alarmList"] = Convert.ToBase64String(buffer);
            }

            var formatKeys = new List<FormatKey>()
            {
                new FormatKey("%name%", nameBox.Text),
                new FormatKey("%email%", emailBox.Text),
                new FormatKey("%tel%", telBox.Text)
            };

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, formatKeys);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                item.Properties["formatKeys"] = Convert.ToBase64String(buffer);
            }

            item.Properties["emailSubject"] = emailSubjectBox.Text;
            item.Properties["emailBody"] = emailBodyBox.Text;
            item.Properties["alarmOffsetSec"] = alarmOffsetNum.Value.ToString();
            item.Properties["attachCamImg"] = attachImageChkBox.Checked.ToString();
            // Fill in any propertuies that should be saved:
            //item.Properties["AKey"] = "some value";
        }

        internal void ClearContent()
        {
            nameBox.Text = "";
            telBox.Text = "";
            emailBox.Text = "";
            newAlarmBox.Text = "";
            alarmList.Clear();
            alarmListView.Items.Clear();
            sendToTelChkBox.Checked = false;
            sendToEmailChkBox.Checked = false;
            emailSubjectBox.Text = "";
            emailBodyBox.Text = "";
            smsMessageBox.Text = "";
            alarmOffsetNum.Value = 1;
            attachImageChkBox.Checked = false;
        }

        private void TryggLarmUserControl_Load(object sender, EventArgs e)
        {

        }

        private void addAlarmBtn_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(newAlarmBox.Text))
            {
                MessageBox.Show("No alarm text specified.", "Error");
                return;
            }
            foreach (string alarm in alarmList)
                if (newAlarmBox.Text == alarm)
                {
                    MessageBox.Show("This alarm is already added to this group.", "Error");
                    return;
                }

            alarmList.Add(newAlarmBox.Text);
            newAlarmBox.Text = "";
            RefreshListView();

            OnUserChange(sender, e);
        }


        void RefreshListView()
        {
            alarmListView.Items.Clear();
            foreach (string s in alarmList)
                alarmListView.Items.Add(s);
        }

        private void newAlarmBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(newAlarmBox.Text))
                addAlarmBtn_Click(sender, new EventArgs());
        }

        private void alarmListContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (alarmListView.SelectedItems.Count < 1)
                e.Cancel = true;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem item in alarmListView.SelectedItems)
            {
                alarmList.Remove(item.Text);
            }
            OnUserChange(sender, e);
            RefreshListView();
        }

    }
}
