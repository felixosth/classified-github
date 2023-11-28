using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Global
{
    public class Config
    {
        public string MilestoneServer { get; set; }

        public int Port { get; set; }
        public string Binding { get; set; }
        public bool UseHttps { get; set; }

        public string Server { get; set; }

        public string LicenseKey { get; set; }

        public CameraConfig Camera { get; set; }

        public bool WaitForMilestoneComReg { get; set; }

        public Shared.Net.IPaddressPair[] AllowAdminFromIPs { get; set; }

        public class CameraConfig
        {
            public int ResWidth { get; set; }
            public int ResHeight { get; set; }
            public int Compression { get; set; }
            public int FPS { get; set; }

            public string ConvertMethod { get; set; }
        }

        public Config()
        {
            MilestoneServer = "http://localhost/";
            Port = 480;
            Binding = "+";
            UseHttps = true;
            Server = (UseHttps ? "https" : "http") + "://" + System.Net.Dns.GetHostEntry("").HostName + ":" + Port + "/";
            LicenseKey = null;
            WaitForMilestoneComReg = false;

            AllowAdminFromIPs = Shared.Net.IPaddressPair.GetIPAddresses();

            Camera = new CameraConfig()
            {
                Compression = 85,
                FPS = 5,
                ConvertMethod = "software",
                ResHeight = 720,
                ResWidth = 1280
            };
        }


        private static Config _instance;
        internal static Config Instance
        {
            get
            {
                if(_instance == null)
                {
                    var configPath = Path.Combine(WorkDir, "config.json");
                    if (File.Exists(configPath))
                    {
                        _instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
                    }
                    else
                    {
                        _instance = new Config();
                        File.WriteAllText(configPath, JsonConvert.SerializeObject(_instance, Formatting.Indented));
                    }
                }
                return _instance;
            }
        }

        private static string _workDir;
        internal static string WorkDir
        {
            get
            {
                if (_workDir == null)
                {
                    _workDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "InSupport Nätverksvideo AB", "TryggSHARE");
                    Directory.CreateDirectory(_workDir);
                }
                return _workDir;
            }
        }

        //internal static void Init()
        //{
        //    Assembly executingAssembly = Assembly.GetExecutingAssembly();
        //    string targetDir = executingAssembly.Location;
        //    var config = ConfigurationManager.OpenExeConfiguration(targetDir);

        //    Port = int.Parse(config.AppSettings.Settings["port"].Value);
        //    Binding = config.AppSettings.Settings["binding"].Value;
        //    UseHttps = config.AppSettings.Settings["secure"].Value.ToLower() == "true";
        //    LicenseKey = config.AppSettings.Settings["license"].Value;
        //    MilestoneServer = config.AppSettings.Settings["milestoneServer"].Value;


        //    Server = config.AppSettings.Settings["server"].Value;
        //    if (Server == "%hostname%")
        //        Server = System.Net.Dns.GetHostEntry("").HostName;
        //    Server = (UseHttps ? "https" : "http") + "://" + Server + ":" + Port + "/";


        //    CameraConfig.ResHeight = int.Parse(config.AppSettings.Settings["cameraResHeight"].Value);
        //    CameraConfig.ResWidth = int.Parse(config.AppSettings.Settings["cameraResWidth"].Value);
        //    CameraConfig.FPS = int.Parse(config.AppSettings.Settings["fps"].Value);
        //    CameraConfig.Compression = int.Parse(config.AppSettings.Settings["compression"].Value);
        //    CameraConfig.ConvertMethod = config.AppSettings.Settings["convertMethod"].Value;
        //}
    }
}
