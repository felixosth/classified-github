using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace InSupport_HostMonitor_Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            if (bool.Parse(Context.Parameters["customuseraccount"]))
            {
                serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.User;
                serviceProcessInstaller1.Username = null;
                serviceProcessInstaller1.Password = null;
            }
            else if (Context.Parameters.ContainsKey("username") && Context.Parameters.ContainsKey("password"))
            {
                serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.User;
                serviceProcessInstaller1.Username = Context.Parameters["username"];
                serviceProcessInstaller1.Password = Context.Parameters["password"];
            }

            int instanceId = -1;
            if (Context.Parameters.ContainsKey("instanceId"))
            {
                if (int.TryParse(Context.Parameters["instanceId"], out instanceId))
                {
                    if (instanceId > -1)
                    {
                        serviceInstaller1.ServiceName += $"-{instanceId}";
                        serviceInstaller1.DisplayName += $"-{instanceId}";
                        serviceInstaller1.Description += $" Instance {instanceId}.";
                    }
                }
            }

            serviceInstaller1.DelayedAutoStart = true;
            Context.Parameters["assemblypath"] = Enquote(Context.Parameters["assemblypath"]) + $" {Enquote("/run")}" + (instanceId > -1 ? $" {Enquote($"/instance {instanceId}")}" : "");

            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            if (Context.Parameters.ContainsKey("name"))
            {
                serviceInstaller1.ServiceName = Context.Parameters["name"];
            }

            base.Uninstall(savedState);
        }

        string Enquote(string text)
        {
            return "\"" + text + "\"";
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
