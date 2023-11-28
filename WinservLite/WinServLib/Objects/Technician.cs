using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLib.Objects
{
    public class Technician
    {
        public string Name { get; set; }
        public string UserName { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}
