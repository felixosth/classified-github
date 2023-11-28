using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;

namespace LoginShared.Network
{
    public class Messaging
    {
        private MessageCommunication MsgCom { get; set; }

        Dictionary<string, Action<MessageData>> registeredRecievers = new Dictionary<string, Action<MessageData>>();
        Dictionary<string, Action<Message>> registeredNativeRecievers = new Dictionary<string, Action<Message>>();
        Dictionary<string, object> msgComObjects = new Dictionary<string, object>();

        public Messaging()
        {
        }

        public void Init(bool ignoreWait = false)
        {
            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            MsgCom = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);


            if(!ignoreWait)
            {
                var wait = DateTime.Now.AddSeconds(15);
                while (DateTime.Now < wait && !MsgCom.IsConnected)
                {
                    Thread.Sleep(1000);
                }

                if (!MsgCom.IsConnected)
                    throw new Exception("Unable to connect to MsgCom");
            }
        }

        public void Transmit(string messageId, MessageData data, FQID recipient = null)
        {
            if (!MsgCom.IsConnected)
                throw new Exception("Not connected to Milestone");

            MsgCom.TransmitMessage(new Message(messageId, data), recipient, null, null);
        }

        public void RegisterReciever(string messageId, Action<MessageData> action)
        {
            if(!registeredRecievers.ContainsKey(messageId))
            {
                msgComObjects[messageId] = MsgCom.RegisterCommunicationFilter(HandleIncomingMessages, new CommunicationIdFilter(messageId));
            }
            registeredRecievers[messageId] = action;
        }

        public void RegisterNativeReciever(string messageId, Action<Message> action)
        {
            if (!registeredNativeRecievers.ContainsKey(messageId))
            {
                msgComObjects[messageId] = MsgCom.RegisterCommunicationFilter(HandleIncomingMessages, new CommunicationIdFilter(messageId));
            }
            registeredNativeRecievers[messageId] = action;
        }

        public void UnregisterReciever(string messageId)
        {
            if(msgComObjects.ContainsKey(messageId))
            {
                MsgCom.UnRegisterCommunicationFilter(msgComObjects[messageId]);
                msgComObjects.Remove(messageId);
            }

            registeredNativeRecievers.Remove(messageId);
            registeredRecievers.Remove(messageId);
        }

        object HandleIncomingMessages(Message message, FQID dest, FQID source)
        {
            if(registeredRecievers.ContainsKey(message.MessageId) && message.Data is MessageData)
            {
                var msgData = message.Data as MessageData;
                msgData.Sender = message.ExternalMessageSourceEndPoint;
                new Thread(() =>
                {
                    registeredRecievers[message.MessageId](msgData);
                }).Start();
            }
            else if(registeredNativeRecievers.ContainsKey(message.MessageId))
            {
                registeredNativeRecievers[message.MessageId]?.Invoke(message);
            }

            return null;
        }
    }
}
