using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenALPR_Milestone.Background
{
    public class ALPR_SearchPlateResponse
    {
        public ALPRPlate[] SearchPlateList { get; set; }
    }

    public class ALPRPlate
    {
        public int pk { get; set; }
        public string model { get; set; }
        public Fields fields { get; set; }
    }

    public class Fields
    {
        public string region_confidence { get; set; }
        public string vehicle_make_model_confidence { get; set; }
        public int best_index { get; set; }
        public DateTime epoch_time_start { get; set; }
        public string site { get; set; }
        public int vehicle_region_height { get; set; }
        public int img_height { get; set; }
        public int camera_id { get; set; }
        public int plate_x3 { get; set; }
        public string vehicle_body_type_confidence { get; set; }
        public string agent_uid { get; set; }
        public string best_confidence { get; set; }
        public string vehicle_make_confidence { get; set; }
        public string vehicle_color { get; set; }
        public string vehicle_make { get; set; }
        public string camera { get; set; }
        public string agent_type { get; set; }
        public string best_plate { get; set; }
        public int vehicle_region_x { get; set; }
        public int vehicle_region_y { get; set; }
        public string best_uuid { get; set; }
        public int vehicle_region_width { get; set; }
        public int plate_y3 { get; set; }
        public int plate_y4 { get; set; }
        public string vehicle_body_type { get; set; }
        public int site_id { get; set; }
        public int company { get; set; }
        public object gps_latitude { get; set; }
        public int img_width { get; set; }
        public DateTime epoch_time_end { get; set; }
        public int plate_y1 { get; set; }
        public int plate_y2 { get; set; }
        public int crop_location { get; set; }
        public object gps_longitude { get; set; }
        public int direction_of_travel_degrees { get; set; }
        public string region { get; set; }
        public int direction_of_travel_id { get; set; }
        public string vehicle_make_model { get; set; }
        public int hit_count { get; set; }
        public string processing_time_ms { get; set; }
        public int plate_x2 { get; set; }
        public int plate_x1 { get; set; }
        public string vehicle_color_confidence { get; set; }
        public int plate_x4 { get; set; }
    }

}
