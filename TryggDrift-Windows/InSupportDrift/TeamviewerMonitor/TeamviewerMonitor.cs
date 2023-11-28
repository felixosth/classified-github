using InSupport.Drift.Monitor;
using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace InSupport.Drift.Plugins
{
    public class TeamViewerMonitor : BaseMonitor
    {
        public override float MonitorVersion => 1.0f;
        private const string x86path = @"HKEY_LOCAL_MACHINE\SOFTWARE\TeamViewer";
        private const string x64path = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\TeamViewer";

        string path { get; set; }

        public TeamViewerMonitor()
            : base()
        {
            this.path = Environment.Is64BitOperatingSystem ? x64path : x86path;
        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
        }

        public string ClientID
        {
            get
            {
                string val = GetValue(path, "ClientID");
                if (val == null)
                    val = GetVer8Value(path, "ClientID");

                return val;
            }
        }

        public string Version
        {
            get
            {
                string val = GetValue(path, "Version");
                if (val == null)
                    val = GetVer8Value(path, "Version");

                return val;
            }
        }

        private string GetValue(string path, string key)
        {
            return Registry.GetValue(path, key, null)?.ToString();
        }

        private string GetVer8Value(string path, string key)
        {
            return Registry.GetValue(path + @"\Version8", key, null)?.ToString();
        }

        public override string MonitorName => "TeamViewerMonitor";
    }
}
