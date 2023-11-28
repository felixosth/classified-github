using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;
using VideoOS.Platform.UI;

namespace InSupport.Client
{
    public partial class InSupportControlPanel : Form
    {
        private MessageCommunication _mc;


        public InSupportControlPanel()
        {
            InitializeComponent();

            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            _mc = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);
            _mc.RegisterCommunicationFilter(IPAddressResponseHandler, new CommunicationIdFilter(MessageId.Server.GetIPAddressResponse));

            ClientControl.Instance.RegisterUIControlForAutoTheming(this);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (ItemPickerForm itemPick = new ItemPickerForm())
            {
                itemPick.StartPosition = FormStartPosition.CenterParent;
                itemPick.KindFilter = Kind.Camera;
                itemPick.ShowDisabledItems = true;
                itemPick.Init(Configuration.Instance.GetItems());
                if(itemPick.ShowDialog() == DialogResult.OK)
                {
                    _mc.TransmitMessage(new VideoOS.Platform.Messaging.Message(VideoOS.Platform.Messaging.MessageId.Server.GetIPAddressRequest, itemPick.SelectedItem.FQID), null, null, null);
                }
            }
        }
        private object IPAddressResponseHandler(VideoOS.Platform.Messaging.Message message, FQID destination, FQID sender)
        {
            string ip = (string)message.Data;
            ip = ip.Replace("http://", "").Replace("/", "");
            using (var enterCreds = new AxisCamRestartForm(ip))
            {
                if (enterCreds.ShowDialog() == DialogResult.OK)
                {

                }
            }

            //MessageBox.Show(ip);
            return null;
        }

    }
}
