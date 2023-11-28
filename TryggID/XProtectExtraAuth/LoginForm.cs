using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform.SDK.UI.LoginDialog;
using VideoOS.Platform.SDK.Platform;
using System.Net;
using System.Diagnostics;
using MessagingWrapper;
using VideoOS.Platform.Messaging;
using VideoOS.Platform;

namespace XProtectExtraAuth
{
    public partial class LoginForm : Form
    {
        //AuthSystem auth;

        Messaging messaging;

        protected string username = "preauthorizationuser", password = @"/Y>QUCg?7ynxDpd\Hg=!`qbuj^pkUy}VLGL4@";


        const string requestFilter = "AuthSystem.Com.LoginRequest";
        const string responseFilter = "AuthSystem.Com.LoginResponse";

        public LoginForm()
        {
            VideoOS.Platform.SDK.Environment.Initialize();

            InitializeComponent();

            personalNrBox.Text = Properties.Settings.Default.LastPersonalNumber;
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            Uri server = new Uri("http://" + serverTxtBox.Text);
            CredentialCache cc = VideoOS.Platform.Login.Util.BuildCredentialCache(server, username, password, "Basic");

            VideoOS.Platform.SDK.Environment.AddServer(server, cc);

            try
            {
                VideoOS.Platform.SDK.Environment.Login(server, false);
            }
            catch (InvalidCredentialsMIPException)
            {
                MessageBox.Show("Could not login. Contact admin", "Invalid credentials");
            }
            catch(ServerNotFoundMIPException)
            {
                MessageBox.Show("Could not connect to server. Please check hostname/IP", "Invalid server");
            }

            var result = VideoOS.Platform.SDK.Environment.IsLoggedIn(server);

            if(result)
            {
                serverTxtBox.Enabled = false;
                personalNrBox.Enabled = false;
                loginBtn.Enabled = false;

                SetupMessaging();

                messaging.SendMessage(requestFilter, new MessageData(personalNrBox.Text));
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //auth?.Close();
            messaging?.Close();
            VideoOS.Platform.SDK.Environment.Logout();

            base.OnClosing(e);
        }

        void SetupMessaging()
        {
            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            var msgCom = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            messaging = new Messaging(msgCom);

            messaging.RegisterCustomMessageReciever(responseFilter).OnMessageRecieved += ServerMessageRecived;
        }

        private void ServerMessageRecived(object sender, MessageData msgData)
        {
            var type = msgData.Parameters[0] as string;
            if (type == "error")
            {
                string title = "Error";
                string message = msgData.Parameters[1] as string;

                MessageBox.Show(message, title);

                this.Invoke(new Action(() =>
                {
                    serverTxtBox.Enabled = true;
                    personalNrBox.Enabled = true;
                    loginBtn.Enabled = true;
                    infoLabel.Text = "";
                }));
            }
            else if (type == "info")
            {
                infoLabel.Invoke(new Action(() =>
                {
                    infoLabel.Text = msgData.Parameters[1] as string;
                }));
            }
            else if (type == "login")
            {
                Properties.Settings.Default.LastPersonalNumber = personalNrBox.Text;
                Properties.Settings.Default.Save();

                string username = msgData.Parameters[1] as string;
                string password = msgData.Parameters[2] as string;

                var args = string.Format("-ServerAddress=\"{0}\" -UserName \"{1}\" -Password \"{2}\" -AuthenticationType Simple", serverTxtBox.Text, username, password);

                Process.Start(@"C:\Program Files\Milestone\XProtect Smart Client\Client.exe", args);

                Application.Exit();
            }
        }
    }


}
