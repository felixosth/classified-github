using System;
using VideoOS.Platform.DriverFramework.Definitions;

namespace VanillaUvapDriver
{
    public static class Constants
    {
        public static readonly ProductDefinition Product1 = new ProductDefinition
        {
            Id = new Guid("16de24c5-4ff7-443c-ace4-fcb88d280ec5"),
            Name = "My UvapBaseDriver"
        };

        public static readonly Guid DriverId = new Guid("64d7328b-be80-480b-b70c-61c97cc6ea38");

        public static readonly Guid MetadataDevice = new Guid("{5DD52E39-3495-4178-9D24-FD0177F6A8CE}");

        public static readonly string KafkaDetectionTopicSetting = "KafkaHeadDetectionTopic";
        public static readonly Guid KafkaDetectionTopicSettingId = new Guid("{B6A3F8B3-0FC7-47D4-BC8F-4D8B3769A5AF}");

        public static readonly string KafkaREIDTopicSetting = "KafkaREIDTopicSetting";
        public static readonly Guid KafkaREIDTopicSettingId = new Guid("{D6A72C83-0ECE-4707-AAD0-BEEA8790144A}");

        public static readonly string TimeOffsetNameSetting = "TimeOffsetNameSetting";
        public static readonly Guid TimeOffsetNameSettingId = new Guid("{84ABFD7A-6B60-4BFC-84FF-B854EE6A334A}");

        public const string ChannelIdSettings = "ChannelIdSettings";
        public static readonly Guid ChannelIdSettingsId = new Guid("{C3D417B9-EA2F-4476-90D2-003D4FBAE4EC}");

        public const string KafkaAgeTopicSetting = "KafkaAgeTopicSetting";
        public static readonly Guid KafkaAgeTopicSettingId = new Guid("{85AAACDE-8FBD-4655-99DA-FF5BF16DDA17}");

        public const string KafkaGenderTopicSetting = "KafkaGenderTopicSetting";
        public static readonly Guid KafkaGenderTopicSettingId = new Guid("{3BFA166B-4746-46BB-AB3A-1FB16800DE95}");


        public static Guid[] MetadataDeviceIDs { get; } = new Guid[]
        {
            new Guid("{52499573-B0AB-4B60-BCB7-6C988C41CE1F}"),
            new Guid("{081C0E94-58B2-44F6-8921-B3AAA95FF6E1}"),
            new Guid("{E2047A85-3A08-4CEE-867B-0E6955A5B1EA}"),
            new Guid("{9879340E-744C-4CBA-A249-46CD819475E0}"),
            new Guid("{F1D43100-1599-46AC-AEE2-23E7BF9962AC}"),
            new Guid("{1C85DA22-6990-4BB1-B857-6C48ACFF706B}"),
            new Guid("{7730B551-5D74-4E32-8C4E-AF84CFF8D0E7}"),
            new Guid("{CEBAFFB1-C201-46E2-A651-34C3388A53F1}"),
        };
    }
}
