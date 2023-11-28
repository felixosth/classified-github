using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginShared
{
    public static class Constants
    {
        public const string MessageID = "InSupport.Login.MsgCom";
        public const string UsersConfigKey = "InSupport.TryggLogin.Users";




        public enum Actions
        {
            LoginRequest,
            LoginRequestAck,
            LoginApproval,
            LoginDataCompletion,
            LoginDenied,
            LoginStatus,
            ConfigurationChanged,
        }
        

        public enum Authenticators
        {
            BankID,
            Yubikey
        }

    }
}
