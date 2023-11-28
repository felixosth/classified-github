using Newtonsoft.Json;

namespace InSupport.Drift.Plugins.RaidMonitor
{
    public class DisksInfo
    {
        public DisksInfoController[] Controllers { get; set; }
    }

    public class DisksInfoController
    {
        [JsonProperty("Command Status")]
        public DisksInfoCommandStatus CommandStatus { get; set; }
        [JsonProperty("Response Data")]
        public DisksInfoResponseData ResponseData { get; set; }
    }

    public class DisksInfoCommandStatus
    {
        public int Controller { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }

    public class DisksInfoResponseData
    {
        [JsonProperty("Drive Information")]
        public DriveInformation[] DriveInformation { get; set; }
    }

    public class DriveInformation
    {
        [JsonProperty("EID:Slt")]
        public string EIDSlt { get; set; }
        public int DID { get; set; }
        public string State { get; set; }
        public string DG { get; set; }
        public string Size { get; set; }
        public string Intf { get; set; }
        public string Med { get; set; }
        public string SED { get; set; }
        public string PI { get; set; }
        public string SeSz { get; set; }
        public string Model { get; set; }
        public string Sp { get; set; }
        public string Type { get; set; }
    }


}
