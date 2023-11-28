using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows;

namespace WinservLite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unexpected exception has occurred. Shutting down the application. Please check the log file for more details.");

            var path = Directory.GetCurrentDirectory() + "\\log.txt";
            File.AppendAllText(path, e.Exception.ToString());

            Process.Start(path);
            // Prevent default unhandled exception processing
            e.Handled = true;

            Environment.Exit(0);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var main = new MainWindow();

            bool showMain = true;

            foreach (string arg in e.Args)
            {
                if (arg.StartsWith("wslite:"))
                {
                    //Debugger.Launch();
                    var jobId = int.Parse(arg.Split(':')[1]);
                    //main.RefreshJobList(specific: jobId.ToString());

                    foreach (var job in main.GetJobList())
                    {
                        if (job.JobID == jobId)
                        {
                            var dj = new DisplayJob(job, main);
                            dj.Closing += new System.ComponentModel.CancelEventHandler((s, ce) => Environment.Exit(0));
                            dj.Show();
                            showMain = false;
                        }
                    }
                }
            }


            //if (!WinservLite.Properties.Settings.Default.UseMagnet && showMain && !WinservLite.Properties.Settings.Default.SkipMagnet)
            //{
            //    if (MessageBox.Show("Do you wish to enable magnet-links?", "Magnet links", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            //    {
            //        var appDir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //        var appPath = Path.Combine(appDir, "WinServLite.exe");

            //        //FixRegistry();
            //        var psi = new ProcessStartInfo();
            //        psi.FileName = Path.Combine(appDir, "Resources\\WSLiteRegistryFix.exe");
            //        psi.Arguments = "\"" + appPath + "\"";
            //        psi.Verb = "runas";

            //        var process = new Process();
            //        process.StartInfo = psi;

            //        try
            //        {
            //            process.Start();
            //            process.WaitForExit();
            //        }
            //        catch
            //        {
            //            MessageBox.Show("Could not open Magnetfixed");
            //        }

            //        WinservLite.Properties.Settings.Default.UseMagnet = true;
            //        WinservLite.Properties.Settings.Default.Save();
            //    }
            //    else
            //    {
            //        WinservLite.Properties.Settings.Default.SkipMagnet = true;
            //        WinservLite.Properties.Settings.Default.Save();
            //    }
            //}



            if(showMain)
                main.Show();
        }


        [PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void FixRegistry()
        {
            var appDir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            var appPath = Path.Combine(appDir, "WinServLite.exe");

            var root = Registry.ClassesRoot.CreateSubKey("wslite");
            root.SetValue("", "\"URL:WinservLite Protocol\"");
            root.SetValue("URL Protocol", "\"\"");

            root.CreateSubKey("DefaultIcon").SetValue("", "\"WinServLite.exe,1\"");

            var cmd = root.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
            cmd.SetValue("", "\"" + appPath + "\" \"%1\"");
        }

        private void Dj_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}
