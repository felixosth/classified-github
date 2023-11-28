using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace InSupport_LicenseSystem
{
    public class LicenseManager
    {
        protected internal string passKey = "6Pap?2R67eshvzaJ7uHAn=+";

        const string baseUrl = "https://portal.tryggconnect.se/api";

        string myMachineGuid;

        string product { get; set; }

        public LicenseActivationType CurrentActivationType = LicenseActivationType.Online;

        public LicenseManager(string machineGuid, string product)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            myMachineGuid = machineGuid;
            this.product = product;
        }

        public LicenseActivationResult ActivateLicense(string license)
        {
            string url = baseUrl + "/activatelicense.php";
            var values = new System.Collections.Specialized.NameValueCollection();
            int responseCode = -1; // 1 is true, 0 is false
            values["mguid"] = myMachineGuid;
            values["product"] = product;
            values["license"] = license;
            using (var client = new WebClient())
            {
                var response = client.UploadValues(url, values);
                var responseString = Encoding.Default.GetString(response);
                if (!int.TryParse(responseString, out responseCode))
                {
                    responseCode = -1;
                }
            }
            return (LicenseActivationResult)Enum.ToObject(typeof(LicenseActivationResult), responseCode);
        }

        public LicenseCheckResult CheckOnlineLicense()
        {
            string url = baseUrl + "/licensecheck.php";
            var values = new System.Collections.Specialized.NameValueCollection();
            int responseCode = -1;
            values["mguid"] = myMachineGuid;
            values["product"] = product;
            using (var client = new WebClient())
            {
                var response = client.UploadValues(url, values);
                string res = Encoding.Default.GetString(response);
                if (!int.TryParse(res, out responseCode))
                {
                    responseCode = -1;
                }
                else
                    CurrentActivationType = LicenseActivationType.Online;
            }
            return (LicenseCheckResult)Enum.ToObject(typeof(LicenseCheckResult), responseCode);
        }

        public string DefaultLicensePath
        {
            get
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string myFolder = Path.Combine(appData, "InSupport." + product);
                Directory.CreateDirectory(myFolder);
                string finalFileName = Path.Combine(myFolder, "license.lic");
                return finalFileName;
            }
        }

        public LicenseInfo GetLicenseInfo()
        {
            LicenseInfo licenseInfo = null;
            string url = baseUrl + "/licenseinfo.php";
            var values = new System.Collections.Specialized.NameValueCollection();
            values["mguid"] = myMachineGuid;
            values["product"] = product;
            //var xmlDoc = new XmlDocument();
            string xml = "";
            using (var client = new WebClient())
            {
                var response = client.UploadValues(url, values);
                xml = Encoding.Default.GetString(response);
                //xmlDoc.LoadXml(xml);
                Console.WriteLine();
            }

            using (TextReader sr = new StringReader(xml))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(LicenseInfo));
                licenseInfo = (LicenseInfo)serializer.Deserialize(sr);
            }
            return licenseInfo;
        }

        public void SaveOfflineLicense(LicenseInfo info, string path)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(LicenseInfo));
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    xsSubmit.Serialize(writer, info, ns);
                    xml = sww.ToString(); // Your XML
                }
            }

            //var bytes = Encoding.Default.GetBytes(xml);
            File.WriteAllText(path, StringCipher.Encrypt(xml, passKey));
        }

        public LicenseCheckResult CheckOfflineLicense(string path)
        {
            if (!File.Exists(path))
                return LicenseCheckResult.None;

            var encryptedXml = File.ReadAllText(path);
            var xml = StringCipher.Decrypt(encryptedXml, passKey);
            var licenseInfo = new LicenseInfo();
            using (TextReader sr = new StringReader(xml))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(LicenseInfo));
                licenseInfo = (LicenseInfo)serializer.Deserialize(sr);
            }
            if (licenseInfo == null)
                return LicenseCheckResult.Error;

            if (myMachineGuid != licenseInfo.MachineGUID)
                return LicenseCheckResult.None;
            else if (licenseInfo.ExpirationDate < DateTime.Now)
                return LicenseCheckResult.Expired;


            CurrentActivationType = LicenseActivationType.Offline;
            return LicenseCheckResult.Valid;
        }

        public LicenseInfo GetLicenseInfoOffline(string path)
        {
            if (!File.Exists(path))
                return null;

            var encryptedXml = File.ReadAllText(path);
            var xml = StringCipher.Decrypt(encryptedXml, passKey);
            var licenseInfo = new LicenseInfo();
            using (TextReader sr = new StringReader(xml))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(LicenseInfo));
                licenseInfo = (LicenseInfo)serializer.Deserialize(sr);
            }
            if (licenseInfo == null)
                return null;
            return licenseInfo;
        }
    }

    public enum LicenseCheckResult
    {
        Error = -1,
        Valid = 0,
        Expired = 1,
        None = 2
    }
    public enum LicenseActivationResult
    {
        Error = -1,
        Success = 0,
        InUse = 1,
        Expired = 2,
        NoLicenseExist = 3
    }
    public enum LicenseActivationType
    {
        Online = 0,
        Offline = 1
    }
}
