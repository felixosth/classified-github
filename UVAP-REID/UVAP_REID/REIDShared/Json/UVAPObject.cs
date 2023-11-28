using Newtonsoft.Json;

namespace REIDShared.Json
{
    public class UVAPObject
    {
        [JsonIgnore]
        public string KafkaKey { get; set; }
    }
}
