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
using VideoOS.Platform.Messaging;

namespace TryggLarm.Background
{
    class LicenseWrapper
    {
        public event EventHandler<LicenseCheckResult> OnLicenseCheck;
        public event EventHandler<LicenseCheckResult> OnLicenseExpired;
        private LicenseManager licenseManager;

        MessageCommunication messageCommunication;
        LicenseInfo latestLicenseInfo;

        public string LicenseKey { get; set; }

        bool closeThread = false;

        public LicenseWrapper(string mguid, MessageCommunication msgCom)
        {
            this.messageCommunication = msgCom;
            licenseManager = new LicenseManager(mguid, "trygglarm");
        }

        public void Init()
        {
            try
            {
                CheckLicense();
            }
            catch(Exception ex)
            {
                EnvironmentManager.Instance.Log(true, "TryggLarm", ex.ToString());
            }
            new Thread(CheckLicenseThread).Start();
        }

        LicenseCheckResult CheckLicense(bool notify = true)
        {
            var checkRes = licenseManager.CheckOnlineLicense();
            //if (checkRes == LicenseCheckResult.Error)
            //{
            //    checkRes = licenseManager.CheckOfflineLicense(licenseManager.DefaultLicensePath);
            //}

            if (checkRes == LicenseCheckResult.Valid || checkRes == LicenseCheckResult.Expired)
            {
                latestLicenseInfo = licenseManager.GetLicenseInfo();
                LicenseKey = latestLicenseInfo.Value;
            }
            if(notify)
                OnLicenseCheck?.Invoke(this, checkRes);

            return checkRes;
        }

        void CheckLicenseThread()
        {
            try
            {
                while(true)
                {
                    var nextCheck = DateTime.Now.AddHours(24);
                    //var nextCheck = DateTime.Now.AddSeconds(30);
                    while (!closeThread)
                    {
                        if (DateTime.Now >= nextCheck)
                        {
                            var res = CheckLicense();

                            if (res != LicenseCheckResult.Valid)
                                OnLicenseExpired?.Invoke(this, res);
                            nextCheck = DateTime.Now.AddHours(6);
                            //nextCheck = DateTime.Now.AddSeconds(30);
                        }

                        Thread.Sleep(60 * 1000);
                    }
                }
            }
            catch(Exception ex)
            {
                EnvironmentManager.Instance.Log(true, "TryggLarm", ex.ToString());
            }
        }

        public object LicenseCommunicationObj(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            LicenseCommunication licCom = Packer.Deserialize(message.Data as string) as LicenseCommunication;

            LicenseCommunication response = null;
            switch (licCom.LicenseComType)
            {
                case LicenseComType.LicenseInfoRequest:

                    response = new LicenseCommunication()
                    {
                        LicenseComType = LicenseComType.LicenseInfoResponse,
                        MessageData = latestLicenseInfo
                    };
                    //string responseSer = "";
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    BinaryFormatter bf = new BinaryFormatter();
                    //    bf.Serialize(ms, response);
                    //    ms.Position = 0;
                    //    byte[] buffer = new byte[(int)ms.Length];
                    //    ms.Read(buffer, 0, buffer.Length);
                    //    responseSer = Convert.ToBase64String(buffer);
                    //}
                    messageCommunication.TransmitMessage(new Message(TryggLarmDefinition.LicenseCommunicationFilter, Packer.Serialize(response)), message.ExternalMessageSourceEndPoint, null, null);

                    break;

                case LicenseComType.LicenseActivationRequest:

                    //var result = LicenseManager.ActivateLicense(licCom.MessageData as string);
                    ActivateLicense(licCom.MessageData as string);
                    //EnvironmentManager.Instance.Log(false, "TryggRetail", "License activation result: " + result.ToString());
                    break;

                case LicenseComType.LicenseRefreshRequest:
                    CheckLicense();
                    response = new LicenseCommunication()
                    {
                        LicenseComType = LicenseComType.LicenseInfoResponse,
                        MessageData = latestLicenseInfo
                    };
                    //string responseSer2 = "";
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    BinaryFormatter bf = new BinaryFormatter();
                    //    bf.Serialize(ms, response2);
                    //    ms.Position = 0;
                    //    byte[] buffer = new byte[(int)ms.Length];
                    //    ms.Read(buffer, 0, buffer.Length);
                    //    responseSer2 = Convert.ToBase64String(buffer);
                    //}
                    //var serializedResponse =;

                    messageCommunication.TransmitMessage(new Message(TryggLarmDefinition.LicenseCommunicationFilter, Packer.Serialize(response)), message.ExternalMessageSourceEndPoint, null, null);
                    break;
            }

            return null;
        }

        public void ActivateLicense(string license)
        {
            licenseManager.ActivateLicense(license);
            CheckLicense();
        }

        public void Close()
        {
            closeThread = true;
        }
    }

    public class LicenseCheckEventArgs : EventArgs
    {
        public LicenseCheckResult LicenseCheckResult { get; set; }

        public LicenseCheckEventArgs(LicenseCheckResult result)
        {
            this.LicenseCheckResult = result;
        }

    }

    [Serializable]
    public class LicenseCommunication
    {
        public LicenseComType LicenseComType { get; set; }
        public object MessageData { get; set; }
    }

    [Serializable]
    public enum LicenseComType
    {
        LicenseInfoRequest,
        LicenseInfoResponse,
        LicenseActivationRequest,
        LicenseActivationResponse,
        LicenseRefreshRequest,
    }

    public static class Packer
    {
        public static object Deserialize(string serializedObject)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(serializedObject)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }

        public static string Serialize(object objectToSerialize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, objectToSerialize);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }
    }
}
