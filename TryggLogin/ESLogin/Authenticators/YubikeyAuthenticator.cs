using LoginShared.Administration;
using LoginShared.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YubicoDotNetClient;

namespace ESLogin.Authenticators
{
    internal class YubikeyAuthenticator : BaseAuthenticator
    {
        public override string Name => "Yubikey";

        private const string yubiClientID = "39335", yubiSecretKey = "3BCCVF5wXedGb13ldW/fibV7x3k=";


        public override event EventHandler<User> OnUserGranted;
        public override event EventHandler<User> OnUserDenied;
        public override event EventHandler<StatusResponseEventArgs> OnStatusResponse;

        public override void Close()
        {
        }

        public override void Init()
        {
        }

        public override void StartLogin(User user, string otp)
        {
            try
            {
                var response = HandleYubikeyLogin(otp, user).Result;

                if(response != null && response.Status == YubicoResponseStatus.Ok)
                {
                    if(response.PublicId == user.AuthData)
                    {
                        OnUserGranted?.Invoke(this, user);
                    }
                    else
                    {
                        OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("Invalid OTP.", user, true));
                        OnUserDenied?.Invoke(this, user);
                    }
                }
                else if(response == null)
                {
                    throw new Exception("Unknown error");
                }
                else
                {
                    OnStatusResponse?.Invoke(this, new StatusResponseEventArgs($"Error: {response.Status.ToString()}.", user, true));
                    throw new Exception(response.Status.ToString());
                }
            }
            catch(Exception ex)
            {
                if(ex is AggregateException || ex is FormatException)
                    OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("Invalid OTP format.", user, true));
                else
                    OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("An unknown error occured. Try again.", user, true));

                Thread.Sleep(2000);
                OnUserDenied?.Invoke(this, user);
            }
        }

        async Task<IYubicoResponse> HandleYubikeyLogin(string otp, User user)
        {
            YubicoClient yubicoClient = new YubicoClient(yubiClientID, yubiSecretKey);
            return await yubicoClient.VerifyAsync(otp);
            
            //try
            //{
            //    var response = await yubicoClient.VerifyAsync(yubikey);

            //    return response != null && response.Status == YubicoResponseStatus.Ok;
            //}
            //catch(FormatException)
            //{
            //    OnStatusResponse?.Invoke(this, new StatusResponseEventArgs("Invalid OTP format", user, true));

            //    return false;
            //}
        }

        public static async Task<IYubicoResponse> TestYubikey(string otp)
        {
            YubicoClient yubicoClient = new YubicoClient(yubiClientID);
            yubicoClient.SetApiKey(yubiSecretKey);

            return await yubicoClient.VerifyAsync(otp);
        }
    }
}
