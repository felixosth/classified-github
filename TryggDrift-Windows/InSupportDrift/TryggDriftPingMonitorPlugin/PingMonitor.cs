using InSupport.Drift.Monitor;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using static TryggDriftPingMonitorPlugin.PingPluginConfigurationWpf;

namespace InSupport.Drift.Plugins
{
    public class PingMonitor : BaseMonitor
    {
        public override float MonitorVersion => 1.21f;
        int pingCount = 5;

        PingItem[] pingItems { get; set; }

        public PingItem[] PingItems
        {
            get
            {
                if (pingItems != null)
                {
                    foreach (var pingItem in pingItems)
                    {
                        pingItem.PingResult = Ping(pingItem.IP, pingCount);
                    }
                }
                return pingItems;
            }
        }

        public PingMonitor() : base()
        {

        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey(_PingItemsCfgKey))
            {
                pingItems = JsonConvert.DeserializeObject<PingItem[]>(settings[_PingItemsCfgKey]);
            }
            else
                this.Enabled = false;

            if (settings.ContainsKey(_PingCountCfgKey))
            {
                int.TryParse(settings[_PingCountCfgKey], out pingCount);
            }
        }

        public override string MonitorName => "Ping";

        PingResult Ping(string address, int count)
        {
            Ping pinger = null;

            int failed = 0;
            int success = 0;
            int averageResponseTime = 0;

            try
            {
                PingOptions options = new PingOptions(128, true);
                int timeout = 5000; //ms
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                pinger = new Ping();
                for (int i = 0; i < count; i++)
                {
                    var reply = pinger.Send(address, timeout, buffer, options);

                    if (reply.Status == IPStatus.Success)
                    {
                        averageResponseTime += (int)reply.RoundtripTime;
                        success++;
                        Thread.Sleep(150);
                    }
                    else
                    {
                        failed++;
                        Thread.Sleep(500);
                    }
                }
            }
            catch
            {
                return null;
            }

            return new PingResult() { SuccessfulPings = success, FailedPings = failed, AverageResponseTime = (averageResponseTime > 0 ? averageResponseTime / success : 0) };
        }
    }
    public class PingResult
    {
        //public string Address { get; set; }
        public int AverageResponseTime { get; set; }
        public int SuccessfulPings { get; set; }
        public int FailedPings { get; set; }
    }
    public class PingItem
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public string IP { get; set; }

        public PingResult PingResult { get; set; }
    }
}
