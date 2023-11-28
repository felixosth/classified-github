using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Client;

namespace TryggSync.Server
{
    [Serializable]
    public class CameraPlaybackInfo
    {
        public string SerializedCamera { get; set; }
        public DateTime PlaybackTime { get; set; }
        public double PlaybackSpeed { get; set; }
        public PlaybackController.PlaybackModeType PlaybackMode { get; set; }

        public CameraPlaybackInfo(string serializedCam, DateTime time, double speed, PlaybackController.PlaybackModeType mode)
        {
            this.SerializedCamera = serializedCam;
            this.PlaybackTime = time;
            this.PlaybackSpeed = speed;
            this.PlaybackMode = mode;
        }
    }
}
