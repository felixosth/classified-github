using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarvAI.Milestone.Verifier.API
{
    public class UltinousResponse
    {
        [JsonProperty("event")]
        public bool Event { get; set; }

        [JsonProperty("score")]
        public float Score { get; set; }
        
        public string LocalVideoFile { get; set; }

        public override string ToString() => $"Event: {Event}, Score: {Score}";
    }
}
