using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XProtectWebStream.Global;
using XProtectWebStream.Global.Auth;

namespace XProtectWebStream.Web
{
    internal class WebSessionsManager
    {
        private List<WebSession> Sessions { get; set; } = new List<WebSession>();
        private object sessionLock = new object();
        private bool running = false;
        
        internal WebSessionsManager()
        {
            running = true;

            new Thread(CheckSessionExpiration) { IsBackground = true, Name = "Web session expiration check" }.Start();
        }

        private void CheckSessionExpiration()
        {
            while(running)
            {
                lock(sessionLock)
                {
                    Sessions.RemoveAll(session => session.IsExpired);
                }

                Thread.Sleep(1000);
            }
        }

        internal WebSession NewSession(TimeSpan validity)
        {
            var sessionId = WebSession.GenerateSessionId();
            var webSession = new WebSession(sessionId, validity);
            lock(sessionLock)
            {
                Sessions.Add(webSession);
            }
            return webSession;
        }

        internal WebSession GetSession(string sessionId)
        {
            WebSession session = null;
            lock(sessionLock)
            {
                session = Sessions.FirstOrDefault(sess => sess.SessionId == sessionId && sess.IsExpired == false);
            }
            return session;
        }

        public void Close()
        {
            running = false;
        }
    }

    internal class WebSession
    {
        internal string SessionId { get; set; }
        internal DateTime Expires { get; set; }
        internal DateTime Created { get; set; }
        internal TimeSpan Validity { get; set; }
        internal DateTime LastSessionRenewal { get; set; }

        internal IPEndPoint RemoteEndpoint { get; set; }

        internal bool IsExpired => Expires < DateTime.Now;

        private List<CameraToken> AccessToTokens { get; set; } = new List<CameraToken>();

        internal BankIDLogin BankIDLogin { get; set; }

        //private Dictionary<string, string> EnteredPasswords { get; set; } = new Dictionary<string, string>();

        internal WebSession(string sessionId, TimeSpan validity)
        {
            SessionId = sessionId;
            Created = DateTime.Now;
            Expires = DateTime.Now.Add(validity);
            LastSessionRenewal = DateTime.Now;
            Validity = validity;
        }
        
        internal bool RenewSessionId()
        {
            var now = DateTime.Now;
            Expires = now.Add(Validity);

            if (LastSessionRenewal.AddMinutes(5) < now)
            {
                SessionId = GenerateSessionId();
                LastSessionRenewal = now;
                return true;
            }
            return false;
        }

        internal bool HaveAccessTo(CameraToken camToken)
        {
            return AccessToTokens.Contains(camToken);
        }

        internal void AddAccessToToken(CameraToken camToken)
        {
            if (!HaveAccessTo(camToken))
                AccessToTokens.Add(camToken);
        }


        //internal string GetEnteredPassword(string token)
        //{
        //    if (EnteredPasswords.ContainsKey(token))
        //        return EnteredPasswords[token];
        //    else return null;
        //}

        //internal void AddEnteredPassword(string token, string password)
        //{
        //    EnteredPasswords[token] = password;
        //}

        internal static string GenerateSessionId() => Shared.Utils.GetTimeToken();
    }
}
