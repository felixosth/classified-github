using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REIDShared.NodeRED
{
    public class NodeREDPerson
    {
        public int id { get; set; }
        public string key { get; set; }
        public string personName { get; set; }
        public int category { get; set; }
        public string categoryName { get; set; }
        public string categoryColor { get; set; }

        public override string ToString() => personName;
    }
}
