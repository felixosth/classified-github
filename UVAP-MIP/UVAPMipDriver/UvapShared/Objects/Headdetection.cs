using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UvapShared.Objects
{
    // Default class for the native head detection json format.
    [Serializable]
    public class HeadDetection
    {
        [JsonProperty("bounding_box")]
        public Bounding_Box BoundingBox { get; set; }

        [JsonProperty("detection_confidence")]
        public float Confidence { get; set; }

        [JsonIgnore]
        public bool EndOfFrame { get; set; }

        [JsonProperty("end_of_frame")]
        private bool end_of_frame { set { EndOfFrame = value; } } // Hack to set this property when deserializing but ignore when serializing the class

        public static HeadDetection FromString(string json)
        {
            return JsonConvert.DeserializeObject<HeadDetection>(json);
        }

        [Serializable]
        public class Bounding_Box
        {
            [JsonProperty("x")]
            public int X { get; set; }
            [JsonProperty("y")]
            public int Y { get; set; }
            [JsonProperty("width")]
            public int Width { get; set; }
            [JsonProperty("height")]
            public int Height { get; set; }
        }
    }

    // Class based on the default head detection json format but with shorter property names to save storage data in the databse when this is recorded and stored.
    public class SlimHeadDetection
    {
        [JsonProperty("b")]
        public Bounding_Box BoundingBox { get; set; }

        [JsonProperty("c")]
        public float Confidence { get; set; }

        public static string ToSlim(string input)
        {
            return input
                .Replace("bounding_box", "b")
                .Replace("detection_confidence", "c")
                .Replace("width", "w")
                .Replace("height", "h");
        }

        public static SlimHeadDetection FromString(string json)
        {
            return JsonConvert.DeserializeObject<SlimHeadDetection>(json);
        }

        [Serializable]
        public class Bounding_Box
        {
            [JsonProperty("x")]
            public int X { get; set; }

            [JsonProperty("y")]
            public int Y { get; set; }

            [JsonProperty("w")]
            public int Width { get; set; }

            [JsonProperty("h")]
            public int Height { get; set; }

            [JsonIgnore]
            public int CenterX
            {
                get
                {
                    return X + (Width / 2);
                }
            }

            [JsonIgnore]
            public int CenterY
            {
                get
                {
                    return Y + (Height / 2);
                }
            }

            public Rect ToRect(float scaleX, float scaleY)
            {
                return new Rect(X * scaleX, Y * scaleY, Width * scaleX, Height * scaleY);
            }
        }
    }
}
