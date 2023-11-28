using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryggRetail.Admin
{
    [Serializable]
    public class LicenseCommunication
    {
        public LicenseComType LicenseComType { get; set; }
        public object MessageData { get; set; }
    }

    [Serializable]
    public enum LicenseComType
    {
        LicenseInfoRequest,
        LicenseInfoResponse,
        LicenseActivationRequest,
        LicenseActivationResponse,
        LicenseRefreshRequest,
    }
}
