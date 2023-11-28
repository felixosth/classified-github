using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Shared
{
    [Serializable]
    public class TokenRequest
    {
        public string CameraId { get; set; }
        public TimeSpan ExpireAfter { get; set; }
        //public DateTime ExpirationDate { get; set; }
        public string RequestToken { get; set; }
        public RequestType ReqType { get; set; } = RequestType.Live;

        public string Password { get; set; }

        public DateTime ExportFrom { get; set; }
        public DateTime ExportTo { get; set; }

        public DateTime RequestTimestamp { get; set; }

        public string Comment { get; set; }

        public string TokenToRevoke { get; set; }

        public bool RequireBankID { get; set; }
        public int[] AccessGroups { get; set; }

        public enum RequestType
        {
            Live = 1,
            Recorded = 2,
            Revoke = 100
        }
    }

    [Serializable]
    public class TokenResponse : INotifyPropertyChanged
    {
        public TokenResponse(string token, TokenRequest request)
        {
            this.Token = token;
            this.Request = request;
            creationTime = DateTime.UtcNow;
        }

        public TokenResponse()
        {
            creationTime = DateTime.UtcNow;
        }

        public string Error { get; set; }
        public string CameraName { get; set; }

        private DateTime creationTime;

        public DateTime CreationTime => creationTime.ToLocalTime();

        public string Token { get; set; }
        public string ServerLink { get; set; }
        public string CompleteLink => ServerLink + "?t=" + Token;

        public string ErrorOrToken => Error ?? Token;

        private bool isRevoked;
        public bool IsRevoked
        {
            get => isRevoked;
            set
            {
                if(value != isRevoked)
                {
                    isRevoked = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(IsNotRevoked));
                }

            }
        }

        public bool IsNotRevoked => !IsRevoked;

        public TokenRequest Request { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class TokenProgress
    {
        public int Progress { get; set; }
        public string Message { get; set; }

        public string Error { get; set; }

        public TokenRequest Request { get; set; }

        public TokenProgress(int progress, string message, TokenRequest request)
        {
            Progress = progress;
            Message = message;
            this.Request = request;
        }

        public TokenProgress(string error, TokenRequest request)
        {
            Error = error;
            this.Request = request;
        }
    }
}
