using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluginUpdater
{
    class Program
    {
        static WebClient webClient = new WebClient();

        static string appStartup = "";


        static void Main(string[] args)
        {
            if(args.Length > 0)
                appStartup = args[0];
            Console.WriteLine("© 2017 InSupport Nätverksvideo AB");
            Console.Title = "InSupport Plugin Updater";
            //Process.Start("cmd.exe", "/c taskkill /f /im Client.exe");
            //Process.Start("cmd.exe", "/c taskkill /f /im VideoOS.Event.Server.exe");
            KillProcess("Client");
            KillProcess("VideoOS.Event.Server");
            Thread.Sleep(500);


            //var dllFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"InSupport.dll");
            //File.Delete(dllFile);

            Console.WriteLine("Starting download");
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;


            var zipFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "RELEASE.zip");
            webClient.DownloadFileAsync(new Uri("http://83.233.164.117/plugin/RELEASE.zip"), zipFile);

            //webClient.DownloadFileAsync(new Uri("http://83.233.164.117/plugin/InSupport.dll"), dllFile); // if dll

            while (true) { }
        }

        private static void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage + "%");
        }

        private static void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //Console.WriteLine(appStartup);
            Console.WriteLine("Done!");
            //Process.Start(@"C:\Program Files\Milestone\XProtect Event Server\VideoOS.Event.Server.exe");

            PerformFolderCleanup();

            UnzipFiles();

            if(appStartup != "")
                Process.Start(appStartup);
            Environment.Exit(0);
        }


        static void UnzipFiles()
        {
            var myPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var theZip = Path.Combine(myPath, "RELEASE.zip");
            try
            {
                ZipFile.ExtractToDirectory(theZip, myPath);
                File.Delete(theZip);
            }
            catch
            {
                Console.WriteLine("Extrahering av .zip-filen misslyckades. Kontakta InSupport på support@insupport.se");
                //Console.ReadLine();
            }
        }

        static void PerformFolderCleanup()
        {
            var files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*", SearchOption.AllDirectories);

            foreach(string file in files)
            {
                var fileName = Path.GetFileName(file);

                switch(fileName)
                {
                    //case "plugin.def":
                    case "RELEASE.zip":
                    case "PluginUpdater.exe":
                        continue;

                    default:
                        Console.WriteLine("Deleting " + fileName);
                        File.Delete(file);
                        break;

                }
            }
        }


        static void KillProcess(string name)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(name);
                if(proc[0] != null)
                    proc[0].Kill();
            }
            catch { }
        }
    }
}
