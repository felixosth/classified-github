using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VideoOS.Platform;

namespace BergendahlsPOS.Client
{
    public partial class BergendahlsPOSSettingsPanelControl : UserControl
    {
        List<CheckboxCamera> checkboxCameras;
        private readonly BergendahlsPOSSettingsPanelPlugin _plugin;
        public BergendahlsPOSSettingsPanelControl(BergendahlsPOSSettingsPanelPlugin plugin)
        {
            _plugin = plugin;

            InitializeComponent();

            storeIdTextBox.Text = _plugin.GetProperty(BergendahlsPOSSettingsPanelPlugin.StoreIDSettingsKey);

            var cameras = BergendahlsPOSWorkSpaceViewItemWpfUserControl.GetAllItems(Configuration.Instance.GetItemsByKind(Kind.Camera, ItemHierarchy.SystemDefined)).Where(c => c.Enabled == true).OrderBy(c => c.Name).ToList();

            string usedCamerasRaw = _plugin.GetProperty(BergendahlsPOSSettingsPanelPlugin.CamerasSettingsKey);
            FQID[] usedCamerasList = new FQID[0];

            if (usedCamerasRaw != null)
            {
                var usedCamerasListStrings = JsonConvert.DeserializeObject<string[]>(usedCamerasRaw);
                usedCamerasList = new FQID[usedCamerasListStrings.Length];
                for (int i = 0; i < usedCamerasListStrings.Length; i++)
                {
                    usedCamerasList[i] = new FQID(usedCamerasListStrings[i]);
                }
            }

            checkboxCameras = new List<CheckboxCamera>();
            foreach(var camera in cameras)
            {
                var checkboxCamera = new CheckboxCamera(camera);
                if (usedCamerasList.Contains(checkboxCamera.CameraItem.FQID))
                    checkboxCamera.IsChecked = true;
                checkboxCameras.Add(checkboxCamera);
            }

            camerasListBox.ItemsSource = checkboxCameras;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _plugin.SetProperty(BergendahlsPOSSettingsPanelPlugin.StoreIDSettingsKey, storeIdTextBox.Text);

            string errorMessage;
            if (!_plugin.TrySaveChanges(out errorMessage))
            {
                MessageBox.Show(errorMessage);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkedCameras = JsonConvert.SerializeObject(checkboxCameras.Where(cc => cc.IsChecked).Select(cc => cc.CameraItem.FQID.ToXmlNode().OuterXml));
            _plugin.SetProperty(BergendahlsPOSSettingsPanelPlugin.CamerasSettingsKey, checkedCameras);

            string errorMessage;
            if (!_plugin.TrySaveChanges(out errorMessage))
            {
                MessageBox.Show(errorMessage);
            }
        }
    }

    class CheckboxCamera
    {
        //private string FQIDXml { get; set; }

        public Item CameraItem;
        public string Name => CameraItem.Name;
        public bool IsChecked { get; set; }

        public CheckboxCamera(Item camera)
        {
            CameraItem = camera;
            //FQIDXml = camera.FQID.ToXmlNode().OuterXml;
        }
    }
}
