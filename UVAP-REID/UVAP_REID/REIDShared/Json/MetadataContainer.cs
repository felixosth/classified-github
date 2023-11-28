using System.Collections.Generic;

namespace REIDShared.Json
{
    public class MetadataContainer
    {
        public string CommonKey { get; set; }

        public UVAPDetection[] Detections { get; set; }

        public MetadataContainer()
        {
        }
    }
}
