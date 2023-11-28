using System;
using System.Collections.Generic;
using System.Linq;
using UvapShared.Driver;
using VideoOS.Platform.DriverFramework.Data;
using VideoOS.Platform.DriverFramework.Data.Settings;
using VideoOS.Platform.DriverFramework.Exceptions;
using VideoOS.Platform.DriverFramework.Managers;
using VideoOS.Platform.DriverFramework.Utilities;

namespace UVAPMipDriver
{
    /// <summary>
    /// Class for overall stream handling. Most methods are inherited by the abstract class SessionEnabledStreamManager and are invoked by Milestone.
    /// </summary>
    public class UVAPMipDriverStreamManager : SessionEnabledStreamManager
    {
        private new UVAPMipDriverContainer Container => base.Container as UVAPMipDriverContainer;

        public UVAPMipDriverStreamManager(UVAPMipDriverContainer container) : base(container)
        {
        }

        // Method generated by the MIPDriver template
        public override GetLiveFrameResult GetLiveFrame(Guid sessionId, TimeSpan timeout)
        {
            try
            {
                return base.GetLiveFrame(sessionId, timeout);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                Toolbox.Log.Trace("UVAPMipDriver.StreamManager.GetLiveFrame: Exception={0}", ex.Message);
                return GetLiveFrameResult.ErrorResult(StreamLiveStatus.NoConnection);
            }
        }

        // Builtin method, invoked by Milestone
        // Used to create the device stream session.
        // We fetch the associated information provided by the user and create the stream on the device.
        protected override BaseStreamSession CreateSession(string deviceId, Guid streamId, Guid sessionId)
        {
            Consumer consumer = null;

            try
            {
                // Fetch all settings assigned to the device
                IDictionary<string, string> settings = new Dictionary<string, string>();
                settings.Add(GetDeviceSettingValue(Constants.KafkaTopicNameSetting, deviceId));
                settings.Add(GetDeviceSettingValue(Constants.UvapModelNameSetting, deviceId));
                settings.Add(GetDeviceSettingValue(Constants.UvapChannelNameSetting, deviceId));
                settings.Add(GetDeviceSettingValue(Constants.UvapTimeOffsetNameSetting, deviceId));

                consumer = Container.ConnectionManager.KafkaConnection.Subscribe(settings[Constants.KafkaTopicNameSetting]);

                int.TryParse(settings[Constants.UvapTimeOffsetNameSetting], out int timeOffset);

                // start correct stream based on model
                switch (settings[Constants.UvapModelNameSetting])
                {
                    case "HeadDetection":

                        settings.Add(GetMetadataSettingValue(Constants.UvapFrameWidthSetting, Constants.HeadDetectionMetadataType, streamId, deviceId)); // frame width
                        settings.Add(GetMetadataSettingValue(Constants.UvapFrameHeightSetting, Constants.HeadDetectionMetadataType, streamId, deviceId)); // frame height

                        settings.Add(GetMetadataSettingValue(Constants.UvapBoundingBoxColorTransparencySetting, Constants.HeadDetectionMetadataType, streamId, deviceId)); // boundingbox transparency 
                        settings.Add(GetMetadataSettingValue(Constants.UvapBoundingBoxColorSetting, Constants.HeadDetectionMetadataType, streamId, deviceId)); // boundingbox color
                        settings.Add(GetMetadataSettingValue(Constants.UvapLineThicknessNameSetting, Constants.HeadDetectionMetadataType, streamId, deviceId)); // line thickness 

                        settings.Add(GetMetadataSettingValue(Constants.UvapHeaddetectionNativemodeNameSetting, Constants.HeadDetectionMetadataType, streamId, deviceId)); // native mode

                        settings.Add(GetMetadataSettingValue(Constants.UvapShowTextInNativeNameSetting, Constants.HeadDetectionMetadataType, streamId, deviceId)); // show text
                        settings.Add(GetMetadataSettingValue(Constants.UvapTextColorSetting, Constants.HeadDetectionMetadataType, streamId, deviceId)); // text color
                        settings.Add(GetMetadataSettingValue(Constants.UvapTextColorTransparencySetting, Constants.HeadDetectionMetadataType, streamId, deviceId)); // text transparency 


                        return new HeadDetectionMetadataStreamSession(settings, consumer, Container.SettingsManager, Container.ConnectionManager, 
                            sessionId, deviceId, streamId, int.Parse(settings[Constants.UvapChannelNameSetting])) { TimeOffset = timeOffset };

                    case "Skeleton":
                        return new SkeletonMetadataStreamSession(consumer, Container.EventManager, Container.SettingsManager, Container.ConnectionManager, 
                            sessionId, deviceId, streamId, int.Parse(settings[Constants.UvapChannelNameSetting])) { TimeOffset = timeOffset };
                }
            }
            catch(Exception ex)
            {
                Toolbox.Log.Trace("UVAPMipDriver.StreamManager.CreateSession: Exception={0}", ex.Message);
                throw new MIPDriverException();
            }

            Toolbox.Log.LogError(nameof(UVAPMipDriverStreamManager), "This device ID: {0} is not supported", deviceId);
            throw new MIPDriverException();
        }

        // Simplified method to fetch the device settings from the builtin SettingsManager
        KeyValuePair<string, string> GetDeviceSettingValue(string key, string device)
        {
            var setting = Container.SettingsManager.GetSetting(new DeviceSetting(key, device, ""));
            if (setting == null)
                throw new MIPDriverException("Setting {" + key + "} for device {" + device + "} is null");
            return new KeyValuePair<string, string>(key, setting.Value);
        }

        // Simplified method to fetch the metadata settings from the builtin SettingsManager
        KeyValuePair<string, string> GetMetadataSettingValue(string key, Guid metadataType, Guid stream, string device)
        {
            var setting = Container.SettingsManager.GetSetting(new MetadataSetting(key, device, stream, metadataType,""));
            if (setting == null)
                throw new MIPDriverException("Setting {" + key + "} for metadatatype {" + metadataType + "} is null");
            return new KeyValuePair<string, string>(key, setting.Value);
        }
    }
}