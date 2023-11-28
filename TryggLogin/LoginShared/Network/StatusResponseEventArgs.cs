using LoginShared.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginShared.Network
{
    public class StatusResponseEventArgs : EventArgs
    {
        public User User { get; set; }
        public string Message { get; set; }
        public bool Error { get; set; }
        public StatusResponseEventArgs(string message, User user, bool error = false)
        {
            this.Message = message;
            this.Error = error;
            this.User = user;
        }
    }
}
