using Microsoft.Win32;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms;
using System.Windows.Media;
using TryggRetail.Background;
using VideoOS.Platform;

namespace TryggRetail.Client
{
    public partial class TryggRetailSettingsPanelControl : UserControl
    {
        private readonly TryggRetailBackgroundPlugin backInstance;
        private const string _propertyId = "aSettingId";
        public TryggRetailSettingsPanelControl(TryggRetailBackgroundPlugin backInstance)
        {
            this.backInstance = backInstance;

            InitializeComponent();

            maxWindowsCountTxtBox.Text = Properties.Settings.Default.MaxWindows.ToString();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int maxWindows = 0;
            if (int.TryParse(maxWindowsCountTxtBox.Text, out maxWindows))
            {
                Properties.Settings.Default.MaxWindows = maxWindows;
                Properties.Settings.Default.Save();
            }
            else
                MessageBox.Show("Unable to parse MaxWindows string.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
