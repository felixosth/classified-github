using InSupport_LicenseSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicenseTester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //string myGuid = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null) as string;
            //LicenseManager licenseManager = new LicenseManager(myGuid);

            //if(licenseManager.CheckForLicense() == LicenseCheckResult.None)
            //    if(licenseManager.ActivateLicense("a66996b3-bfd2-41f4-ad87-06f594b612a8") != LicenseActivationResult.Success)
            //        return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
