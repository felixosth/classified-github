using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InSupportDriftMilestonePlugin
{
    public class EventTimestamp
    {
        public DateTime LastTimestamp { get; set; }
        public float MaxAllowedInactivityHours { get; set; }
        [JsonIgnore]
        public bool Alarm => DateTime.Now > LastTimestamp + TimeSpan.FromHours(MaxAllowedInactivityHours);
        public EventTimestamp(float hours)
        {
            LastTimestamp = DateTime.Now;
            MaxAllowedInactivityHours = hours;
        }
    }

    public class CameraEventTimestamps
    {
        public string CameraName;
        public Dictionary<string, EventTimestamp> Events { get; set; }

        public CameraEventTimestamps(string cameraName, Dictionary<string, EventTimestamp> events)
        {
            CameraName = cameraName;
            Events = events;
        }

        public CameraEventTimestamps(string cameraName)
        {
            CameraName = cameraName;
            Events = new Dictionary<string, EventTimestamp>();
        }

        public static Dictionary<string, CameraEventTimestamps> FromConfig(MilestoneEventConfig config)
        {
            var result = new Dictionary<string, CameraEventTimestamps>();
            foreach (var cam in config.CamerasAndEvents)
            {
                result[cam.CameraId] = new CameraEventTimestamps(cam.CameraName);
                foreach (var e in cam.Events)
                {
                    result[cam.CameraId].Events[e.Name] = new EventTimestamp(
                        e.MaxAllowedInactivityHours ?? config.MaxAllowedInactivityHours
                        );
                }
            }
            return result;
        }

        public void UpdateTimestamp(string eventName)
        {
            Events[eventName].LastTimestamp = DateTime.Now;
        }
    }
}
