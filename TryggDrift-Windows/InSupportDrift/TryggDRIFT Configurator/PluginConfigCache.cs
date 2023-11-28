using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace TryggDRIFT_Configurator
{
    public class PluginConfigCache
    {
        [JsonIgnore]
        internal const string _FILENAME = "pluginconfig.json";

        public List<PluginConfig> PluginConfigs { get; set; } = new List<PluginConfig>();

        public void Save()
        {
            File.WriteAllText(Path.Combine(App.MyDir, _FILENAME), JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static PluginConfigCache FromDefaultFilePath()
        {
            return JsonConvert.DeserializeObject<PluginConfigCache>(File.ReadAllText(_FILENAME));
        }

    }

    public class PluginConfig
    {
        public int PluginID { get; set; }
        public Dictionary<string, string> Config { get; set; }
    }

}
