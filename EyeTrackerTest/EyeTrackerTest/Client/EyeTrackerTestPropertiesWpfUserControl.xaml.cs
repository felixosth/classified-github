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
using VideoOS.Platform;
using VideoOS.Platform.UI;

namespace EyeTrackerTest.Client
{
    /// <summary>
    /// This UserControl contains the visible part of the Property panel during Setup mode. <br/>
    /// If no properties is required by this ViewItemPlugin, the GeneratePropertiesUserControl() method on the ViewItemManager can return a value of null.
    /// <br/>
    /// When changing properties the ViewItemManager should continuously be updated with the changes to ensure correct saving of the changes.
    /// <br/>
    /// As the user click on different ViewItem, the displayed property UserControl will be disposed, and a new one created for the newly selected ViewItem.
    /// </summary>
    public partial class EyeTrackerTestPropertiesWpfUserControl : PropertiesWpfUserControl
    {

        private EyeTrackerTestViewItemManager _viewItemManager;


        /// <summary>
        /// This class is created by the ViewItemManager.  
        /// </summary>
        /// <param name="viewItemManager"></param>
        public EyeTrackerTestPropertiesWpfUserControl(EyeTrackerTestViewItemManager viewItemManager)
        {
            InitializeComponent();
            _viewItemManager = viewItemManager;
            ptzChkBox.IsChecked = viewItemManager.IsPTZ;
            try
            {
                if (viewItemManager.CameraFQID != null)
                    camLabel.Content = Configuration.Instance.GetItem(viewItemManager.CameraFQID).Name;
            }
            catch { }
        }

        /// <summary>
        /// Setup events and message receivers and load stored configuration.
        /// </summary>
        public override void Init()
        {
        }

        /// <summary>
        /// Perform any cleanup stuff and event -=
        /// </summary>
        public override void Close()
        {
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ItemPickerForm itemPicker = new ItemPickerForm();
            itemPicker.KindFilter = Kind.Camera;
            itemPicker.Init();

            if(itemPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                camLabel.Content = itemPicker.SelectedItem.Name;
                _viewItemManager.SaveCam(itemPicker.SelectedItem);
            }
        }

        private void ptzChkBox_Checked(object sender, RoutedEventArgs e)
        {
            _viewItemManager.IsPTZ = true;
        }

        private void ptzChkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _viewItemManager.IsPTZ = false;
        }
    }
}
