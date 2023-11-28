using System;
using System.Net.Http.Headers;
using VideoOS.Platform.DriverFramework.Definitions;

namespace REIDMipDriver
{
    public static class Constants
    {
        public static readonly ProductDefinition NodeREDProduct = new ProductDefinition
        {
            Id = new Guid("d8c098ca-b971-4864-b41d-0900496db7f7"),
            Name = "My NodeREDREIDMipDriver"
        };

        public static readonly Guid NodeREDDriverId = new Guid("a47ad2d6-3569-46b6-a037-ee93c19c52c8");

        public static readonly Guid MetadataDevice = new Guid("b280d31a-6b52-4a47-91b8-909849b781d9");

        public static readonly string KafkaDetectionTopicSetting = "KafkaHeadDetectionTopic";
        public static readonly Guid KafkaDetectionTopicSettingId = new Guid("{D16B5754-3E60-45FC-9B6D-B55EE188ED8A}");

        public static readonly string KafkaREIDTopicSetting = "KafkaREIDTopicSetting";
        public static readonly Guid KafkaREIDTopicSettingId = new Guid("{AEFF456A-01AD-4512-BE00-BE5DFC2AAB0E}");

        public static readonly string TimeOffsetNameSetting = "TimeOffsetNameSetting";
        public static readonly Guid TimeOffsetNameSettingId = new Guid("{15EC2968-7DE9-436C-A4CF-F966E4697256}");

        public const string ChannelIdSettings = "ChannelIdSettings";
        public static readonly Guid ChannelIdSettingsId = new Guid("{7F78103A-8667-4EEF-A193-108C7CC50660}");

        public const string KafkaAgeTopicSetting = "KafkaAgeTopicSetting";
        public static readonly Guid KafkaAgeTopicSettingId = new Guid("{1AB44706-495D-4C3A-AA29-DF2A028761E3}");

        public const string KafkaGenderTopicSetting = "KafkaGenderTopicSetting";
        public static readonly Guid KafkaGenderTopicSettingId = new Guid("{2096B293-21CE-4880-B47B-32A6B1D8A534}");


        public static Guid[] MetadataDeviceIDs { get; } = new Guid[]
        {
            new Guid("{88FBED64-6DF6-4F05-8420-DC318656C710}"),
            new Guid("{E7C6421B-FE93-4FF1-9C82-DE35F6FD83BC}"),
            new Guid("{EDE90120-8C84-4DED-B3D8-B02ACE0BF847}"),
            new Guid("{9370AA02-5068-47D4-BE4B-6E68FF1B8B91}"),
            new Guid("{C70643A9-EFB9-4FEF-BDC1-2A580B3CAFE1}"),
            new Guid("{21DFA404-D835-43C1-AD28-D1930B26E523}"),
            new Guid("{DB6A9A89-8B92-4DE4-8613-4B304691E7A5}"),
            new Guid("{DD381C04-6153-424B-9F76-2507F3EB8927}"),
        };
    }
}
