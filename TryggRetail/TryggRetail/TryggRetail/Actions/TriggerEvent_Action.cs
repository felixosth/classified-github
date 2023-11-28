using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Messaging;

namespace TryggRetail.Actions
{
    [Serializable]
    class TriggerEvent_Action : CustomAction
    {
        public string SerializedEvent;

        public override void Execute()
        {
            var eventToTrigger = Item.Deserialize(SerializedEvent);
            EventSource eventSource = new EventSource()
            {
                FQID = eventToTrigger.FQID,
                Name = eventToTrigger.Name
            };
            EventHeader eventHeader = new EventHeader()
            {
                ID = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                Message = "External Event",
                Name = "External Event",
                Source = eventSource
            };

            EventData eventData = new EventData()
            {
                EventHeader = eventHeader
            };

            EnvironmentManager.Instance.SendMessage(
                new VideoOS.Platform.Messaging.Message(MessageId.Server.NewEventCommand) { Data = eventData });
        }
    }
}
