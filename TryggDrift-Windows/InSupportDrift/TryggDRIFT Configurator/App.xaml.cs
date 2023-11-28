using InSupport.Drift.Service;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace TryggDRIFT_Configurator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string MyDir { get; private set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            if (e.Args.Length == 6 && e.Args[0] == "-plugin")
            {
                // args = "-plugin" "{0}" "{1}" "{2}" "{3}" "{4}"", package.ID, package.UnzippedPath, this.Left + this.Width/2, this.Top + this.Height/2, Debugger.IsAttached
                var pluginConfig = new PluginConfigWindow(e.Args[2], int.Parse(e.Args[1]));
                pluginConfig.Left = double.Parse(e.Args[3]) - pluginConfig.Width / 2;
                pluginConfig.Top = double.Parse(e.Args[4]) - pluginConfig.Height / 2;

                if (bool.Parse(e.Args[5])) // Debugger.IsAttached
                    Debugger.Launch();

                pluginConfig.Show();
            }
            else if (e.Args.Length == 1 && e.Args[0] == "-scan")
            {
                var loader = new PluginLoader();

                if (Directory.Exists(Path.Combine(MyDir, ConfiguratorWindow._SERVICE_PATH)))
                {
                    TryggDRIFTInstallCache installCache = TryggDRIFTInstallCache.OpenOrCreate();

                    var files = Directory.EnumerateFiles(Path.Combine(MyDir, ConfiguratorWindow._SERVICE_PATH), "*.*", SearchOption.AllDirectories)
                        .Where(file => file.EndsWith(".exe") || file.EndsWith(".dll"));

                    foreach (var file in files)
                    {
                        try
                        {
                            var monitor = loader.GetObjectOfParent<InSupport.Drift.Monitor.BaseMonitor>(file);
                            var fileDir = Path.GetFullPath(Path.GetDirectoryName(file));

                            if (monitor != null)
                            {
                                if (installCache.Packages.FirstOrDefault(p => Path.GetFullPath(p.Path) == fileDir) == null)
                                {
                                    installCache.Packages.Add(new TryggDRIFTInstalledPackage()
                                    {
                                        ID = StringToInt(monitor.MonitorName),
                                        Name = monitor.MonitorName,
                                        Path = Path.GetDirectoryName(file),
                                        Version = monitor.MonitorVersion.ToString().Replace(',', '.')
                                    });
                                }
                            }
                        }
                        catch { }
                    }

                    installCache.Save();
                }
                Environment.Exit(0);
            }
            else if (e.Args.Length == 1 && e.Args[0] == "-uninstall")
            {
                var serviceInstaller = new CustomServiceInstaller("TryggDrift", Path.Combine(MyDir, ConfiguratorWindow._SERVICE_PATH, "TryggDrift.exe"));
                serviceInstaller.UninstallService();
                Environment.Exit(0);
            }
            else
            {
                var procs = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

                if (procs.Length > 1)
                {
                    MessageBox.Show("The configurator is already running!");
                    return;
                }

                var main = new ConfiguratorWindow();
                main.Show();
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Newtonsoft"))
            {
                return Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Newtonsoft.Json.dll"));
            }
            return null;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }

        static int StringToInt(string text)
        {
            using (MD5 md5Hasher = MD5.Create())
                return BitConverter.ToInt32(md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(text)), 0);
        }
    }
}
