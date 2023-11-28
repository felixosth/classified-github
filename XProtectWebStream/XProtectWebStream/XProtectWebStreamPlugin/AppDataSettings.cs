using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStreamPlugin
{
    internal class Settings
    {
        public List<string> PreviousEmails { get; set; } = new List<string>();
        public List<string> PreviousPhoneNumbers { get; set; } = new List<string>();
    }

    internal class AppDataSettings
    {
        private string AppDataFolder { get; set; }

        public AppDataSettings(string folder)
        {
            AppDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                folder);
            Directory.CreateDirectory(AppDataFolder);
        }

        public T GetFromJsonFile<T>(string filePath)
        {
            filePath = Path.Combine(AppDataFolder, filePath);
            if (!File.Exists(filePath))
                return default;

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }

        public void WriteJsonToFile(string file, string json)
        {
            File.WriteAllText(Path.Combine(AppDataFolder, file), json);
        }

        public void WriteJsonToFile(string file, object objToSerialize)
        {
            File.WriteAllText(Path.Combine(AppDataFolder, file), JsonConvert.SerializeObject(objToSerialize));
        }
    }
}
