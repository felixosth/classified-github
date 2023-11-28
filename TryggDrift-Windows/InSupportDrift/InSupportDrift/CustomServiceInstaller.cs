using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;

namespace InSupport.Drift.Service
{
    public class CustomServiceInstaller
    {
        string serviceName;
        public string ExePath { get; private set; }
        public bool CustomUserAccount { get; set; }
        string username;
        string password;
        int instanceId;

        public CustomServiceInstaller(string serviceName, string path, bool userAccount = false, string username = null, string password = null, int instanceId = -1)
        {
            this.CustomUserAccount = userAccount;
            this.serviceName = serviceName;
            this.ExePath = path;
            this.username = username;
            this.password = password;
            this.instanceId = instanceId;
        }

        public bool IsInstalled()
        {
            using (ServiceController controller =
                new ServiceController(serviceName))
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
            using (ServiceController controller =
                new ServiceController(serviceName))
            {
                if (!IsInstalled()) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        private AssemblyInstaller GetInstaller(string path)
        {
            if (username != null && password != null)
                return new AssemblyInstaller(path, new string[]
                    {
                        "customuseraccount= " + CustomUserAccount,
                        "username=" + username,
                        "password=" + password,
                        "instanceId=" + instanceId
                    });
            else
                return new AssemblyInstaller(path, new string[]
                    { "customuseraccount= " + CustomUserAccount, "instanceId=" + instanceId });
        }

        public bool InstallService(out Exception ex)
        {
            ex = null;
            if (IsInstalled()) return false;

            try
            {
                using (AssemblyInstaller installer = GetInstaller(this.ExePath))
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch (Exception _ex)
                    {
                        ex = _ex;
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch { }

                        return false;
                    }
                }
            }
            catch (Exception _ex)
            {
                ex = _ex;
                return false;
            }
            return true;
        }

        public void UninstallService()
        {
            if (!IsInstalled()) return;
            try
            {
                using (AssemblyInstaller installer = GetInstaller(this.ExePath))
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

        public bool StartService()
        {
            if (!IsInstalled()) return false;

            using (ServiceController controller =
                new ServiceController(serviceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(20));
                    }
                    return true;
                }
                catch
                {
                    throw;
                }
            }
        }

        public bool StopService()
        {
            if (!IsInstalled()) return false;
            using (ServiceController controller =
                new ServiceController(serviceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped,
                             TimeSpan.FromSeconds(20));
                    }
                    return true;
                }
                catch
                {
                    throw;
                }
            }
        }
    }

    //internal class CustomInstaller
    //{
    //    public string ServiceName { get; set; }

    //    public CustomInstaller(string name)
    //    {
    //        this.ServiceName = name;
    //    }

    //    public bool IsInstalled()
    //    {
    //        using (ServiceController controller =
    //            new ServiceController(ServiceName))
    //        {
    //            try
    //            {
    //                ServiceControllerStatus status = controller.Status;
    //            }
    //            catch
    //            {
    //                return false;
    //            }
    //            return true;
    //        }
    //    }

    //    public bool IsRunning()
    //    {
    //        using (ServiceController controller =
    //            new ServiceController(ServiceName))
    //        {
    //            if (!IsInstalled()) return false;
    //            return (controller.Status == ServiceControllerStatus.Running);
    //        }
    //    }

    //    private AssemblyInstaller GetInstaller()
    //    {
    //        AssemblyInstaller installer = new AssemblyInstaller(
    //            typeof(InSupportDriftService).Assembly, new string[] { "name=" + ServiceName });
    //        installer.UseNewContext = true;
    //        return installer;
    //    }

    //    public bool InstallService()
    //    {
    //        if (IsInstalled()) return false;

    //        try
    //        {
    //            using (AssemblyInstaller installer = GetInstaller())
    //            {
    //                IDictionary state = new Hashtable();
    //                try
    //                {
    //                    installer.Install(state);
    //                    installer.Commit(state);
    //                }
    //                catch
    //                {
    //                    try
    //                    {
    //                        installer.Rollback(state);
    //                    }
    //                    catch { }

    //                    return false;
    //                }
    //            }
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //        return true;
    //    }

    //    public void UninstallService()
    //    {
    //        if (!IsInstalled()) return;
    //        try
    //        {
    //            using (AssemblyInstaller installer = GetInstaller())
    //            {
    //                IDictionary state = new Hashtable();
    //                try
    //                {
    //                    installer.Uninstall(state);
    //                }
    //                catch
    //                {
    //                    throw;
    //                }
    //            }
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //    }

    //    public void StartService()
    //    {
    //        if (!IsInstalled()) return;

    //        using (ServiceController controller =
    //            new ServiceController(ServiceName))
    //        {
    //            try
    //            {
    //                if (controller.Status != ServiceControllerStatus.Running)
    //                {
    //                    controller.Start();
    //                    controller.WaitForStatus(ServiceControllerStatus.Running,
    //                        TimeSpan.FromSeconds(10));
    //                }
    //            }
    //            catch
    //            {
    //                throw;
    //            }
    //        }
    //    }

    //    public void StopService()
    //    {
    //        if (!IsInstalled()) return;
    //        using (ServiceController controller =
    //            new ServiceController(ServiceName))
    //        {
    //            try
    //            {
    //                if (controller.Status != ServiceControllerStatus.Stopped)
    //                {
    //                    controller.Stop();
    //                    controller.WaitForStatus(ServiceControllerStatus.Stopped,
    //                         TimeSpan.FromSeconds(10));
    //                }
    //            }
    //            catch
    //            {
    //                throw;
    //            }
    //        }
    //    }
    //}
}
