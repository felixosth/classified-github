using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SarvAI.Milestone.Verifier
{
    internal class CustomServiceInstaller<T>
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

        public bool InstallService(bool useCustomUserAccount)
        {
            if (IsInstalled()) return false;

            try
            {
                using (AssemblyInstaller installer = GetInstaller(new string[] { $"{Constants.Service.Installer.CustomUserAccountKey}={useCustomUserAccount}" }))
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
