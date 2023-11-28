using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace WinServLite2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Graph.GraphAPI.Init();

            if(new Config(WinServLite2.MainWindow.CompanyName).LoadConfig() == null)
            {
                new FirstTimeSetupWindow().Show();
            }
            else
            {
                new MainWindow().Show();
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unexpected exception has occurred. Shutting down the application. Please check the log file for more details.");

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), WinServLite2.MainWindow.CompanyName, Assembly.GetExecutingAssembly().GetName().Name);
            var file = path + "\\error.txt";
            File.WriteAllText(file, e.Exception.ToString());

            Process.Start(file);
        }
    }
}
