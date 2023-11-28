using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLib.Objects
{
    public class JobTimeType
    {
        public string Name { get; set; }
        public string Code { get; set; } 
        public long Recnum { get; set; }
        //public string Article { get; set; }

        public override string ToString() => Name;
    }
}
