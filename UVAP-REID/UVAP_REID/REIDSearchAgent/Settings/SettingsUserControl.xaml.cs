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

namespace REIDSearchAgent.Settings
{
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsUserControl : UserControl
    {
        ReidSettingsPlugin settingsPlugin;

        public string NodeREDUrl => nodeRedUrlTxtBox.Text;

        public SettingsUserControl(ReidSettingsPlugin settingsPlugin)
        {
            InitializeComponent();
            this.settingsPlugin = settingsPlugin;

            var urlVal = settingsPlugin.GetProperty(ReidSettingsPlugin.NodeREDUrl);
            if(urlVal != null)
            {
                nodeRedUrlTxtBox.Text = urlVal;
            }
        }
    }
}
