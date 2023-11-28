using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XProtectWebStream.Database;

namespace XProtectWebStream.Global
{
    internal class TokenManager
    {
        private object tokensLock = new object();
        private List<CameraToken> tokens = new List<CameraToken>();
        private bool running;
        LocalDatabase dbLogger;

        internal event EventHandler<string> OnLog;

        public TokenManager(LocalDatabase dbLogger)
        {
            this.dbLogger = dbLogger;
            running = true;

            new Thread(CheckTokenExpirations) { IsBackground = true, Name = "Token expiration check" }.Start();
        }

        private void CheckTokenExpirations()
        {
            while(running)
            {
                lock(tokensLock)
                {
                    foreach(var token in tokens)
                    {
                        token.CheckExpirationDate();
                    }
                }

                Thread.Sleep(100);
            }
        }

        internal CameraToken CreateNewLiveToken(string cameraId, DateTime expirationDate, string password, string comment, bool requireBankId, int[] accessGroups)
        {
            var token = GetValidToken();
            CameraToken camToken = null;

            lock (tokensLock)
            {
                camToken = CameraToken.NewLiveCameraToken(token, cameraId, expirationDate, password, comment, requireBankId, accessGroups);
                camToken.TokenExpired += CamToken_TokenExpired;
                camToken.OnLog += CamToken_OnLog;
                OnLog?.Invoke(this, $"Token created for {cameraId}, expires at {expirationDate}: {camToken.Token}");
                tokens.Add(camToken);
            }
            return camToken;
        }

        internal CameraToken CreateNewLiveToken(string cameraId, TimeSpan expirationAfterActivation, string password, string comment, bool requireBankId, int[] accessGroups)
        {
            var token = GetValidToken();
            CameraToken camToken = null;

            lock (tokensLock)
            {
                camToken = CameraToken.NewLiveCameraToken(token, cameraId, expirationAfterActivation, password, comment, requireBankId, accessGroups);
                camToken.TokenExpired += CamToken_TokenExpired;
                camToken.OnLog += CamToken_OnLog;
                OnLog?.Invoke(this, $"Token created for {cameraId}, expires after {expirationAfterActivation}: {camToken.Token}");
                tokens.Add(camToken);
            }
            return camToken;
        }

        internal CameraToken CreateNewRecordedToken(string filePath, DateTime expirationDate, string password, string comment, bool requireBankId, int[] accessGroups)
        {
            var token = GetValidToken();
            CameraToken camToken = null;

            lock (tokensLock)
            {
                camToken = CameraToken.NewRecordedCameraToken(token, filePath, expirationDate, password, comment, requireBankId, accessGroups);
                camToken.TokenExpired += CamToken_TokenExpired;
                camToken.OnLog += CamToken_OnLog;
                OnLog?.Invoke(this, $"Token created for {filePath}, expires at {expirationDate}: {camToken.Token}");
                tokens.Add(camToken);
            }

            return camToken;
        }

        internal CameraToken CreateNewRecordedToken(string filePath, TimeSpan expirationAfterActivation, string password, string comment, bool requireBankId, int[] accessGroups)
        {
            var token = GetValidToken();
            CameraToken camToken = null;
            lock (tokensLock)
            {
                camToken = CameraToken.NewRecordedCameraToken(token, filePath, expirationAfterActivation, password, comment, requireBankId, accessGroups);
                camToken.TokenExpired += CamToken_TokenExpired;
                camToken.OnLog += CamToken_OnLog;
                OnLog?.Invoke(this, $"Token created for {filePath}, expires after {expirationAfterActivation}: {camToken.Token}");
                tokens.Add(camToken);
            }

            return camToken;
        }


        private void CamToken_OnLog(object sender, string e)
        {
            OnLog?.Invoke(this, e);
        }

        private void CamToken_TokenExpired(object sender, EventArgs e)
        {
            lock(tokensLock)
            {
                var cameraToken = sender as CameraToken;
                tokens.Remove(cameraToken);

                OnLog?.Invoke(this, $"Token '{cameraToken.Token}' expired");
            }
        }

        internal CameraToken GetCameraToken(string token)
        {
            var camToken = tokens.FirstOrDefault(t => t.Token == token);

            if(camToken != null && camToken.HaveExpired == false)
                return camToken;

            return null;
        }

        private string GetValidToken()
        {
            var token = UrlToken.GenerateToken();
            while (tokens.FirstOrDefault(t => t.Token == token) != null)
                token = UrlToken.GenerateToken();
            return token;
        }

        public void Close()
        {
            running = false;
        }
    }

    internal class CameraToken
    {
        internal event EventHandler TokenExpired;

        internal bool HaveExpired { get; private set; }

        internal string Token { get; private set; }

        internal string Password { get; private set; }

        internal TokenVideoType VideoType { get; set; }

        internal string CameraId { get; private set; }

        internal string CameraName { get; set; }
        internal string Comment { get; set; }
        internal string CreatedBy { get; set; }

        internal string VideoFile { get; private set; }

        internal DateTime ExpirationDate { get; private set; }

        internal event EventHandler<string> OnLog;

        internal bool IsActivated { get; private set; } = false;
        private TimeSpan ExpirationAfterConnect { get; set; }
        private DateTime ActivateDate { get; set; }
        
