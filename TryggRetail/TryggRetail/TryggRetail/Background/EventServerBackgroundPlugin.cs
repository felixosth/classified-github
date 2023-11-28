using InSupport_LicenseSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.License;
using VideoOS.Platform.Messaging;

namespace TryggRetail.Background
{
    public class EventServerBackgroundPlugin : BackgroundPlugin
    {
        MessageCommunication _messageCommunication;
        internal InSupport_LicenseSystem.LicenseManager LicenseManager;
        internal LicenseCheckResult LicCheckResult = LicenseCheckResult.None;
        internal LicenseInfo MyLicenseInfo;

        internal const string LicenseCommunicationFilter = "InSupport.TryggRetail.EventServer.LicenseCommunicationFilter";
        object _licComObj;
        bool isRunning = false;

        List<object> msgComObjects = new List<object>();

        bool closeThread = false;

        public override void Init()
        {
            EnvironmentManager.Instance.Log(false, "TryggRetail", "Init");

            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            _messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);
            _licComObj = _messageCommunication.RegisterCommunicationFilter(LicenseCommunication, new CommunicationIdFilter(LicenseCommunicationFilter));

            string myGuid = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null) as string;
            if(string.IsNullOrEmpty(myGuid))
            {
                EnvironmentManager.Instance.Log(true, "TryggRetail", "No MGUID found. ");
            }
            else
            {
                LicenseManager = new InSupport_LicenseSystem.LicenseManager(myGuid, "tryggretail");
                CheckLicense();

                //LicenseManager.ActivateLicense()

                //StartServices();
                new Thread(() =>
                {
                    var nextCheck = DateTime.Now.AddHours(6);
                    //var nextCheck = DateTime.Now.AddSeconds(30);
                    while (!closeThread)
                    {
                        if(DateTime.Now >= nextCheck)
                        {
                            CheckLicense();

                            nextCheck = DateTime.Now.AddHours(6);
                            //nextCheck = DateTime.Now.AddSeconds(30);
                        }

                        Thread.Sleep(60 * 1000);
                    }

                }).Start();
            }
        }

        internal void CheckLicense()
        {
            LicCheckResult = LicenseManager.CheckOnlineLicense();
            if (LicCheckResult == LicenseCheckResult.Error)
            {
                EnvironmentManager.Instance.Log(false, "TryggRetail License", "Online check failed. Checking Offline..");
                LicCheckResult = LicenseManager.CheckOfflineLicense(LicenseManager.DefaultLicensePath);
            }
            EnvironmentManager.Instance.Log(false, "TryggRetail License", "License check result: " + LicCheckResult.ToString());

            if (LicCheckResult == LicenseCheckResult.Valid && !isRunning)
            {
                EnvironmentManager.Instance.Log(false, "TryggRetail License", "Starting services.");
                StartServices();
            }

            if (LicCheckResult == LicenseCheckResult.Valid || LicCheckResult == LicenseCheckResult.Expired)
            {
                MyLicenseInfo = LicenseManager.GetLicenseInfo();
                if (MyLicenseInfo == null)
                    MyLicenseInfo = LicenseManager.GetLicenseInfoOffline(LicenseManager.DefaultLicensePath);
            }

            if (LicCheckResult != LicenseCheckResult.Valid && isRunning)
                StopServices();
        }

        private object LicenseCommunication(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            Admin.LicenseCommunication licCom = null;
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(message.Data as string)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                licCom = (Admin.LicenseCommunication)bf.Deserialize(ms);
            }

            //var licCom = message.Data as TryggRetail.Admin.LicenseCommunication;

            switch (licCom.LicenseComType)
            {
                case Admin.LicenseComType.LicenseInfoRequest:

                    var response = new Admin.LicenseCommunication()
                    {
                        LicenseComType = Admin.LicenseComType.LicenseInfoResponse,
                        MessageData = MyLicenseInfo
                    };
                    string responseSer = "";
                    using (MemoryStream ms = new MemoryStream())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(ms, response);
                        ms.Position = 0;
                        byte[] buffer = new byte[(int)ms.Length];
                        ms.Read(buffer, 0, buffer.Length);
                        responseSer = Convert.ToBase64String(buffer);
                    }
                    _messageCommunication.TransmitMessage(new Message(LicenseCommunicationFilter, responseSer), message.ExternalMessageSourceEndPoint, null, null);

                    break;

                case Admin.LicenseComType.LicenseActivationRequest:

                    var result = LicenseManager.ActivateLicense(licCom.MessageData as string);
                    EnvironmentManager.Instance.Log(false, "TryggRetail", "License activation result: " + result.ToString());
                    //LicenseManager.CheckOnlineLicense();
                    CheckLicense();
                    break;

                case Admin.LicenseComType.LicenseRefreshRequest:
                    CheckLicense();
                    var response2 = new Admin.LicenseCommunication()
                    {
                        LicenseComType = Admin.LicenseComType.LicenseInfoResponse,
                        MessageData = MyLicenseInfo
                    };
                    string responseSer2 = "";
                    using (MemoryStream ms = new MemoryStream())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(ms, response2);
                        ms.Position = 0;
                        byte[] buffer = new byte[(int)ms.Length];
                        ms.Read(buffer, 0, buffer.Length);
                        responseSer2 = Convert.ToBase64String(buffer);
                    }
                    _messageCommunication.TransmitMessage(new Message(LicenseCommunicationFilter, responseSer2), message.ExternalMessageSourceEndPoint, null, null);
                    break;
            }

            return null;
        }

        private void StartServices()
        {
            isRunning = true;

            msgComObjects.Clear();
            msgComObjects.AddRange(new object[]
                {
                    _messageCommunication.RegisterCommunicationFilter(GetClientApprovalRequest, new CommunicationIdFilter(TryggRetailDefinition.ClientApprovalFilter)),
                    _messageCommunication.RegisterCommunicationFilter(NewAlarmMessageHandler, new CommunicationIdFilter(MessageId.Server.NewAlarmIndication))
                });
            //EnvironmentManager.Instance.Log(false, "TryggRetail", "Registered events");
        }

        private void StopServices()
        {
            isRunning = false;

            foreach (var obj in msgComObjects)
            {
                _messageCommunication.UnRegisterCommunicationFilter(obj);
            }

            EnvironmentManager.Instance.Log(false, "TryggRetail", "Unregistered events");

        }

        public override void Close()
        {
            closeThread = false;
            EnvironmentManager.Instance.UnRegisterReceiver(_licComObj);
            StopServices();
        }

        private object NewAlarmMessageHandler(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            // if license is active blabla
            EnvironmentManager.Instance.Log(false, "TryggRetail", "New Alarm. Broadcasting to all clients.");
            _messageCommunication.TransmitMessage(new Message(TryggRetailDefinition.NewAlarmFilter, message.Data), null, null, null);

            return null;
        }

        private object GetClientApprovalRequest(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            var current = GetCurrentUsers;
            EnvironmentManager.Instance.Log(false, "TryggRetail", "Client request approval, current clients: " + current + "/" + MyLicenseInfo.MaxCurrentUsers);

            if (current < MyLicenseInfo.MaxCurrentUsers)
            {
                EnvironmentManager.Instance.Log(false, "TryggRetail", "Client request approval: Approved");
                _messageCommunication.TransmitMessage(new VideoOS.Platform.Messaging.Message(TryggRetailDefinition.ApproveClientFilter, MyLicenseInfo), message.ExternalMessageSourceEndPoint, null, null);
            }
            else
                EnvironmentManager.Instance.Log(false, "TryggRetail", "Client request approval: Denied");


            return null;
        }

        private int GetCurrentUsers
        {
            get
            {
                ConcurrentLicenseUsed[] used = EnvironmentManager.Instance.LicenseManager.ConcurrentLicenseManager.
                    GetConcurrentUseList(
                    TryggRetailDefinition.TryggRetailPluginId, TryggRetailDefinition.LicenseID);
                return used.Length;
            }
        }

        public override Guid Id => TryggRetailDefinition.TryggRetailEventServerBackgroundPlugin;

        public override string Name => "TryggRetail Event Server Background Plugin";

        public override List<EnvironmentType> TargetEnvironments => new List<EnvironmentType>() { EnvironmentType.Service };
    }
}
