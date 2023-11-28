using Newtonsoft.Json;
using System.Windows;

namespace REIDShared.Json
{
    public class DetectionJsonEntry : UVAPObject
    {
        [JsonIgnore]
        public string type { get; set; }
        public Bounding_Box bounding_box { get; set; }
        public float detection_confidence { get; set; }
        public bool end_of_frame { get; set; }
    }

    public class Bounding_Box
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public Rect ToRect(float scaleX, float scaleY)
        {
            return new Rect(x * scaleX, y * scaleY, width * scaleX, height * scaleY);
        }
    }

}
