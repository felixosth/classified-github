using InSupport;
using InSupport.Drift.Monitor;
using InSupport.Drift.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace InSupport_HostMonitor_Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var argsList = args.ToList();
            InSupportDriftService service = service = new InSupportDriftService(-1);
            //var installer = new CustomInstaller("TryggDrift");
            //installer.serv

            if (args.Length > 0)
            {
                var instanceArg = args.FirstOrDefault(a => a.StartsWith("/instance"));
                int instanceId = -1;
                if (instanceArg != null)
                {
                    instanceId = int.Parse(instanceArg.Split(' ')[1]);
                }
                service = new InSupportDriftService(instanceId);

                switch (args[0])
                {
                    case "/run":
                        ServiceBase.Run(service);
                        break;

                    //case "/install":
                    //    installer.InstallService();
                    //    break;

                    case "/uninstall":

                        var services = GetTryggDriftServices(instanceId);

                        foreach (var serviceName in services)
                        {
                            Console.WriteLine("Uninstalling {0}...", serviceName);
                            var installer = new CustomServiceInstaller(serviceName, Assembly.GetExecutingAssembly().Location);
                            installer.StopService();
                            installer.UninstallService();
                        }
                        Console.WriteLine("Done!");

                        break;

                    case "/cfg":
                        Process.Start("explorer", Path.GetDirectoryName(Cfg.GetPath()));
                        break;

                    case "/checkplugins":
                        CheckPlugins();
                        break;
                    case "/debug":



                        Debugger.Launch();
                        service.DebugStart(null);

                        break;
                }
            }
            else if (Debugger.IsAttached)
            {
                service.DebugStart(null);
                while (service.Running)
                {
                    Thread.Sleep(1000);
                }
            }
            //else
            //    ServiceBase.Run(service);
        }

        static string[] GetTryggDriftServices(int instanceId)
        {
            return ServiceController.GetServices().Where(sc => instanceId > -1 ? sc.ServiceName.StartsWith("TryggDrift") && sc.ServiceName.EndsWith("-" + instanceId) : sc.ServiceName.StartsWith("TryggDrift")).Select(sc => sc.ServiceName).ToArray();
        }

        static void CheckPlugins()
        {
            List<string> plugins = new List<string>();
            var dir = AppDomain.CurrentDomain.BaseDirectory + "plugins";
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(file);
                        var bmType = typeof(BaseMonitor);
                        foreach (var type in assembly.DefinedTypes)
                        {
                            if (type.IsSubclassOf(bmType))
                            {
                                var plugin = Activator.CreateInstance(type, new object[] { new Dictionary<string, string>() }) as BaseMonitor;
                                if (plugin != null)
                                {
                                    plugins.Add(plugin.MonitorName);
                                }
                            }
                        }
                    }
                    catch { }
                }

            }
            File.WriteAllLines("plugins.txt", plugins);
            Process.Start("plugins.txt");
        }
    }
}
