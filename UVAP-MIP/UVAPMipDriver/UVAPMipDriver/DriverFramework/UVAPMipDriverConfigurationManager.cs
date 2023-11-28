using System;
using System.Collections.Generic;
using System.Linq;
using UvapShared;
using UvapShared.Driver;
using VideoOS.Platform.DriverFramework.Definitions;
using VideoOS.Platform.DriverFramework.Exceptions;
using VideoOS.Platform.DriverFramework.Managers;

namespace UVAPMipDriver
{
    /// <summary>
    /// This class returns information about the hardware including capabilities and settings supported. Edit this class to change hardcoded information and user input configuration.
    /// </summary>
    public class UVAPMipDriverConfigurationManager : ConfigurationManager
    {
        private const string _firmware = "UVAPMipDriver Firmware";
        private const string _firmwareVersion = "1.0";
        private const string _hardwareName = "UVAPMipDriver Hardware";
        private const string _serialNumber = "12345";

        private new UVAPMipDriverContainer Container => base.Container as UVAPMipDriverContainer;

        public UVAPMipDriverConfigurationManager(UVAPMipDriverContainer container) : base(container)
        {
        }
        
        // Builtin method, invoked by Milestone, return the driver product information
        protected override ProductInformation FetchProductInformation()
        {
            if (!Container.ConnectionManager.IsConnected)
            {
                throw new ConnectionLostException("Connection not established");
            }

            var driverInfo = Container.Definition.DriverInfo;
            var product = driverInfo.SupportedProducts.FirstOrDefault();
            var macAddress = GetMacAddressWithSeed(Helper.StringToSeed(Container.Uri.ToString()));

            return new ProductInformation
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductVersion = driverInfo.Version,
                MacAddress = macAddress,
                FirmwareVersion = _firmwareVersion,
                Firmware = _firmware,
                HardwareName = _hardwareName,
                SerialNumber = _serialNumber
            };
        }

        // Builtin method, invoked by Milestone
        // Returns the available setting-fields in the driver that the hardware, device or streams can use.
        protected override ICollection<ISetupField> BuildFields()
        {
            return new List<ISetupField>()
            {
                new StringSetupField() // Kafka topic name setting
                {
                    Key = Constants.KafkaTopicNameSetting,
                    DisplayName = "Kafka topic",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = false,
                    ReferenceId = Constants.SettingKafkaTopicFieldRefId,
                    DefaultValue = "Topic name"
                },
                new EnumSetupField() // Enum containing the available models
                {
                    Key = Constants.UvapModelNameSetting,
                    DisplayName = "Model",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = false,
                    ReferenceId = Constants.SettingUvapModelFieldRefId,
                    EnumList = new[]
                    {
                        new StringSetupField()
                        {
                            Key = Constants.UvapModels.HeadDetection.ToString(),
                            DefaultValue = Constants.UvapModels.HeadDetection.ToString(),
                            DisplayName = "Head detection",
                            DisplayNameReferenceId = Guid.Empty,
                            ReferenceId = Constants.SettingUvapModeFieldRefIdHeadDetection
                        },
                        new StringSetupField()
                        {
                            Key = Constants.UvapModels.Skeleton.ToString(),
                            DefaultValue = Constants.UvapModels.Skeleton.ToString(),
                            DisplayName = "Skeleton",
                            DisplayNameReferenceId = Guid.Empty,
                            ReferenceId = Constants.SettingUvapModeFieldRefIdSkeleton
                        }
                    },
                    DefaultValue = Constants.UvapModels.HeadDetection.ToString()
                },
                new NumberSetupField() // Channel ID, hardcoded by the driver for internal use
                {
                    Key = Constants.UvapChannelNameSetting,
                    DisplayName = "Channel ID",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = true,
                    ReferenceId = Constants.SettingUvapChannelFieldRefId,
                    MaxValue = 100,
                    MinValue = 1,
                    DefaultValue = 1
                },
                new NumberSetupField() // Time offset setting for video and metadata sync
                {
                    Key = Constants.UvapTimeOffsetNameSetting,
                    DisplayName = "Time offset (ms)",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = false,
                    ReferenceId = Constants.SettingUvapTimeOffsetFieldRefId,
                    MaxValue = 5000,
                    MinValue = -5000,
                    DefaultValue = 0,
                },
                new NumberSetupField() // Image width
                {
                    Key = Constants.UvapFrameWidthSetting,
                    DisplayName = "Frame width (pixels)",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = false,
                    ReferenceId = Constants.SettingsUvapFrameWidthRefId,
                    MaxValue = 5000,
                    MinValue = 600,
                    DefaultValue = 1920,
                },
                new NumberSetupField() // image height
                {
                    Key = Constants.UvapFrameHeightSetting,
                    DisplayName = "Frame height (pixels)",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = false,
                    ReferenceId = Constants.SettingsUvapFrameHeightRefId,
                    MaxValue = 5000,
                    MinValue = 300,
                    DefaultValue = 1080,
                },
                new StringSetupField() // Head detections boundingbox color
                {
                    Key = Constants.UvapBoundingBoxColorSetting,
                    DisplayName = "Bounding box color (Hex)",
                    ReferenceId = Constants.SettingUvapBoundingBoxColorFieldRefId,
                    DefaultValue = "#7CFC00"
                },
                new NumberSetupField() // Head detections boundingbox color transparency
                {
                    Key = Constants.UvapBoundingBoxColorTransparencySetting,
                    DisplayName = "Bounding box color transparency percentage",
                    ReferenceId = Constants.SettingUvapBoundingBoxColorTransparencyFieldRefId,
                    DefaultValue = 100,
                    MinValue = 0,
                    MaxValue = 100
                },
                new BoolSetupField() // Use native bounding boxes or not, true = the Smart Client can parse and display boundingboxes without a plugin
                {
                    Key = Constants.UvapHeaddetectionNativemodeNameSetting,
                    DisplayName = "Native bounding boxes",
                    ReferenceId = Constants.SettingHeaddetectionNativemodeFieldRefId,
                    DefaultValue = false
                },
                new BoolSetupField() // Show the confidence percentage text if the Native bounding boxes setting is true
                {
                    Key = Constants.UvapShowTextInNativeNameSetting,
                    DisplayName = "Show confidence percentage text",
                    ReferenceId = Constants.SettingShowTextInNativeFieldRefId,
                    DefaultValue = true
                },
                new StringSetupField() // Head detections text color if the Native bounding boxes setting is true
                {
                    Key = Constants.UvapTextColorSetting,
                    DisplayName = "Confidence text color (Hex)",
                    ReferenceId = Constants.SettingUvapTextColorFieldRefId,
                    DefaultValue = "#ff0000"
                },
                new NumberSetupField() // Head detections text color transparency if the Native bounding boxes setting is true
                {
                    Key = Constants.UvapTextColorTransparencySetting,
                    DisplayName = "Confidence text color transparency percentage",
                    ReferenceId = Constants.SettingUvapTextColorTransparencyFieldRefId,
                    DefaultValue = 100,
                    MinValue = 0,
                    MaxValue = 100
                },
                new NumberSetupField() // Bounding box line thickness if the Native bounding boxes setting is true
                {
                    Key = Constants.UvapLineThicknessNameSetting,
                    DisplayName = "Bounding box line thickness",
                    ReferenceId = Constants.SettingUvapLineThicknessFieldRefId,
                    DefaultValue = 1,
                    MinValue = 1,
                    MaxValue = 20
                }
            };
        }

