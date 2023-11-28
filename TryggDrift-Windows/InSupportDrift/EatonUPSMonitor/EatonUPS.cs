using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

namespace EatonUPSMonitor
{
    public class EatonUPS
    {
        private Uri UPSCompanionURI { get; set; }


        [JsonProperty(Order = 2)]
        public string ErrorMessage { get; set; }

        public EatonUPS(Uri url)
        {
            this.UPSCompanionURI = url;
        }

        [JsonProperty(Order = 1)]
        public API.Data Data
        {
            get
            {
                ErrorMessage = null;
                try
                {
                    string rawData = WebUtility.HtmlDecode(GetData("/server/data_srv.js?action=getData"));
                    return JsonConvert.DeserializeObject<API.GetDataResult>(rawData).data;
                }
                catch
                {
                    ErrorMessage = "Unable to communicate with UPS";
                    return null;
                }
            }
        }

        private string GetData(string url)
        {
            var address = new Uri(UPSCompanionURI, url);
            using (var wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                return wc.DownloadString(address);
            }
        }

    }

}
