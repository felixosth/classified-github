using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;

namespace TryggSync.Client
{
    public partial class MessageTester: Form
    {
        //MessageCommunication msgCom;
        object msgObj;
        public MessageTester()
        {
            InitializeComponent();

            //MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            //msgCom = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            //msgObj = msgCom.RegisterCommunicationFilter(HandleMessages, null);
            msgObj = EnvironmentManager.Instance.RegisterReceiver(HandleMessages, null);
        }

        private object HandleMessages(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            if(InvokeRequired)
            {
                logBox.Invoke(new Action(() => HandleMessages(message, dest, source)));
                return null;
            }

            var data = message.Data == null ? "no data" : message.Data.ToString();


            logBox.AppendText(message.MessageId + ": " + data + Environment.NewLine);

            return null;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //msgCom.UnRegisterCommunicationFilter(msgObj);
            EnvironmentManager.Instance.UnRegisterReceiver(msgObj);

            base.OnClosing(e);
        }
    }
}
