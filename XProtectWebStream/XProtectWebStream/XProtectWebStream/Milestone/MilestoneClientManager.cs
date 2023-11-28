using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;

namespace XProtectWebStream.Milestone
{
    internal class MilestoneClientManager
    {
        object msgComObj1, msgComObj2, msgComObj3;

        MessageCommunication msgCom;

        Dictionary<FQID, string> clients = new Dictionary<FQID, string>();

        internal MilestoneClientManager(MessageCommunication msgCom)
        {
            this.msgCom = msgCom;
            msgComObj1 = RegisterFilter(msgCom, NewEndpointMessageCommunication, MessageCommunication.NewEndPointIndication);
            msgComObj2 = RegisterFilter(msgCom, EndPointCloseMessageCommunication, MessageCommunication.EndPointCloseIndication);
            msgComObj3 = RegisterFilter(msgCom, WhoAreOnlineMessageCommunication, MessageCommunication.WhoAreOnlineResponse);

            msgCom.TransmitMessage(new Message(MessageCommunication.WhoAreOnlineRequest), null, null, null);
        }

        internal string GetClientId(FQID clientFqid)
        {
            string clientId = null;
            clients.TryGetValue(clientFqid, out clientId);
            return clientId;
        }

        internal object RegisterFilter(MessageCommunication msgCom, MessageReceiver messageReceiver, string messageId)
        {
            var comidFilter = new CommunicationIdFilter(messageId);
            var obj = msgCom.RegisterCommunicationFilter(messageReceiver, comidFilter);
            msgCom.WaitForCommunicationFilterRegistration(comidFilter);
            return obj;
        }

        private object NewEndpointMessageCommunication(Message message, FQID dest, FQID source)
        {
            var endPoint = message.Data as EndPointIdentityData;

            if (endPoint != null)
            {
                clients[endPoint.EndPointFQID] = endPoint.IdentityName;
            }

            return null;
        }

        private object EndPointCloseMessageCommunication(Message message, FQID dest, FQID source)
        {
            var endPoint = message.Data as EndPointIdentityData;

            if (endPoint != null)
            {
                clients.Remove(endPoint.EndPointFQID);
            }
            return null;
        }

        private object WhoAreOnlineMessageCommunication(Message message, FQID dest, FQID source)
        {
            var endPointList = message.Data as IEnumerable<EndPointIdentityData>;

            foreach(var endPoint in endPointList)
            {
                clients[endPoint.EndPointFQID] = endPoint.IdentityName;
            }

            return null;
        }

        internal void Close()
        {
            msgCom.UnRegisterCommunicationFilter(msgComObj1);
            msgCom.UnRegisterCommunicationFilter(msgComObj2);
            msgCom.UnRegisterCommunicationFilter(msgComObj3);
        }
    }
}
