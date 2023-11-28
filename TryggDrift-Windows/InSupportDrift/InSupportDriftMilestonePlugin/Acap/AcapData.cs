using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSupport.Drift.Plugins
{
    public class AcapData
    {
        public Performance Performance { get; set; }
    }

    public class BaseMessage
    {
        public ulong Timestamp { get; set; }
    }

    public class Performance : BaseMessage
    {
        public float Cpu { get; set; }
        public float Network { get; set; }
    }
}
