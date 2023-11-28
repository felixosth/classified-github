using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XProtectWebStream
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new XProtectWebStreamService();

            if(args.Length > 0)
            {
                switch(args[0].ToLower())
                {
                    case "-install":
                    case "/install":
                        service.ServiceInstaller.InstallService();
                        break;

                    case "-uninstall":
                    case "/uninstall":
                        service.ServiceInstaller.UninstallService();
                        break;

                    case "-dumpdbpwd":
                    case "/dumpdbpwd":
                        var pw = Database.LocalDatabase.GetMyDbPassword();
                        var tmp = Path.GetTempFileName();
                        File.WriteAllText(tmp, pw);
                        Process.Start("notepad.exe", tmp);
                        Thread.Sleep(1000);
                        File.Delete(tmp);
                        break;
                }
            }
            else if(Debugger.IsAttached)
            {
                service.DebugStart();
            }
            else
            {
                ServiceBase.Run(service);
            }

        }
    }
}
