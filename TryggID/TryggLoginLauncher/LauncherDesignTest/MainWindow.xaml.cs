using MessagingWrapper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;
using VideoOS.Platform.SDK.Platform;

namespace TryggLoginLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected string username = "preauthorizationuser", milestonePassword = @"/Y>QUCg?7ynxDpd\Hg=!`qbuj^pkUy}VLGL4@";

        protected const string requestFilter = "AuthSystem.Com.LoginRequest", responseFilter = "AuthSystem.Com.LoginResponse";

        Messaging messaging;
        bool loading = false;

        public MainWindow()
        {
            InitializeComponent();

            VideoOS.Platform.SDK.Environment.Initialize();
            serverBox.Text = Properties.Settings.Default.LastHostname;

            if (!File.Exists(Properties.Settings.Default.SmartClientPath))
            {
                MessageBox.Show("Couldn't detect the Smart Client installation path. Please select it manually", "Error");
                var fd = new OpenFileDialog();
                fd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                fd.Filter = "XProtect Smart Client | Client.exe";
                if (fd.ShowDialog() == true)
                {
                    Properties.Settings.Default.SmartClientPath = fd.FileName;
                    Properties.Settings.Default.Save();
                }
            }

            keyBox.Focus();

        }

        void SetupMessaging(MessageData initialMsg)
        {
            loading = true;

            if (messaging == null)
            {
                MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
                var msgCom = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

                messaging = new Messaging(msgCom);

                messaging.RegisterCustomMessageReciever(responseFilter).OnMessageRecieved += ServerMessageRecived;
            }

            try
            {
                messaging.SendMessage(requestFilter, initialMsg);
            }
            catch 
            {
                ShowError("Could not connect to the Event Server");
                loading = false;
                this.Dispatcher.Invoke(new Action(() => SetEnableControls(true)));
            }
        }

        bool MilestoneLogin(string hostname)
        {
            var split = hostname.Split(':');
            int port = 80;
            if (split.Length > 1)
            {
                if (!int.TryParse(split[1], out port))
                {
                    MessageBox.Show("Invalid port", "Error");
                    return false;
                }

                hostname = split[0];
            }

            UriBuilder server = new UriBuilder("http", hostname, port);

            CredentialCache cc = VideoOS.Platform.Login.Util.BuildCredentialCache(server.Uri, username, milestonePassword, "Basic");
            VideoOS.Platform.SDK.Environment.AddServer(server.Uri, cc);

            try
            {
                VideoOS.Platform.SDK.Environment.Login(server.Uri, false);
            }
            catch (InvalidCredentialsMIPException)
            {
                //MessageBox.Show("Could not login. Contact admin", "Invalid credentials");
                ShowError("Could not login. Contact the administrator. (Invalid credentials)");
                return false;
            }
            catch (ServerNotFoundMIPException)
            {
                //MessageBox.Show("Could not connect to server. Please check hostname/IP", "Invalid server");
                ShowError("Could not connect to server. Please check hostname/IP");
                return false;
            }

            return VideoOS.Platform.SDK.Environment.IsLoggedIn(server.Uri);
        }


        void SetEnableControls(bool isEnabled)
        {
            //bankIdHostnameTxtBox.IsEnabled = isEnabled;
            //personalNrTxtBox.IsEnabled = isEnabled;
            loginBtn.IsEnabled = isEnabled;
            //controlsBorder.IsEnabled = isEnabled;
            mainGrid.IsEnabled = isEnabled;
            this.Cursor = isEnabled ? Cursors.Arrow : Cursors.Wait;

            //yubiHostnameTxtBox.IsEnabled = isEnabled;
            //yubiKeyTxtBox.IsEnabled = isEnabled;

            //loginBtn.Background = isEnabled ? Brushes.White : Brushes.Black;
        }

        void ShowError(string msg)
        {
            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ShowError(msg));
            }
            else
            {
                statusLabel.Content = msg;
                statusLabel.Foreground = Brushes.Red;
            }
        }


        void ShowInfo(string msg)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ShowError(msg));
            }
            else
            {
                statusLabel.Content = msg;
                statusLabel.Foreground = Brushes.White;
            }
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            if (loading)
            {
                e.Cancel = MessageBox.Show("The login operation is currently running. Are you sure you want to quit?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No;
            }

            if (!e.Cancel)
                return;

            messaging?.Close();
            VideoOS.Platform.SDK.Environment.Logout();

            base.OnClosing(e);
        }

        private void ExitBtn_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void TopBar_Click(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(serverBox.Text))
            {
                MessageBox.Show("Enter hostname", "Empty hostname");
                return;
            }
            if (string.IsNullOrEmpty(keyBox.Text))
            {
                MessageBox.Show("Enter key", "Empty key");
                return;
            }

            SetEnableControls(false);

            string hostname = serverBox.Text;
            string key = keyBox.Text;
            string pw = passBox.Password;

            new Thread(() =>
            {
                var result = MilestoneLogin(hostname);

                if (result)
                {
                    SetupMessaging(new MessageData(key, pw, GetIPAddress(Dns.GetHostName())));
                }
                else
                {
                    this.Dispatcher.Invoke(new Action(() => SetEnableControls(true)));
                }
            }).Start();
        }

        private void keyBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                passBox.Focus();
                e.Handled = true;
            }
        }

        private void ServerMessageRecived(object sender, MessageData msgData)
        {
            var type = msgData.Parameters[0] as string;
            if (type == "error")
            {
                //string title = "Error";
                string message = msgData.Parameters[1] as string;

                //MessageBox.Show(message, title);

                loading = false;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    SetEnableControls(true);
                    ShowError(message);
                }));
            }
            else if (type == "info")
            {
                statusLabel.Dispatcher.Invoke(new Action(() =>
                {
                    ShowInfo(msgData.Parameters[1] as string);
                }));
            }
            else if (type == "login")
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.Hide();
                    loading = false;
                    string username = msgData.Parameters[1] as string;
                    string password = msgData.Parameters[2] as string;

                    var args = string.Format("-ServerAddress=\"{0}\" -UserName \"{1}\" -Password \"{2}\" -AuthenticationType Simple", serverBox.Text, username, password);

                    try
                    {
                        Process.Start(Properties.Settings.Default.SmartClientPath, args);
                    }
                    catch
                    {
                        //var path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                        MessageBox.Show("Could not open the Smart Client.", "Error");
                        //Process.Start("notepad.exe", path);
                    }


                    Properties.Settings.Default.LastHostname = serverBox.Text;

                    Properties.Settings.Default.Save();
                    Application.Current.Shutdown();
                }));
            }
        }

        public static string GetIPAddress(string hostname)
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(hostname);

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    //System.Diagnostics.Debug.WriteLine("LocalIPadress: " + ip);
                    return ip.ToString();
                }
            }
            return string.Empty;
        }
    }

}
