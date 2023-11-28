using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.IO.Compression;
using System.Reflection;
using System.Threading;

namespace TryggSTORE.Updater
{
    class Program
    {
        protected const string fileToUpdate = "..\\TryggSTORE.Service.exe";
        protected const string serviceName = "TryggSTORE";
        protected const string appBranch = "71B744BD-F56F-4F0C-AC24-3696B799BEE9";
        protected const string downloadKey = "55EDCECC-5250-4E04-A92D-993535704574";
        protected const string url = "https://portal.tryggconnect.se/api/updater/updater.php";

        protected static bool serviceStart = false;

        static void Main(string[] args)
        {
            Console.WriteLine("This is a update utillity for {0}.", serviceName);
            Console.WriteLine("-------------------------------------------------");

            if(!File.Exists(fileToUpdate))
            {
                Console.WriteLine("Couldn't find the file to update :(");
                Console.ReadKey();
                Environment.Exit(0);
            }

            var fileVerInfo = FileVersionInfo.GetVersionInfo(fileToUpdate);
            Console.WriteLine("Installed version: {0}", fileVerInfo.ProductVersion);

            Console.WriteLine("Checking for new version...");
            var appInfo = GetAppInfo();

            Console.WriteLine("Current version: {0}", appInfo.Version);

            var verComparison = appInfo.Version.CompareTo(new Version(fileVerInfo.ProductVersion));

            if(verComparison > 0)
            {
                Console.Write("Do you want to update the application to {0}? (yes/no): ", appInfo.Version);
                var userInput = Console.ReadLine().ToLower();
                if(userInput == "yes" || userInput == "y")
                {
                    Console.WriteLine("Downloading new files...");
                    var archivePath = DownloadAppArchive();

                    Console.WriteLine("Stopping service...");
                    StopService();

                    Console.WriteLine("Unpacking files...");
                    UnzipArchive(archivePath, Path.GetDirectoryName(Path.GetFullPath(fileToUpdate)));

                    if(serviceStart)
                    {
                        Console.WriteLine("Starting service...");
                        StartService();
                    }

                    Console.WriteLine("Cleaning up...");
                    File.Delete(archivePath);

                    Console.WriteLine("Done!");
                }
                else
                {
                    Console.WriteLine("Update aborted");
                }
            }
            else
            {
                Console.WriteLine("No update available");
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static AppInfo GetAppInfo()
        {
            string parameters = string.Format("?action=getVersion&branch={0}", appBranch);
            using (var wc = new WebClient())
            {
                string data = wc.DownloadString(url + parameters);
                return JsonConvert.DeserializeObject<AppInfo>(data);
            }
        }

        static string DownloadAppArchive()
        {
            string parameters = string.Format("?action=getFile&branch={0}&downloadKey={1}", appBranch, downloadKey);
            string tmpFilePath = Path.GetTempFileName();
            using (var wc = new WebClient())
            {
                try
                {
                    wc.DownloadFile(url + parameters, tmpFilePath);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error downloading file: {0}", e.Message);
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            return tmpFilePath;
        }

        static void UnzipArchive(string zipPath, string destinationPath)
        {
            using(var zipFileStream = new FileStream(zipPath,FileMode.Open, FileAccess.Read))
            {
                var zip = new ZipArchive(zipFileStream);
                foreach (ZipArchiveEntry file in zip.Entries)
                {
                    string fileDestinationPath = Path.Combine(destinationPath, file.FullName);

                    if (System.IO.Path.GetFileName(fileDestinationPath).Length == 0) // It's a directory
                    {
                        if (file.Length != 0)
                            throw new IOException("Directory entry with data.");

                        Directory.CreateDirectory(fileDestinationPath);
                    }
                    else // It's a file
                    {
                        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileDestinationPath));
                        Console.WriteLine("Extracting \"{0}\"...", file.FullName);
                        file.ExtractToFile(fileDestinationPath, overwrite: true);
                    }
                }
            }
        }

        static void StopService()
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                service.Stop();
                Thread.Sleep(1000);
            }
            catch(InvalidOperationException)
            {
                return;
            }
            var timeout = new TimeSpan(0, 0, 5); // 5seconds

            try
            {
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch(System.ServiceProcess.TimeoutException)
            {
                foreach (var process in Process.GetProcessesByName(fileToUpdate))
                {
                    process.Kill();
                }
            }

            serviceStart = true;
        }
        static void StartService()
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                service.Start();
            }
            catch(InvalidOperationException)
            {
            }
        }
    }

    class AppInfo
    {
        [JsonProperty("version")]
        public Version Version { get; set; }
        [JsonProperty("productName")]
        public string ProductName { get; set; }
    }

}

