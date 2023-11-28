using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DigitalRevision.Global
{
    internal static class EmailSender
    {
        private const string license = "0FAD3732-FDDC-4D97-BEE4-CE5BBA1B6487"; // Hard coded license for emails

        internal static Task SendEmailWithHTMLAsync(string email, string subject, string htmlMessage, string sender)
        {
            return Notify(sender, email, subject, htmlMessage, useHtml: true);
        }

        internal static Task SendEmailAsync(string email, string subject, string message, string sender)
        {
            return Notify(sender, email, subject, message, useHtml: false);
        }

        private static async Task Notify(string sender, string recipient, string subject, string message, bool useHtml = false)
        {
            var url = new Uri("https://portal.tryggconnect.se/api/email.php");

            using (var httpClient = new HttpClient())
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("license", license),
                    new KeyValuePair<string, string>("recipient", recipient),
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("message", message),
                    new KeyValuePair<string, string>("from", sender),
                    new KeyValuePair<string, string>("useHTML", useHtml.ToString().ToLower()),
                });

                var response = await httpClient.PostAsync(url, content);

                var responseStr = await response.Content.ReadAsStringAsync();
            }
        }
    }
}
