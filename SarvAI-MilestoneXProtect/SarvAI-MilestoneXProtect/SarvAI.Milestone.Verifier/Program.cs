using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SarvAI.Milestone.Verifier
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            CustomServiceInstaller<SarvAIService> customServiceInstaller = new CustomServiceInstaller<SarvAIService>(Constants.Service.ServiceName);
            var service = new SarvAIService();

            if (args.Length > 0)
            {
                if(args[0] == "/install")
                {
                    customServiceInstaller.InstallService(useCustomUserAccount: true);
                }
                else if(args[0] == "/uninstall")
                {
                    customServiceInstaller.UninstallService();
                }
            }
            else if (System.Diagnostics.Debugger.IsAttached)
            {
                service.DebugStart();
                while (true)
                {
                    Thread.Sleep(100);
                }
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }
}
