using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TryggRetail.Actions;

namespace TryggRetail.Admin
{
    [Serializable]
    public class PopupConfig
    {
        public PopupConfig()
        {
            CustomActions = new List<CustomAction>();
        }

        public string AlarmName { get; set; }
        public float AckTime { get; set; }
        public float AlarmPlaybackOffsetBefore { get; set; }
        public float AlarmPlaybackOffsetAfter { get; set; }

        public bool ButtonBlink { get; set; }

        public bool IsFullscreen { get; set; }

        public WindowSize WindowSize;
        public WindowPos WindowPosition;

        public string AlarmSignal { get; set; }
        public bool LoopAlarmSignal { get; set; }

        public bool StopSoundOnExit { get; set; }
        public bool LoopByDefault { get; set; }

        public List<CustomAction> CustomActions { get; set; }

        public string Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }

        public static PopupConfig Deserialize(string serialized)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(serialized)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (PopupConfig)bf.Deserialize(ms);
            }
        }

    }
    [Serializable]
    public struct WindowSize
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }
    [Serializable]
    public struct WindowPos
    {
        public int X { get; set; }
        public int Y { get; set; }
    }


}
