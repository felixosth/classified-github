using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using TryggLarm.Admin;
using TryggLarm.Nodes;
using VideoOS.Platform;
using VideoOS.Platform.Data;

namespace TryggLarm.Background
{
    class NotificationSender
    {
        public NotificationSender()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
            (sender, cert, chain, sslPolicyErrors) => true;
        }

        public void SendEmail(EmailRecipientNode emailNode, Alarm alarm, List<MemoryStream> alarmImages = null)
        {
            if(emailNode == null)
            {
                EnvironmentManager.Instance.Log(true, "Trygglarm", "Email node is null", null);
                return;
            }

            var settings = GetCustomSettings();
            if (settings == null)
            {
                EnvironmentManager.Instance.Log(true, "Trygglarm", "No saved configuration!", null);
                return;
            }
            using (var mailClient = new SmtpClient())
            {
                try
                {
                    mailClient.Connect(settings.SMTPHostName, settings.SMTPPort, settings.SocketOptions);
                }
                catch
                {
                    EnvironmentManager.Instance.Log(true, "Trygglarm", "Could not connect to MailServer!", null);
                    return;
                }

                var mail = new MimeMessage();
                mail.From.Add(new MailboxAddress("TryggLarm", "support@insupport.se"));
                mail.Sender = new MailboxAddress(settings.AuthEmail);
                mail.To.Add(new MailboxAddress(emailNode.EmailAddress));

                List<FormatKey> formatKeys = new List<FormatKey>();
                //using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(emailNode.Properties["formatKeys"])))
                //{
                //    BinaryFormatter bf = new BinaryFormatter();
                //    formatKeys = (List<FormatKey>)bf.Deserialize(ms);
                //}
                formatKeys.Add(new FormatKey("%alarm%", alarm.EventHeader.Name));

                mail.Subject = ReplaceKeywords(emailNode.SubjectFormatting, formatKeys);
                var body = new TextPart(MimeKit.Text.TextFormat.Html);
                body.Text = ReplaceKeywords(emailNode.BodyFormatting, formatKeys);


                if (emailNode.AttachCameraImage)
                {
                    var multipart = new Multipart("mixed");

                    if (alarmImages == null)
                        alarmImages = CreateImages(alarm, (double)emailNode.AlarmTimeOffset);
                    //var imgs = CreateImages(alarm, (double)emailNode.AlarmTimeOffset);
                    multipart.Add(body);
                    int i = 1;
                    foreach (var img in alarmImages)
                    {
                        MimePart alarmImgAttachment = new MimePart(new ContentType("image", "jpeg"))
                        {
                            Content = new MimeContent(img),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = i + ".jpeg"
                        };
                        multipart.Add(alarmImgAttachment);
                        i++;
                    }

                    mail.Body = multipart;
                }
                else
                    mail.Body = body;

                mail.Priority = MessagePriority.Urgent;

                mailClient.MessageSent += MailClient_MessageSent;

                try
                {
                    mailClient.Authenticate(settings.AuthEmail, settings.AuthPassword);
                }
                catch/*(Exception ex)*/
                {
                    //EnvironmentManager.Instance.Log(false, "TryggLarm", "Error authenticating: \r\n" + ex.ToString(), null);
                }

                mailClient.Send(mail);

                //emailNode.LastActiveDate = DateTime.Now;
                mailClient.Disconnect(true);
            }
        }

        private void MailClient_MessageSent(object sender, MailKit.MessageSentEventArgs e)
        {
            EnvironmentManager.Instance.Log(false, "TryggLarm", "Mail successfully sent.", null);
        }

