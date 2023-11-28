using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilestoneToDrive
{
    public class Config
    {
        public string DriveFolderId { get; set; }
        public string MilestoneServer { get; set; } = "http://localhost";
        public string LogFile { get; set; } = Path.Combine(WorkDir, "log.txt");
        public string ConvertMethod { get; set; } = "copy";
        public string VideoExportTempFolder { get; set; } = Path.Combine(WorkDir, "Videos");

        internal void Save()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_instance, Formatting.Indented));
        }

        private static Config _instance;
        internal static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (File.Exists(ConfigPath))
                    {
                        _instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath));
                    }
                    else
                    {
                        _instance = new Config();
                        File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_instance, Formatting.Indented));
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
                    _workDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "InSupport Nätverksvideo AB", "MilestoneToDrive");
                    Directory.CreateDirectory(_workDir);
                }
                return _workDir;
            }
        }

        private static string ConfigPath => Path.Combine(WorkDir, "config.json");

    }
}
