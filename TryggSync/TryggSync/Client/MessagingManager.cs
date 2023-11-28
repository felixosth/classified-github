using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using TryggSync.Server;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;

namespace TryggSync.Client
{
    public class MessagingManager
    {
        public event EventHandler<MessageEventArgs> OnOwnershipChanged;
        public event EventHandler<MessageEventArgs> OnViewersChanged;
        public event EventHandler<MessageEventArgs> OnCameraChanged;
        public event EventHandler<MessageEventArgs> OnNewPaintStroke;
        public event EventHandler<MessageEventArgs> OnNewUserConnected;

        private MessageCommunication messageCommunication;
        OperatorUser myUser;
        List<object> msgComObjs;


        public MessagingManager(OperatorUser myUser, MessageCommunication msgCom)
        {
            this.messageCommunication = msgCom;
            this.myUser = myUser;

            msgComObjs = new List<object>()
            {
                messageCommunication.RegisterCommunicationFilter(HandleTryggSyncMsgCommandResponse, new CommunicationIdFilter(TryggSyncDefinition.TryggSyncMsgCommandResponse))
            };
        }

        private object HandleTryggSyncMsgCommandResponse(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            var response = Packer.DeserializeObject(message.Data as string) as TryggSyncMsgCommandResponse;
            OperatorUser op = null;
            var parameters = response.ResponseParameters;

            switch (response.MsgCommandType)
            {
                case MsgCommandTypes.OwnershipStatus:

                    //op = Packer.DeserializeObject(parameters[0] as string) as OperatorUser;
                    op = parameters[0] as OperatorUser;
                    //if (op != null)
                    OnOwnershipChanged?.Invoke(this, new MessageEventArgs(op));

                    break;

                case MsgCommandTypes.UpdateViewList:

                    //var viewers = Packer.DeserializeObject(response.ResponseParameters[0] as string) as List<string>;
                    var viewers = parameters[0] as List<OperatorUser>;
                    OnViewersChanged?.Invoke(this, new MessageEventArgs(viewers));

                    break;

                case MsgCommandTypes.CameraPlayback:

                    OnCameraChanged?.Invoke(this, new MessageEventArgs(parameters[0]));

                    break;

                case MsgCommandTypes.PainStroke:

                    OnNewPaintStroke?.Invoke(this, new MessageEventArgs(parameters[0]));

                    break;

                case MsgCommandTypes.JustConnectedInfo:

                    var sendTo = Item.Deserialize(parameters[0] as string).FQID;
                    OnNewUserConnected?.Invoke(this, new MessageEventArgs(sendTo));
                    break;
            }

            return null;
        }

        public void SendPaintStrokes(MemoryStream strokeStream)
        {
            new TryggSyncMsgCommandRequest(MsgCommandTypes.PainStroke, myUser, strokeStream).SendRequest(messageCommunication);
        }

        public void JustConnectedInfo()
        {
            new TryggSyncMsgCommandRequest(MsgCommandTypes.JustConnectedInfo).SendRequest(messageCommunication);
        }

        public void Close()
        {
            foreach (var obj in msgComObjs)
            {
                messageCommunication.UnRegisterCommunicationFilter(obj);
            }
        }

        public void SendPlaybackInfo(CameraPlaybackInfo info, FQID specialEndPoint = null)
        {
            new TryggSyncMsgCommandRequest(MsgCommandTypes.CameraPlayback, myUser, info).SendRequest(messageCommunication, endPoint:specialEndPoint);
        }

        public void SendToEventServer(MsgCommandTypes msgType, params object[] parameters)
        {
            new TryggSyncMsgCommandRequest(msgType, parameters).SendRequest(messageCommunication);
        }

    }

    public class MessageEventArgs : EventArgs
    {
        public object MessageData { get; set; }
        public MessageEventArgs(object msg)
        {
            this.MessageData = msg;
        }
    }
}