        // Builtin method, invoked by Milestone
        // Return the available devices in the driver, this returns 16 metadata devices
        protected override ICollection<DeviceDefinitionBase> BuildDevices()
        {
            var devices = new List<DeviceDefinitionBase>();

            int metadataDevices = 16;

            for (int i = 0; i < metadataDevices; i++)
            {
                devices.Add(new MetadataDeviceDefinition()
                {
                    DisplayName = "Channel " + (i+1),
                    DeviceId = Constants.MetadataDeviceIDs[i].ToString(),
                    Streams = BuildMetadataStreams(),
                    Settings = new Dictionary<string, string>() // return the settings we want to use for this device, this setting mus exist in the BuildFields() method
                    {
                        { Constants.KafkaTopicNameSetting, "Topic name" },
                        { Constants.UvapModelNameSetting, Constants.UvapModels.HeadDetection.ToString() },
                        { Constants.UvapTimeOffsetNameSetting, "0" },
                        { Constants.UvapChannelNameSetting, (i+1).ToString() },
                    },
                    DeviceEvents = new List<EventDefinition> // List the HandsUp event in the device
                    {
                        new EventDefinition()
                        {
                            DisplayName = Constants.HandsUpEventDisplayName,
                            ReferenceId = Constants.HandsUpEventReferenceId
                        }
                    }
                });
            }
            return devices;
        }
        
        // List available streams, only used to separate settings in the UI. I'm not entierly sure how this interacts with the driver/streams.
        private static ICollection<StreamDefinition> BuildMetadataStreams()
        {
            ICollection<StreamDefinition> streams = new List<StreamDefinition>
            {
                new StreamDefinition()
                {
                    DisplayName = "UVAPMipDriver metadata stream",
                    ReferenceId = MetadataType.BoundingBoxDisplayId.ToString(),
                    MetadataTypes = new List<MetadataTypeDefinition>()
                    {
                        new MetadataTypeDefinition() // This needs to exist for some reason?
                        {
                            DisplayName = "Bounding boxes",
                            DisplayNameId = MetadataType.BoundingBoxDisplayId,
                            MetadataType = MetadataType.BoundingBoxTypeId,
                            ValidTime = TimeSpan.FromSeconds(1)
                        },
                        new MetadataTypeDefinition() // Settings for the headdetection model
                        {
                            DisplayName = "Head detection",
                            DisplayNameId = Constants.HeadDetectionStreamReferenceId,
                            MetadataType = Constants.HeadDetectionMetadataType,
                            ValidTime = TimeSpan.FromSeconds(1),
                            Settings = new Dictionary<string, string>()
                            {
                                { Constants.UvapFrameWidthSetting, "1920" },
                                { Constants.UvapFrameHeightSetting, "1080" },
                                { Constants.UvapBoundingBoxColorSetting, "#7CFC00" },
                                { Constants.UvapBoundingBoxColorTransparencySetting, "100" },
                                { Constants.UvapHeaddetectionNativemodeNameSetting, "False" },
                                { Constants.UvapShowTextInNativeNameSetting, "True" },
                                { Constants.UvapTextColorSetting, "#ff0000" },
                                { Constants.UvapTextColorTransparencySetting, "100" },
                                { Constants.UvapLineThicknessNameSetting, "1" }
                            }
                        },
                        new MetadataTypeDefinition() // Settings for the skeleton model
                        {
                            DisplayName = "Skeleton",
                            DisplayNameId = Constants.SkeletonStreamReferenceId,
                            MetadataType =  Constants.SkeletonMetadataType,
                            ValidTime = TimeSpan.FromSeconds(1)
                        }
                    }
                }
            };
            return streams;
        }

        // Return a Mac address based on seed
        public static string GetMacAddressWithSeed(int seed)
        {
            var random = new Random(seed);
            var buffer = new byte[6];
            random.NextBytes(buffer);
            var result = String.Concat(buffer.Select(x => string.Format("{0}:", x.ToString("X2"))).ToArray());
            return result.TrimEnd(':');
        }
    }
}
