using InSupport_LicenseSystem;
using OpenALPR_Milestone;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;

namespace InSupport_LicenseSystem
{
    class LicenseWrapper
    {
        public event EventHandler<LicenseCheckResult> OnLicenseCheck;
        public event EventHandler<LicenseCheckResult> OnLicenseExpired;
        private LicenseManager licenseManager;

        public LicenseInfo LatestLicenseInfo { get; set; }
        public LicenseCheckResult LastLicenseCheck { get; set; }

        public string LicenseKey { get; set; }

        int failedChecks = 0;

        bool closeThread = false;

        public LicenseWrapper(string mguid)
        {
            licenseManager = new LicenseManager(mguid, "openalpr");
        }

        public void Init()
        {
            //CheckLicense(false);
            new Thread(CheckLicenseThread).Start();
        }

        public LicenseCheckResult CheckLicense(bool notify = true)
        {
            var checkRes = licenseManager.CheckOnlineLicense();

            if (checkRes == LicenseCheckResult.Valid || checkRes == LicenseCheckResult.Expired)
            {
                LatestLicenseInfo = licenseManager.GetLicenseInfo();
                LicenseKey = LatestLicenseInfo.Value;
            }


            if (notify)
                OnLicenseCheck?.Invoke(this, checkRes);

            LastLicenseCheck = checkRes;
            return checkRes;
        }

        void CheckLicenseThread()
        {
            try
            {
                while (true)
                {
                    var nextCheck = Debugger.IsAttached ? DateTime.Now.AddSeconds(30) : DateTime.Now.AddDays(1);

                    while (!closeThread)
                    {
                        if (DateTime.Now >= nextCheck)
                        {
                            var res = CheckLicense(notify: true);

                            if (res != LicenseCheckResult.Valid)
                            {
                                if (failedChecks > 0)
                                    OnLicenseExpired?.Invoke(this, res);

                                failedChecks++;
                            }
                            else
                                failedChecks = 0;

                            nextCheck = Debugger.IsAttached ? DateTime.Now.AddSeconds(30) : DateTime.Now.AddDays(1);
                        }

                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
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
                        MessageData = LatestLicenseInfo
                    };

                    EnvironmentManager.Instance.PostMessage(new Message(OpenALPR_MilestoneDefinition.LicenseCommunicationFilter, Packer.Serialize(response)), message.ExternalMessageSourceEndPoint, null);

                    break;

                case LicenseComType.LicenseActivationRequest:

                    ActivateLicense(licCom.MessageData as string);

                    break;

                case LicenseComType.LicenseRefreshRequest:
                    CheckLicense();
                    response = new LicenseCommunication()
                    {
                        LicenseComType = LicenseComType.LicenseInfoResponse,
                        MessageData = LatestLicenseInfo
                    };

                    EnvironmentManager.Instance.PostMessage(new Message(OpenALPR_MilestoneDefinition.LicenseCommunicationFilter, Packer.Serialize(response)), message.ExternalMessageSourceEndPoint, null);
                    break;
            }

            return null;
        }

        public LicenseActivationResult ActivateLicense(string license)
        {
            var result = licenseManager.ActivateLicense(license);
            CheckLicense();
            return result;
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
