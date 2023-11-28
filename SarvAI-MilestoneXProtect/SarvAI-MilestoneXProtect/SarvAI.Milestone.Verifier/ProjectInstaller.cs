using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace SarvAI.Milestone.Verifier
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            this.serviceInstaller.DisplayName = Constants.Service.ServiceDisplayName;
            this.serviceInstaller.ServiceName = Constants.Service.ServiceName;
        }

        public override void Install(IDictionary stateSaver)
        {
            if(Context.Parameters.ContainsKey(Constants.Service.Installer.CustomUserAccountKey) && 
                bool.Parse(Context.Parameters[Constants.Service.Installer.CustomUserAccountKey]) == true)
            {
                // Will prompt user for login
                serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.User;
                serviceProcessInstaller.Username = null;
                serviceProcessInstaller.Password = null;
            }


            base.Install(stateSaver);
        }

        private void installer_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
