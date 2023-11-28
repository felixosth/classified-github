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
using VideoOS.Platform.License;
using InSupport_LicenseSystem;
using TryggRetail.Background;
using VideoOS.Platform.Messaging;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace TryggRetail.Admin
{
    public partial class HelpPage : ItemNodeUserControl
    {
        object _licComObj;
        MessageCommunication _messageCommunication;

        LicenseInfo myLicenseInfo;
        string myLicenseID;
        Guid myPluginID;
        /// <summary>
        /// User control to display help page
        /// </summary>	
        public HelpPage(string licenseId, Guid pluginId)
        {
            this.myLicenseID = licenseId;
            myPluginID = pluginId;
            InitializeComponent();
        }

        /// <summary>
        /// Display information from or about the Item selected.
        /// </summary>
        /// <param name="item"></param>
        public override void Init(Item item)
        {
            CheckCurrentUsers();

            //UpdateLicInfo();

            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            _messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);
            _licComObj = _messageCommunication.RegisterCommunicationFilter(LicenseCommunication, new CommunicationIdFilter(EventServerBackgroundPlugin.LicenseCommunicationFilter));


            RequestLicenseInfo();
            //_messageCommunication.TransmitMessage(new VideoOS.Platform.Messaging.Message(EventServerBackgroundPlugin.LicenseCommunicationFilter,
            //    new LicenseCommunication() { LicenseComType = LicenseComType.LicenseInfoRequest }), null, null, null);
        }

        private void RequestLicenseInfo()
        {
            var licCom = new LicenseCommunication() { LicenseComType = LicenseComType.LicenseInfoRequest };


            _messageCommunication.TransmitMessage(new VideoOS.Platform.Messaging.Message(EventServerBackgroundPlugin.LicenseCommunicationFilter, SerializeLicCom(licCom)), null, null, null);
        }

        private string SerializeLicCom(Admin.LicenseCommunication licToSerialize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, licToSerialize);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }

        private void CheckCurrentUsers()
        {
            curClientsListView.Items.Clear();
            try
            {
                ConcurrentLicenseUsed[] used = EnvironmentManager.Instance.LicenseManager.ConcurrentLicenseManager.
                    GetConcurrentUseList(
                    TryggRetailDefinition.TryggRetailPluginId, TryggRetailDefinition.LicenseID);

                if (used != null)
                {
                    foreach (ConcurrentLicenseUsed u in used)
                    {
                        curClientsListView.Items.Add(u.IPAddress + "   " + u.MachineName + "   " + u.WindowsUser);
                    }
                }
            }
            catch
            {
                curClientsListView.Items.Add("Error fetching users");
            }
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
                Admin.LicenseCommunication licCom = null;
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(message.Data as string)))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    licCom = (Admin.LicenseCommunication)bf.Deserialize(ms);
                }
                switch (licCom.LicenseComType)
                {
                    case LicenseComType.LicenseInfoResponse:

                        myLicenseInfo = licCom.MessageData as LicenseInfo;

                        UpdateLicInfo();

                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Close any session and release any resources used.
        /// </summary>
        public override void Close()
        {

        }

        private void HelpPage_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckCurrentUsers();
        }

        private void activateOnlineBtn_Click(object sender, EventArgs e)
        {
            _messageCommunication.TransmitMessage(new VideoOS.Platform.Messaging.Message(EventServerBackgroundPlugin.LicenseCommunicationFilter,
                SerializeLicCom(new LicenseCommunication() { LicenseComType = LicenseComType.LicenseActivationRequest, MessageData = licKeyTxtBox.Text })), null, null, null);

            new Thread(() =>
            {
                Thread.Sleep(1000);
                RequestLicenseInfo();

            }).Start();
        }

        private void UpdateLicInfo()
        {
            var lic = myLicenseInfo;
            if (lic != null)
            {
                licStatusLabel.Text = lic.ExpirationDate.Date >= DateTime.Now.Date ? "Valid" : "Expired";

                if (licStatusLabel.Text != "None")
                {
                    foreach (Control control in licActivationGrp.Controls)
                    {
                        control.Enabled = false;
                    }
                }

                licenseInfoBox.Lines = new string[]
                {
                    "Expiration Date: " + lic.ExpirationDate.ToShortDateString(),
                    "License key: " + lic.Value,
                    "Customer: " + lic.Customer,
                    "Site: " + lic.Site,
                    "Max current users: " + lic.MaxCurrentUsers
                };
            }
            else
                licStatusLabel.Text = "No license";
        }

        private void refreshLicenseBtn_Click(object sender, EventArgs e)
        {
            _messageCommunication.TransmitMessage(new VideoOS.Platform.Messaging.Message(EventServerBackgroundPlugin.LicenseCommunicationFilter,
    SerializeLicCom(new LicenseCommunication() { LicenseComType = LicenseComType.LicenseRefreshRequest })), null, null, null);
        }
    }
}
