using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicenseTester
{
    public partial class Form1 : Form
    {
        InSupport_LicenseSystem.LicenseManager licenseManager;
        bool hasValidLicense = false;
        public Form1()
        {
            InitializeComponent();
            string myGuid = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null) as string;
            licenseManager = new InSupport_LicenseSystem.LicenseManager(myGuid, "LicenseTester");

            try
            {
                CheckLicense();
            }
            catch
            {
                throw;
            }
        }

        void CheckLicense()
        {
            //string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //string myFolder = Path.Combine(appData, "InSupport.LicenseTest");
            //string finalFileName = Path.Combine(myFolder, "license.lic");

            InSupport_LicenseSystem.LicenseCheckResult checkResult = licenseManager.CheckOnlineLicense();
            //InSupport_LicenseSystem.LicenseCheckResult checkResult = licenseManager.CheckOfflineLicense(finalFileName);
            if (checkResult == InSupport_LicenseSystem.LicenseCheckResult.Error)
                checkResult = licenseManager.CheckOfflineLicense(licenseManager.DefaultLicensePath);

            switch (checkResult)
            {
                case InSupport_LicenseSystem.LicenseCheckResult.None:
                    lStatusLabel.Text = "No license active. Please activate your license.";
                    lStatusLabel.ForeColor = Color.Red;
                    LockControls();
                    hasValidLicense = false;
                    break;

                case InSupport_LicenseSystem.LicenseCheckResult.Expired:
                    lStatusLabel.Text = "License activation expired. Please renew your license.";
                    lStatusLabel.ForeColor = Color.Orange;
                    hasValidLicense = false;
                    LockControls();
                    break;

                case InSupport_LicenseSystem.LicenseCheckResult.Valid:
                    lStatusLabel.Text = "License activated (" + licenseManager.CurrentActivationType.ToString() + ")";
                    lStatusLabel.ForeColor = Color.Green;
                    hasValidLicense = true;
                    UnlockControls();
                    break;

                case InSupport_LicenseSystem.LicenseCheckResult.Error:
                    lStatusLabel.Text = "Unknown error when fetching license information.";
                    lStatusLabel.ForeColor = Color.Red;
                    hasValidLicense = false;
                    LockControls();
                    break;
            }
        }

        protected void LockControls()
        {
            foreach(Control control in groupBox1.Controls)
            {
                control.Enabled = false;
            }
        }

        protected void UnlockControls()
        {
            foreach (Control control in groupBox1.Controls)
            {
                control.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Yes you're legit!");
        }

        private void activateLicenseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (new LicenseActivation(licenseManager).ShowDialog() == DialogResult.OK)
            {
                CheckLicense();
            }
        }

        private void refreshLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckLicense();
        }

        private void activateLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hasValidLicense)
                activateLicenseToolStripMenuItem1.Enabled = false;
            else
                activateLicenseToolStripMenuItem1.Enabled = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LicenseInfoForm(licenseManager).ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
