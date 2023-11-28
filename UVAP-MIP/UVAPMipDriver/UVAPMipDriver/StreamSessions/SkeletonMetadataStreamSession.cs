using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvapShared.Driver;
using UvapShared.Objects;
using VideoOS.Platform.DriverFramework.Data;
using VideoOS.Platform.DriverFramework.Data.Settings;
using VideoOS.Platform.DriverFramework.Managers;
using VideoOS.Platform.DriverFramework.Utilities;
using VideoOS.Platform.Metadata;

namespace UVAPMipDriver
{
    internal class SkeletonMetadataStreamSession : UVAPBaseStreamSession
    {
        IEventManager eventManager;

        string deviceId;
        Consumer consumer;

        DateTime lastHandsUp;

        public SkeletonMetadataStreamSession(Consumer consumer, IEventManager eventManager, ISettingsManager settingsManager, UVAPMipDriverConnectionManager connectionManager, Guid sessionId, string deviceId, Guid streamId, int channel) :
            base(settingsManager, connectionManager, sessionId, deviceId, streamId)
        {
            lastHandsUp = DateTime.MinValue;
            this.consumer = consumer;
            this.deviceId = deviceId;
            Channel = channel;
            this.eventManager = eventManager;
        }

        // The frame data is fetched from the Kafka server in this method. Collect all frames produced by Kafka untill the EndOfFrame is true.
        protected override bool GetLiveFrameInternal(TimeSpan timeout, out BaseDataHeader header, out byte[] data)
        {
            header = null;

            var skeletonPoints = new List<Skeleton>();
            DateTime dt;
            
            while (true)
            {
                var consumption = consumer.Consume();
                var skeleton = Skeleton.FromString(consumption.Value);

                if (!skeleton.EndOfFrame)
                {
                    skeletonPoints.Add(skeleton);

                    if(lastHandsUp.AddSeconds(10) < DateTime.UtcNow && CheckHandsUp(skeleton)) // Only trigger the HandsUp event every 10th second
                    {
                        lastHandsUp = DateTime.UtcNow;
                        eventManager.NewEvent(deviceId, Constants.HandsUpEventReferenceId); // Trigger the event using the builtin EventManager
                    }
                }
                else
                {
                    dt = consumption.Timestamp.UtcDateTime;
                    break;
                }
            }

            data = Encoding.UTF8.GetBytes(
                SlimSkeleton.ToSlim(
                    JsonConvert.SerializeObject(skeletonPoints)
                    ));

            if (data.Length == 0)
            {
                return false;
            }

            header = new MetadataHeader()
            {
                Length = (ulong)data.Length,
                SequenceNumber = _sequence++,
                Timestamp = dt
            };

            return true;
        }

        protected override void DeviceSettingChanged(DeviceSetting setting)
        {
            base.DeviceSettingChanged(setting);
        }

        protected override void InnerClose()
        {
            consumer.Close();
        }


        /// <summary>
        /// Temporary method to trigger HandsUp event if the criteria is reached.
        /// </summary>
        bool CheckHandsUp(Skeleton format)
        {
            var rightWrist = format.Points.FirstOrDefault(p => p.Type == "RIGHT_WRIST");
            var leftWrist = format.Points.FirstOrDefault(p => p.Type == "LEFT_WRIST");
            var shoulder = format.Points.FirstOrDefault(p => p.Type.Contains("SHOULDER"));

            if (rightWrist == null || leftWrist == null || shoulder == null)
                return false;

            return rightWrist.Y < shoulder.Y && leftWrist.Y < shoulder.Y;
        }
    }
}
