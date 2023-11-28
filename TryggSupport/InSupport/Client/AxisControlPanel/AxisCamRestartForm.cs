using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VideoOS.Platform;

namespace InSupport.Client
{
    public partial class AxisCamRestartForm : Form
    {
        private string username { get; set; }
        private string password { get; set; }

        private string ip { get; set; }

        enum RestartMethod
        {
            Vapix,
            FTP
        }

        public AxisCamRestartForm(string ip)
        {
            this.ip = ip;
            InitializeComponent();
            ClientControl.Instance.RegisterUIControlForAutoTheming(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RestartCamera(RestartMethod.Vapix);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RestartCamera(RestartMethod.FTP);
        }

        private void RestartCamera(RestartMethod method)
        {
            username = usernameBox.Text;
            password = passwordBox.Text;

            if (method == RestartMethod.FTP)
            {

                var cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.UseShellExecute = false;
                //cmd.StartInfo.Arguments = "/K ftp " + ip;
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.Start();

                using (var input = cmd.StandardInput)
                {
                    using (var output = cmd.StandardOutput)
                    {
                        input.WriteLine("ftp " + ip);
                        Thread.Sleep(50);
                        input.WriteLine(username);
                        Thread.Sleep(50);
                        input.WriteLine(password);
                        Thread.Sleep(50);
                        input.WriteLine("quote site reboot");
                        Thread.Sleep(50);
                        input.WriteLine("quote site reboot");
                        Thread.Sleep(50);
                        input.WriteLine("bye");
                        Thread.Sleep(50);
                        input.WriteLine("exit");
                    }
                }

                cmd.WaitForExit();
                cmd.Close();
            }
            else
            {
                using (var wc = new WebClient())
                {
                    wc.Credentials = new NetworkCredential(username, password);
                    try
                    {
                        var result = wc.DownloadString("http://" + ip + "/axis-cgi/admin/restart.cgi");
                    }
                    catch
                    {
                        MessageBox.Show("Something went wrong!\r\nCheck the login credentials and try again.", "Error");
                        if (Properties.Settings.Default.Debug)
                            throw;

                        return;
                    }
                }
            }


            if (openPingCheckBox.Checked)
            {
                ProcessStartInfo pInfo = new ProcessStartInfo("cmd.exe");
                pInfo.Arguments = "/k ping " + ip + " -t";
                Process.Start(pInfo);

            }

            this.DialogResult = DialogResult.OK;
        }

        private void FTPRestartForm_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var getLogin = new LoginBookmarks.GetLoginBookmarkForm())
            {
                if(getLogin.ShowDialog() == DialogResult.OK)
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
