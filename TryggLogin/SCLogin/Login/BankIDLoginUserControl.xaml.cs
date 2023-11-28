using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using VideoOS.Platform.Client;

namespace SCLogin.Login
{
    /// <summary>
    /// Interaction logic for SCLoginUserControl.xaml
    /// </summary>
    public partial class BankIDLoginUserControl : LoginUserControl
    {
        MyLoginPlugin loginPlugin;
        public BankIDLoginUserControl(MyLoginPlugin loginPlugin)
        {
            InitializeComponent();
            this.loginPlugin = loginPlugin;
        }

        public override void Init()
        {
            //new Thread(WaitForApproval).Start();
        }

        //void WaitForApproval()
        //{
        //    while(loginPlugin.WaitingForApproval)
        //    {
        //        Thread.Sleep(100);

        //        //if(loginPlugin.WaitingForAck && (DateTime.Now - start).TotalSeconds > 2)
        //        //{ // Plugin is probably not installed on server
        //        //    SCLoginDefinition.SuppressRefresh = true;
        //        //    break;
        //        //}
        //    }

        //    ClientControl.Instance.CallOnUiThread(() => SignalCompleted());
        //}

    }
}
