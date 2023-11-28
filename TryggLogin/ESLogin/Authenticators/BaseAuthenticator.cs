using LoginShared.Administration;
using LoginShared.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;

namespace ESLogin.Authenticators
{
    internal abstract class BaseAuthenticator
    {
        public abstract event EventHandler<User> OnUserGranted;
        public abstract event EventHandler<User> OnUserDenied;
        public abstract event EventHandler<StatusResponseEventArgs> OnStatusResponse;

        public abstract string Name { get; }

        public abstract void Init();

        public abstract void StartLogin(User user, string data);

        public abstract void Close();
        
        protected void Log(string msg, bool error = false)
        {
            EnvironmentManager.Instance.Log(error, Name, msg);
        }
    }


}
