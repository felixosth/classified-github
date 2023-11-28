using InSupport_LicenseSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TryggLogin;
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

        bool closeThread = false;

        public LicenseWrapper(string mguid)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            licenseManager = new LicenseManager(mguid, "trygglogin");
        }

        public void Init()
        {
            CheckLicense();
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
                    var nextCheck = Debugger.IsAttached ? DateTime.Now.AddSeconds(30) : DateTime.Now.AddHours(4);

                    while (!closeThread)
                    {
                        if (DateTime.Now >= nextCheck)
                        {
                            var res = CheckLicense();

                            if (res != LicenseCheckResult.Valid)
                                OnLicenseExpired?.Invoke(this, res);

                            nextCheck = Debugger.IsAttached ? DateTime.Now.AddSeconds(30) : DateTime.Now.AddHours(4);
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
