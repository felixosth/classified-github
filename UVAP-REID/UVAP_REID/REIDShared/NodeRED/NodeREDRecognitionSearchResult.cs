using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REIDShared.NodeRED
{
    public class NodeREDRecognitionSearchResult
    {
        public string person { get; set; }
        public string personName { get; set; }
        public float score { get; set; }
        public string stream { get; set; }
        public DateTime time { get; set; }
        public int? personId { get; set; }
        public int? category { get; set; }
        public string categoryName { get; set; }
    }
}
