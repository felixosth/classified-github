using Newtonsoft.Json;
using REIDShared.NodeRED;
using System.Linq;

namespace REIDShared.Json
{
    public class ReidJsonEntry : UVAPObject
    {
        //[JsonIgnore]
        //public string type { get; set; }
        public Reid_Event reid_event { get; set; }

        //[JsonIgnore]
        //public Reg_Event reg_event { get; set; }

        public NodeREDPerson defined_person { get; set; }
    }

    public class Reid_Event
    {
        [JsonIgnore]
        public string first_key => match_list.FirstOrDefault().id.first_detection_key;

        public Match[] match_list { get; set; }
        [JsonIgnore]
        public string input_stream_id { get; set; }
    }

    public class Match
    {
        public Id id { get; set; }
        public float score { get; set; }
    }

    public class Id
    {
        [JsonIgnore]
        public string first_detection_time { get; set; }
        public string first_detection_key { get; set; }
        [JsonIgnore]
        public string first_detection_stream_id { get; set; }
    }

    public class Reg_Event
    {
        public Cluster_Id cluster_id { get; set; }
        public Cluster cluster { get; set; }
        public string input_stream_id { get; set; }
    }

    public class Cluster_Id
    {
        public string first_detection_time { get; set; }
        public string first_detection_key { get; set; }
        public string first_detection_stream_id { get; set; }
    }

    public class Cluster
    {
        public Representative_Fv representative_fv { get; set; }
        public int num_observations { get; set; }
        public bool is_realized { get; set; }
    }

    public class Representative_Fv
    {
        public string model_id { get; set; }
        public float[] feature { get; set; }
        public string type { get; set; }
    }

}
