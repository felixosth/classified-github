using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Database.Objects
{
    public class AccessUser
    {
        public int Id { get; set; }
        public int AccessGroupId { get; set; }
        public string Name { get; set; }

        private bool HidePNR { get; set; }

        private string fullPnr;
        public string PNR => HidePNR ? fullPnr.Remove(fullPnr.Length - 4, 4) + "-XXXX" : fullPnr;

        internal AccessUser(int id, int accessGroupId, string name, string pnr, bool hidePnr = true)
        {
            Id = id;
            AccessGroupId = accessGroupId;
            Name = name;
            fullPnr = pnr;
            HidePNR = hidePnr;
        }

        internal bool MatchPNR(string pnrToMatch)
        {
            return fullPnr == pnrToMatch;
        }

        public override string ToString() => $"({Id}) {Name}";
    }
}
