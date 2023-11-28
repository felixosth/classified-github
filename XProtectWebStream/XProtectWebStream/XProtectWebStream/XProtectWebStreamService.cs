using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using XProtectWebStream.Web;
using XProtectWebStream.Global;

namespace XProtectWebStream
{
    public class XProtectWebStreamService : ServiceBase
    {
        internal const string _ServiceName = "XProtectWebStream";
        WebServer webServer;

        public XProtectWebStreamService()
        {
            this.ServiceName = _ServiceName;

            this.EventLog.Source = this.ServiceName;
            this.EventLog.Log = "Application";

            try
            {
                ((ISupportInitialize)(this.EventLog)).BeginInit();
                if (!EventLog.SourceExists(this.EventLog.Source))
                {
                    EventLog.CreateEventSource(this.EventLog.Source, this.EventLog.Log);
                }
                ((ISupportInitialize)(this.EventLog)).EndInit();
            }
            catch
            {
            }
        }

        protected override void OnStart(string[] args)
        {
            if (string.IsNullOrWhiteSpace(Config.Instance.LicenseKey))
            {
                Log("Please enter a valid license key in the config file", EventLogEntryType.Error);
                base.ExitCode = 1;
                base.Stop();
            }
            else
            {
                LicenseCheckResponse check = null;

                try
                {
                    LicenseSystem.LicenseKey = Config.Instance.LicenseKey;
                    check = LicenseSystem.CheckLicense();
                }
                catch(Exception ex)
                {
                    Log("Error checking license:\r\n" + ex.ToString(), EventLogEntryType.Error);
                    base.ExitCode = 5;
                    base.Stop();
                    return;
                }

                if (check.error == null && check.license != null)
                {
                    LicenseSystem.StartCheckThread();

                    webServer = new WebServer();
                    webServer.OnLog += Ws_OnLog; ;
                    webServer.Start();
                }
                else if (check.error != null)
                {
                    Log("License check: " + check.error, EventLogEntryType.Error);
                    base.ExitCode = 2;
                    base.Stop();
                }
                else
                {
                    Log("No valid license found, please contact your supplier for a key.", EventLogEntryType.Error);
                    base.ExitCode = 3;
                    base.Stop();
                }
            }
        }

        private void Ws_OnLog(object sender, string e)
        {
            Log(e);
        }

        internal void Log(string msg, EventLogEntryType logType = EventLogEntryType.Information, int eventId = 0)
        {
            try
            {
                this.EventLog.WriteEntry(msg, logType, eventId);
            }
            catch { }
        }

        protected override void OnStop()
        {
            webServer?.Close();
            base.OnStop();
        }

        internal void DebugStart()
        {
            OnStart(null);
        }

        private CustomServiceInstaller<XProtectWebStreamService> serviceInstaller;
        internal CustomServiceInstaller<XProtectWebStreamService> ServiceInstaller
        {
            get
            {
                if (serviceInstaller == null)
                    serviceInstaller = new CustomServiceInstaller<XProtectWebStreamService>(_ServiceName);
                return serviceInstaller;
            }
        }
    }

    [RunInstaller(true)]
    public class XProtectWebStreamServiceInstaller : Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public XProtectWebStreamServiceInstaller()
        {
            this.serviceInstaller = new ServiceInstaller();
            this.serviceInstaller.DelayedAutoStart = true;
            this.serviceInstaller.Description = "Prototype by InSupport Nätverksvideo AB";
            this.serviceInstaller.DisplayName = XProtectWebStreamService._ServiceName;
            this.serviceInstaller.ServiceName = XProtectWebStreamService._ServiceName;
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.User;
            processInstaller.Username = null;
            processInstaller.Password = null;

            this.Installers.AddRange(new Installer[]
            {
                serviceInstaller, processInstaller
            });
        }
    }

    internal class CustomServiceInstaller<T> where T : ServiceBase
    {
        public string ServiceName { get; set; }

        public CustomServiceInstaller(string name)
        {
            this.ServiceName = name;
        }

        public bool IsInstalled()
        {
            using (ServiceController controller = new ServiceController(ServiceName))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsRunning()
        {
            using (ServiceController controller = new ServiceController(ServiceName))
            {
                if (!IsInstalled()) return false;
                return controller.Status == ServiceControllerStatus.Running;
            }
        }

        private AssemblyInstaller GetInstaller(string[] args = null)
        {
            AssemblyInstaller installer = new AssemblyInstaller(typeof(T).Assembly, args);
            installer.UseNewContext = true;
            return installer;
        }

        public bool InstallService()
        {
            if (IsInstalled()) return false;

            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch { }

                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void UninstallService()
        {
            if (!IsInstalled()) return;
            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Uninstall(state);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void StartService()
        {
            if (!IsInstalled()) return;

            using (ServiceController controller =
                new ServiceController(ServiceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running,
                            TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public void StopService()
        {
            if (!IsInstalled()) return;
            using (ServiceController controller =
                new ServiceController(ServiceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped,
                             TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
