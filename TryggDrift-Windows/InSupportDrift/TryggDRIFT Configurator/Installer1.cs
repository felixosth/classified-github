using InSupport.Drift.Service;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Threading;

namespace TryggDRIFT_Configurator
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        private void installer2_AfterInstall(object sender, InstallEventArgs e)
        {
            //Process.Start(new ProcessStartInfo(Assembly.GetExecutingAssembly().Location) { UseShellExecute = false });
        }

        private void installer2_BeforeUninstall(object sender, InstallEventArgs e)
        {
            if (Directory.Exists(Path.Combine(App.MyDir, ConfiguratorWindow._SERVICE_PATH))) // Cleanup
            {
                var serviceInstaller = new CustomServiceInstaller(ConfiguratorWindow._SERVICE_NAME, Path.Combine(App.MyDir, ConfiguratorWindow._SERVICE_PATH, ConfiguratorWindow._SERVICE_EXE));
                serviceInstaller.StopService();
                serviceInstaller.UninstallService();

                if (File.Exists(Path.Combine(App.MyDir, PluginConfigCache._FILENAME)))
                    File.Delete(Path.Combine(App.MyDir, PluginConfigCache._FILENAME));

                if (File.Exists(Path.Combine(App.MyDir, TryggDRIFTInstallCache.FileName)))
                    File.Delete(Path.Combine(App.MyDir, TryggDRIFTInstallCache.FileName));

                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        Directory.Delete(Path.Combine(App.MyDir, ConfiguratorWindow._SERVICE_PATH), true);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(250);
                    }
                }
            }
        }
    }
}
