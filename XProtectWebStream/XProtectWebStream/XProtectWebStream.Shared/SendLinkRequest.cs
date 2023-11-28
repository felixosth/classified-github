using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Shared
{
    [Serializable]
    public class SendLinkRequest
    {
        public string Recipient { get; set; }
        public SendLinkType LinkType { get; set; }
        public string Token { get; set; }
        public string FullLink { get; set; }

        public SendLinkRequest(string recipient, string token, string fullLink, SendLinkType linkType)
        {
            Recipient = recipient;
            LinkType = linkType;
            this.Token = token;
            this.FullLink = fullLink;
        }

        public enum SendLinkType
        {
            Email = 1,
            SMS = 2
        }
    }
}
