using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using VideoOS.Platform.Login;
using VideoOS.Platform.UI;
using MimeKit;
using MailKit.Net.Smtp;
using VideoOS.Platform.Messaging;
using System.Runtime.InteropServices;
using InSupport.ProblemSolver;

namespace InSupport.Client
{
    public partial class InSupportWorkSpaceViewItemUserControl : ViewItemUserControl
    {
        const float _VER = 1.382f;

        DateTime internetDateTime;
        bool haveClickedInEmailForm = true;

        string newsUrl = "http://83.233.164.117/pluginhttp/news.php";

        private object _themeChangedReceiver;
        private object ThemeChangedIndicationHandler(
                VideoOS.Platform.Messaging.Message message, FQID destination, FQID source)
        {
            newsFeedBrowser.Navigate(newsUrl + "?theme=" + message.Data.ToString());
            ChangeLogo(message.Data.ToString());
            return null;
        }

        public InSupportWorkSpaceViewItemUserControl()
        {
            InitializeComponent();

            _themeChangedReceiver = EnvironmentManager.Instance.RegisterReceiver(
                new MessageReceiver(ThemeChangedIndicationHandler),
                new MessageIdFilter(MessageId.SmartClient.ThemeChangedIndication));

            if (Properties.Settings.Default.ControlPanelEnabled)
            {
                Button button = new Button();
                button.Size = new Size((int)((float)button.Size.Width * 1.5f), button.Size.Height);
                button.Text = "Axis Kontrollpanel";
                button.Click += (s, ea) =>
                {
                    new InSupportControlPanel().Show();
                };
                flowLayoutPanel1.Controls.Add(button);
            }

            ClientControl.Instance.RegisterUIControlForAutoTheming(this);

            emailTele.Click += new EventHandler(emailBox_Click);
            emailCompany.Click += new EventHandler(emailBox_Click);
            emailName.Click += new EventHandler(emailBox_Click);
            emailEmail.Click += new EventHandler(emailBox_Click);
            emailBody.Click += new EventHandler(emailBox_Click);

            checkForUpdateWorker.RunWorkerCompleted += CheckForUpdateWorker_RunWorkerCompleted;
        }



        public override void Init()
        {
        }

        public override void Close()
        {
            EnvironmentManager.Instance.UnRegisterReceiver(_themeChangedReceiver);
        }

        /// <summary>
        /// Do not show the sliding toolbar!
        /// </summary>
        public override bool ShowToolbar
        {
            get { return false; }
        }

        private void ViewItemUserControlClick(object sender, EventArgs e)
        {
            FireClickEvent();
        }

        private void ViewItemUserControlDoubleClick(object sender, EventArgs e)
        {
            FireDoubleClickEvent();
        }

        void ChangeLogo(string theme)
        {
            if (theme == "Dark")
            {
                logoPicBox.Image = Resources.Resource1.logo_darktheme;
            }
            else
            {
                logoPicBox.Image = Resources.Resource1.logo_lighttheme2;
            }
        }

        private void InSupportWorkSpaceViewItemUserControl_Load(object sender, EventArgs e)
        {
            var theme = ClientControl.Instance.Theme.ThemeType.ToString();
            newsFeedBrowser.Navigate(newsUrl + "?theme=" + theme);
            ChangeLogo(theme);

            try
            {
                internetDateTime = NTPTime.GetNetworkTime();
            }
            catch
            {
                updateTimeTimer.Stop();
                ntpTimeLabel.Text = "Unable to connect :(";

                if (Properties.Settings.Default.Debug)
                    throw;
            }

            using (var webClient = new WebClient())  // get external IP
            {
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler((s, args) => DownloadStringCompleted(s, args, DownloadedStringType.ExternalIP)) ;
                //webClient.DownloadStringCompleted += GetExternalIp_DownloadStringCompleted;
                webClient.DownloadStringAsync(new Uri("http://83.233.164.117/pluginhttp/getmyip.php"));
            }

            using (var webClient = new WebClient())  // critical info
            {
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler((s, args) => DownloadStringCompleted(s, args, DownloadedStringType.CriticalInfo)) ;
                webClient.DownloadStringAsync(new Uri("http://83.233.164.117/pluginhttp/criticalInfo.txt"));
            }

            label3.Left = (this.ClientSize.Width - label3.Width) / 2;

            checkForUpdateWorker.RunWorkerAsync();

            groupBox2.Text = string.Format("InSupport TryggC Plugin v{0}", _VER.ToString().Replace(',', '.'));

            var info = LoginSettingsCache.GetLoginSettings(EnvironmentManager.Instance.MasterSite);

            pcNameLabel.Text = Environment.MachineName;
            var user = info.UserName;
            if (!info.IsBasicUser)
                user += " (Windows user)";
            else
                user += " (Basic user)";
            userInfoLabel.Text = user;
            
            localIpLabel.Text = GetLocalIPAddress();

            //var message = new SmartClientMessageData();
        }

