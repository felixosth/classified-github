using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoOS.Platform;
using XProtectWebStream.Shared;

namespace XProtectWebStreamPlugin.UI
{
    /// <summary>
    /// Interaction logic for ShowLinkDialog.xaml
    /// </summary>
    public partial class ShowLinkDialog : UserControl
    {
        AppDataSettings appDataSettings;
        string appDataSettingsFile;
        Settings serverSettings;

        ShowLinkDialogViewModel myViewModel;

        ClientCommunication clientCommunication;
        private string token;

        public ShowLinkDialog(ClientCommunication clientCommunication, string link, string token)
        {
            appDataSettings = new AppDataSettings(@"InSupport Nätverksvideo AB\XProtectWebStream");
            appDataSettingsFile = EnvironmentManager.Instance.MasterSite.ServerId.Id.ToString().MakePathSafe();
            serverSettings = appDataSettings.GetFromJsonFile<Settings>(appDataSettingsFile) ?? new Settings();

            myViewModel = new ShowLinkDialogViewModel(serverSettings.PreviousPhoneNumbers, serverSettings.PreviousEmails);
            this.DataContext = myViewModel;

            InitializeComponent();

            this.clientCommunication = clientCommunication;

            sendLinkSmsBtn.IsEnabled = sendLinkSmsTxtBox.IsEnabled = clientCommunication.LastFeatureResponse.CanSendSMS == true;

            this.token = token;
            linkTxtBox.Text = link;
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            linkTxtBox.SelectAll();
            linkTxtBox.Copy();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void sendLinkEmailBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!sendLinkEmailTxtBox.Text.IsValidEmail())
            {
                MessageBox.Show("Invalid email");
                return;
            }

            if(!string.IsNullOrEmpty(sendLinkEmailTxtBox.Text))
            {
                clientCommunication.SendEmail(sendLinkEmailTxtBox.Text, token, linkTxtBox.Text);

                if (myViewModel.Emails.Contains(sendLinkEmailTxtBox.Text) == false)
                {
                    myViewModel.Emails.Add(sendLinkEmailTxtBox.Text);

                    serverSettings.PreviousEmails.Add(sendLinkEmailTxtBox.Text);
                    appDataSettings.WriteJsonToFile(appDataSettingsFile, serverSettings);
                }

                MessageBox.Show("Email sent!");

                sendLinkEmailTxtBox.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("No email specified");
            }
        }

        private void sendLinkSmsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(sendLinkSmsTxtBox.Text))
            {
                clientCommunication.SendSMS(sendLinkSmsTxtBox.Text, token, linkTxtBox.Text);

                if(myViewModel.PhoneNumbers.Contains(sendLinkSmsTxtBox.Text) == false)
                {
                    myViewModel.PhoneNumbers.Add(sendLinkSmsTxtBox.Text);
                    serverSettings.PreviousPhoneNumbers.Add(sendLinkSmsTxtBox.Text);
                    appDataSettings.WriteJsonToFile(appDataSettingsFile, serverSettings);
                }

                MessageBox.Show("SMS sent!");

                sendLinkSmsTxtBox.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("No email specified");
            }
        }

        private void sendLinkEmailTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                sendLinkEmailBtn_Click(sender, new RoutedEventArgs());
            }
        }

        private void sendLinkSmsTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                sendLinkSmsBtn_Click(sender, new RoutedEventArgs());
            }
        }
    }

    public class ShowLinkDialogViewModel
    {
        public ObservableCollection<string> PhoneNumbers { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Emails { get; set; } = new ObservableCollection<string>();

        public ShowLinkDialogViewModel(IEnumerable<string> phoneNumbers, IEnumerable<string> emails)
        {
            foreach(var str in phoneNumbers)
            {
                PhoneNumbers.Add(str);
            }

            foreach(var str in emails)
            {
                Emails.Add(str);
            }
        }
    }
}
