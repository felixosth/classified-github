using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;
using AuthenticationSystem;
using System.Reflection;
using System.IO;
using TryggLogin.Admin;
using InSupport_LicenseSystem;

namespace TryggLogin.Background
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
    public class TryggLoginBackgroundPlugin : BackgroundPlugin
    {
        LicenseWrapper licenseWrapper;

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get { return TryggLoginDefinition.TryggLoginBackgroundPlugin; }
        }

        AuthSystem auth;

        /// <summary>
        /// The name of this background plugin
        /// </summary>
        public override String Name
        {
            get { return "TryggLogin BackgroundPlugin"; }
        }

        /// <summary>
        /// Called by the Environment when the user has logged in.
        /// </summary>
        public override void Init()
        {
            var myMguid = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null) as string;
            licenseWrapper = new LicenseWrapper(myMguid);
            licenseWrapper.OnLicenseExpired += LicenseWrapper_OnLicenseExpired;
            var check = licenseWrapper.CheckLicense(false);

            if(check != LicenseCheckResult.Valid)
            {
                Logger.Log("Invalid license: " + check.ToString(), true);
                return;
            }
            licenseWrapper.Init();


            List<TryggLoginUser> users = null;
            BankIDService.Enviroment enviroment = BankIDService.Enviroment.test;
            var items = Configuration.Instance.GetItemConfigurations(TryggLoginDefinition.TryggLoginPluginId, null, TryggLoginDefinition.TryggLoginKind);
            if (items == null)
            {
                Logger.Log("Configurationitem is null.", true);
                return;
            }

            var item = items.Find(x => x.Name == "Users");

            enviroment = item.Properties.ContainsKey("bankidenviroment") ? (BankIDService.Enviroment)Enum.Parse(typeof(BankIDService.Enviroment), item.Properties["bankidenviroment"]) : BankIDService.Enviroment.test;


            if (item.Properties.ContainsKey("users"))
            {
                users = MessagingWrapper.Packer.Deserialize<TryggLogin.Admin.SubControls.UserManagement>(item.Properties["users"]).UserList;
            }

            auth = new AuthSystem(myMguid, users, enviroment);

            Logger.Log("Initialized TryggLogin " + AuthSystem.Version);
        }

        private void LicenseWrapper_OnLicenseExpired(object sender, LicenseCheckResult e)
        {
            Logger.Log("License status: " + e.ToString() + ". Contact InSupport for renewal.");
            auth.Close();
        }

        /// <summary>
        /// Called by the Environment when the user log's out.
        /// You should close all remote sessions and flush cache information, as the
        /// user might logon to another server next time.
        /// </summary>
        public override void Close()
        {
            auth.Close();
        }

        /// <summary>
        /// Define in what Environments the current background task should be started.
        /// </summary>
        public override List<EnvironmentType> TargetEnvironments
        {
            get { return new List<EnvironmentType>() { EnvironmentType.Service }; } // Default will run in the Event Server
        }

    }
}
