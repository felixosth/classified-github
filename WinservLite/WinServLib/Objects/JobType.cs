using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLib.Objects
{
    public class JobType
    {
        public string NR { get; set; }
        public string Name { get; set; }
        public string Article { get; set; }
        public override string ToString() => Name;
    }
}
