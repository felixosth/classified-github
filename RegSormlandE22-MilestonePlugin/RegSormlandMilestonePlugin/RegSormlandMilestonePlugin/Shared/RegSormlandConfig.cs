using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;

namespace RegSormlandMilestonePlugin.Shared
{
    [Serializable]
    public class RegSormlandConfig
    {
        internal const string _CFG_KEY = "RegSormlandConfigKey";
        public Dictionary<string, CameraEvent> CamerasEventsDictionary { get; set; }

        public RegSormlandConfig()
        {
            CamerasEventsDictionary = new Dictionary<string, CameraEvent>();
        }

        public string Serialize() => Helper.ToBase64(this);
        public static RegSormlandConfig Deserialize(string str) => Helper.FromBase64<RegSormlandConfig>(str);
    }

    [Serializable]
    public class CameraEvent
    {
        public string EventFQIDXml { get; set; }
        public FQID EventToFQID() => EventFQIDXml != null ? new FQID(EventFQIDXml) : null;

        public string ButtonEventFQIDXml { get; set; }
        public FQID ButtonEventToFQID() => ButtonEventFQIDXml != null ? new FQID(ButtonEventFQIDXml) : null;
        public string ButtonEventDisplayText { get; set; }

        public int TimeToDisplay { get; set; }

    }
}
