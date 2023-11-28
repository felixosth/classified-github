using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;

namespace TryggSync.Client
{
    [Serializable]
    public class TryggSyncMsgCommandRequest
    {
        public MsgCommandTypes MsgCommandType { get; set; }
        public object[] RequestParameters { get; set; }

        public TryggSyncMsgCommandRequest(MsgCommandTypes msgCommand, params object[] parameters)
        {
            this.MsgCommandType = msgCommand;
            this.RequestParameters = parameters;
        }

        public void SendRequest(MessageCommunication msgCom, FQID endPoint = null)
        {
            msgCom.TransmitMessage(new Message(TryggSyncDefinition.TryggSyncMsgCommandRequest, Packer.SerializeObject(this)), msgCom.ServerEndPointFQID, endPoint, null);
        }

        public override string ToString() => MsgCommandType.ToString();
    }



    [Serializable]
    public enum MsgCommandTypes
    {
        OwnershipStatus = 0,
        TakeOwnership = 1,
        LeaveOwnership = 2,
        UpdateViewList = 3,
        CameraPlayback = 4,
        PainStroke = 5,
        JustConnectedInfo = 6
    }
}
