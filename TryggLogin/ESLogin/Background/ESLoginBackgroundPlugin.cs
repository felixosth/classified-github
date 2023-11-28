using ESLogin.Admin;
using ESLogin.Authenticators;
using LoginShared;
using LoginShared.Administration;
using LoginShared.Network;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Threading;
using System.Xml;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace ESLogin.Background
{
    /// <summary>
    /// A background plugin will be started during application start and be running until the user logs off or application terminates.<br/>
    /// The Environment will call the methods Init() and Close() when the user login and logout, 
    /// so the background task can flush any cached information.<br/>
    /// The base class implementation of the LoadProperties can get a set of configuration, 
    /// e.g. the configuration saved by the Options Dialog in the Smart Client or a configuration set saved in one of the administrators.  
    /// Identification of which configuration to get is done via the GUID.<br/>
    /// The SaveProperties method can be used if updating of configuration is relevant.
    /// <br/>
    /// The configuration is stored on the server the application is logged into, and should be refreshed when the ApplicationLoggedOn method is called.
    /// Configuration can be user private or shared with all users.<br/>
    /// <br/>
    /// This plugin could be listening to the Message with MessageId == Server.ConfigurationChangedIndication to when when to reload its configuration.  
    /// This event is send by the environment within 60 second after the administrator has changed the configuration.
    /// </summary>
    public class ESLoginBackgroundPlugin : BackgroundPlugin
    {
        public override Guid Id
        {
            get { return ESLoginDefinition.ESLoginBackgroundPlugin; }
        }

        public override String Name
        {
            get { return "TryggLogin Server Background Plugin"; }
        }

        private Messaging messaging;
        private Dictionary<FQID, User> onlineUsers = new Dictionary<FQID, User>();

        private List<User> users = new List<User>();


        private Dictionary<FQID, User> usersDataCompletionDict = new Dictionary<FQID, User>();

        private Dictionary<Constants.Authenticators, BaseAuthenticator> authenticators = new Dictionary<Constants.Authenticators, BaseAuthenticator>();

        private Item configItem;

        public override void Init()
        {
            messaging = new Messaging();
            RefreshConfigItem();

            var mguid = GetMGUID();
            authenticators[Constants.Authenticators.BankID] = new BankIDAuthenticator(mguid, BankIDAuthenticator.Environment.live);
            authenticators[Constants.Authenticators.Yubikey] = new YubikeyAuthenticator();

            foreach(var authenticator in authenticators.Values)
            {
                authenticator.OnUserGranted += Authenticator_OnUserGranted;
                authenticator.OnUserDenied += Authenticator_OnUserDenied;
                authenticator.OnStatusResponse += Authenticator_OnStatusResponse;
            }

            messaging.Init();
            messaging.RegisterReciever(Constants.MessageID, HandleIncomingMessages);
            messaging.RegisterNativeReciever(MessageCommunication.EndPointCloseIndication, EndPointClosed);
            messaging.RegisterNativeReciever(MessageCommunication.NewEndPointIndication, NewEndPoint);

            Log("Init (debug version)");
        }

        private void Authenticator_OnStatusResponse(object sender, StatusResponseEventArgs e)
        {
            messaging.Transmit(Constants.MessageID, new MessageData(Constants.Actions.LoginStatus, e), e.User.EndPoint);
        }

        private void Authenticator_OnUserDenied(object sender, User user)
        {
            Log(user.DisplayName + " is denied login");
            messaging.Transmit(Constants.MessageID, new MessageData(Constants.Actions.LoginDenied), user.EndPoint);
        }

        private void Authenticator_OnUserGranted(object sender, User user)
        {
            Log(user.DisplayName + " is granted login");
            foreach (var role in user.Roles)
            {
                ConfigApiWrapper.AddUserToRole(user.SID, role);
            }

            onlineUsers[user.EndPoint] = user;
            messaging.Transmit(Constants.MessageID, new MessageData(Constants.Actions.LoginApproval), user.EndPoint);
        }

        private void RefreshConfigItem()
        {
            Log("Refreshing config");
            Configuration.Instance.RefreshConfiguration(ESLoginDefinition.ESLoginKind);

            configItem = Configuration.Instance.GetItemConfiguration(ESLoginDefinition.ESLoginPluginId, ESLoginDefinition.ESLoginKind, ESLoginItemManager.ConfigItemId);

            if (configItem != null)
                lock (users)
                {
                    if (configItem.Properties.ContainsKey(Constants.UsersConfigKey))
                        users = Packer.Deserialize<List<User>>(configItem.Properties[Constants.UsersConfigKey]);
                    else
                        users = new List<User>();
                }
            else
                Log("No configuration found", true);
        }

        private void Log(string msg, bool error = false)
        {
            EnvironmentManager.Instance.Log(error, "TryggLogin", msg);
        }

        private void NewEndPoint(Message message)
        {
            var data = message.Data as EndPointIdentityData;
            if(data != null)
            {
                var item = Configuration.Instance.GetItem(data.EndPointFQID);
            }
        }

        private void EndPointClosed(Message message)
        {
            var data = message.Data as EndPointIdentityData;

            if(onlineUsers.ContainsKey(data.EndPointFQID))
            {
                Log(onlineUsers[data.EndPointFQID].DisplayName + " logged out");
                foreach(var role in onlineUsers[data.EndPointFQID].Roles)
                {
                    ConfigApiWrapper.RemoveUserFromRole(onlineUsers[data.EndPointFQID].SID, role);
                }
                onlineUsers.Remove(data.EndPointFQID);
            }
        }

        private void HandleIncomingMessages(MessageData message)
        {
            switch(message.Action)
            {
                case Constants.Actions.LoginRequest: HandleLoginRequest(message); break;
                case Constants.Actions.ConfigurationChanged: RefreshConfigItem(); break;
                case Constants.Actions.LoginDataCompletion: DataCompletionHandling(message); break;
            }
        }

        private void DataCompletionHandling(MessageData messageData)
        {
            if(usersDataCompletionDict.ContainsKey(messageData.Sender))
            {
                var user = usersDataCompletionDict[messageData.Sender];
                var data = messageData.Deserialize<string>();
                new Thread(() => authenticators[user.Authenticator].StartLogin(user, data)).Start();
            }
        }

        private void HandleLoginRequest(MessageData message)
        {
            var sid = message.Deserialize<string>();
            User user = GetUser(sid);

            if (user == null)
                return;

            user.EndPoint = message.Sender;

            Log(user.DisplayName + " is requesting login");
            switch(user.Authenticator)
            {
                case Constants.Authenticators.BankID:
                    new Thread(() => authenticators[user.Authenticator].StartLogin(user, null)).Start(); // Start login process
                    break;
                case Constants.Authenticators.Yubikey:
                    Log("Missing data, awaiting completion..");
                    usersDataCompletionDict[user.EndPoint] = user;
                    break;
            }

            
            messaging.Transmit(Constants.MessageID, new MessageData(Constants.Actions.LoginRequestAck, user.Authenticator));
        }

        User GetUser(string sid)
        {
            lock (users)
            {
                var foundUser =  users.Where(u => u.SID == sid).FirstOrDefault();

                if(foundUser == null)
                {
                    var user = new User(sid);

                    List<string> roles = new List<string>();

                    var adUserGroups = user.GetGroups();
                    foreach(var grp in users.Where(u => u.IsGroup))
                    {
                        if(adUserGroups.Any(g => g.Sid.Value == grp.SID))
                        {
                            user.Authenticator = grp.Authenticator;
                            roles.AddRange(grp.Roles);
                            //return grp;
                        }
                    }

                    if(roles.Count > 0)
                    {
                        user.Roles = roles.ToArray();
                        return user;
                    }
                }
                else
                    return foundUser;
            }
            return null;
        }

        public override void Close()
        {
            Log("Close");
            messaging.UnregisterReciever(Constants.MessageID);
            messaging.UnregisterReciever(MessageCommunication.EndPointCloseIndication);
        }

        public override List<EnvironmentType> TargetEnvironments
        {
            get { return new List<EnvironmentType>() { EnvironmentType.Service }; } // Default will run in the Event Server
        }

        public static string GetMGUID()
        {
            RegistryKey keyBaseX64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey keyBaseX86 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey keyX64 = keyBaseX64.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
            RegistryKey keyX86 = keyBaseX86.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
            object resultObjX64 = keyX64.GetValue("MachineGuid", (object)"default");
            object resultObjX86 = keyX86.GetValue("MachineGuid", (object)"default");
            keyX64.Close();
            keyX86.Close();
            keyBaseX64.Close();
            keyBaseX86.Close();
            keyX64.Dispose();
            keyX86.Dispose();
            keyBaseX64.Dispose();
            keyBaseX86.Dispose();
            if (resultObjX64 != null && resultObjX64.ToString() != "default")
            {
                return resultObjX64.ToString();
            }
            if (resultObjX86 != null && resultObjX86.ToString() != "default")
            {
                return resultObjX86.ToString();
            }

            return "not found";
        }
    }
}
