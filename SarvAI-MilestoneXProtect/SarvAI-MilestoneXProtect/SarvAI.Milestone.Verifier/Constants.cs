using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarvAI.Milestone.Verifier
{
    public static class Constants
    {
        public static class Service
        {
            public const string ServiceDisplayName = "SarvAI Milestone Verifier";
            public const string ServiceName = "SarvAIVerifier";

            public static class EventID
            {
                public const int Error = 25530;
                public const int SuccessfulPost = 25531;
            }

            public static class Installer
            {
                public const string CustomUserAccountKey = "CustomUserAccount";
            }
        }

    }

}
