using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;
using XProtectWebStream.Web;

namespace XProtectWebStream.Global.Auth
{
    internal class BankIDLogin
    {
        private string MachineID { get; set; }

        BankIDLogin.Environment BankIDEnvironment = Environment.live;

        private TimeSpan CollectTimeout { get; set; } = TimeSpan.FromMinutes(1);
        private DateTime Start { get; set; }

        internal bool LoginExpired => DateTime.Now > Start.Add(CollectTimeout);

        private readonly BankIDResponses.Order order;

        internal string AutoStartToken => order?.autoStartToken;

        internal BankIDResponses.Order Order => order;

        internal BankIDResponses.Completiondata Data { get; set; }
        internal BankIDResponses.HintCodes LastHintCode { get; set; }
        internal BankIDResponses.Status LastStatus { get; set; }

        internal BankIDLogin()
        {

#if DEBUG
            BankIDEnvironment = Environment.test;
#endif

            MachineID = Shared.Utils.GetMGUID();
            order = CreateIdentifyOrder();
            Start = DateTime.Now;

        }

        private BankIDResponses.Order CreateIdentifyOrder()
        {
            object values = new
            {
                mguid = MachineID,
                environment = BankIDEnvironment.ToString(),
                method = "auth",
            };

            var resultString = ApiCall(values);

            return JsonConvert.DeserializeObject<BankIDResponses.Order>(resultString);
        }

        internal BankIDResponses.Status Collect()
        {
            if (order.orderRef == null)
                throw new NullReferenceException("orderRef is null");

            if(DateTime.Now > Start.AddMinutes(1))
            {
                CancelOrder();
                return BankIDResponses.Status.failed;
            }

            var values = new
            {
                mguid = MachineID,
                environment = BankIDEnvironment.ToString(),
                method = "collect",
                order.orderRef
            };

            var resultString = ApiCall(values);

            var result = JsonConvert.DeserializeObject<BankIDResponses.Collect>(resultString);
            LastHintCode = result.hintCode;
            LastStatus = result.status;

            if (result.completionData != null)
                this.Data = result.completionData;

            return result.status;
        }

        private void CancelOrder()
        {
            var values = new
            {
                mguid = MachineID,
                environment = BankIDEnvironment.ToString(),
                method = "cancel",
                orderRef = order.orderRef
            };

            ApiCall(values);
        }

        private string ApiCall(object values)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://portal.tryggconnect.se");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var payload = JsonConvert.SerializeObject(values);

                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var result = client.PostAsync("api/v2/bankid.php", content).Result;
                var resultString = result.Content.ReadAsStringAsync().Result;
                return resultString;
            }
        }

        public enum Environment
        {
            test,
            live
        }
    }

    internal class BankIDResponses
    {
        internal class Order
        {
            public string orderRef { get; set; }
            public string autoStartToken { get; set; }

            // Bankid v5.1
            public string qrStartToken { get; set; }
            public string qrStartSecret { get; set; }

            private long UnixStartTime { get; set; }

            internal Order()
            {
                UnixStartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            }

            internal string GetAnimQRData()
            {
                var timeSinceOrderStr = (DateTimeOffset.Now.ToUnixTimeSeconds() - UnixStartTime).ToString();

                var qrAuthCode = "";

                using (HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(qrStartSecret)))
                {
                    qrAuthCode = BitConverter.ToString(hmac.ComputeHash(Encoding.ASCII.GetBytes(timeSinceOrderStr))).Replace("-", "").ToLower();
                }

                var qrData = $"bankid.{qrStartToken}.{timeSinceOrderStr}.{qrAuthCode}";
                return qrData;
            }
        }


        internal class Collect
        {
            public string orderRef { get; set; }
            public Status status { get; set; }
            public HintCodes hintCode { get; set; }

            public Completiondata completionData { get; set; }

            public string errorCode { get; set; }
            public string details { get; set; }
        }


        internal class Completiondata
        {
            public User user { get; set; }
            public Device device { get; set; }
            public Cert cert { get; set; }
            public string signature { get; set; }
            public string ocspResponse { get; set; }
        }

        internal class User
        {
            public string personalNumber { get; set; }
            public string name { get; set; }
            public string givenName { get; set; }
            public string surname { get; set; }
        }

        internal class Device
        {
            public string ipAddress { get; set; }
        }

        internal class Cert
        {
            public string notBefore { get; set; }
            public string notAfter { get; set; }
        }

        internal enum Status
        {
            pending,
            complete,
            failed
        }

        internal enum HintCodes
        {
            outstandingTransaction,
            noClient,
            started,
            userSign,
            expiredTransaction,
            certificateErr,
            userCancel,
            cancelled,
            startFailed
        }
    }
}
