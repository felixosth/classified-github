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

namespace UVAPMipPlugin.Settings
{
    // This is a regular user control that uses the OverlaySettingsPlugin class to set and save the properties. The user control layout is defined in the OverlaySettingsUserControl.xaml file.
    public partial class OverlaySettingsUserControl : UserControl
    {
        OverlaySettingsPlugin plugin;

        bool init = false;

        public OverlaySettingsUserControl(OverlaySettingsPlugin plugin)
        {
            InitializeComponent();
            this.plugin = plugin;

            var skeletonVal = plugin.GetProperty(OverlaySettingsPlugin.SkeletonKey); // Builtin method from the SettingsPanelPlugin base class
            if (skeletonVal != null)
            {
                skeletonChkBox.IsChecked = bool.Parse(skeletonVal);
            }
            else skeletonChkBox.IsChecked = true;

            var headVal = plugin.GetProperty(OverlaySettingsPlugin.HeadKey);
            if (headVal != null)
            {
                headChkBox.IsChecked = bool.Parse(headVal);
            }
            else headChkBox.IsChecked = true;

            init = true;
        }

        private void CheckboxChanged(object sender, RoutedEventArgs e)
        {
            if (init == false)
                return;

            if(sender == skeletonChkBox)
            {
                plugin.SetProperty(OverlaySettingsPlugin.SkeletonKey, skeletonChkBox.IsChecked.ToString()); // Builtin method from the SettingsPanelPlugin base class
                UVAPMipPlugin.Background.VisualizerBackgroundPlugin.EnableSkeleton = skeletonChkBox.IsChecked == true;
            }
            else if(sender == headChkBox)
            {
                plugin.SetProperty(OverlaySettingsPlugin.HeadKey, headChkBox.IsChecked.ToString()); // Builtin method from the SettingsPanelPlugin base class
                UVAPMipPlugin.Background.VisualizerBackgroundPlugin.EnableHeaddetection = headChkBox.IsChecked == true;
            }

            plugin.TrySaveChanges(out _);
        }
    }
}
