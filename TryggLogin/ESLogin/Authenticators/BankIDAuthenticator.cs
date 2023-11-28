using LoginShared;
using LoginShared.AD;
using LoginShared.Administration;
using LoginShared.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;

namespace ESLogin.Authenticators
{
    internal class BankIDAuthenticator : BaseAuthenticator
    {
        public override event EventHandler<User> OnUserGranted;
        public override event EventHandler<User> OnUserDenied;
        public override event EventHandler<StatusResponseEventArgs> OnStatusResponse;

        private string MachineID { get; set; }

        public override string Name => "BankID";

        private Environment BankIDEnvironment;

        public BankIDAuthenticator(string mguid, Environment env)
        {
            this.BankIDEnvironment = env;
            this.MachineID = mguid;
        }

        public override void Init()
        {
        }

        public override void StartLogin(User user, string data)
        {
            try
            {
                OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("Contacting BankID service...", user));
                
                var order = Sign(user, "Authorize login to Milestone XProtect server " + ESLoginDefinition.ServerName);

                if(order == null)
                {
                    Log("Order is null with user " + user.DisplayName);
                    //OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("PNR is null.", user));
                    OnUserDenied?.Invoke(this, user);
                }

                if (Collect(order, user)) // Will block thread
                {
                    OnUserGranted?.Invoke(this, user);
                }
                else
                {
                    OnUserDenied?.Invoke(this, user);
                }
            }
            catch(Exception ex)
            {
                Log(ex.ToString(), true);
                OnUserDenied?.Invoke(this, user);
            }
        }

        string GetPNR(string sid)
        {
            using (var context = Helper.GetPrincipalContext())
            {
                UserPrincipalEx u = UserPrincipalEx.FindByIdentity(context, IdentityType.Sid, sid);
                if(u.EmployeeNumber == null)
                {
                    throw new Exception("EmployeeNumber is null (at GetPNR, user " + sid + ")");
                }

                return u.EmployeeNumber;
            }
        }

        public override void Close()
        {
        }

        bool Collect(BankIDResponses.Order auth, User user)
        {
            if (auth.orderRef == null)
                throw new NullReferenceException("orderRef is null");

            var started = DateTime.Now;
            while (true)
            {
                var values = new
                {
                    mguid = MachineID,
                    environment = BankIDEnvironment.ToString(),
                    method = "collect",
                    auth.orderRef
                };

                var resultString = ApiCall(values);

                var result = JsonConvert.DeserializeObject<BankIDResponses.Collect>(resultString);

                if (result.completionData != null)
                    return true;
                else if (result.status != "pending")
                {
                    if(result.status == "failed")
                    {
                        switch(result.hintCode)
                        {
                            case BankIDResponses.HintCodes.cancelled:
                                OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("Action cancelled. Please try again.", user, true));
                                break;
                            case BankIDResponses.HintCodes.userCancel:
                                OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("Action cancelled.", user, true));
                                break;
                            case BankIDResponses.HintCodes.certificateErr:
                                OnStatusResponse?.Invoke(this, 
                                    new StatusResponseEventArgs(
                                        "The BankID you are trying to use is revoked or too old. Please use another BankID or order a new one from your internet bank.", user, true));
                                break;
                            default:
                                OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("Unknown error. Please try again.", user, true));
                                break;
                        }

                        Thread.Sleep(5000);
                    }

                    return false;
                }
                else if (DateTime.Now >= started.AddMinutes(1))
                {
                    CancelLogin(auth.orderRef);
                    OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("Action cancelled. Please try again.", user, true));
                    Thread.Sleep(5000);
                    return false;
                }
                else
                {
                    switch(result.hintCode)
                    {
                        case BankIDResponses.HintCodes.outstandingTransaction:
                        case BankIDResponses.HintCodes.noClient:
                            OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("Start your BankID app.", user));
                            break;
                        case BankIDResponses.HintCodes.userSign:
                        case BankIDResponses.HintCodes.started:
                            OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("Enter your security code in the BankID app and select Sign.", user));
                            break;
                    }
                    Thread.Sleep(1000 / 3);
                }
            }
        }

        private BankIDResponses.Order Identify(User user)
        {
            string pnr = string.IsNullOrEmpty(user.AuthData) ? GetPNR(user.SID) : user.AuthData;

            if (string.IsNullOrEmpty(pnr))
                return null;

           object values = new
            {
                mguid = MachineID,
                environment = BankIDEnvironment.ToString(),
                method = "auth",
                personalNumber = pnr
           };

            var resultString = ApiCall(values);

            return JsonConvert.DeserializeObject<BankIDResponses.Order>(resultString);
        }

        private BankIDResponses.Order Sign(User user, string userData)
        {
            string pnr = string.IsNullOrEmpty(user.AuthData) ? GetPNR(user.SID) : user.AuthData;

            if (string.IsNullOrEmpty(pnr))
                return null;

            object values = new
            {
                mguid = MachineID,
                environment = BankIDEnvironment.ToString(),
                method = "sign",
                userData,
                personalNumber = pnr
            };

            var resultString = ApiCall(values);

            return JsonConvert.DeserializeObject<BankIDResponses.Order>(resultString);
        }

        private void CancelLogin(string orderRef)
        {
            var values = new
            {
                mguid = MachineID,
                environment = BankIDEnvironment.ToString(),
                method = "cancel",
                orderRef
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

                //Log("Sending data: " + payload);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var result = client.PostAsync("api/bankid.php", content).Result;
                var resultString = result.Content.ReadAsStringAsync().Result;
                //Log("Recieved data: " + resultString);
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
        }


        internal class Collect
        {
            public string orderRef { get; set; }
            public string status { get; set; }
            public HintCodes hintCode { get; set; }

            public Completiondata completionData { get; set; }

            //public string SourceString { get; set; }

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
