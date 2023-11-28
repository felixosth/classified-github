using REIDShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VideoOS.Platform.DriverFramework.Definitions;
using VideoOS.Platform.DriverFramework.Exceptions;
using VideoOS.Platform.DriverFramework.Managers;

namespace VanillaUvapDriver
{
    /// <summary>
    /// This class returns information about the hardware including capabilities and settings supported.
    /// TODO: Update it to match what is supported by your hardware.
    /// </summary>
    public class VanillaUvapDriverConfigurationManager : ConfigurationManager
    {
        private const string _firmware = "UvapBaseDriver Firmware";
        private const string _firmwareVersion = "1.0";
        private const string _hardwareName = "UvapBaseDriver Hardware";
        private const string _serialNumber = "12345";

        internal const string DisabledValue = "*Disabled";

        private new VanillaUvapDriverContainer Container => base.Container as VanillaUvapDriverContainer;

        public VanillaUvapDriverConfigurationManager(VanillaUvapDriverContainer container) : base(container)
        {
        }

        protected override ProductInformation FetchProductInformation()
        {
            if (!Container.ConnectionManager.IsConnected)
            {
                throw new ConnectionLostException("Connection not established");
            }

            var driverInfo = Container.Definition.DriverInfo;
            var product = driverInfo.SupportedProducts.FirstOrDefault();
            var macAddress = Helper.GetMacAddressWithSeed(Helper.StringToSeed(Container.Uri.ToString()));

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
        protected override IDictionary<string, string> BuildHardwareSettings()
        {
            return new Dictionary<string, string>()
            {
            };
        }

        protected override ICollection<ISetupField> BuildFields()
        {
            var topics = Container.ConnectionManager.KafkaConnection.Topics;
            List<StringSetupField> topicFields = new List<StringSetupField>()
            {
                new StringSetupField()
                {
                    Key = DisabledValue,
                    DisplayName = DisabledValue,
                    DefaultValue = DisabledValue,
                    ReferenceId = Helper.StringToGuid(DisabledValue)
                }
            };

            foreach(var topic in topics.OrderBy(t => t))
            {
                topicFields.Add(new StringSetupField()
                {
                    Key = topic,
                    DefaultValue = topic,
                    DisplayName = topic,
                    ReferenceId = Helper.StringToGuid(topic)
                });
            }
            
            var fields = new List<ISetupField>()
            {
                new EnumSetupField() // Kafka topic name setting
                {
                    Key = Constants.KafkaDetectionTopicSetting,
                    DisplayName = "Detection topic",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = false,
                    ReferenceId = Constants.KafkaDetectionTopicSettingId,
                    DefaultValue = "",
                    EnumList = topicFields
                },
                new EnumSetupField() // Kafka topic name setting
                {
                    Key = Constants.KafkaREIDTopicSetting,
                    DisplayName = "REID topic",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = false,
                    ReferenceId = Constants.KafkaREIDTopicSettingId,
                    DefaultValue = "",
                    EnumList = topicFields
                },
                new EnumSetupField()
                {
                    Key = Constants.KafkaAgeTopicSetting,
                    DisplayName = "Age topic",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = false,
                    ReferenceId = Constants.KafkaAgeTopicSettingId,
                    DefaultValue = "",
                    EnumList = topicFields
                },
                new EnumSetupField() // Kafka topic name setting
                {
                    Key = Constants.KafkaGenderTopicSetting,
                    DisplayName = "Gender topic",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = false,
                    ReferenceId = Constants.KafkaGenderTopicSettingId,
                    DefaultValue = "",
                    EnumList = topicFields
                },
                new NumberSetupField() // Time offset setting for video and metadata sync
                {
                    Key = Constants.TimeOffsetNameSetting,
                    DisplayName = "Time offset (ms)",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = false,
                    ReferenceId = Constants.TimeOffsetNameSettingId,
                    MaxValue = 5000,
                    MinValue = -5000,
                    DefaultValue = 0,
                },
                new NumberSetupField()
                {
                    Key = Constants.ChannelIdSettings,
                    DisplayName = "Channel ID",
                    DisplayNameReferenceId = Guid.Empty,
                    IsReadOnly = true,
                    ReferenceId = Constants.ChannelIdSettingsId,
                    MaxValue = 100,
                    MinValue = 1,
                    DefaultValue = 1
                },

            };

            return fields;
        }

        protected override ICollection<EventDefinition> BuildHardwareEvents()
        {
            var hardwareEvents = new List<EventDefinition>();


            return hardwareEvents;
        }

        protected override ICollection<DeviceDefinitionBase> BuildDevices()
        {
            var devices = new List<DeviceDefinitionBase>();

            for (int i = 0; i < 8; i++)
            {
                devices.Add(new MetadataDeviceDefinition()
                {
                    DisplayName = "Channel " + (i + 1),
                    //DeviceId = "REIDMetadataChannel_" + i,
                    DeviceId = Constants.MetadataDeviceIDs[i].ToString(),
                    Streams = BuildMetadataStreams(),
                    Settings = new Dictionary<string, string>()
                    {
                        { Constants.KafkaDetectionTopicSetting, DisabledValue },
                        { Constants.KafkaREIDTopicSetting, DisabledValue },
                        { Constants.KafkaAgeTopicSetting, DisabledValue },
                        { Constants.KafkaGenderTopicSetting, DisabledValue },
                        { Constants.TimeOffsetNameSetting, "0" },
                        { Constants.ChannelIdSettings, (i+1).ToString() }
                    }
                });
            }

            return devices;
        }


        private static ICollection<StreamDefinition> BuildMetadataStreams()
        {
            ICollection<StreamDefinition> streams = new List<StreamDefinition>();
            streams.Add(new StreamDefinition()
            {
                DisplayName = "REIDMipDriver metadata stream",
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
                }
            });
            return streams;
        }
    }
}
