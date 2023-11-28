using Confluent.Kafka;
using Newtonsoft.Json;
using REIDShared;
using REIDShared.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VideoOS.Platform.DriverFramework.Data;
using VideoOS.Platform.DriverFramework.Data.Settings;
using VideoOS.Platform.DriverFramework.Managers;
using VideoOS.Platform.DriverFramework.Utilities;

namespace VanillaUvapDriver
{
    /// <summary>
    /// Class for working with one metadata stream session
    /// TODO: Implement request for fetching metadata
    /// </summary>
    internal class VanillaUvapDriverMetadataStreamSession : BaseVanillaUvapDriverStreamSession
    {
        IDictionary<string, string> settings;

        int timeOffset;

        KafkaWorker kafkaWorker;
        VanillaUvapDriverConnectionManager connectionManager;

        IEventManager eventManager;

        Dictionary<int?, DateTime> lastFiredEvent;

        public VanillaUvapDriverMetadataStreamSession(IEventManager eventManager, ISettingsManager settingsManager, VanillaUvapDriverConnectionManager connectionManager, Guid sessionId, string deviceId, Guid streamId, int channel, IDictionary<string, string> settings, string kafkaBroker) :
            base(settingsManager, connectionManager, sessionId, deviceId, streamId)
        {
            this.eventManager = eventManager;
            this.settings = settings;
            Channel = channel;
            this.connectionManager = connectionManager;
            lastFiredEvent = new Dictionary<int?, DateTime>();

            timeOffset = int.Parse(settings[Constants.TimeOffsetNameSetting]);

            kafkaWorker = new KafkaWorker(kafkaBroker, Channel);

            if (settings[Constants.KafkaDetectionTopicSetting] != VanillaUvapDriverConfigurationManager.DisabledValue)
                kafkaWorker.AddConsumer(settings[Constants.KafkaDetectionTopicSetting]);

            if (settings[Constants.KafkaREIDTopicSetting] != VanillaUvapDriverConfigurationManager.DisabledValue)
                kafkaWorker.AddConsumer(settings[Constants.KafkaREIDTopicSetting]);


            if (settings[Constants.KafkaAgeTopicSetting] != VanillaUvapDriverConfigurationManager.DisabledValue &&
                settings[Constants.KafkaGenderTopicSetting] != VanillaUvapDriverConfigurationManager.DisabledValue)
            {
                kafkaWorker.AddConsumer(settings[Constants.KafkaAgeTopicSetting]);
                kafkaWorker.AddConsumer(settings[Constants.KafkaGenderTopicSetting]);
            }

            kafkaWorker.Start();
        }

        public override void InnerClose()
        {
            kafkaWorker.Close();
        }

        protected override bool GetLiveFrameInternal(TimeSpan timeout, out BaseDataHeader header, out byte[] data)
        {
            header = null;

            IDictionary<string, List<ConsumeResult<string, string>>> kafkaData = null;

            MetadataContainer container = new MetadataContainer();

            List<UVAPObject> uvapObjects = new List<UVAPObject>();
            List<UVAPDetection> uvapDetections = new List<UVAPDetection>();

            //while(kafkaData == null)
            //{
            //}
            kafkaData = kafkaWorker.GetConsumption();

            if (kafkaData != null)
                foreach (var pair in kafkaData)
                {
                    if (container.CommonKey == null)
                        container.CommonKey = pair.Value.FirstOrDefault()?.Message.Key.Split('_')[0];

                    if (pair.Key == settings[Constants.KafkaREIDTopicSetting])
                    {
                        foreach (var reidEntry in pair.Value)
                        {
                            var reidEvent = JsonConvert.DeserializeObject<ReidJsonEntry>(reidEntry.Message.Value);
                            reidEvent.KafkaKey = reidEntry.Message.Key;
                            uvapObjects.Add(reidEvent);
                        }
                    }
                    else if (pair.Key == settings[Constants.KafkaDetectionTopicSetting])
                    {
                        foreach (var detEntry in pair.Value)
                        {
                            var detEvent = JsonConvert.DeserializeObject<DetectionJsonEntry>(detEntry.Message.Value);
                            detEvent.KafkaKey = detEntry.Message.Key;
                            uvapObjects.Add(detEvent);
                        }
                    }
                    else if (pair.Key == settings[Constants.KafkaAgeTopicSetting])
                    {
                        foreach (var ageEntry in pair.Value)
                        {
                            var ageEvent = JsonConvert.DeserializeObject<AgeJsonEntry>(ageEntry.Message.Value);
                            ageEvent.KafkaKey = ageEntry.Message.Key;
                            uvapObjects.Add(ageEvent);
                        }
                    }
                    else if (pair.Key == settings[Constants.KafkaGenderTopicSetting])
                    {
                        foreach (var genderEntry in pair.Value)
                        {
                            var genderEvent = JsonConvert.DeserializeObject<GenderJsonEntry>(genderEntry.Message.Value);
                            genderEvent.KafkaKey = genderEntry.Message.Key;
                            uvapObjects.Add(genderEvent);
                        }
                    }
                }

            List<UVAPDetection> detections = new List<UVAPDetection>();

            foreach (var detGrp in uvapObjects.GroupBy(o => o.KafkaKey))
            {
                UVAPDetection uvapDetection = new UVAPDetection();
                foreach (var det in detGrp)
                {
                    if (det is ReidJsonEntry && (det as ReidJsonEntry).reid_event != null)
                    {
                        uvapDetection.Reid = det as ReidJsonEntry;

                        // Disabled due to client poll
                        //uvapDetection.Reid.defined_person = connectionManager.NodeREDConnection.Persons.FirstOrDefault(p => p.key == uvapDetection.Reid.reid_event.first_key);
                    }
                    else if (det is AgeJsonEntry)
                    {
                        uvapDetection.Age = det as AgeJsonEntry;
                    }
                    else if (det is GenderJsonEntry)
                    {
                        uvapDetection.Gender = det as GenderJsonEntry;
                    }
                    else if (det is DetectionJsonEntry)
                        uvapDetection.Detection = det as DetectionJsonEntry;
                }

                if (uvapDetection.Detection?.end_of_frame == false)
                    detections.Add(uvapDetection);
            }


            container.Detections = detections.ToArray();
            data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(container));

            if (data.Length == 0)
            {
                return false;
            }
            DateTime dt = DateTime.UtcNow;

            if (container.CommonKey != null)
                dt = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(container.CommonKey)).UtcDateTime.AddMilliseconds(timeOffset);

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
            if (setting.Key == Constants.TimeOffsetNameSetting)
            {
                timeOffset = int.Parse(setting.Value);
            }
        }
    }
}
