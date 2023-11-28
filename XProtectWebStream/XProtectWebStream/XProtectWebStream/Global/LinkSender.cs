using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XProtectWebStream.Global
{
    internal class LinkSender
    {
        internal event EventHandler<string> OnError;
        private const string _TryggPortaEmailURL = "https://portal.tryggconnect.se/api/email.php";
        private const string _TryggPortaSMSURL = "https://portal.tryggconnect.se/api/sms.php";

        private string TryggPortalLicense { get; set; }
        private string EmailSender { get; set; }
        private string SmsSender { get; set; }

        internal LinkSender(string tryggPortalLicense, string emailSender, string smsSender)
        {
            if(string.IsNullOrEmpty(tryggPortalLicense))
                OnError?.Invoke(this, "No email license, email disabled");

            EmailSender = emailSender;
            SmsSender = smsSender;
            TryggPortalLicense = tryggPortalLicense;
        }
        

        internal void SendEmail(string subject, string message, params string[] recipients)
        {
            if (string.IsNullOrEmpty(TryggPortalLicense))
                return;

            using (var wc = new WebClient())
            {
                string query = "?";
                foreach(var rec in recipients)
                {
                    query += (query.Length != 1 ? "&" : "") +  "recipient[]=" + HttpUtility.UrlEncode(rec);
                }
                query += "&from=" + EmailSender + "&subject=" + HttpUtility.UrlEncode(subject) + "&message=" + HttpUtility.UrlEncode(message) + "&license=" + TryggPortalLicense;

                wc.DownloadString(_TryggPortaEmailURL + query);
            }
        }

        internal void SendSMS(string message, string recipient)
        {
            if (string.IsNullOrEmpty(TryggPortalLicense))
                return;

            using (var wc = new WebClient())
            {
                string query = string.Format("?reciever={0}&message={1}&license={2}&sender={3}", HttpUtility.UrlEncode(recipient), HttpUtility.UrlEncode(message), HttpUtility.UrlEncode(TryggPortalLicense), HttpUtility.UrlEncode(SmsSender));

                var res = wc.DownloadString(_TryggPortaSMSURL + query);
            }
        }


    }
}
