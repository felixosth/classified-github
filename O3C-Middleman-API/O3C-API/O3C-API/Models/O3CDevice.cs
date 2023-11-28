using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O3C_API.Models
{
    public class O3CDevice
    {
        public int id { get; set; }
        public string client_id { get; set; }
        public string client_srcaddr { get; set; }
        public string o3c_server { get; set; }

        public string mac { get; set; }
    }
}
