using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;
using InSupport_LicenseSystem;

namespace TryggLogin.Admin.SubControls
{
    public partial class LicenseUserControl : BaseSubUserControl
    {
        LicenseWrapper licenseWrapper;

        public LicenseUserControl(AdminUserControl parent) : base(parent)
        {
            InitializeComponent();

            var myMguid = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null) as string;
            licenseWrapper = new LicenseWrapper(myMguid);

        }

        public LicenseUserControl()
        {

        }

        public override void Fill(Item item)
        {
            base.Fill(item);

        }

        public override void UpdateItem(Item item)
        {
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

        private void ActivateLicenseBtn_Click(object sender, EventArgs e)
        {
            var activationResult = licenseWrapper.ActivateLicense(licKeyBox.Text);
            if (activationResult != LicenseActivationResult.Success)
                MessageBox.Show("License error: " + activationResult.ToString());
            else
                CheckLicense();
        }

        private void LicenseUserControl_Load(object sender, EventArgs e)
        {
            CheckLicense();
        }
    }

}
