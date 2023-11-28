using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Login;
using VideoOS.Platform.Messaging;
using MessagingWrapper;
using YubicoDotNetClient;
using System.Net;

namespace AuthenticationSystem
{
    public class AuthSystem
    {
        public const string Version = "Beta v0.3";

        UserManagement userManagement;
        BankIDService bankID;

        MessageCommunication msgCom;
        List<object> msgComObjects = new List<object>();

        const string requestFilter = "AuthSystem.Com.LoginRequest";
        const string responseFilter = "AuthSystem.Com.LoginResponse";
        public const string updateUsersFilter = "AuthSystem.Com.UsersUpdate";

        const string yubiClientID = "39335", yubiSecretKey = "3BCCVF5wXedGb13ldW/fibV7x3k=";

        List<TryggLoginUser> addedUsers;

        List<LoginRequestUser> loginRequests = new List<LoginRequestUser>();
        Messaging messaging;

        public AuthSystem(string mguid, List<TryggLoginUser> addedUsers, BankIDService.Enviroment enviroment)
        {
            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            msgCom = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            messaging = new Messaging(msgCom);

            messaging.RegisterCustomMessageReciever(requestFilter).OnMessageRecieved += OnLoginRequestRecieved;
            messaging.RegisterCustomMessageReciever(updateUsersFilter).OnMessageRecieved += OnUsersUpdate;
            messaging.RegisterNativeMessageReciever(MessageCommunication.EndPointCloseIndication).OnMessageRecieved += OnEndpointClosed;

            userManagement = new UserManagement();
            bankID = new BankIDService(mguid, enviroment);
            bankID.OnUserCollect += BankID_OnUserCollect;
            bankID.OnLog += (s, e) =>
            {
                EnvironmentManager.Instance.Log(false, "TryggLogin - BankID", e);
            };
            bankID.OnUserCollectFailed += BankID_OnUserCollectFailed;

            this.addedUsers = addedUsers;

            //new Thread(OnlineTempUsersCheck).Start();
        }

        private void OnUsersUpdate(object sender, MessageData e)
        {
            lock(addedUsers)
            {
                var newList = e.Parameters[0] as List<TryggLoginUser>;
                addedUsers = newList;
                Log("Users updated.");
            }
        }

        public void RefreshUsers(List<TryggLoginUser> users)
        {
            addedUsers.Clear();
            addedUsers = users;
        }

        private void OnEndpointClosed(object sender, Message e)
        {
            var data = e.Data as EndPointIdentityData;
            //Log("Endpointclosed");
            if (data != null)
            {
                foreach (var user in userManagement.TempUsers)
                {
                    if (user.Name == data.IdentityName)
                    {
                        userManagement.DeleteUser(user);
                    }
                }
            }
        }

        private void OnLoginRequestRecieved(object sender, MessageData msgData)
        {
            var msgEndpoint = ((RegisteredCustomMessageReciever)sender).Message.ExternalMessageSourceEndPoint;

            var key = msgData.Parameters[0] as string;
            var pw = msgData.Parameters[1] as string;
            var ip = msgData.Parameters[2] as string;

            Log("Incoming login request from " + ip);

            var usr = userManagement.VerifyUser(addedUsers.ToArray(), key, pw);


            if (usr == null) // check yubico
            {
                try
                {
                    var value = HandleYubiKey(key, pw, msgEndpoint).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Log(ex.ToString(), error: true);
                }
            }
            else if(usr.BelongingRolePath == "error")
            {
                msgCom.TransmitMessage(new Message(responseFilter, Packer.Serialize(new MessageData("error", "Invalid login."))), msgEndpoint, null, null);
            }
            else if (usr.KeyType == AuthKeyTypes.BankID)
            {
                Log(usr.DisplayName + " is requesting a BankID login.");
                Log("Sending BankID authorization request for " + usr.DisplayName);
                var res = bankID.Auth(usr.Key, ip);
                Log("Starting collection for orderRef " + res.orderRef);
                bankID.StartCollect(res.orderRef);

                msgCom.TransmitMessage(new Message(responseFilter, Packer.Serialize(new MessageData("info", "Öppna BankID"))), msgEndpoint, null, null);

                loginRequests.Add(new LoginRequestUser()
                {
                    PersonalNumber = usr.Key,
                    Endpoint = msgEndpoint,
                    RolePath = usr.BelongingRolePath,
                    OrderRef = res.orderRef
                });
            }
            else
            {
                msgCom.TransmitMessage(new Message(responseFilter, Packer.Serialize(new MessageData("error", "Invalid login."))), msgEndpoint, null, null);
            }
        }

