using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WinServLite2
{
    internal class Config
    {
        private string DirectoryPath { get; set; }
        internal string ConfigPath { get; private set; }

        public Config(string company)
        {
            DirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), company, Assembly.GetExecutingAssembly().GetName().Name);

            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

            ConfigPath = Path.Combine(DirectoryPath, "configuration.json");
        }


        public Dictionary<string, object> LoadConfig()
        {
            if (!File.Exists(ConfigPath))
                return null;

            string content = File.ReadAllText(ConfigPath);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
        }

        public void SaveConfig(Dictionary<string, object> properties)
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(properties, Formatting.Indented));
        }
    }
}
