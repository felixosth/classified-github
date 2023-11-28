
using System;
using VideoOS.Platform.DriverFramework.Data;
using VideoOS.Platform.DriverFramework.Exceptions;
using VideoOS.Platform.DriverFramework.Utilities;
using VideoOS.Platform.DriverFramework.Managers;
using VideoOS.Platform.DriverFramework.Data.Settings;
using VideoOS.Platform.DriverFramework.Definitions;
using System.Linq;
using System.Collections.Generic;
using UvapShared.Driver;

namespace UVAPMipDriver
{
    /// <summary>
    /// Base stream session class. Specialized in other classes for specific devices. Inherits from BaseStreamSession so most methods are from there.
    /// </summary>
    internal abstract class UVAPBaseStreamSession : BaseStreamSession
    {
        public Guid Id { get; }
        public int Channel { get; protected set; }
        protected readonly string _deviceId;
        protected readonly Guid _streamId;
        public int TimeOffset { get; set; }
        protected readonly UVAPMipDriverConnectionManager _connectionManager;
        protected readonly ISettingsManager _settingsManager;
        protected int _sequence = 0;

        protected abstract bool GetLiveFrameInternal(TimeSpan timeout, out BaseDataHeader header, out byte[] data);

        public UVAPBaseStreamSession(ISettingsManager settingsManager, UVAPMipDriverConnectionManager connectionManager, Guid sessionId, string deviceId, Guid streamId)
        {
            Id = sessionId;
            _settingsManager = settingsManager;
            _connectionManager = connectionManager;
            _deviceId = deviceId;
            _streamId = streamId;

            settingsManager.OnSettingsChanged += SettingsManager_OnSettingsChanged; // Subscribe to the builtin OnSettingsChanged event
        }

        
        // Let our devices that derive from this class know when the settings are being changed
        private void SettingsManager_OnSettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            foreach(var setting in e.Settings)
            {
                if (setting is DeviceSetting && ((DeviceSetting)setting).DeviceId == _deviceId)
                {
                    var ds = setting as DeviceSetting;
                    if (ds.DeviceId == _deviceId)
                    {
                        if(ds.Key == Constants.UvapTimeOffsetNameSetting)
                        {
                            TimeOffset = int.Parse(setting.Value);
                            Toolbox.Log.Trace(ds.Key + " changed to " + ds.Value);
                        }
                        else
                            DeviceSettingChanged(setting as DeviceSetting);
                    }
                }
                else if (setting is MetadataSetting && ((MetadataSetting)setting).DeviceId == _deviceId)
                    MetadataSettingChanged(setting as MetadataSetting);
                else if (setting is StreamSetting && ((StreamSetting)setting).DeviceId == _deviceId && ((StreamSetting)setting).StreamId == _streamId)
                    StreamSettingChanged(setting as StreamSetting);
            }
        }

        // Builtin method, invoked by Milestone. Fetch the frame of data from the class that derive from this class.
        public sealed override bool GetLiveFrame(TimeSpan timeout, out BaseDataHeader header, out byte[] data)
        {
            try
            {
                var result = GetLiveFrameInternal(timeout, out header, out data); // Data fetched from the device

                if(header is MetadataHeader)
                {
                    (header as MetadataHeader).Timestamp = (header as MetadataHeader).Timestamp.AddMilliseconds(TimeOffset);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Toolbox.Log.LogError(GetType().Name,
                    "{0}, Channel {1}: {2}", nameof(GetLiveFrame), Channel, ex.Message + ex.StackTrace);
                throw new ConnectionLostException(ex.Message + ex.StackTrace);
            }
        }

        public override void Close()
        {
            try
            {
                _sequence = 0;
                InnerClose();
            }
            catch (Exception ex)
            {
                Toolbox.Log.LogError(this.GetType().Name, "Error calling Destroy: {0}", ex.Message);
            }
        }

        // Custom methods for the classes that derive from this class.
        protected abstract void InnerClose();
        protected virtual void DeviceSettingChanged(DeviceSetting setting) { }
        protected virtual void MetadataSettingChanged(MetadataSetting setting) { }
        protected virtual void StreamSettingChanged(StreamSetting setting) { }
    }
}
