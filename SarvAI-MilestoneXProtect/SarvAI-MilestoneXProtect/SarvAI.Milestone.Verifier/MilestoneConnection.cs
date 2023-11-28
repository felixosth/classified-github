using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Messaging;
using VideoOS.Platform.Proxy.AlarmClient;
using static VideoOS.Platform.Messaging.MessageId;

namespace SarvAI.Milestone.Verifier
{
    public class MilestoneConnection
    {
        private object _msgObj1, _msgObj2;
        private MessageCommunication _messageCommunication;
        internal event EventHandler<Alarm> OnNewAlarm;
        internal event EventHandler<ChangedAlarmData> OnChangedAlarm;
        AlarmClientManager _alarmClientManager;


        public MilestoneConnection()
        {
            VideoOS.Platform.SDK.Environment.Initialize();
            VideoOS.Platform.SDK.Export.Environment.Initialize();
        }

        public void Login(bool subscribeToAlarms)
        {
            Uri server = new Uri("http://localhost");


            VideoOS.Platform.SDK.Environment.AddServer(false, server, System.Net.CredentialCache.DefaultNetworkCredentials);
            VideoOS.Platform.SDK.Environment.Login(server);


            if(VideoOS.Platform.SDK.Environment.IsLoggedIn(server) == false)
            {
                throw new Exception("Not connected");
            }
            else
            {
                //_msgObj = VideoOS.Platform.EnvironmentManager.Instance.RegisterReceiver(AlarmMsgReciever, new MessageIdFilter(MessageId.Server.NewAlarmIndication));
                MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
                _messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

                if(subscribeToAlarms)
                {
                    _msgObj1 = _messageCommunication.RegisterCommunicationFilter(NewAlarmMsgReciever, new CommunicationIdFilter(MessageId.Server.NewAlarmIndication), null, EndPointType.Server);
                    _msgObj2 = _messageCommunication.RegisterCommunicationFilter(ChangedAlarmMsgReciever, new CommunicationIdFilter(MessageId.Server.ChangedAlarmIndication), null, EndPointType.Server);
                }

                _alarmClientManager = new AlarmClientManager();
            }
        }

        private IAlarmClient GetDefaultAlarmClient()
        {
            return _alarmClientManager.GetAlarmClient(EnvironmentManager.Instance.MasterSite.ServerId);
        }

        public IAlarmClient GetAlarmClient(FQID fqid)
        {
            return _alarmClientManager.GetAlarmClient(fqid.ServerId);
        }


        internal void SendAlarm(FQID sourceCamera, string message, ushort priority, DateTime date)
        {
            var client = GetDefaultAlarmClient();

            EventSource eventSource = new EventSource() { FQID = sourceCamera };

            EventHeader eventHeader = new EventHeader()
            {
                ID = Guid.NewGuid(),
                Class = "SarvAIAlarm",
                Type = "SarvAI",
                Timestamp = date.ToUniversalTime(),
                Message = message,
                Name = "SarvAI",
                Priority = priority,
                Source = eventSource
            };

            Alarm alarm = new Alarm() { EventHeader = eventHeader };

            client.Add(alarm);

            //EnvironmentManager.Instance.SendMessage(new Message(MessageId.Server.NewAlarmCommand) { Data = alarm });
            //_messageCommunication.TransmitMessage(new Message(Server.NewAlarmCommand, alarm), null, null, null);
        }

        private object NewAlarmMsgReciever(Message msg, FQID fqid1, FQID fqid2)
        {
            var alarm = msg.Data as Alarm;

            if (alarm != null)
            {
                OnNewAlarm?.Invoke(this, alarm);
            }

            return null;
        }
        private object ChangedAlarmMsgReciever(Message msg, FQID fqid1, FQID fqid2)
        {
            ChangedAlarmData changedAlarmData = msg.Data as ChangedAlarmData;

            if (changedAlarmData != null)
            {
                OnChangedAlarm?.Invoke(this, changedAlarmData);
            }

            return null;
        }


        public void Close()
        {
            if(_msgObj1 != null)
                _messageCommunication.UnRegisterCommunicationFilter(_msgObj1);
            if(_msgObj2 != null)
                _messageCommunication.UnRegisterCommunicationFilter(_msgObj2);
            //EnvironmentManager.Instance.UnRegisterReceiver(_msgObj);
            //VideoOS.Platform.SDK.Environment.Logout();
            VideoOS.Platform.SDK.Environment.RemoveAllServers();
        }
    }
}
