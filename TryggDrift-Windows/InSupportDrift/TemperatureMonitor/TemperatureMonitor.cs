using InSupport.Drift.Monitor;
using OHMWrapper;
using System.Collections.Generic;

namespace InSupport.Drift.Plugins
{
    public class TemperatureMonitor : BaseMonitor
    {
        public TemperatureMonitor() : base()
        {
        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
        }

        public override string MonitorName => "TemperatureMonitor";

        public override float MonitorVersion => 1;

        public Dictionary<string, IEnumerable<SimpleSensor>> Sensors => TemperatureSensors.GetSensorsDictionary();
    }
}
