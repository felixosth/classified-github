using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform.SDK.UI.LoginDialog;

namespace TryggRetail_Configurator
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
            VideoOS.Platform.EnvironmentManager.Instance.EnvironmentOptions["UsePing"] = "No";

            DialogLoginForm loginForm = new DialogLoginForm(SetLoginResult);
            //loginForm.ServerConfiguration = DialogLoginForm.ServerConfigurationEnum.MasterAndSlaves;
            loginForm.ServerConfiguration = DialogLoginForm.ServerConfigurationEnum.AllServersIndividually;
            //loginForm.ServerConfiguration = DialogLoginForm.ServerConfigurationEnum.MasterOnly;
            VideoOS.Platform.SDK.Environment.Properties.ConfigurationRefreshIntervalInMs = 5000;
            Application.Run(loginForm);

            if (Connected)
            {
                Application.Run(new ConfigForm());
            }
        }


        private static bool Connected = false;
        private static void SetLoginResult(bool connected)
        {
            Connected = connected;
        }
    }
}