        async Task<bool> HandleYubiKey(string yubikey, string password, FQID endpoint)
        {
            YubicoClient yubicoClient = new YubicoClient(yubiClientID, yubiSecretKey);
            try
            {
                var response = await yubicoClient.VerifyAsync(yubikey);
                if (response != null && response.Status == YubicoResponseStatus.Ok)
                {
                    var usr = userManagement.VerifyUser(addedUsers.ToArray(), response.PublicId, password);

                    if (usr != null)
                    {
                        if (usr.BelongingRolePath == "error")
                        {
                            msgCom.TransmitMessage(new Message(responseFilter, Packer.Serialize(new MessageData("error", "Invalid login."))), endpoint, null, null);
                        }
                        else
                        {

                            new Thread(() =>
                            {
                                CreateAndSendUser(usr.BelongingRolePath, endpoint);
                            }).Start();
                            return true;
                        }
                    }
                    else
                    {
                        msgCom.TransmitMessage(new Message(responseFilter, Packer.Serialize(new MessageData("error", "Invalid login."))), endpoint, null, null);
                    }
                }
                else
                {
                    msgCom.TransmitMessage(new Message(responseFilter, Packer.Serialize(new MessageData("error", "Yubikey error: " + response?.Status.ToString()))), endpoint, null, null);
                }
            }
            catch (FormatException)
            {
                msgCom.TransmitMessage(new Message(responseFilter, Packer.Serialize(new MessageData("error", "Invalid login."))),
                    endpoint, null, null);
            }
            catch(Exception ex)
            {
                Log(ex.ToString());
            }


            return false;
        }

        private void BankID_OnUserCollectFailed(object sender, BankIDResponses.Collect e)
        {
            foreach(var request in loginRequests)
            {
                if(request.OrderRef == e.orderRef)
                {
                    string message = "";
                    switch(e.hintCode)
                    {
                        case BankIDResponses.HintCodes.userCancel:
                            message = "The action was cancelled. Please try again.";
                            break;

                        case BankIDResponses.HintCodes.cancelled:
                            message = "The action was cancelled. Please try again.";
                            break;
                        default:
                            message = "An error has occured " + e.details + " (" + e.errorCode + ")";
                            break;
                    }

                    msgCom.TransmitMessage(new Message(responseFilter, Packer.Serialize(new MessageData("error", message))),
                        request.Endpoint, null, null);

                    loginRequests.Remove(request);
                    break;
                }
            }
        }

        private void BankID_OnUserCollect(object sender, BankIDResponses.Collect e)
        {
            var tmpRequests = new LoginRequestUser[loginRequests.Count];
            loginRequests.CopyTo(tmpRequests);

            foreach(var request in tmpRequests)
            {
                if(request.OrderRef == e.orderRef)
                {
                    loginRequests.Remove(request);

                    CreateAndSendUser(request.RolePath, request.Endpoint);
                }
            }
        }

        private void CreateAndSendUser(string rolePath, FQID endpoint)
        {
            string tmpUsername = GenerateUsername();
            bool isUnique = false;
            var allUsers = userManagement.GetAllBasicUsers();
            while (!isUnique)
            {
                bool found = false;
                foreach (var basicUser in allUsers)
                {
                    found = basicUser.DisplayName == tmpUsername;
                    if (found)
                    {
                        tmpUsername = GenerateUsername();
                        break;
                    }
                }
                isUnique = !found;
            }

            string tmpPassword = GeneratePassword();

            bool result = false;
            try
            {
                result = userManagement.CreateTmpUser(tmpUsername, tmpPassword, rolePath);
            }
            catch (Exception ex)
            {
                Log("Error creating user", true);
                Log(ex.ToString(), true);
            }

            if (result)
            {
                Log("Created user " + tmpUsername + " assigned to " + rolePath);
                var data = new MessageData("login", tmpUsername, tmpPassword);
                msgCom.TransmitMessage(new Message(responseFilter, Packer.Serialize(data)), endpoint, null, null);
            }
        }

        public void Close()
        {
            Log("Closing");

            var tempUsers = new MilestoneUser[userManagement.TempUsers.Count];
            userManagement.TempUsers.CopyTo(tempUsers);

            foreach (var user in tempUsers)
            {
                userManagement.DeleteUser(user);
            }

            foreach (var msgComObj in msgComObjects)
            {
                msgCom.UnRegisterCommunicationFilter(msgComObj);
            }

            messaging.Close();
            msgCom.Dispose();
        }

        void Log(string msg, bool error = false)
        {
            EnvironmentManager.Instance.Log(error, "TryggLogin", msg);
        }

        private string GenerateUsername()
        {
            return RandomString(random.Next(10, 30));
        }

        private string GeneratePassword()
        {
            //return System.Web.Security.Membership.GeneratePassword(random.Next(18, 40), 0).Replace('=', char.MinValue);
            return RandomPassword(random.Next(20, 50));
        }

        private Random random = new Random();
        string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        string RandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#¤%&()?*^-_";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class LoginRequestUser
    {
        public string PersonalNumber { get; set; }
        public FQID Endpoint { get; set; }
        public string OrderRef { get; set; }
        public string RolePath { get; set; }
    }
}
