using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XProtectWebStream.Global
{
    internal static class LicenseSystem
    {
        internal static string LicenseKey { get; set; }
        private const string LicensePortalUrl = "https://portal.tryggconnect.se/";

        internal static event EventHandler OnLicenseExpired;
        internal static event EventHandler OnLicenseValid;

        internal static LicenseCheckResponse LastCheck { get; set; }

        internal static LicenseCheckResponse CheckLicense()
        {
            using (var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(LicensePortalUrl)
            })
            {
                var formData = new Dictionary<string, string>()
                {
                    { "license", LicenseKey },
                    { "mguid", Shared.Utils.GetMGUID() },
                    { "product", "tryggstream" }
                };

                FormUrlEncodedContent content = new FormUrlEncodedContent(formData);
                var postResult = httpClient.PostAsync("/api/checkOrActivate.php", content).Result;

                var stringResponse = postResult.Content.ReadAsStringAsync().Result;

                var check = JsonConvert.DeserializeObject<LicenseCheckResponse>(stringResponse);
                LastCheck = check;
                return check;
            }
        }

        static bool threadIsStarted = false;
        internal static void StartCheckThread()
        {
            if(!threadIsStarted)
            {
                threadIsStarted = true;
                new Thread(CheckThread) { Name = "License check thread", IsBackground = true }.Start();
            }
        }

        private static void CheckThread()
        {
            TimeSpan defaultAddSpan = TimeSpan.FromHours(8);

#if DEBUG
            defaultAddSpan = TimeSpan.FromMinutes(1);
#endif

            DateTime nextCheck = DateTime.Now.Add(defaultAddSpan);
            while(true)
            {
                if(DateTime.Now > nextCheck)
                {
                    try
                    {
                        var lastCheck = LastCheck;

                        var check = CheckLicense();
                        if(lastCheck.license != null && check.license == null) // Not valid anymore
                        {
                            OnLicenseExpired?.Invoke(null, new EventArgs());
                        }
                        else if(lastCheck.license == null && check.license != null) // Valid again
                        {
                            OnLicenseValid?.Invoke(null, new EventArgs());
                        }
                    }
                    catch
                    {
                    }

                    nextCheck = LastCheck.license == null ? DateTime.Now.AddMinutes(1) : DateTime.Now.Add(defaultAddSpan);
                }

                Thread.Sleep(1000);
            }
        }


       
    }


    public class LicenseCheckResponse
    {
        public string result { get; set; }
        public string error { get; set; }
        public License license { get; set; }
    }

    public class License
    {
        //public int ID { get; set; }
        public string Product { get; set; }
        public string Customer { get; set; }
        public string Site { get; set; }
        public string LicenseGUID { get; set; }
        public string MachineGUID { get; set; }
        //public int MaxCurrentUsers { get; set; }
        //public string DateAdded { get; set; }
        public string ExpirationDate { get; set; }
        //public string AddedBy { get; set; }
        public int? SMS { get; set; }
        public int? BankID { get; set; }
        //public object BankIDLimit { get; set; }
        //public int SMSLimit { get; set; }
    }

}
