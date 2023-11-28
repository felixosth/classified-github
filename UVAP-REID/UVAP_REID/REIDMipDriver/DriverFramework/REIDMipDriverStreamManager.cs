using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VideoOS.Platform.DriverFramework.Data;
using VideoOS.Platform.DriverFramework.Data.Settings;
using VideoOS.Platform.DriverFramework.Exceptions;
using VideoOS.Platform.DriverFramework.Managers;
using VideoOS.Platform.DriverFramework.Utilities;

namespace REIDMipDriver
{
    /// <summary>
    /// Class for overall stream handling.
    /// </summary>
    public class REIDMipDriverStreamManager : SessionEnabledStreamManager
    {
        private new REIDMipDriverContainer Container => base.Container as REIDMipDriverContainer;

        public REIDMipDriverStreamManager(REIDMipDriverContainer container) : base(container)
        {
        }

        public override GetLiveFrameResult GetLiveFrame(Guid sessionId, TimeSpan timeout)
        {
            try
            {
                return base.GetLiveFrame(sessionId, timeout);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                Toolbox.Log.Trace("REIDMipDriver.StreamManager.GetLiveFrame: Exception={0}", ex.Message);
                return GetLiveFrameResult.ErrorResult(StreamLiveStatus.NoConnection);
            }
        }

        internal BaseStreamSession GetSession(int channel)
        {
            return GetAllSessions().OfType<BaseREIDMipDriverStreamSession>()
                .FirstOrDefault(s => s.Channel == channel);
        }

        protected override BaseStreamSession CreateSession(string deviceId, Guid streamId, Guid sessionId)
        {
            IDictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add(GetDeviceSettingValue(Constants.KafkaDetectionTopicSetting, deviceId));
            settings.Add(GetDeviceSettingValue(Constants.KafkaREIDTopicSetting, deviceId));
            settings.Add(GetDeviceSettingValue(Constants.TimeOffsetNameSetting, deviceId));

            settings.Add(GetDeviceSettingValue(Constants.KafkaAgeTopicSetting, deviceId));
            settings.Add(GetDeviceSettingValue(Constants.KafkaGenderTopicSetting, deviceId));

            var id = int.Parse(GetDeviceSettingValue(Constants.ChannelIdSettings, deviceId).Value);

            return new REIDMipDriverMetadataStreamSession(Container.EventManager, Container.SettingsManager, Container.ConnectionManager, 
                sessionId, deviceId, streamId, id, settings, Container.ConnectionManager.KafkaConnection.Broker);
        }

        KeyValuePair<string, string> GetDeviceSettingValue(string key, string device)
        {
            var setting = Container.SettingsManager.GetSetting(new DeviceSetting(key, device, ""));
            if (setting == null)
                throw new MIPDriverException("Setting {" + key + "} for device {" + device + "} is null");
            return new KeyValuePair<string, string>(key, setting.Value);
        }

        //KeyValuePair<string, string> GetMetadataSettingValue(string key, Guid metadataType, Guid stream, string device)
        //{
        //    var setting = Container.SettingsManager.GetSetting(new MetadataSetting(key, device, stream, metadataType, ""));
        //    if (setting == null)
        //        throw new MIPDriverException("Setting {" + key + "} for metadatatype {" + metadataType + "} is null");
        //    return new KeyValuePair<string, string>(key, setting.Value);
        //}
    }
}
