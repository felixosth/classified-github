using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneToDrive
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log(Config.Instance.LogFile, e.ExceptionObject.ToString());

            GoogleDrive drive;

            if (args.Length > 0 && args[0].ToLower().Contains("setupdrive"))
            {
                Log(Config.Instance.LogFile, "Setting up Google Drive...");

                drive = new GoogleDrive();
                Config.Instance.DriveFolderId = drive.CreateMyDirectory();
                Log(Config.Instance.LogFile, "Uploading to folder " + Config.Instance.DriveFolderId);
                Config.Instance.Save();
            }
            else if(Config.Instance.DriveFolderId != null)
            {
                drive = new GoogleDrive();

                var parsedArgs = ParseArgs(args);

                if (parsedArgs.ContainsKey("cameraId") && parsedArgs.ContainsKey("lastHours"))
                {
                    var lastHours = double.Parse(parsedArgs["lastHours"]);
                    var to = parsedArgs.ContainsKey("to") ? DateTime.Parse(parsedArgs["to"]) : DateTime.Now;

                    var from = to.AddHours(-lastHours);

                    Log(Config.Instance.LogFile, $"Init export job with cameraId {parsedArgs["cameraId"]}, from {from}, to {to}");

                    MilestoneConnection milestoneConnection = new MilestoneConnection();

                    var export = milestoneConnection.GetExportJob(parsedArgs["cameraId"], from, to);
                    export.OnLog += (s, e) => Log(Config.Instance.LogFile, e);
                    export.OnReportProgress += (s, e) => Log(Config.Instance.LogFile, "Export job: " + (!string.IsNullOrWhiteSpace(e.Error) ? e.Error : $"{e.Message} {e.Progress}"));

                    var exportedFile = export.ExportAndConvert();

                    if (exportedFile == null)
                    {
                        Log(Config.Instance.LogFile, "ERROR, exportedFile is null!");
                    }
                    else
                    {
                        var totalBytes = new FileInfo(exportedFile).Length;
                        Action<Google.Apis.Upload.IUploadProgress> uploadProgressAction = (up) => 
                        {
                            Log(Config.Instance.LogFile, "Google Drive: " + up.Status + " " + ((double)up.BytesSent / totalBytes).ToString("0.00%")); 
                        };
                        drive.UploadFile($"{from} till {to}.mp4", exportedFile, uploadProgressAction);
                        Thread.Sleep(500);
                        File.Delete(exportedFile);
                        Log(Config.Instance.LogFile, $"Deleted {exportedFile}.");
                        Log(Config.Instance.LogFile, "Export and upload complete.");
                    }
                }
            }
            else
            {
                Log(Config.Instance.LogFile, "Google Drive is not set up");
            }

        }


        private static void Log(string file, string msg)
        {
            File.AppendAllText(file, $"[{DateTime.Now}] {msg}{Environment.NewLine}");
        }

        static Dictionary<string, string> ParseArgs(string[] args)
        {
            Dictionary<string, string> parsed = new Dictionary<string, string>();

            foreach(var arg in args)
            {
                var split = arg.Split('=');
                if(split.Length == 2)
                {
                    parsed.Add(split[0], split[1]);
                }
            }
            return parsed;
        }
    }
}
