using System.Collections.Generic;

namespace InSupport.Drift.Plugins
{
    public class ServiceMonitorConfig
    {
        public IEnumerable<string> ServicesToMonitor { get; set; }
        public bool AutoStartStoppedServices { get; set; }
        public double AutoStartServiceAfterMinutes { get; set; } = 10.0;
    }
}
