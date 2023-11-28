using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicenseTester
{
    public partial class LicenseInfoForm : Form
    {
        InSupport_LicenseSystem.LicenseManager licenseManager;

        public LicenseInfoForm(InSupport_LicenseSystem.LicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
            InitializeComponent();

            var licenseCheck = licenseManager.CheckOnlineLicense();
            if (licenseCheck != InSupport_LicenseSystem.LicenseCheckResult.None && licenseCheck != InSupport_LicenseSystem.LicenseCheckResult.Error)
            {
                var info = licenseManager.GetLicenseInfo();
                licenseKeyLabel.Text = "License: " + info.Value;
                statusLabel.Text = "Status: " + licenseCheck.ToString();
                expirationLabel.Text = "Expiration date: " + info.ExpirationDate.ToShortDateString();
                customerLabel.Text = "Customer: " + info.Customer;
                siteLabel.Text = "Site: " + info.Site;
            }
            else
                statusLabel.Text = "Status: " + licenseCheck.ToString();
        }

        private void LicenseInfoForm_Load(object sender, EventArgs e)
        {
            
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
