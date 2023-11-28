using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Shared
{
    public static class Constants
    {
        public static class Messaging
        {
            public const string GlobalMessageId = "XProtectWebStream.Communication.Global.MessageId";
            public const string PrivateMessageId = "XProtectWebStream.Communication.Private.MessageId";

            public const string GlobalLinkSenderMessageId = "XProtectWebStream.Communication.Global.LinkSender.MessageId";

            public const string GlobalFeatureRequestMessageId = "XProtectWebStream.Communication.Global.FeatureRequest.MessageId";
            public const string PrivateFeatureRequestMessageId = "XProtectWebStream.Communication.Private.FeatureRequest.MessageId";

            public const string PrivateProgressMessageId = "XProtectWebStream.Communication.Private.Progress.MessageId";

            public const string GlobalAccessGroupsRequestMessageId = "XProtectWebStream.Communication.Global.AccessGroupsRequest.MessageId";
            public const string GlobalAccessGroupsResponseMessageId = "XProtectWebStream.Communication.Global.AccessGroupsResponse.MessageId";
        }
    }
}
