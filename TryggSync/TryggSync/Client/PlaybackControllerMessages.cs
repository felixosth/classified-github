using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace TryggSync.Client
{
    public class PlaybackControllerMessages
    {
        public event EventHandler<PlaybackController.PlaybackModeType> OnPlaybackIndicationChanged;
        public event EventHandler<DateTime> OnPlaybackCurrentTimeChanged;


        List<object> comObjects;

        public PlaybackControllerMessages()
        {
            comObjects = new List<object>()
            {
                EnvironmentManager.Instance.RegisterReceiver(HandlePlaybackIndication, new MessageIdFilter(MessageId.SmartClient.PlaybackIndication)),
                //EnvironmentManager.Instance.RegisterReceiver(HandlePlaybackCurrentTimeIndication, new MessageIdFilter(MessageId.SmartClient.PlaybackCurrentTimeIndication))
            };
        }

        public void Close()
        {
            foreach(var msgComObj in comObjects)
            {
                EnvironmentManager.Instance.UnRegisterReceiver(msgComObj);
            }
        }

        private object HandlePlaybackCurrentTimeIndication(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            OnPlaybackCurrentTimeChanged?.Invoke(this, (DateTime)message.Data);

            return null;
        }

        private object HandlePlaybackIndication(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            var data = message.Data as PlaybackCommandData;

            PlaybackController.PlaybackModeType type = PlaybackController.PlaybackModeType.Stop;
            switch(data.Command)
            {
                case "PlayStop":
                    type = PlaybackController.PlaybackModeType.Stop;
                    break;
                case "PlayForward":
                    type = PlaybackController.PlaybackModeType.Forward;
                    break;
            }

            OnPlaybackIndicationChanged?.Invoke(this, type);
            return null;
        }
    }
}
