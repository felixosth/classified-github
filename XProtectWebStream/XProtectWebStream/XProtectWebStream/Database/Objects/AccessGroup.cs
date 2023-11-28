using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Database.Objects
{
    public class AccessGroup
    {
        public const int UnassignedGroupId = -2;
        public const int AnonGroupId = -1;

        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<AccessUser> Users { get; set; }

        internal AccessGroup(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => $"({Id}) {Name}";
    }
}
