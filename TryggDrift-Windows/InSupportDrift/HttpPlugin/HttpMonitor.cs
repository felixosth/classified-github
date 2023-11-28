using InSupport.Drift.Monitor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace HttpPlugin
{
    public class HttpMonitor : BaseMonitor
    {
        internal const string _settingsKey = "HttpMonitor";

        string[] addresses;


        public HttpMonitor() : base()
        {

        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            if (!settings.ContainsKey(_settingsKey))
            {
                Enabled = false;
            }
            else
            {
                addresses = JsonConvert.DeserializeObject<string[]>(settings[_settingsKey]);
            }
        }

        public GetResponse[] GetResponses
        {
            get
            {
                GetResponse[] getResponses = new GetResponse[addresses.Length];

                using (var httpClient = new HttpClient() { })
                {
                    for (int i = 0; i < addresses.Length; i++)
                    {
                        if (string.IsNullOrEmpty(addresses[i]) || string.IsNullOrWhiteSpace(addresses[i]))
                            continue;

                        getResponses[i] = new GetResponse() { Address = addresses[i] };
                        try
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, addresses[i]);
                            var response = httpClient.SendAsync(request).Result;
                            response.Content.ReadAsStreamAsync().Result.Close();
                            getResponses[i].StatusCode = (int)response.StatusCode;
                            getResponses[i].Status = response.StatusCode.ToString();
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException is HttpRequestException)
                            {
                                if (ex.InnerException.InnerException is WebException)
                                {
                                    getResponses[i].Status = (ex.InnerException.InnerException as WebException).Status.ToString();
                                    //getResponses[i].StatusCode = (int)(ex.InnerException.InnerException as WebException).Status;
                                }
                                getResponses[i].Error = ex.InnerException.InnerException.Message;
                                //(ex.InnerException as HttpRequestException)
                            }
                        }
                    }
                }


                return getResponses;
            }
        }

        public override string MonitorName => "HttpPlugin";

        public override float MonitorVersion => 1;
    }

    public class GetResponse
    {
        public string Address { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
    }
}
