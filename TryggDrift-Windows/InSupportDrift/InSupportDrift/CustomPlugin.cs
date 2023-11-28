using System.Collections.Generic;

namespace InSupport.Drift.Plugins
{
    public abstract class CustomPlugin
    {
        public abstract string PluginName { get; }
        public abstract string Version { get; }
        protected HostMonitor HostMonitor { get; set; }
        protected Dictionary<string, string> Settings { get; set; }

        public CustomPlugin(Dictionary<string, string> settings, HostMonitor hostMonitor)
        {
            this.HostMonitor = hostMonitor;
            this.Settings = settings;
        }
    }
}
