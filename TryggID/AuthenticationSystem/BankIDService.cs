using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;

namespace AuthenticationSystem
{
    public class BankIDService
    {
        public event EventHandler<BankIDResponses.Collect> OnUserCollect;
        public event EventHandler<BankIDResponses.Collect> OnUserCollectFailed;

        public event EventHandler<string> OnLog;

        private string MGUID { get; set; }
        private string URL { get; set; }

        private Enviroment enviroment;

        internal BankIDService(string mguid, Enviroment bankidEnviroment)
        {
            enviroment = bankidEnviroment;
            URL = "https://portal.tryggconnect.se/api/bankid.php";
            MGUID = mguid;
        }

        /// <summary>
        /// Authorize with BankID
        /// </summary>
        /// <param name="text">Message to user</param>
        /// <returns>BankID Auth Response</returns>
        public BankIDResponses.Auth Auth(string personalNumber, string endUserIp)
        {
            object values = new object();

            if (personalNumber != "")
            {
                values = new
                {
                    mguid = MGUID,
                    enviroment = enviroment.ToString(),
                    method = "auth",
                    personalNumber
                };
            }


            var resultString = ApiCall(values);

            return JsonConvert.DeserializeObject<BankIDResponses.Auth>(resultString);
        }

        public void StartCollect(string orderRef)
        {
            new Thread(() =>
            {
                var started = DateTime.Now;
                while (true)
                {
                    var values = new
                    {
                        mguid = MGUID,
                        enviroment = enviroment.ToString(),
                        method = "collect",
                        orderRef
                    };

                    var resultString = ApiCall(values);

                    var result = JsonConvert.DeserializeObject<BankIDResponses.Collect>(resultString);
                    result.SourceString = resultString;

                    if (result.completionData != null)
                    {
                        OnUserCollect?.Invoke(this, result);
                        break;
                    }
                    else if(result.status != "pending")
                    {
                        OnUserCollectFailed?.Invoke(this, result);
                        break;
                    }
                    else if(DateTime.Now >= started.AddMinutes(2))
                    {
                        OnUserCollectFailed?.Invoke(this, result);
                        Cancel(orderRef);
                        break;
                    }
                    else
                        Thread.Sleep(2000);
                }

            }).Start();
        }

        private string ApiCall(object values, bool printResult = false)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://portal.tryggconnect.se");
                //client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var payload = JsonConvert.SerializeObject(values);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                //content.Headers.ContentType.CharSet = string.Empty;

                var result = client.PostAsync("api/bankid.php", content).Result;
                var resultString = result.Content.ReadAsStringAsync().Result;

                if (printResult)
                    Log(resultString);

                return resultString;
            }
        }

        private void Log(string msg)
        {
            OnLog?.Invoke(this, msg);
        }

        public void Cancel(string orderRef)
        {
            Log("Cancelling " + orderRef);
            var values = new
            {
                mguid = MGUID,
                enviroment = enviroment.ToString(),
                method = "cancel",
                orderRef
            };

            ApiCall(values);
        }

        public enum Enviroment
        {
            test,
            live
        }
    }



    public class BankIDResponses
    {
        public class Auth
        {
            public string orderRef { get; set; }
            public string autoStartToken { get; set; }
        }


        public class Collect
        {
            public string orderRef { get; set; }
            public string status { get; set; }
            public HintCodes hintCode { get; set; }

            public Completiondata completionData { get; set; }

            public string SourceString { get; set; }

            public string errorCode { get; set; }
            public string details { get; set; }
        }

        //{"errorCode":"invalidParameters","details":"Invalid request"}

        public class Completiondata
        {
            public User user { get; set; }
            public Device device { get; set; }
            public Cert cert { get; set; }
            public string signature { get; set; }
            public string ocspResponse { get; set; }
        }

        public class User
        {
            public string personalNumber { get; set; }
            public string name { get; set; }
            public string givenName { get; set; }
            public string surname { get; set; }
        }

        public class Device
        {
            public string ipAddress { get; set; }
        }

        public class Cert
        {
            public string notBefore { get; set; }
            public string notAfter { get; set; }
        }


        public enum HintCodes
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
