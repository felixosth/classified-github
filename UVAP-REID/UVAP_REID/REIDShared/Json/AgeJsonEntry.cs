using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REIDShared.Json
{
    public class AgeJsonEntry : UVAPObject
    {
        public int age { get; set; }
        public float confidence { get; set; }
        //public bool end_of_frame { get; set; }
    }
}
