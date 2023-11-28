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
using VideoOS.Platform.Client;

namespace SCLogin.Login
{
    /// <summary>
    /// Interaction logic for YubikeyLoginUserControl.xaml
    /// </summary>
    public partial class YubikeyLoginUserControl : LoginUserControl
    {
        MyLoginPlugin loginPlugin;
        public YubikeyLoginUserControl(MyLoginPlugin loginPlugin)
        {
            InitializeComponent();
            this.loginPlugin = loginPlugin;
        }

        public override void Init()
        {
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            loginPlugin.SendMoreData(otpTextBox.Text);
        }

        private void otpTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                loginBtn_Click(sender, new RoutedEventArgs());
            }
        }

        private void LoginUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            otpTextBox.Focus();
        }
    }
}
