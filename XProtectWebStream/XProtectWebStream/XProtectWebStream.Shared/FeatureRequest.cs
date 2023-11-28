using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Shared
{
    [Serializable]
    public class FeatureRequest
    {

        public bool? LicenseIsValid { get; set; }
        public bool? CanShareLive { get; set; }
        public bool? CanShareRecorded { get; set; }
        public bool? CanSendSMS { get; set; }
        public bool? CanUseBankID { get; set; }

        public int MaxValidMinutes { get; set; }
        public int DefaultValidMinutes { get; set; }
    }
}
