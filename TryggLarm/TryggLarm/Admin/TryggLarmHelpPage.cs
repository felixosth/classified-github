using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using TryggLarm.Background;
using VideoOS.Platform.Messaging;
using InSupport_LicenseSystem;
using System.Threading;

namespace TryggLarm.Admin
{
    public partial class HelpPage : ItemNodeUserControl
    {
        public static Guid TryggLarmSettingsID = new Guid("DAF4F41E-2BD3-40DA-B964-681617EF838D");
        //public static Guid TryggLarmSettingsID = new Guid("DAF4F41E-2BE3-40DA-A964-481617EF838D"); // old
        object _licComObj;
        MessageCommunication _messageCommunication;

        LicenseInfo licenseInfo;
        CustomSettings mySettings;
        /// <summary>
        /// User control to display help page
        /// </summary>	
        public HelpPage()
        {
            InitializeComponent();
            var item = Configuration.Instance.GetOptionsConfiguration(TryggLarmSettingsID, false);
            socketOptionsBox.DataSource = Enum.GetNames(typeof(MailKit.Security.SecureSocketOptions));

            try
            {
                if (item != null)
                {
                    //myItem = item;
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(item.InnerText)))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        mySettings = (CustomSettings)bf.Deserialize(ms);
                    }

                    hostBox.Text = mySettings.SMTPHostName;
                    portBox.Text = mySettings.SMTPPort.ToString();
                    emailBox.Text = mySettings.AuthEmail;
                    passwordBox.Text = mySettings.AuthPassword;
                    //useSslChkBox.Checked = mySettings.UseSSL;
                    sendasBox.Text = mySettings.SMS_SendAs;

                    socketOptionsBox.SelectedIndex = (int)mySettings.SocketOptions;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            lastPort = portBox.Text;
        }

        /// <summary>
        /// Display information from or about the Item selected.
        /// </summary>
        /// <param name="item"></param>
        public override void Init(Item item)
        {
            try
            {
                MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
                _messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);
                _licComObj = _messageCommunication.RegisterCommunicationFilter(LicenseCommunication, new CommunicationIdFilter(TryggLarmDefinition.LicenseCommunicationFilter));

                RequestLicenseInfo();
            }
            catch
            {
                MessageBox.Show("Unable to communicate with the Event Server", "Error");


                if (InvokeRequired)
                {
                    this.Invoke(new Action(DisableAll));
                }
                else
                    DisableAll();
            }
        }

        void DisableAll()
        {
            foreach (Control control in this.Controls)
            {
                DisableChildren(control);
                control.Enabled = false;
            }
        }

        void DisableChildren(Control control)
        {
            var children = control.Controls;

            foreach(Control child in children)
            {
                DisableChildren(child);
                child.Enabled = false;
            }
        }


        private void RequestLicenseInfo()
        {
            var licCom = new LicenseCommunication() { LicenseComType = LicenseComType.LicenseInfoRequest };


            _messageCommunication.TransmitMessage(new VideoOS.Platform.Messaging.Message(TryggLarmDefinition.LicenseCommunicationFilter, Packer.Serialize(licCom)), null, null, null);
        }

        private object LicenseCommunication(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    LicenseCommunication(message, dest, source);
                }));
            }
            else
            {
                LicenseCommunication licCom = Packer.Deserialize(message.Data as string) as LicenseCommunication;
                switch (licCom.LicenseComType)
                {
                    case LicenseComType.LicenseInfoResponse:

                        licenseInfo = licCom.MessageData as LicenseInfo;

                        UpdateLicInfo(licenseInfo);

                        break;
                }
            }

            return null;
        }

        private void UpdateLicInfo(LicenseInfo lic)
        {
            if (lic != null)
            {
                licenseStatusLabel.Text = lic.ExpirationDate.Date >= DateTime.Now.Date ? "Valid" : "Expired";

                if (licenseStatusLabel.Text != "None")
                {
                    foreach (Control control in licActivationGrp.Controls)
                    {
                        control.Enabled = false;
                    }
                }

                licenseInfoTxtBox.Lines = new string[]
                {
                    "Expiration Date: " + lic.ExpirationDate.ToShortDateString(),
                    "License key: " + lic.Value,
                    "Customer: " + lic.Customer,
                    "Site: " + lic.Site,
                    "Max current users: " + lic.MaxCurrentUsers
                };
            }
            else
                licenseStatusLabel.Text = "No license";
        }

        /// <summary>
        /// Close any session and release any resources used.
        /// </summary>
        public override void Close()
        {
            _messageCommunication.UnRegisterCommunicationFilter(_licComObj);
        }

        private void HelpPage_Load(object sender, EventArgs e)
        {

        }

        private void sendTestEmailBtn_Click(object sender, EventArgs e)
        {
            new NotificationSender().SendTestEmail(testEmailBox.Text);
            testEmailBox.Text = "";
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (portBox.Text == "")
                if(MessageBox.Show("No port specified", "Error") == DialogResult.OK)
                    return;

            if (mySettings == null)
                mySettings = new CustomSettings();

            mySettings.SMTPHostName = hostBox.Text ;
            mySettings.SMTPPort = int.Parse(portBox.Text);
            mySettings.AuthEmail = emailBox.Text;
            mySettings.AuthPassword = passwordBox.Text;
            //mySettings.UseSSL = useSslChkBox.Checked;
            mySettings.SocketOptions = (MailKit.Security.SecureSocketOptions)socketOptionsBox.SelectedIndex;
            mySettings.SMS_SendAs = sendasBox.Text;

            string serializedSettings = "";
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, mySettings);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                serializedSettings = Convert.ToBase64String(buffer);
            }

            if (serializedSettings != "")
            {
                Configuration.Instance.SaveOptionsConfiguration(TryggLarmSettingsID, false, ToXml("CustomSettingsClass", serializedSettings));
                MessageBox.Show("Settings saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        XmlElement ToXml(string key, string value)
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("root");
            doc.AppendChild(root);
            var child = doc.CreateElement(key);
            child.InnerText = value;
            root.AppendChild(child);
            return root;
        }

        string lastPort;
        private void portBox_TextChanged(object sender, EventArgs e)
        {
            if (portBox.Text == "")
                return;
            try
            {
                lastPort = int.Parse(portBox.Text).ToString();
                //lastPort = portBox.Text;
            }
            catch
            {
                portBox.Text = lastPort;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void activateLicenseBtn_Click(object sender, EventArgs e)
        {
            _messageCommunication.TransmitMessage(new VideoOS.Platform.Messaging.Message(TryggLarmDefinition.LicenseCommunicationFilter,
    Packer.Serialize(new LicenseCommunication() { LicenseComType = LicenseComType.LicenseActivationRequest, MessageData = licenseActTxtBox.Text })), null, null, null);

            new Thread(() =>
            {
                Thread.Sleep(1000);
                RequestLicenseInfo();

            }).Start();
        }
    }

    [Serializable]
    public class CustomSettings
    {
        public string SMTPHostName { get; set; }
        public int SMTPPort { get; set; }
        public MailKit.Security.SecureSocketOptions SocketOptions = MailKit.Security.SecureSocketOptions.None;

        public string AuthEmail { get; set; }
        public string AuthPassword { get; set; }
        //public bool UseSSL { get; set; }

        public string SMS_SendAs { get; set; }
        //public string SMS_CustomerID { get; set; }
        //public string SMS_UniqueGUID { get; set; }
    }
}