        public List<MemoryStream> CreateImages(Alarm alarm, double offsetSeconds)
        {
            var msList = new List<MemoryStream>();

            var timeOfImg = alarm.EventHeader.Timestamp.ToLocalTime();
            timeOfImg = timeOfImg.AddSeconds(offsetSeconds);  //offset
            
            EnvironmentManager.Instance.Log(false, "TryggLarm", "Searching for images around " + timeOfImg, null);

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string myFolder = Path.Combine(appData, "TryggLarm.GetAlarmImage");
            Directory.CreateDirectory(myFolder);
                                                                                   
            var lines = new List<string>();
            foreach(var cam in alarm.ReferenceList)
            {
                lines.Add(Configuration.Instance.GetItem(cam.FQID).Serialize());
            }
            var p = Path.Combine(myFolder, "refCams.txt");
            File.WriteAllLines(p, lines.ToArray());


            string uniqueKey = RandomString(random.Next(10));
            var imageGetter = @"C:\Program Files\Milestone\MIPPlugins\TryggLarm\GetAlarmImage.exe";
            var procStartInfo = new ProcessStartInfo(imageGetter, "\"" + timeOfImg +"\"" + " " + p +" \"" + uniqueKey + "\"");
            procStartInfo.WorkingDirectory = Path.GetDirectoryName(imageGetter);
            //procStartInfo.UseShellExecute = true;

            Thread.Sleep((int)offsetSeconds);
            var proc = Process.Start(procStartInfo);

            proc.WaitForExit(10 * 1000);

            var imgFiles = Directory.GetFiles(myFolder, "*.jpeg");
            foreach(var file in imgFiles)
            {
                var fileName = Path.GetFileName(file);
                //EnvironmentManager.Instance.Log(false, "TryggLarm", "StartsWith: " + uniqueKey + " - File: " + fileName + "(" + fileName.StartsWith(uniqueKey) + ")", null);

                if (fileName.StartsWith(uniqueKey))
                {
                    EnvironmentManager.Instance.Log(false, "TryggLarm", "Found " + fileName, null);

                    var ms = new MemoryStream(File.ReadAllBytes(file));
                    ms.Position = 0;
                    msList.Add(ms);
                }
                File.Delete(file);
            }

            return msList;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Sends a test mail. Should only be called from UI
        /// </summary>
        /// <param name="mailTo"></param>
        public void SendTestEmail(string mailTo)
        {
            var settings = GetCustomSettings();
            if (settings == null)
            {
                MessageBox.Show("No configuration saved. Make sure to save your changes.", "Error");
                return;
            }
            var log = Path.Combine(Path.GetTempPath(), "mailLog.txt");
            using (var mailClient = new SmtpClient(new ProtocolLogger(log)))
            {
                try
                {
                    mailClient.Connect(settings.SMTPHostName, settings.SMTPPort, settings.SocketOptions);
                    //try
                    //{
                    //}
                    //catch
                    //{
                    //    MessageBox.Show("Could not connect to mailserver!", "Error");
                    //    //EnvironmentManager.Instance.Log(true, "Trygglarm", "Could not connect to MailServer!", null);
                    //    return;
                    //}

                    //mailClient.SslProtocols = System.Security.Authentication.SslProtocols.
                    mailClient.MessageSent += MailClient_TestMsgSent;

                    var mail = new MimeMessage();
                    //mail.From.Add(new MailboxAddress("TryggLarm", "support@insupport.se"));

                    mail.Sender = new MailboxAddress(settings.AuthEmail);

                    mail.From.Add(new MailboxAddress("TryggLarm", "support@insupport.se"));
                    mail.To.Add(new MailboxAddress(mailTo));

                    mail.Subject = "TryggLarm testmail";
                    var body = new TextPart(MimeKit.Text.TextFormat.Html);
                    body.Text = "<p>Testmail.</p>";

                    mail.Body = body;
                    mail.Priority = MessagePriority.Normal;

                    mailClient.MessageSent += MailClient_MessageSent;


                    try
                    {
                        mailClient.Authenticate(settings.AuthEmail, settings.AuthPassword);
                    }
                    catch(Exception ex)
                    {
                        EnvironmentManager.Instance.Log(false, "TryggLarm", "Error authenticating: \r\n" + ex.ToString(), null);
                    }
                    mailClient.Send(mail);
                    //try
                    //{
                    //    mailClient.Authenticate(settings.AuthEmail, settings.AuthPassword);
                    //    mailClient.Send(mail);
                    //}
                    //catch (Exception ex)
                    //{
                    //    //EnvironmentManager.Instance.Log(false, "TryggLarm", "Error authenticating: \r\n" + ex.ToString(), null);
                    //    MessageBox.Show(ex.ToString());
                    //}

                    mailClient.Disconnect(true);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    if(File.Exists(log))
                        Process.Start("notepad.exe", log);
                }
            }
        }

        private void MailClient_TestMsgSent(object sender, MailKit.MessageSentEventArgs e)
        {
            MessageBox.Show("Message sent!", "Success!");
        }

        string ReplaceKeywords(string original, List<FormatKey> formatKeys)
        {
            foreach(var keyWord in formatKeys)
            {
                original = original.Replace(keyWord.Key, keyWord.Value);
            }

            return original;
        }

        public string SendSMS(SMSRecipientNode smsNode, Alarm alarm, string mguid, string license)
        {
            if(smsNode == null)
            {
                //EnvironmentManager.Instance.Log(true, "Trygglarm", "SMS node null", null);
                return "SMS node null";
            }

            var settings = GetCustomSettings();
            if (settings == null)
            {
                //EnvironmentManager.Instance.Log(true, "Trygglarm", "No saved configuration!", null);
                return "No saved configuration!";
            }

            List<FormatKey> formatKeys = new List<FormatKey>();
            //using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(smsNode.Properties["formatKeys"])))
            //{
            //    BinaryFormatter bf = new BinaryFormatter();
            //    formatKeys = (List<FormatKey>)bf.Deserialize(ms);
            //}
            formatKeys.Add(new FormatKey("%alarm%", alarm.EventHeader.Name));

            var webClient = new WebClient();

            var msg = ReplaceKeywords(smsNode.Formatting, formatKeys);
            byte[] msgInBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(msg);

            var decoded = Encoding.GetEncoding("ISO-8859-1").GetString(msgInBytes);
            var sender = settings.SMS_SendAs;
            //var customerId = settings.SMS_CustomerID;
            //var uniqueGuid = settings.SMS_UniqueGUID;
            var reciever = smsNode.TelephoneNumber;

            //string finalMsg = Uri.EscapeDataString(decoded);
            string finalMsg = System.Web.HttpUtility.UrlEncode(decoded, Encoding.GetEncoding("ISO-8859-1"));
                
            var uri = "https://portal.tryggconnect.se/api/sms.php";

            try
            {
                //using (var client = new WebClient())
                using (var client = new HttpClient())
                {
                    //var response = client.UploadValues(url, values);
                    //EnvironmentManager.Instance.Log(false, "TryggLarm", response.);

                    var values = new List<KeyValuePair<string, string>>();

                    // add values to data for post
                    values.Add(new KeyValuePair<string, string>("mguid", mguid));
                    values.Add(new KeyValuePair<string, string>("license", license));
                    values.Add(new KeyValuePair<string, string>("sender", sender));
                    values.Add(new KeyValuePair<string, string>("reciever", reciever));
                    values.Add(new KeyValuePair<string, string>("message", msg));


                    FormUrlEncodedContent content = new FormUrlEncodedContent(values);

                    // Post data
                    var result = client.PostAsync(uri, content).Result;
                    return result.Content.ReadAsStringAsync().Result;
                }
            }
            catch(Exception ex)
            {
                //EnvironmentManager.Instance.Log(true, "TryggLarm", "Error connecting to SMS service\r\n" + ex.ToString(), null);
                return ex.ToString();
            }
        }

        CustomSettings GetCustomSettings()
        {
            var item = Configuration.Instance.GetOptionsConfiguration(HelpPage.TryggLarmSettingsID, false);

            if (item == null)
                return null;

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(item.InnerText)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (CustomSettings)bf.Deserialize(ms);
            }
        }
    }
}
