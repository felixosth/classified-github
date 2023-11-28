using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLib.Objects
{
    public class Site
    {
        public string Name { get; set; } // INSTALL
        public string SiteID { get; set; }
        public string CustomerID { get; set; }
        public string ModelType { get; set; }  // MTYP
        public string Model { get; set; } // MODELL
        public string PostNR { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string RefCode { get; set; }
        public override string ToString() => Name;
    }
}
