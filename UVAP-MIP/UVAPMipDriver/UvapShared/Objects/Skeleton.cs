using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VideoOS.Platform.Metadata;

namespace UvapShared.Objects
{
    // Default class for the native skeleton json format.
    [Serializable]
    public class Skeleton
    {
        [JsonProperty("points")]
        public Point[] Points { get; set; }

        [JsonIgnore]
        public bool EndOfFrame { get; set; }

        [JsonProperty("end_of_frame")]
        private bool end_of_frame { set { EndOfFrame = value; } }// Hack to set this property when deserializing but ignore when serializing the class

        public static Skeleton FromString(string json)
        {
            return JsonConvert.DeserializeObject<Skeleton>(json);
        }

        public string ToSlimJson()
        {
            return JsonConvert.SerializeObject(this)
                .Replace("points", "p")
                .Replace("end_of_frame", "eof")
                .Replace("confidence", "c")
                .Replace("type", "t");
        }

        [Serializable]
        public class Point
        {
            [JsonProperty("x")]
            public float X { get; set; }

            [JsonProperty("y")]
            public float Y { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("confidence")]
            public float Confidence { get; set; }

            public override string ToString() => Type;
        }

        public static class PointColors
        {
            private static Dictionary<string, Color> colorCache = new Dictionary<string, Color>();
            private static Dictionary<string, DisplayColor> displayColorCache = new Dictionary<string, DisplayColor>();

            public static DisplayColor GetDisplayColor(string type)
            {
                if (displayColorCache.ContainsKey(type))
                    return displayColorCache[type];

                int rSeed = Helper.StringToSeed("Red: " + type);
                int gSeed = Helper.StringToSeed("Green:" + type);
                int bSeed = Helper.StringToSeed("Blue: " + type);

                var dp = new DisplayColor(Helper.RandomByte(rSeed), Helper.RandomByte(gSeed), Helper.RandomByte(bSeed));
                displayColorCache[type] = dp;
                return dp;
            }

            public static Color GetColor(string type)
            {
                if (colorCache.ContainsKey(type))
                    return colorCache[type];

                int rSeed = Helper.StringToSeed("Red: " + type);
                int gSeed = Helper.StringToSeed("Green:" + type);
                int bSeed = Helper.StringToSeed("Blue: " + type);

                Color c = Color.FromArgb(255, Helper.RandomByte(rSeed), Helper.RandomByte(gSeed), Helper.RandomByte(bSeed));
                colorCache[type] = c;
                return c;
            }
        }
    }

    // Class based on the default skeleton json format but with shorter property names to save storage data in the databse when this is recorded and stored.
    public class SlimSkeleton
    {
        [JsonProperty("p")]
        public Point[] Points { get; set; }

        public static SlimSkeleton FromString(string json)
        {
            return JsonConvert.DeserializeObject<SlimSkeleton>(json);
        }

        public static string ToSlim(string skeletonJson)
        {
            return skeletonJson
                .Replace("points", "p")
                .Replace("confidence", "c")
                .Replace("type", "t");
        }

        public class Point
        {
            [JsonProperty("x")]
            public float X { get; set; }
            [JsonProperty("y")]
            public float Y { get; set; }
            [JsonProperty("t")]
            public string Type { get; set; }
            [JsonProperty("c")]
            public float Confidence { get; set; }

            public override string ToString() => Type;
        }
    }

}
