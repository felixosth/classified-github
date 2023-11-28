using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;
using LoginShared;
using VideoOS.Platform.Login;
using LoginShared.Network;

namespace SCLogin.Login
{
    public class MyLoginPlugin : LoginPlugin
    {
        public override Guid Id => new Guid("{279D9BF6-69D0-46B2-90BD-908664E0E151}");
        public override bool IncludeInLoginFlow => true;
        public override string Name => "TryggLoginSC";

        Messaging messaging;

        Constants.Authenticators authenticator;

        LoginUserControl userControl;

        //BankIDLoginUserControl bankIDLoginUserControl;
        //YubikeyLoginUserControl yubikeyLoginUserControl;
        string lastUsrControlMsg;

        public bool WaitingForAck = true;

        public bool WaitingForApproval = true;

        public override void Close()
        {
            messaging.UnregisterReciever(Constants.MessageID);
            //bankIDLoginUserControl = null;
            messaging = null;
        }

        public override LoginUserControl GenerateCustomLoginUserControl()
        {
            if (WaitingForAck)
                return null;

            switch(authenticator)
            {
                case Constants.Authenticators.BankID:
                    userControl = new BankIDLoginUserControl(this);
                    break;
                case Constants.Authenticators.Yubikey:
                    userControl = new YubikeyLoginUserControl(this);
                    break;
            }
            return userControl;
        }

        public override void Init(string surveilanceUsername)
        {
            EnvironmentManager.Instance.Log(false, "TryggLogin", "Init");

            messaging = new Messaging();
            messaging.Init();

            SCLoginDefinition.SuppressRefresh = false;

            WaitingForAck = true;
            WaitingForApproval = true;

            messaging.RegisterReciever(Constants.MessageID, HandleIncomingMessages);
        }

        public void SendMoreData(string data)
        {
            messaging.Transmit(Constants.MessageID, new MessageData(Constants.Actions.LoginDataCompletion, data));
        }

        void HandleIncomingMessages(MessageData data)
        {
            EnvironmentManager.Instance.Log(false, "TryggLogin", "Incoming message: " + data.Action.ToString());

            switch (data.Action)
            {
                case Constants.Actions.LoginApproval: 
                    WaitingForApproval = false;
                    ClientControl.Instance.CallOnUiThread(() =>
                    {
                        userControl.SignalCompleted();
                    });
                    break;
                case Constants.Actions.LoginRequestAck:
                    authenticator = data.Deserialize<Constants.Authenticators>();
                    WaitingForAck = false;
                    break;
                case Constants.Actions.LoginStatus:
                    var statusResponse = data.Deserialize<StatusResponseEventArgs>();
                    if (userControl == null)
                        break;

                    ClientControl.Instance.CallOnUiThread(() =>
                    {
                        if (!statusResponse.Error)
                            userControl.ProgressMessage = statusResponse.Message;
                        else
                            userControl.ErrorMessage = statusResponse.Message;
                        lastUsrControlMsg = statusResponse.Message;
                    });
                    break;
                case Constants.Actions.LoginDenied:

                    var now = DateTime.Now;
                    var terminate = now.AddSeconds(10);

                    while(terminate > now)
                    {
                        now = DateTime.Now;

                        var remainingSeconds = Math.Round((terminate - now).TotalSeconds);
                        ClientControl.Instance.CallOnUiThread(() =>
                        {
                            userControl.ErrorMessage = lastUsrControlMsg + " Terminating application in " + remainingSeconds + "s";
                        });
                        Thread.Sleep(250);
                    }
                    Environment.Exit(0);
                    break;
            }
        }

        public override void LoginFlowExecute()
        {
            DateTime start = DateTime.Now;
            LoginSettings ls = LoginSettingsCache.GetLoginSettings(EnvironmentManager.Instance.MasterSite);

            messaging.Transmit(Constants.MessageID, new MessageData(Constants.Actions.LoginRequest, ls.UserIdentity));
            while(WaitingForAck)
            {
                Thread.Sleep(50);

                if (WaitingForAck && (DateTime.Now - start).TotalSeconds > 10)
                { // Plugin is probably not installed on server
                    SCLoginDefinition.SuppressRefresh = true;
                    break;
                }
            }
        }
    }
}