        private void DownloadStringCompleted(object s, DownloadStringCompletedEventArgs e, DownloadedStringType stringType)
        {
            switch(stringType)
            {
                case DownloadedStringType.CriticalInfo:
                    if (e.Error == null)
                        criticalInformationLabel.Text = e.Result;
                    else
                        criticalInformationLabel.Text = "";
                    break;

                case DownloadedStringType.ExternalIP:
                    if (e.Error == null)
                        externalIpLabel.Text = e.Result;
                    else
                        externalIpLabel.Text = "Error getting IP";
                    break;
            }
        }

        string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "No IP";
        }

        private void emailSendBtn_Click(object sender, EventArgs e)
        {
            if (emailBody.Text == "" ||
                emailEmail.Text == "" || emailEmail.Text == "E-Post" ||
                emailName.Text == "" || emailName.Text == "Namn" ||
                emailCompany.Text == "" || emailCompany.Text == "Företag" ||
                emailTele.Text == "" || emailTele.Text == "Telefon nr")
            {
                MessageBox.Show("Fyll i alla fält!");
                return;
            }

            //AllocConsole();
            using (var mailClient = new SmtpClient())
            {
                try
                {
                    mailClient.Connect("mail.teleoffice.nu", 25, false);
                    //mailClient.Connect("mailcluster.loopia.se", 587, false);
                }
                catch
                {
                    if (Properties.Settings.Default.Debug)
                        throw;

                    MessageBox.Show("Kunde inte skicka mailet.. Kontakta oss direkt istället!");
                    return;
                }



                var mail = new MimeMessage();
                 
                mail.From.Add(new MailboxAddress(emailName.Text, emailEmail.Text));  // aswell as this
                mail.To.Add(new MailboxAddress("support@insupport.se"));

                //mail.Sender = new MailboxAddress(emailEmail.Text);  this was the sender email
                mail.Sender = new MailboxAddress("service.forward@insupport.se"); 

                mail.Subject = "Supportärende från " + Environment.UserName + " (" + Environment.MachineName + ")";

                var body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "<p>" + emailName.Text + " (" + emailCompany.Text + ")<br>" + emailEmail.Text + "<br>" + emailTele.Text + "<br><br>Meddelande:<br>" + emailBody.Text + "<br><br>" + localIpLabel.Text + "<br>" + userInfoLabel.Text + "</p>" };

                mail.Priority = MessagePriority.Urgent;
                MimePart sysInfoAttachment = new MimePart("text/plain", "charset=utf-8")
                {
                    ContentObject = new ContentObject(GenerateSystemInfo()),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = "systeminfo.txt"
                };
                var multipart = new Multipart("mixed");
                multipart.Add(body);
                multipart.Add(sysInfoAttachment);

                if (camSnapIsAttached)
                {
                    camImageStream.Position = 0;
                    MimePart camSnapAttachment = new MimePart(new ContentType("image", "jpeg"))
                    {
                        ContentObject = new ContentObject(camImageStream),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = camName + ".jpeg"
                    };
                    multipart.Add(camSnapAttachment);
                }

                mail.Body = multipart;
                

                mailClient.MessageSent += MailClient_MessageSent;
                try
                {
                    mailClient.Authenticate("service.forward@insupport.se", "vallgatan5bsolna");
                }
                catch
                {
                    if (Properties.Settings.Default.Debug)
                        throw;
                }
                //mailClient.Authenticate("service.forward@trygghetscenter.se", "vallgatan5bsolna");
                emailSendBtn.Enabled = false;
                mailClient.Send(mail);
                mailClient.Disconnect(true);
            }
        }

        private void MailClient_MessageSent(object sender, MailKit.MessageSentEventArgs e)
        {
            emailSendBtn.Enabled = true;
            if(Properties.Settings.Default.Debug)
            {
                MessageBox.Show(e.Response);
            }

            MessageBox.Show("Ditt email har skickats!\nVi hör av oss så fort vi kan.", "InSupport", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearEmailForm();
        }

        private MemoryStream GenerateSystemInfo()
        {
            MemoryStream textStream = new MemoryStream();
            StreamWriter tw = new StreamWriter(textStream);
            tw.WriteLine(SysInfo.GetMACAddress2());
            tw.WriteLine(SysInfo.SystemInformation());
            tw.WriteLine();
            tw.WriteLine(SysInfo.DeviceInformation(1));
            tw.WriteLine();
            tw.WriteLine(SysInfo.DeviceInformation(2));
            tw.WriteLine();
            tw.WriteLine(SysInfo.DeviceInformation(3));
            
            textStream.Position = 0;
            return textStream;
        }

        private void emailBox_Click(object sender, EventArgs e)
        {
            if(!haveClickedInEmailForm)
            {
                if(MessageBox.Show("Innan du skickar, ta en titt på Problemlösningen för de enklaste lösningarna! Om inte det löser problemen, var god maila oss.\n\nVill du öppna den nu?", "InSupport",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    helpBtn.PerformClick();
                }

                haveClickedInEmailForm = true;
            }
        }

        #region Contact-fields

        void ClearEmail()
        {
            if (emailEmail.Text == "E-Post")
                emailEmail.Text = "";
        }

        private void emailEmail_Click(object sender, EventArgs e)
        {
            ClearEmail();
        }

        void ClearName()
        {
            if (emailName.Text == "Namn")
                emailName.Text = "";
        }

        private void emailName_Click(object sender, EventArgs e)
        {
            ClearName();
        }

        void ClearCompany()
        {
            if (emailCompany.Text == "Företag")
                emailCompany.Text = "";
        }

        private void emailCompany_Click(object sender, EventArgs e)
        {
            ClearCompany();
        }

        void ClearTele()
        {
            if (emailTele.Text == "Telefon nr")
                emailTele.Text = "";
        }

        private void emailTele_Click(object sender, EventArgs e)
        {
            ClearTele();
        }

        private void emailCompany_Leave(object sender, EventArgs e)
        {
            if (emailCompany.Text == "")
                emailCompany.Text = "Företag";
        }

        private void emailTele_Leave(object sender, EventArgs e)
        {
            if (emailTele.Text == "")
                emailTele.Text = "Telefon nr";
        }

        private void emailName_Leave(object sender, EventArgs e)
        {
            if (emailName.Text == "")
                emailName.Text = "Namn";
        }

        private void emailEmail_Leave(object sender, EventArgs e)
        {
            if (emailEmail.Text == "")
                emailEmail.Text = "E-Post";
        }

        private void emailCompany_Enter(object sender, EventArgs e)
        {
            ClearCompany();
        }

        private void emailTele_Enter(object sender, EventArgs e)
        {
            ClearTele();
        }

        private void emailName_Enter(object sender, EventArgs e)
        {
            ClearName();
        }

        private void emailEmail_Enter(object sender, EventArgs e)
        {
            ClearEmail();
        }

        #endregion

        private void logoPicBox_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.insupport.se/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //new ProblemBrowser().Show();
            new ProblemSolver2().Show();
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Milestone Smart Client kommer att stängas för en snabb uppdatering.\nVill du fortsätta?\n","InSupport Plugin Update", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(Application.StartupPath + @"\MIPPlugins\InSupport\PluginUpdater.exe");
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Verb = "runas";
                p.StartInfo.Arguments = "\"" + Application.StartupPath + "\\Client.exe\"";
                p.Start();

                Application.Exit();
            }
        }


        private bool updateAvalible = false;
        private void checkForUpdateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            updateAvalible = new UpdateCheck().NewUpdateReleased(_VER);
        }

        private void CheckForUpdateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(updateAvalible)
            {
                updateLabel.Text = string.Format("Det finns en ny uppdatering tillgänglig.");
                updateLabel.BackColor = Color.Red;
                updateBtn.Enabled = true;
            }
            else
            {
                updateLabel.Text = "Du har den senaste versionen.";
                updateLabel.BackColor = Color.Transparent;
            }
            //throw new NotImplementedException();
        }

        bool camSnapIsAttached = false;
        MemoryStream camImageStream;
        string camName;
        private void CloseCamImage()
        {
            if (camImageStream == null)
                return;
            camImageStream.Close();
            camImageStream.Dispose();
            camImageStream = null;
        }

        private void attachCamBtn_Click(object sender, EventArgs e)
        {
            if(camSnapIsAttached)
            {
                CloseCamImage();

                attachCamBtn.Text = "Bifoga kamerabild";
                attachedLabel.Text = "";
                camSnapIsAttached = false;
                return;
            }

            using (ItemPickerForm itemPicker = new ItemPickerForm())
            {
                itemPicker.StartPosition = FormStartPosition.CenterParent;
                itemPicker.KindFilter = Kind.Camera;
                itemPicker.ShowDisabledItems = false;
                //itemPicker.AutoAccept = true;
                itemPicker.Init(Configuration.Instance.GetItems());
                if (itemPicker.ShowDialog() == DialogResult.OK)
                {
                    var camera = itemPicker.SelectedItem;

                    camName = camera.Name;
                    using (var camSnap = new GetCameraSnapshotForm(camera))
                    {
                        if (camSnap.ShowDialog() == DialogResult.OK)
                        {
                            camImageStream = camSnap.CameraStream;
                            attachedLabel.Text = "Bifogad kamerabild";
                            camSnapIsAttached = true;
                            attachCamBtn.Text = "Ta bort";
                        }
                        else
                            CloseCamImage();
                    }
                }
            }
        }

        private void ClearEmailForm()
        {
            emailBody.Text = "";
            emailName.Text = "Namn";
            emailCompany.Text = "Företag";
            emailTele.Text = "Telefon nr";
            emailEmail.Text = "E-Post";
            attachCamBtn.Text = "Bifoga kamerabild";

            attachedLabel.Text = "";
            camSnapIsAttached = false;
            CloseCamImage();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            internetDateTime = internetDateTime.AddSeconds(1);
            ntpTimeLabel.Text = internetDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://get.teamviewer.com/zrngg8w");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://get.teamviewer.com/insupport");
        }
    }
    internal enum DownloadedStringType
    {
        CriticalInfo,
        ExternalIP
    }
}
