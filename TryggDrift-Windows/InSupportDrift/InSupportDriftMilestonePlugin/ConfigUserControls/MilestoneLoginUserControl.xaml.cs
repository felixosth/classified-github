using System;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace InSupportDriftMilestonePlugin
{
    /// <summary>
    /// Interaction logic for MilestoneLoginUserControl.xaml
    /// </summary>
    public partial class MilestoneLoginUserControl : UserControl
    {
        public MilestoneLoginUserControl()
        {
            InitializeComponent();


        }

        private void windowsAuthCurrentRadio_Checked(object sender, RoutedEventArgs e)
        {
            usernameTxtBox.Text = WindowsIdentity.GetCurrent().Name;
            passwordTxtBox.Password = "";
            passwordTxtBox.IsEnabled = false;
        }

        private void windowsAuthRadio_Checked(object sender, RoutedEventArgs e)
        {
            passwordTxtBox.IsEnabled = true;
        }

        private void basicAuthRadio_Checked(object sender, RoutedEventArgs e)
        {
            passwordTxtBox.IsEnabled = true;
        }

        private void Login()
        {
            var serverAddress = new Uri(serverAddressTxtBox.Text);
            var credentials = new CredentialCache();

            var userName = usernameTxtBox.Text;
            var password = passwordTxtBox.Password;
            var domainName = "";

            if (windowsAuthCurrentRadio.IsChecked == true)
            {
                credentials.Add(serverAddress, "Negotiate", CredentialCache.DefaultNetworkCredentials);
            }
            else if (windowsAuthRadio.IsChecked == true)
            {
                string[] unSplit = userName.Split('\\');
                if (unSplit.Length == 1)
                {
                    userName = unSplit[0];
                    domainName = string.Empty;
                }
                else
                {
                    if (unSplit.Length != 2)
                    {
                        throw new Exception("LoginInvalidUsername");
                    }
                    domainName = unSplit[0];
                    userName = unSplit[1];
                }
                credentials.Add(serverAddress, "Negotiate", new NetworkCredential(userName, password, domainName));
            }
            else
            {
                credentials.Add(serverAddress, "Basic", new NetworkCredential(userName, password));
            }

            loginBtn.IsEnabled = false;
            controlsGrid.IsEnabled = false;

            new Thread(() =>
            {
                VideoOS.Platform.SDK.Environment.AddServer(secureOnly: false, serverAddress, credentials);

                try
                {
                    VideoOS.Platform.SDK.Environment.Login(serverAddress);

                    if (VideoOS.Platform.SDK.Environment.IsLoggedIn(serverAddress))
                    {
                        Dispatcher.Invoke(() => Window.GetWindow(this).DialogResult = true);
                    }
                    else
                    {
                        MessageBox.Show("Unable to login");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                Dispatcher.Invoke(() =>
                {
                    loginBtn.IsEnabled = true;
                    controlsGrid.IsEnabled = true;
                    progressBar.IsIndeterminate = false;
                });

            }).Start();

        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            progressBar.IsIndeterminate = true;
            Login();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = false;

        }
    }
}
