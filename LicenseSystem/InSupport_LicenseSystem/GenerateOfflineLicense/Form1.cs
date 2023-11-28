using System;
using System.Collections.Generic;
//using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using InSupport_LicenseSystem;

namespace GenerateOfflineLicense
{
    public partial class Form1 : Form
    {
        LicenseManager licenseManager;

        public Form1()
        {
            InitializeComponent();
        }

        private void createLicenseBtn_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new SaveFileDialog())
            {
                fileDialog.Filter = "License file | *.lic";
                fileDialog.Title = "Save the license file";
                //fileDialog.DefaultExt = "*.lic";
                fileDialog.FileName = "licenseFile";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    licenseManager = new LicenseManager(mguidTxtBox.Text, productTextBox.Text);
                    //if (licenseManager.CheckOnlineLicense() == LicenseCheckResult.None)
                        if (licenseManager.ActivateLicense(licenseTxtBox.Text) == LicenseActivationResult.Success)
                        {
                            var info = licenseManager.GetLicenseInfo();

                            licenseManager.SaveOfflineLicense(info, fileDialog.FileName);
                            MessageBox.Show("License saved successfully!", "Success");
                            licenseTxtBox.Text = "";
                            mguidTxtBox.Text = "";
                        }
                        else
                            MessageBox.Show("Error activating license.");
                }
            }
        }
    }
}
