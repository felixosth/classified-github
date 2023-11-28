using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;

namespace MessagingWrapper
{
    /// <summary>
    /// XProtect Milestone SDK Messaging lib by Felix Östh - InSupport Nätverksvideo 2018-2019
    /// </summary>
    public class Messaging
    {
        /// <summary>
        /// Versionnumber of lib
        /// </summary>
        public const float Version = 0.2f;

        List<object> msgComObjects = new List<object>();
        List<object> enviromentManagerComObjects = new List<object>();
        MessageCommunication msgCom;

        List<RegisteredMessageReciever> recievers = new List<RegisteredMessageReciever>();

        /// <summary>
        /// Constructor for all platforms
        /// </summary>
        /// <param name="msgCom"></param>
        public Messaging(MessageCommunication msgCom)
        {
            this.msgCom = msgCom;
        }


        /// <summary>
        /// For Event Server only, uses EnvironmentManager instead
        /// </summary>
        public Messaging()
        {

        }

        /// <summary>
        /// Broadcast a message to other recievers
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="data"></param>
        /// <param name="destination"></param>
        public void SendMessage(string messageId, MessageData data, FQID destination = null)
        {
            if(msgCom != null)
            {
                msgCom.TransmitMessage(new Message(messageId, Packer.Serialize(data)), destination, null, null);
            }
            else
            {
                EnvironmentManager.Instance.SendMessage(new Message(messageId, Packer.Serialize(data)), destination, null);
            }
        }


        /// <summary>
        /// Register a custom reciever, subscribe to the returned event.
        /// </summary>
        /// <param name="MessageID"></param>
        /// <returns></returns>
        public RegisteredCustomMessageReciever RegisterCustomMessageReciever(string MessageID)
        {
            RegisteredCustomMessageReciever reciever = new RegisteredCustomMessageReciever();
            reciever.MessageID = MessageID;
            recievers.Add(reciever);

            if(msgCom != null)
            {
                msgComObjects.Add(msgCom.RegisterCommunicationFilter(HandleIncomingMessages, new CommunicationIdFilter(MessageID)));
            }
            else
            {
                enviromentManagerComObjects.Add(EnvironmentManager.Instance.RegisterReceiver(HandleIncomingMessages, new MessageIdFilter(MessageID)));
            }

            return reciever;
        }


        /// <summary>
        /// Register a XProtect SDK native reciever, subscribe to the returned event.
        /// </summary>
        /// <param name="MessageID"></param>
        /// <returns></returns>
        public RegisteredNativeMessageReciever RegisterNativeMessageReciever(string MessageID)
        {
            RegisteredNativeMessageReciever reciever = new RegisteredNativeMessageReciever();
            reciever.MessageID = MessageID;
            recievers.Add(reciever);

            if(msgCom != null)
            {
                msgComObjects.Add(msgCom.RegisterCommunicationFilter(HandleIncomingMessages, new CommunicationIdFilter(MessageID)));
            }
            else
            {
                enviromentManagerComObjects.Add(EnvironmentManager.Instance.RegisterReceiver(HandleIncomingMessages, new MessageIdFilter(MessageID)));
            }

            return reciever;
        }

        object HandleIncomingMessages(Message message, FQID dest, FQID source)
        {
            new Thread(() => TriggerMessage(message)).Start();

            return null;
        }

        void TriggerMessage(Message message)
        {
            foreach (var reciever in recievers)
            {
                if (message.MessageId == reciever.MessageID)
                {
                    try
                    {
                        reciever.Trigger(message);
                    }
                    catch(Exception ex)
                    {
                        EnvironmentManager.Instance.Log(true, "TryggLogin", ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Close all recievers
        /// </summary>
        public void Close()
        {
            foreach(var msgComObj in msgComObjects)
            {
                msgCom?.UnRegisterCommunicationFilter(msgComObj);
            }

            foreach(var msgComObj in enviromentManagerComObjects)
            {
                EnvironmentManager.Instance.UnRegisterReceiver(msgComObj);
            }

            recievers.Clear();

            msgCom?.Dispose();
        }
    }


    public class RegisteredMessageReciever
    {
        public string MessageID { get; set; }
        public virtual void Trigger(Message msg)
        {
        }
    }

    public class RegisteredCustomMessageReciever : RegisteredMessageReciever
    {
        public event EventHandler<MessageData> OnMessageRecieved;
        public Message Message { get; set; }

        public override void Trigger(Message msg)
        {
            this.Message = msg;

            var msgData = Packer.Deserialize<MessageData>(msg.Data as string);

            OnMessageRecieved?.Invoke(this, msgData);
        }
    }

    public class RegisteredNativeMessageReciever : RegisteredMessageReciever
    {
        public event EventHandler<Message> OnMessageRecieved;

        public override void Trigger(Message msg)
        {
            OnMessageRecieved?.Invoke(this, msg);
        }
    }

    public static class Packer
    {
        public static T Deserialize<T>(string serializedObject)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(serializedObject)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
            }
        }

        public static string Serialize(object objectToSerialize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, objectToSerialize);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }
    }

    [Serializable]
    public class MessageData
    {
        public MessageData(params object[] parameters)
        {
            this.Parameters = parameters;
        }

        public object[] Parameters { get; set; }
    }
}
