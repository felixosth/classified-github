using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;

namespace LoginShared.Network
{
    [Serializable]
    public class MessageData
    {
        public string JsonData { get; set; }

        public Constants.Actions Action { get; set; }

        public FQID Sender { get; set; }

        public MessageData(Constants.Actions action, object data)
        {
            Action = action;
            JsonData = JsonConvert.SerializeObject(data);
        }

        public MessageData(Constants.Actions action)
        {
            Action = action;
        }

        public T Deserialize<T>()
        {
            return JsonConvert.DeserializeObject<T>(JsonData);
        }
    }
}
