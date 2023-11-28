using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TryggSTORE.Http
{
    internal class OccupancyCommuncation
    {

        private int lastCount = -1;

        public void Start()
        {
            new Thread(FetchCounterDataThread) { IsBackground = true, Name = "Fetch counter data thread" }.Start();
        }

        private void FetchCounterDataThread()
        {
            while (true)
            {
                try
                {
                    var occupancyData = FetchOccupancyData();

                    var dif = Math.Abs(occupancyData.occupancy - lastCount);

                    if (lastCount == -1 || dif < ConfigFile.Instance.MaxCountChange)
                    {
                        lastCount = occupancyData.occupancy;
                        ConfigFile.Instance.CurrentCount = occupancyData.occupancy;
                    }
                }
                catch
                {
                    ConfigFile.Instance.CurrentCount = -1;
                }
                Thread.Sleep(1000);
            }
        }

        LiveOccupancyJson FetchOccupancyData()
        {
            using (var wc = new AxisCameraWebClient(ConfigFile.Instance.OccupancyCameraUsername, ConfigFile.Instance.OccupancyCameraPassword))
            {
                var downloadedString = wc.DownloadString($"http://{ConfigFile.Instance.OccupancyCameraIP}/local/occupancy-estimator/.api?live-occupancy.json");
                return JsonConvert.DeserializeObject<LiveOccupancyJson>(downloadedString);
            }
        }
    }

    class AxisCameraWebClient : WebClient
    {
        public AxisCameraWebClient(string username, string password)
        {
            this.Credentials = new NetworkCredential(username, password);
            //UseDefaultCredentials = true;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var wr = base.GetWebRequest(address);
            wr.Timeout = 5000;
            return wr;

        }
    }

    public class LiveOccupancyJson
    {
        public int occupancy { get; set; }
        public int averagevisittime { get; set; }
        public int totalin { get; set; }
        public int totalout { get; set; }
        public int unixtime { get; set; }
        public string name { get; set; }
        public string serial { get; set; }
        public string timestamp { get; set; }
    }
}
