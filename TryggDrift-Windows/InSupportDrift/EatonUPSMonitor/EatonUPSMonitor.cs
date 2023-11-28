using InSupport.Drift.Monitor;
using System;
using System.Collections.Generic;

namespace EatonUPSMonitor
{
    public class EatonUPSMonitor : BaseMonitor
    {
        internal const string SettingsKey = "EatonUPSAddress";

        public EatonUPSMonitor() : base()
        {

        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            Uri uri = new Uri("http://localhost:4679/");

            if (settings.ContainsKey(SettingsKey))
                uri = new Uri(settings[SettingsKey]);

            EatonUPS = new EatonUPS(uri);
        }

        public EatonUPS EatonUPS { get; set; }

        public override string MonitorName => "EatonUPSMonitor";

        public override float MonitorVersion => 1.0f;
    }
}
