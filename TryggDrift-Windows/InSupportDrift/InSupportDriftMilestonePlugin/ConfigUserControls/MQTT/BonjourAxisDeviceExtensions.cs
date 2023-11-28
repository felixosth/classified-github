using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InSupportDriftMilestonePlugin.ConfigUserControls.MQTT
{
    /// <summary>
    /// Extensions/operations for BonjourAxisDevice
    /// </summary>
    internal static class BonjourAxisDeviceExtensions
    {

        private const string _BDI_JSON_DATA = "{\"apiVersion\":\"1.0\",\"context\":\"Client defined request ID\",\"method\":\"getAllProperties\"}";


        internal static async Task<Propertylist> GetBasicDataInfoAsync(this BonjourAxisDevice device, string username, string password)
        {
            var client = new RestClient($"http://{device.IPAddress}")
            {
                FollowRedirects = false
            };

            var request = new RestRequest("/axis-cgi/basicdeviceinfo.cgi", Method.POST, DataFormat.Json)
            {
                Credentials = new NetworkCredential(username, password)
            };

            request.AddParameter("application/json", _BDI_JSON_DATA, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var locHeader = response.Headers.FirstOrDefault(h => h.Name == "Location");
                request = new RestRequest(new Uri((string)locHeader.Value).AbsoluteUri, Method.POST)
                {
                    Credentials = new NetworkCredential(username, password)
                };

                request.AddParameter("application/json", _BDI_JSON_DATA, ParameterType.RequestBody);

                response = await client.ExecuteAsync(request);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();
            else if (response.IsSuccessful == false)
                return null;

            var bdi = JsonConvert.DeserializeObject<BasicDeviceInfo>(response.Content);

            if (bdi?.data?.propertyList == null)
                throw new Exception("PropertyList is null");
            bdi.data.propertyList.UseHttps = response.ResponseUri.Scheme == "https";
            return bdi.data.propertyList;
        }


        internal static async Task<AxisApplicationListResponse> GetApplicationListAsync(this BonjourAxisDevice device, string username, string password, bool useHttps)
        {
            var client = new RestClient($"{(useHttps ? "https" : "http")}://{device.IPAddress}");

            var request = new RestRequest("/axis-cgi/applications/list.cgi", Method.GET, DataFormat.Xml)
            {
                Credentials = new NetworkCredential(username, password)
            };

            var response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();
            else if (response.IsSuccessful == false)
                return null;

            return DeserializeXML<AxisApplicationListResponse>(response.Content);
        }

        internal static async Task<UploadACAPResults> UploadACAPAsync(this BonjourAxisDevice device, string username, string password, Stream acapDataStream, string filename, bool useHttps)
        {
            var client = new RestClient($"{(useHttps ? "https" : "http")}://{device.IPAddress}");
            var request = new RestRequest("/axis-cgi/applications/upload.cgi", Method.POST, DataFormat.None)
            {
                Credentials = new NetworkCredential(username, password)
            };

            request.AddFile("packfil", (stream) => acapDataStream.CopyTo(stream), filename, acapDataStream.Length, "application/octet-stream");
            var response = await client.ExecuteAsync(request);
            var content = response.Content; // Raw content as string

            if (content.Replace("\r\n", "") == "OK")
                return UploadACAPResults.OK;
            else
                return (UploadACAPResults)int.Parse(content.Replace("Error: ", ""));
        }

        internal static async Task<ApplicationControlResults> ApplicationControlAsync(this BonjourAxisDevice device, string username, string password, string package, ApplicationControlActions action, bool useHttps)
        {
            var client = new RestClient($"{(useHttps ? "https" : "http")}://{device.IPAddress}");
            var request = new RestRequest($"/axis-cgi/applications/control.cgi?action={action.ToString().ToLower()}&package={package}", Method.POST, DataFormat.None)
            {
                Credentials = new NetworkCredential(username, password)
            };

            var response = await client.ExecuteAsync(request);

            if (response.Content.Replace("\r\n", "") == "OK")
                return ApplicationControlResults.OK;
            else
                return (ApplicationControlResults)int.Parse(response.Content.Replace("Error: ", ""));
        }

        private static T DeserializeXML<T>(string data) where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(data))
                return (T)ser.Deserialize(sr);
        }
    }


    internal enum ApplicationControlActions
    {
        Start,
        Stop,
        Restart,
        Remove
    }

    internal enum ApplicationControlResults
    {
        OK = 0,
        AppNotFound = 4,
        AppAlreadyRunning = 6,
        AppNotRunning = 7,
        CouldNotStart = 8,
        TooManyAppsRunning = 9,
        Unspecified = 10
    }

    internal enum UploadACAPResults
    {
        OK = 0,
        NoValidPackage = 1,
        VerificationFailed = 2,
        PackageTooLargeOrDiskFull = 3,
        PackageNotCompatible = 5,
        Unspecified = 10
    }



    #region BasicDeviceInfo JSOn
    public class BasicDeviceInfo
    {
        public string apiVersion { get; set; }
        public Data data { get; set; }
        public string context { get; set; }
    }

    public class Data
    {
        public Propertylist propertyList { get; set; }
    }

    public class Propertylist
    {
        public string Architecture { get; set; }
        public string ProdNbr { get; set; }
        public string HardwareID { get; set; }
        public string ProdFullName { get; set; }
        public string Version { get; set; }
        public string ProdType { get; set; }
        public string SocSerialNumber { get; set; }
        public string Soc { get; set; }
        public string Brand { get; set; }
        public string WebURL { get; set; }
        public string ProdVariant { get; set; }
        public string SerialNumber { get; set; }
        public string ProdShortName { get; set; }
        public string BuildDate { get; set; }

        public bool UseHttps { get; set; }
    }
    #endregion


    #region Application list XML
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "reply")]
    public class AxisApplicationListResponse
    {

        private AxisApplication[] applicationField;

        private string resultField;

        public AxisApplicationListResponse()
        {

        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("application")]
        public AxisApplication[] application
        {
            get
            {
                return this.applicationField;
            }
            set
            {
                this.applicationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class AxisApplication
    {

        private string nameField;

        private string niceNameField;

        private string vendorField;

        private string versionField;

        private uint applicationIDField;

        private string licenseField;

        private string statusField;

        private string configurationPageField;

        private string vendorHomePageField;

        private string licenseNameField;

        public AxisApplication()
        {
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NiceName
        {
            get
            {
                return this.niceNameField;
            }
            set
            {
                this.niceNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Vendor
        {
            get
            {
                return this.vendorField;
            }
            set
            {
                this.vendorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint ApplicationID
        {
            get
            {
                return this.applicationIDField;
            }
            set
            {
                this.applicationIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string License
        {
            get
            {
                return this.licenseField;
            }
            set
            {
                this.licenseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ConfigurationPage
        {
            get
            {
                return this.configurationPageField;
            }
            set
            {
                this.configurationPageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string VendorHomePage
        {
            get
            {
                return this.vendorHomePageField;
            }
            set
            {
                this.vendorHomePageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LicenseName
        {
            get
            {
                return this.licenseNameField;
            }
            set
            {
                this.licenseNameField = value;
            }
        }
    }


    #endregion


}