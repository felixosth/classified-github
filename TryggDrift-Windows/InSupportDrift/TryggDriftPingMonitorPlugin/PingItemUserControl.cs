using InSupport.Drift.Plugins;
using System.Windows.Forms;

namespace TryggDriftPingMonitorPlugin
{
    public partial class PingItemUserControl : UserControl
    {
        public PingItemUserControl()
        {
            InitializeComponent();
        }

        public PingItem GetPingItem()
        {
            return new PingItem()
            {
                Label = labelTxtBox.Text,
                Type = typeTxtBox.Text,
                IP = ipTxtBox.Text
            };
        }
    }
}
