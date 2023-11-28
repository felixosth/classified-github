using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace WebcamBridge
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : System.Configuration.Install.Installer
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;

        public ServiceInstaller()
        {
            InitializeComponent();

            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;

            this.serviceInstaller1.Description = "Axis PTZ webcam bridge by InSupport Nätverksvideo.";
            this.serviceInstaller1.DisplayName = Constants.ServiceName;
            this.serviceInstaller1.ServiceName = Constants.ServiceName;
            this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.serviceInstaller1});
        }
    }
}
