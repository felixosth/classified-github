namespace REIDShared.Json
{
    public class UVAPDetection
    {
        public DetectionJsonEntry Detection { get; set; }
        public ReidJsonEntry Reid { get; set; }

        public AgeJsonEntry Age { get; set; }
        public GenderJsonEntry Gender { get; set; }

        public UVAPDetection()
        {
        }
    }
}
