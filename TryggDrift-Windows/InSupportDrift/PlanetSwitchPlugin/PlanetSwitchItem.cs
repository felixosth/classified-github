using System;
using System.Windows.Forms;

namespace PlanetSwitchPlugin
{
    public partial class PlanetSwitchItem : UserControl
    {
        public event EventHandler OnCloseBtn;

        public string Username => usernameTxtBox.Text;
        public string Password => passwordTxtBox.Text;
        public string IP => ipTxtBox.Text;

        public PlanetSwitchItem()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            OnCloseBtn?.Invoke(sender, e);
        }
    }
}
