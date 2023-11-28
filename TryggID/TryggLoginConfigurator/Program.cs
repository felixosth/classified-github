using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TryggLoginConfigurator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            VideoOS.Platform.SDK.Environment.Initialize();

            VideoOS.Platform.SDK.UI.LoginDialog.DialogLoginForm dialogLoginForm = new VideoOS.Platform.SDK.UI.LoginDialog.DialogLoginForm(SetConnected);
            Application.Run(dialogLoginForm);

            if (isConnected)
                Application.Run(new Form1());


        }
        static bool isConnected = false;
        static void SetConnected(bool status)
        {
            isConnected = status;
        }
    }
}
