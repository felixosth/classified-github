using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.UI;
using FlxHelperLib;
using VideoOS.Platform.Messaging;
using InSupport_LicenseSystem;

namespace OpenALPR_Milestone.Admin
{
    /// <summary>
    /// This UserControl only contains a configuration of the Name for the Item.
    /// The methods and properties are used by the ItemManager, and can be changed as you see fit.
    /// </summary>
    public partial class OpenALPR_MilestoneUserControl : UserControl
    {
        internal event EventHandler ConfigurationChangedByUser;

        LicenseWrapper licenseWrapper;

        internal static string _company_id_key = "alpr_company_id", _site_key = "alpr_site", 
            _last_check_key = "alpr_last_check", _cameras_key = "alpr_cameras", _event_key = "alpr_event", _method_key = "alpr_method";

        List<Item> cameras = new List<Item>();
        Item lprEvent;

        public OpenALPR_MilestoneUserControl()
        {
            InitializeComponent();

            var myMguid = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null) as string;
            if (string.IsNullOrEmpty(myMguid))
            {
                MessageBox.Show("Error getting MGUID. Contact InSupport");
            }
            else
            {
                licenseWrapper = new LicenseWrapper(myMguid);
                //licenseWrapper.CheckLicense();
                //licenseWrapper.Init();
            }
        }

        /// <summary>
        /// Ensure that all user entries will call this method to enable the Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnUserChange(object sender, EventArgs e)
        {
            if (ConfigurationChangedByUser != null)
                ConfigurationChangedByUser(this, new EventArgs());
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var activationResult = licenseWrapper.ActivateLicense(licKeyBox.Text);
            if (activationResult != LicenseActivationResult.Success)
                MessageBox.Show("License error: " + activationResult.ToString());
            else
                CheckLicense();
        }

        void CheckLicense()
        {
            licenseStatusLabel.Text = licenseWrapper.CheckLicense().ToString();
            if (licenseWrapper.LatestLicenseInfo != null)
            {
                expirationLabel.Text = licenseWrapper.LatestLicenseInfo.ExpirationDate.ToShortDateString();
                licKeyBox.Text = licenseWrapper.LatestLicenseInfo.Value;
            }

            var enabled = licenseWrapper.LastLicenseCheck != LicenseCheckResult.Valid;
            activateLicenseBtn.Enabled = enabled;
            licKeyBox.ReadOnly = !enabled;
        }

        private void OpenALPR_MilestoneUserControl_Load(object sender, EventArgs e)
        {
            CheckLicense();
        }

        internal void FillContent(Item item)
        {
            if (item.Properties.ContainsKey(_company_id_key))
                companyIdTxtBox.Text = item.Properties[_company_id_key];

            if (item.Properties.ContainsKey(_site_key))
                siteTxtBox.Text = item.Properties[_site_key];

            if (item.Properties.ContainsKey(_method_key))
            {
                switch(item.Properties[_method_key])
                {
                    case "alarm":
                        analyticMethodRadio.Checked = false;
                        alarmMethodRadio.Checked = true;
                        break;
                    case "analytics":
                        analyticMethodRadio.Checked = true;
                        alarmMethodRadio.Checked = false;
                        break;
                }
            }

            if (item.Properties.ContainsKey(_event_key))
            {
                lprEvent = Configuration.Instance.GetItem(Guid.Parse(item.Properties[_event_key]), Kind.TriggerEvent);
            }
            lprEventTxtBox.Text = lprEvent?.Name ?? "No event selected";

            if (item.Properties.ContainsKey(_cameras_key))
            {
                var cameraIds = item.Properties[_cameras_key].FlxHelper().FromBase64<List<Guid>>();
                cameras.Clear();

                foreach(var camId in cameraIds)
                {
                    var camItem = Configuration.Instance.GetItem(camId, Kind.Camera);
                    cameras.Add(camItem);
                }
            }
        }

        private void SetEventBtn_Click(object sender, EventArgs e)
        {
            using (ItemPickerForm form = new ItemPickerForm())
            {
                form.Init(Configuration.Instance.GetItemsByKind(Kind.TriggerEvent), ItemHierarchy.SystemDefined);
                form.KindFilter = Kind.TriggerEvent;
                form.TopLabel = "Select event";

                if(form.ShowDialog() == DialogResult.OK)
                {
                    lprEvent = form.SelectedItem;
                    lprEventTxtBox.Text = lprEvent.Name;
                    OnUserChange(sender, e);
                }
            }
        }

        private void setCamerasBtn_Click(object sender, EventArgs e)
        {
            using (CamerasPickerForm camPicker = new CamerasPickerForm(cameras))
            {
                if(camPicker.ShowDialog() == DialogResult.OK)
                {
                    var items = camPicker.SelectedItems;
                    cameras.Clear();
                    foreach(var item in items)
                    {
                        cameras.Add(item);
                    }
                    OnUserChange(sender, e);
                }
            }
        }

        internal void UpdateItem(Item item)
        {
            item.Properties[_company_id_key] = companyIdTxtBox.Text;
            item.Properties[_site_key] = siteTxtBox.Text;
            item.Properties[_last_check_key] = DateTime.UtcNow.ToString();
            item.Properties[_event_key] = lprEvent.FQID.ObjectId.ToString();
            item.Properties[_method_key] = alarmMethodRadio.Checked ? "alarm" : "analytics";

            var guidList = new List<Guid>();
            foreach(var camItem in cameras)
            {
                guidList.Add(camItem.FQID.ObjectId);
            }
            item.Properties[_cameras_key] = guidList.FlxHelper().ToBase64();
        }

        internal void ClearContent()
        {
            companyIdTxtBox.Text = "";
            siteTxtBox.Text = "";
            cameras.Clear();
            lprEventTxtBox.Text = "";
        }
    }
}
