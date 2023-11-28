using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REIDShared.NodeRED
{
    public class NodeREDCategory
    {
        public string name { get; set; }
        public int? id { get; set; }

        public override string ToString() => $"{id}: {name}";

        public Guid GetGuid()
        {
            return Helper.StringToGuid(this.ToString());
        }
    }
}