        private bool ExpireAfterConnect { get; set; }

        //internal bool RequireBankID { get; set; }

        internal AuthType Authentication { get; set; }
        internal int[] AccessGroupIds { get; set; } // Only for bankid


        private CameraToken()
        {
        }

        internal static CameraToken NewLiveCameraToken(string token, string cameraId, TimeSpan expirationAfterConnect, string password, string comment, bool requireBankId, int[] accessGroups)
        {
            var cameraToken = new CameraToken()
            {
                Token = token,
                CameraId = cameraId,
                ExpirationAfterConnect = expirationAfterConnect,
                ExpireAfterConnect = true,
                VideoType = TokenVideoType.Live,
                Password = password,
                Comment = comment,
                //RequireBankID = requireBankId,
                Authentication = requireBankId ? AuthType.BankID : (password != null ? AuthType.Password : AuthType.None),
                AccessGroupIds = accessGroups
            };
            return cameraToken;
        }

        internal static CameraToken NewLiveCameraToken(string token, string cameraId, DateTime expiration, string password, string comment, bool requireBankId, int[] accessGroups)
        {
            var cameraToken = new CameraToken()
            {
                Token = token,
                CameraId = cameraId,
                ExpirationDate = expiration,
                ExpireAfterConnect = false,
                VideoType = TokenVideoType.Live,
                Password = password,
                Comment = comment,
                //RequireBankID = requireBankId,
                Authentication = requireBankId ? AuthType.BankID : (password != null ? AuthType.Password : AuthType.None),
                AccessGroupIds = accessGroups
            };
            return cameraToken;
        }

        internal static CameraToken NewRecordedCameraToken(string token, string filePath, TimeSpan expirationAfterConnect, string password, string comment, bool requireBankId, int[] accessGroups)
        {
            var cameraToken = new CameraToken()
            {
                Token = token,
                VideoFile = filePath,
                ExpirationAfterConnect = expirationAfterConnect,
                ExpireAfterConnect = true,
                VideoType = TokenVideoType.Recorded,
                Password = password,
                Comment = comment,
                //RequireBankID = requireBankId,
                Authentication = requireBankId ? AuthType.BankID : (password != null ? AuthType.Password : AuthType.None),
                AccessGroupIds = accessGroups
            };
            return cameraToken;
        }

        internal static CameraToken NewRecordedCameraToken(string token, string filePath, DateTime expiration, string password, string comment, bool requireBankId, int[] accessGroups)
        {
            var cameraToken = new CameraToken()
            {
                Token = token,
                VideoFile = filePath,
                ExpirationDate = expiration,
                ExpireAfterConnect = false,
                VideoType = TokenVideoType.Recorded,
                Password = password,
                Comment = comment,
                //RequireBankID = requireBankId,
                Authentication = requireBankId ? AuthType.BankID : (password != null ? AuthType.Password : AuthType.None),
                AccessGroupIds = accessGroups
            };
            return cameraToken;
        }

        internal void Revoke()
        {
            HaveExpired = true;
            ThreadPool.QueueUserWorkItem((state) =>
            {
                TokenExpired?.Invoke(this, new EventArgs());
            });
        }

        internal void ActivateToken()
        {
            if(!IsActivated)
            {
                IsActivated = true;
                ActivateDate = DateTime.Now;
                OnLog?.Invoke(this, $"Token {Token} is now activated");
            }
        }

        internal void CheckExpirationDate()
        {
            if (HaveExpired == false && 
                ((ExpireAfterConnect == false && ExpirationDate < DateTime.Now) || 
                (IsActivated && ExpireAfterConnect == true && ActivateDate.Add(ExpirationAfterConnect) < DateTime.Now)))
            {
                HaveExpired = true;
                ThreadPool.QueueUserWorkItem((state) => 
                {
                    TokenExpired?.Invoke(this, new EventArgs());
                });

            }
        }

        internal DateTime GetRelativeExpirationDate()
        {
            if (ExpireAfterConnect)
                return ActivateDate.Add(ExpirationAfterConnect).ToUniversalTime();
            else
                return ExpirationDate.ToUniversalTime();
        }

        internal enum TokenVideoType
        {
            Live = 1,
            Recorded = 2
        }

        internal enum AuthType
        {
            None,
            Password,
            BankID
        }
    }

    static class UrlToken
    {

        /// <summary>
        /// Generate a fixed length token that can be used in url without endcoding it
        /// </summary>
        /// <returns></returns>
        public static string GenerateToken(int numberOfRandomBytes = 32)
        {
            // get secure array bytes
            byte[] secureArray = GenerateRandomBytes(numberOfRandomBytes);

            // convert in an url safe string
            string urlToken = WebEncoders.Base64UrlEncode(secureArray);

            return urlToken;
        }

        /// <summary>
        /// Generate a cryptographically secure array of bytes with a fixed length
        /// </summary>
        /// <returns></returns>
        private static byte[] GenerateRandomBytes(int length)
        {
            using (System.Security.Cryptography.RNGCryptoServiceProvider provider = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] byteArray = new byte[length];
                provider.GetBytes(byteArray);

                return byteArray;
            }
        }


    }
}
