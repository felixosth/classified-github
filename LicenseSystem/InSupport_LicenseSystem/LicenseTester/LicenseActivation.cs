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
    public partial class LicenseActivation : Form
    {
        InSupport_LicenseSystem.LicenseManager licenseManager;
        public LicenseActivation(InSupport_LicenseSystem.LicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch(licenseManager.ActivateLicense(keyTxtBox.Text))
            {
                case InSupport_LicenseSystem.LicenseActivationResult.Success:
                    MessageBox.Show("License activated.", "Success!");
                    break;
                case InSupport_LicenseSystem.LicenseActivationResult.InUse:
                    MessageBox.Show("License is already in use.", "Error");
                    break;
                case InSupport_LicenseSystem.LicenseActivationResult.NoLicenseExist:
                    MessageBox.Show("No existing license with that key exists.", "Error");
                    break;
                case InSupport_LicenseSystem.LicenseActivationResult.Expired:
                    MessageBox.Show("The license you tried to activate has expired and is no longer valid.", "Error");
                    break;
            }


            DialogResult = DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "License file | *.lic";

                if(fileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (licenseManager.CheckOfflineLicense(fileDialog.FileName) == InSupport_LicenseSystem.LicenseCheckResult.Valid)
                    {
                        //string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        //string myFolder = Path.Combine(appData, "InSupport.LicenseTest");
                        //Directory.CreateDirectory(myFolder);
                        //string finalFileName = Path.Combine(myFolder, "license.lic");
                        if(File.Exists(licenseManager.DefaultLicensePath))
                        {
                            if (MessageBox.Show("There's already a license file. Overwrite?", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                File.Delete(licenseManager.DefaultLicensePath);
                            }
                            else
                                return;
                        }
                        File.Copy(fileDialog.FileName, licenseManager.DefaultLicensePath);
                        MessageBox.Show("Offline activation successful.", "Success");
                        DialogResult = DialogResult.OK;
                    }
                    else
                        MessageBox.Show("License invalid");
                }
            }
        }
    }
}
