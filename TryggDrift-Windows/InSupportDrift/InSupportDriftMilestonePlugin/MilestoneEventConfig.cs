using System.Collections.Generic;

namespace InSupportDriftMilestonePlugin
{
    public class MilestoneEventConfig
    {
        public float MaxAllowedInactivityHours { get; set; } = 24f;
        public MilestoneEventCamera[] CamerasAndEvents { get; set; }
    }

    public class MilestoneEventCamera
    {
        public string CameraId { get; set; }
        public string CameraName { get; set; }
        public List<MilestoneEventCameraEvent> Events { get; set; }
    }

    public class MilestoneEventCameraEvent
    {
        public string Name { get; set; }
        public float? MaxAllowedInactivityHours { get; set; } //If null, use value from MilestoneEventConfig
    }

}
