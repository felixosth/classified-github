using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryggSync.Client;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;

namespace TryggSync.Server
{
    [Serializable]
    public class TryggSyncMsgCommandResponse
    {
        public MsgCommandTypes MsgCommandType { get; set; }

        public object[] ResponseParameters { get; set; }

        public TryggSyncMsgCommandResponse(MsgCommandTypes msgCommand, params object[] parameters)
        {
            this.MsgCommandType = msgCommand;
            this.ResponseParameters = parameters;
        }

        public void Respond(FQID sender, MessageCommunication msgCom)
        {
            if(sender == null) // if sender is null, broadcast to all clients
                sender = new FQID() { ObjectId = Guid.Empty };

            msgCom.TransmitMessage(new Message(TryggSyncDefinition.TryggSyncMsgCommandResponse, Packer.SerializeObject(this)), sender, null, null);
        }

        public override string ToString() => MsgCommandType.ToString();
    }
}
