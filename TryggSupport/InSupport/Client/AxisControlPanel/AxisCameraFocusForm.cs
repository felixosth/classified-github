using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;
using VideoOS.Platform.UI;

namespace InSupport.Client
{
    public partial class AxisCameraFocusForm : Form
    {
        string ip;

        public AxisCameraFocusForm(string ip)
        {
            InitializeComponent();
            this.ip = ip;
            ClientControl.Instance.RegisterUIControlForAutoTheming(this);

        }

        private void AxisCameraFocusForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var wc = new WebClient())
            {
                wc.Credentials = new NetworkCredential(usernameBox.Text, passwordBox.Text);
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    //wc.DownloadStringCompleted += Wc_DownloadStringCompleted;
                    var result = wc.DownloadString(new Uri("http://" + ip + "/axis-cgi/opticssetup.cgi?autofocus=perform"));

                    if (result.Trim() == "ok")
                    {
                        MessageBox.Show("Successfully performed the operation.", "Autofocus complete");
                    }
                    else
                        throw new Exception("Did not get 'ok' from camera.");

                    //MessageBox.Show(result);
                    button1.Enabled = false;
                    usernameBox.Enabled = false;
                    passwordBox.Enabled = false;
                }
                catch
                {
                    Cursor.Current = Cursors.Arrow;

                    MessageBox.Show("Something went wrong!\r\nThe camera might not support autofocus or the login credentials are invalid.", "Error");

                    if (Properties.Settings.Default.Debug)
                        throw;
                }

                this.DialogResult = DialogResult.OK;
            }
        }

        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Cursor.Current = Cursors.Arrow;

            if(e.Result.Trim() == "ok")
            {
                MessageBox.Show("Successfully performed the operation.", "Autofocus complete");
            }
            else
            {
                if(Properties.Settings.Default.Debug)
                    MessageBox.Show(e.Result, "Error");

                MessageBox.Show("Something went wrong!\r\nThe camera might not support autofocus or the login credentials are invalid.", "Error");
            }
            //throw new NotImplementedException();
            this.DialogResult = DialogResult.OK;
        }

        private void bookmarksButton_Click(object sender, EventArgs e)
        {
            using (var getLogin = new LoginBookmarks.GetLoginBookmarkForm())
            {
                if (getLogin.ShowDialog() == DialogResult.OK)
                {
                    if (getLogin.SelectedLogin == null)
                        return;
                    usernameBox.Text = getLogin.SelectedLogin.Username;
                    passwordBox.Text = getLogin.SelectedLogin.DecryptPassword(LoginBookmarks.GetLoginBookmarkForm._key);
                }
            }
        }
    }
}
