using InSupport.Drift.Plugins;
using System;
using System.Windows.Forms;

namespace AxisPeopleCounterPlugin
{
    public partial class CounterUserControl : UserControl
    {
        public event EventHandler OnRemove;

        public string Username => userTxtBox.Text;
        public string CounterName => nameTxtBox.Text;
        public string EncryptedPassword => StringCipher.Encrypt(pwdTxtBox.Text, PeopleCounterMonitor._EncryptKey);
        public string IP => ipTxtBox.Text;

        public CounterUserControl()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            OnRemove(this, null);
        }
    }
}
