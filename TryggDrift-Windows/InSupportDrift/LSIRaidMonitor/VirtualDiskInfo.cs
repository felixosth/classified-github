using Newtonsoft.Json;

namespace InSupport.Drift.Plugins.RaidMonitor
{
    public class VirtualDiskInfo
    {
        public Controller[] Controllers { get; set; }
    }

    public class Controller
    {
        [JsonProperty("Command Status")]
        public CommandStatus CommandStatus { get; set; }
        [JsonProperty("Response Data")]
        public ResponseData ResponseData { get; set; }
    }

    public class CommandStatus
    {
        public int Controller { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }

    public class ResponseData
    {
        [JsonProperty("Virtual Drives")]
        public VirtualDrive[] VirtualDrives { get; set; }
    }

    public class VirtualDrive
    {
        [JsonProperty("DG/VD")]
        public string DGVD { get; set; }
        public string TYPE { get; set; }
        public string State { get; set; }
        public string Access { get; set; }
        public string Consist { get; set; }
        public string Cache { get; set; }
        public string Cac { get; set; }
        public string sCC { get; set; }
        public string Size { get; set; }
        public string Name { get; set; }
    }


}
