using InSupport.Drift.Monitor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace InSupport.Drift.Plugins
{
    public class ACSMonitor : BaseMonitor
    {
        internal const string _GetCameraListCall = "/Acs/Api/CameraListFacade/GetCameraList?{\"range\":{\"StartIndex\":0,\"NumberOfElements\":1000}}";

        WebClient wc = new WebClient() { UseDefaultCredentials = true };

        Uri acsServerUri;

        private Dictionary<string, DateTime> camerasLastOnlineDictionary = new Dictionary<string, DateTime>();


        public override string MonitorName => "AxisCameraStationMonitor";

        public override float MonitorVersion => 1;

        private const string _acsServerKey = "ACSServer";
        private const string _acsUsername = "ACSUsername";
        private const string _acsPassword = "ACSPassword";

        public ACSMonitor() : base()
        {


            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            string server = "https://localhost:55756";

            if (settings.ContainsKey(_acsServerKey))
                server = settings[_acsServerKey];

            if (settings.ContainsKey(_acsUsername) && settings.ContainsKey(_acsPassword))
            {
                wc.UseDefaultCredentials = false;
                wc.Credentials = new NetworkCredential(settings[_acsUsername], settings[_acsPassword]);
            }

            acsServerUri = new Uri(server);
        }

        public Camera[] Cameras
        {
            get
            {
                var cameras = JsonConvert.DeserializeObject<GetCameraListResult>(wc.DownloadString(new Uri(acsServerUri, _GetCameraListCall))).Cameras;

                foreach (var camera in cameras)
                {
                    if (!camerasLastOnlineDictionary.ContainsKey(camera.DeviceSerialNumber) || camera.Status == (int)CameraStatus.OK)
                    {
                        camerasLastOnlineDictionary[camera.DeviceSerialNumber] = DateTime.UtcNow;
                    }

                    camera.LastOnlineUTC = camerasLastOnlineDictionary[camera.DeviceSerialNumber];
                }

                return cameras;
            }
        }
    }


    internal class GetCameraListResult
    {
        public Camera[] Cameras { get; set; }
    }

    public class Camera
    {
        [JsonIgnore]
        public CameraId _cameraId;
        public CameraId CameraId { set { _cameraId = value; } }

        public DateTime LastOnlineUTC { get; set; }

        public string Name { get; set; }
        public int Status { get; set; } // CameraStatus

        public string DeviceSerialNumber { get; set; }
    }

    public class CameraId
    {
        public string Id { get; set; }

        public override string ToString() => Id;
    }

    internal enum CameraStatus
    {
        OK = 1,
        Unauthenticated = 2,
        NotAccessible = 3
    }
}
