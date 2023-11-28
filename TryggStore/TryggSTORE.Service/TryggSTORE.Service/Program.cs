using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TryggSTORE.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new TryggSTOREService()
            //};
            //ServiceBase.Run(ServicesToRun);

            TryggSTOREService service = new TryggSTOREService();

            if (args.Length > 0)
            {
                var installer = new CustomInstaller<TryggSTOREService>(Constants.ServiceName);

                switch (args[0])
                {
                    case "/install":
                        if (installer.InstallService())
                        {
                            Console.WriteLine("Service installed.");
                        }
                        else
                        {
                            Console.WriteLine("Error installing service.");
                        }
                        break;
                    case "/uninstall":
                        installer.StopService();
                        installer.UninstallService();
                        break;
                }
            }
            else if (Debugger.IsAttached)
            {
                service.Debug();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }
}
