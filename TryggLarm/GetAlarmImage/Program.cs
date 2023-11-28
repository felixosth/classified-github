using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Data;

namespace GetAlarmImage
{
    static class Program
    {
        static string ownFolder;
        static string key;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main(string[] args)
        {
            string extraComments = "";
            extraComments += Application.StartupPath;
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            ownFolder = Path.Combine(appData, "TryggLarm.GetAlarmImage");
            Directory.CreateDirectory(ownFolder);
            try
            {
                if (args.Length < 1)
                    return;
                VideoOS.Platform.SDK.Environment.Initialize();
                VideoOS.Platform.SDK.Environment.AddServer(new Uri("http://localhost"), CredentialCache.DefaultNetworkCredentials);
                //VideoOS.Platform.SDK.Export.Environment.Initialize();

                //extraComments += string.Join(",", args);
                key = args[2];

                var items = new List<Item>();
                foreach (string line in File.ReadAllLines(args[1]))
                {
                    items.Add(Item.Deserialize(line));
                }

                for (int i = 0; i < items.Count; i++)
                {
                    GetAndSaveImage(items[i], DateTime.Parse(args[0]), i);
                    Console.WriteLine("Saved " + i + ".jpeg");
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(ownFolder, "logs.txt"), extraComments + "\r\n\r\n" + ex.ToString());
            }
        }

        static void GetAndSaveImage(Item camera, DateTime alarmDate, int i)
        {
            JPEGVideoSource videoSource = new JPEGVideoSource(camera);
            videoSource.Init();
            videoSource.AllowUpscaling = true;

            Thread.Sleep(2000);
            JPEGData data = videoSource.GetNearest(alarmDate) as JPEGData;
            
            if (data == null)
                return;

            var path = Path.Combine(ownFolder, key + (i + 1) + ".jpeg");
            if (File.Exists(path))
                File.Delete(path);
            Console.WriteLine(path);

            MemoryStream ms = new MemoryStream(data.Bytes);
            ms.Position = 0;
            Image.FromStream(ms).Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
