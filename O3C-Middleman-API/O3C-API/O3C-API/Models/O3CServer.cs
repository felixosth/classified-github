using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O3C_API.Models
{
    public class O3CServer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public int AdminPort { get; set; }
        public int ClientPort { get; set; }
    }
}
