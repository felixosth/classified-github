using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.DriverFramework.Definitions;

namespace UvapShared.Driver
{
    // Constant values, used by the driver. This is put in the shared project if any other plugin need to get these values.
    public static class Constants
    {
        public static readonly Guid DriverId = new Guid("{5A6B5C15-C761-48A3-8A5C-691B74049009}");

        public static readonly ProductDefinition Product = new ProductDefinition
        {
            Id = DriverId,
            Name = "UVAP driver"
        };

        // Event group
        public static readonly Guid EventGroupID = new Guid("{B030276C-68C5-4A0F-B98A-43D58B004DAE}");
        public const string EventGroupName = "UVAP";

        // Device guid
        public static readonly Guid MetadataDevice = new Guid("{7F4DC29C-2656-4957-BAC3-CCE4164F78E8}");


        // Topic Name
        public const string KafkaTopicNameSetting = "KafkaTopicNameSetting";
        public static readonly Guid SettingKafkaTopicFieldRefId = new Guid("{6E9E1C7A-AB80-46DB-8F75-FF951D21560D}");

        // Mode enum
        public const string UvapModelNameSetting = "UvapModeNameSetting";
        public static readonly Guid SettingUvapModelFieldRefId = new Guid("{B74AD847-C087-4523-AB60-DB08242F4FE4}");
        public static readonly Guid SettingUvapModeFieldRefIdHeadDetection = new Guid("{A6B34499-D33F-4DAB-B1CB-D2E05545C74A}");
        public static readonly Guid SettingUvapModeFieldRefIdSkeleton = new Guid("{B7A075F0-2211-4645-AE6E-ACDC4765A1C9}");

        // Channel Id
        public const string UvapChannelNameSetting = "UvapChannelNameSetting";
        public static readonly Guid SettingUvapChannelFieldRefId = new Guid("{17095415-9E6E-44DC-9C5A-A7BACFD9FACF}");

        // Frame Width
        public const string UvapFrameWidthSetting = "UvapFrameWidthSetting";
        public static readonly Guid SettingsUvapFrameWidthRefId = new Guid("{A5632852-F98C-405C-810F-BF5A0AF09532}");
        // Frame Height
        public const string UvapFrameHeightSetting = "UvapFrameHeightSetting";
        public static readonly Guid SettingsUvapFrameHeightRefId = new Guid("{0BA86645-3F2E-4CC4-B4D8-B98AC074AB87}");

        // Time offset
        public const string UvapTimeOffsetNameSetting = "UvapTimeOffsetNameSetting";
        public static readonly Guid SettingUvapTimeOffsetFieldRefId = new Guid("{33026100-85FE-42D9-A4A3-6601061A1D67}");

        // Head detection native mode (builtin boundingboxes or not)
        public const string UvapHeaddetectionNativemodeNameSetting = "UvapHeaddetectionNativemodeNameSetting";
        public static readonly Guid SettingHeaddetectionNativemodeFieldRefId = new Guid("{577C6E69-4DC4-4E41-BD00-4C000B2F037C}");

        // Show confidence text in native mode
        public const string UvapShowTextInNativeNameSetting = "UvapShowTextInNativeNameSetting";
        public static readonly Guid SettingShowTextInNativeFieldRefId = new Guid("{34559898-D202-4AEB-9547-61D57C93908C}");

        // Head detections text color
        public const string UvapTextColorSetting = "UvapTextColorSetting";
        public static readonly Guid SettingUvapTextColorFieldRefId = new Guid("{039090EF-2633-4936-9BA0-6E13096C174A}");

        // Head detections text color transparency
        public const string UvapTextColorTransparencySetting = "UvapTextColorTransparencySetting";
        public static readonly Guid SettingUvapTextColorTransparencyFieldRefId = new Guid("{11A4D94B-A41F-4733-B828-399E95D1F7EE}");

        // Time offset
        public const string UvapLineThicknessNameSetting = "UvapLineThicknessNameSetting";
        public static readonly Guid SettingUvapLineThicknessFieldRefId = new Guid("{2F8FD329-40EE-4F27-A3CE-15E14F808533}");


        // Head detections boundingbox color
        public const string UvapBoundingBoxColorSetting = "UvapBoundingBoxColorSetting";
        public static readonly Guid SettingUvapBoundingBoxColorFieldRefId = new Guid("{FB2BAA74-64D0-4C4E-A803-2559960E7160}");


        // Head detections boundingbox color transparency
        public const string UvapBoundingBoxColorTransparencySetting = "UvapBoundingBoxColorTransparencySetting";
        public static readonly Guid SettingUvapBoundingBoxColorTransparencyFieldRefId = new Guid("{141FDEE3-AEAA-4E5C-A95B-B9E407ECC4F2}");

        //stream settings
        public static readonly Guid SkeletonStreamReferenceId = new Guid("{E76FF1D4-6406-4E08-924D-5B0CAB5CD77D}");
        public static readonly Guid HeadDetectionStreamReferenceId = new Guid("{43BE9175-9A10-4055-B83B-84E54D7B3560}");

        // Metadata types
        public static readonly Guid HeadDetectionMetadataType = new Guid("{E7B9344F-05D0-42B5-BB50-48304D88DFF4}");
        public static readonly Guid SkeletonMetadataType = new Guid("{32FE8FFA-11F6-4BD4-B9D5-E5C81E5E737C}");

        // Metadata HandsUp Event
        public static readonly Guid HandsUpEventReferenceId = new Guid("{23A6DCBD-B8F3-4131-9640-E9F1F7855CF8}");
        public const string HandsUpEventDisplayName = "Hands Up";

        public enum UvapModels
        {
            HeadDetection = 1,
            Skeleton = 2
        }

        // Hardcoded guid's for all metadata devices.
        public static Guid[] MetadataDeviceIDs { get; } = new Guid[]
        {
            new Guid("{7E20867D-FD80-4D05-9C13-6670333BABDF}"),
            new Guid("{3D87BCCB-692D-4BD7-9945-7D7E3D03934B}"),
            new Guid("{600CB388-2A25-4A4E-BEC7-069DAF55BA37}"),
            new Guid("{19FC98F6-7C0A-4FED-8E8E-8659D8D49E5F}"),
            new Guid("{8A72AE85-7062-4B12-AEA1-4167112CD614}"),
            new Guid("{59C650DE-D7E9-41F4-BECE-D0342FDB17A9}"),
            new Guid("{9FEF9EF8-1603-4EAD-AD0B-7F8EA7897B4F}"),
            new Guid("{C17176F7-512D-43B1-8621-AF654876150D}"),
            new Guid("{D3DD3A58-7E30-49B8-B316-A053E8D098E9}"),
            new Guid("{56AAE020-D45A-4D80-B9A5-EB4C3FD7FA69}"),
            new Guid("{C5654CDD-EF1E-429D-83F7-7BBA555F62CD}"),
            new Guid("{0DAF88C2-82BD-41DD-A9F5-24E7EC980426}"),
            new Guid("{ED17EFF1-FD9B-4B37-85EC-220061E92370}"),
            new Guid("{C4E303D3-F346-4B3A-B44F-673F87D68CD5}"),
            new Guid("{14FC0526-1A4C-4FDE-A5E0-6E6D02373D51}"),
            new Guid("{6F63F923-3B3B-46AE-88CD-00FBAFB351D3}"),
        };
    }
}
