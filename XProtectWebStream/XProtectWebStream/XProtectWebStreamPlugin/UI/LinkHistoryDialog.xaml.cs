using System;
using System.Collections.Generic;
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
using XProtectWebStream.Shared;

namespace XProtectWebStreamPlugin.UI
{
    /// <summary>
    /// Interaction logic for LinkHistoryDialog.xaml
    /// </summary>
    public partial class LinkHistoryDialog : UserControl
    {
        ClientCommunication clientCommunication;

        public LinkHistoryDialog(ClientCommunication clientCommunication)
        {
            this.clientCommunication = clientCommunication;
            InitializeComponent();
            linksListBox.ItemsSource = clientCommunication.ResponseHistory;
        }

        private void LinkButton_Click(object sender, RoutedEventArgs e)
        {
            var tokenResponse = (sender as Button).DataContext as TokenResponse;

            Window.GetWindow(this).Content = new ShowLinkDialog(clientCommunication, tokenResponse.CompleteLink, tokenResponse.Token);
        }

        private void RevokeButton_Click(object sender, RoutedEventArgs e)
        {
            var tokenResponse = (sender as Button).DataContext as TokenResponse;
            clientCommunication.RevokeToken(tokenResponse.Token);
        }
    }
}
