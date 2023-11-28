using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using TryggRetail.Client;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Data;
using VideoOS.Platform.Messaging;
using InSupport_LicenseSystem;
using TryggRetail.Playback;
using TryggRetail.PopupWindow.Acknowledge;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TryggRetail.Admin;
using VideoOS.Platform.License;
using TryggRetail.PopupWindow.Live;

namespace TryggRetail.Background
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
    public class TryggRetailBackgroundPlugin : BackgroundPlugin
    {
        public Alarm CurrentAlarm;

        //public MultiWindowCommandData lastData;
        public MultiWindowCommandData data;
        public FQID dataWindowFQID;


        bool closeThread = false;
        //public static bool PlaybackAlarm = false;
        public PlaybackAlarmUserControl CurrentPlaybacker;

        public PopupConfig CurrentPopupConfig;

        private Guid _licenseObject = Guid.Empty;


        private object _alarmObj, _reqApprovalObj;
        public static MessageCommunication _messageCommunication;


        public /*static*/ LicenseCheckResult LicenseStatus = LicenseCheckResult.Error;

        bool isRunning = false;


        //bool checkLicThread = true;
        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get { return TryggRetailDefinition.TryggRetailBackgroundPlugin; }
        }

        /// <summary>
        /// The name of this background plugin
        /// </summary>
        public override String Name
        {
            get { return "TryggRetail BackgroundPlugin"; }
        }

        /// <summary>
        /// Called by the Environment when the user has logged in.
        /// </summary>
        public override void Init()
        {
            new Thread(ReqPermission).Start();
            //ReqPermission();
        }

        private void ReqPermission()
        {
            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            _messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            _reqApprovalObj = _messageCommunication.RegisterCommunicationFilter(GetServerApproval, new CommunicationIdFilter(TryggRetailDefinition.ApproveClientFilter));


            try
            {
                while (!isRunning || !closeThread)
                {
                    ConcurrentLicenseUsed[] used = EnvironmentManager.Instance.LicenseManager.ConcurrentLicenseManager.
                        GetConcurrentUseList(
                        TryggRetailDefinition.TryggRetailPluginId, TryggRetailDefinition.LicenseID);
                    bool instanceExists = false;
                    foreach (var lic in used)
                    {
                        if (lic.MachineName == Environment.MachineName)
                        {
                            instanceExists = true;
                            break;
                        }
                    }

                    if (!instanceExists)
                    {
                        _messageCommunication.TransmitMessage(new VideoOS.Platform.Messaging.Message(TryggRetailDefinition.ClientApprovalFilter), null, null, null);
                    }

                    if (closeThread)
                        break;
                    //Thread.Sleep(new TimeSpan(0, 10, 0)); // default, 10 min timeout
                    Thread.Sleep(5 * 1000);
                }
            }
            catch { }
        }

        public void StartServices()
        {
            isRunning = true;

            _licenseObject = EnvironmentManager.Instance.LicenseManager.ConcurrentLicenseManager.RegisterConcurrentLicense(
                                TryggRetailDefinition.TryggRetailPluginId,
                                TryggRetailDefinition.LicenseID);

            _alarmObj = _messageCommunication.RegisterCommunicationFilter(NewAlarmMessageHandler, new CommunicationIdFilter(TryggRetailDefinition.NewAlarmFilter));

        }

        private object GetServerApproval(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            StartServices();

            return null;
        }

        public FQID GetDataWindowFQID()
        {
            int slept = 0;
            while (dataWindowFQID == null)
            {
                slept += 50;
                Thread.Sleep(50);

                if (slept > 1000)
                    break;
            }

            return dataWindowFQID;
        }


        public void NullifyWindowData()
        {
            dataWindowFQID = null;
        }

        /// <summary>
        /// Called by the Environment when the user log's out.
        /// You should close all remote sessions and flush cache information, as the
        /// user might logon to another server next time.
        /// </summary>
        public override void Close()
        {
            //checkLicThread = false;
            closeThread = true;
            StopServices();
        }

        private void StopServices()
        {
            isRunning = false;
            if(_alarmObj != null)
                _messageCommunication.UnRegisterCommunicationFilter(_alarmObj);
            if(_reqApprovalObj != null)
                _messageCommunication.UnRegisterCommunicationFilter(_reqApprovalObj);

            try
            {
                if (_licenseObject != Guid.Empty)
                    EnvironmentManager.Instance.LicenseManager.ConcurrentLicenseManager.UnRegisterConcurrentLicense(_licenseObject);
            }
            catch { }
        }

        //public int MaxWindows { get; set; }
        public int WindowCount = 0;

        private object NewAlarmMessageHandler(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            Alarm alarm = message.Data as Alarm;
            if (alarm.ReferenceList.Count < 1)
                return null;

            EnvironmentManager.Instance.SendMessage(new Message(TryggRetailDefinition.CloseAllWindowsFilter));

            CurrentAlarm = alarm;
            Configuration.Instance.RefreshConfiguration(TryggRetailDefinition.TryggRetailPluginId);
            var configItems = Configuration.Instance.GetItemConfigurations(TryggRetailDefinition.TryggRetailPluginId, null, TryggRetailDefinition.TryggRetailKind);
            PopupConfig config = null;
            bool foundConfig = false;
            foreach (var item in configItems)
            {
                if (item.Properties.ContainsKey("configuration"))
                {
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(item.Properties["configuration"])))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        config = (PopupConfig)bf.Deserialize(ms);
                    }
                    if (config.AlarmName == alarm.EventHeader.Name)
                    {
                        foundConfig = true;
                        break;
                    }
                }
            }
            if (!foundConfig)
                return null;

            //if (WindowCount >= Properties.Settings.Default.MaxWindows)
            //    return null;
            //WindowCount++;

            CurrentPopupConfig = config;
            string viewName = "Popup";
            string groupName = "PopupViewGroup";
            var groups = ClientControl.Instance.GetViewGroupItems();
            bool found = false;
            ConfigItem tempGroupItem = null;
            ConfigItem groupItem = null;

            foreach (Item item in groups)
            {
                if (item.Name == groupName)
                {
                    found = true;
                    tempGroupItem = (ConfigItem)item;
                    var children = tempGroupItem.GetChildren();
                    for (int i = 0; i < children.Count; i++)
                    {
                        if (children[i].Name == "AlarmViewGroup")
                            groupItem = (ConfigItem)children[i];
                    }
                    break;
                }
            }
            if (!found)
            {
                tempGroupItem = (ConfigItem)ClientControl.Instance.CreateTemporaryGroupItem(groupName);
                groupItem = tempGroupItem.AddChild("AlarmViewGroup", Kind.View, FolderType.UserDefined);
                ViewAndLayoutItem viewAndLayoutItem = (ViewAndLayoutItem)groupItem.AddChild(viewName, Kind.View, FolderType.No);
                viewAndLayoutItem.Layout = new System.Drawing.Rectangle[]
                {
                                new System.Drawing.Rectangle(0,0,499,700),
                                new System.Drawing.Rectangle(500,0,500,700),
                                new System.Drawing.Rectangle(0,701,1000,299)
                };
                //Dictionary<String, String> cameraViewItemProperties = new Dictionary<string, string>();
                //cameraViewItemProperties.Add("CameraId", alarm.ReferenceList[0].FQID.ObjectId.ToString());

                //viewAndLayoutItem.InsertBuiltinViewItem(0, ViewAndLayoutItem.CameraBuiltinId, cameraViewItemProperties);
                viewAndLayoutItem.InsertViewItemPlugin(0, new LiveViewManager(this), new Dictionary<string, string>());
                viewAndLayoutItem.InsertViewItemPlugin(1, new PlaybackAlarmManager(this), new Dictionary<string, string>());
                //viewAndLayoutItem.InsertViewItemPlugin(1, new PlaybackAlarmManager(this), new Dictionary<string, string>());
                viewAndLayoutItem.InsertViewItemPlugin(2, new AcknowledgeWpfUserControlManager(config, this), new Dictionary<string, string>());

                ClientControl.Instance.CallOnUiThread((() =>
                {
                    viewAndLayoutItem.Save();
                    tempGroupItem.PropertiesModified();

                }));
            }

            data = new MultiWindowCommandData();
            List<Item> screens = Configuration.Instance.GetItemsByKind(Kind.Screen);

            if (screens != null && screens.Count > 0)
            {
                data.Screen = screens[0].FQID;
                List<Item> windows = Configuration.Instance.GetItemsByKind(Kind.Window);
                if (windows != null && windows.Count > 0)
                {
                    data.Window = windows[0].FQID;
                    foreach (Item view in groupItem.GetChildren())
                    {
                        if (view.Name.Equals(viewName))
                        {
                            data.View = view.FQID;
                            data.X = config.WindowPosition.X;
                            data.Y = config.WindowPosition.Y;
                            data.Width = config.WindowSize.Width;
                            data.Height = config.WindowSize.Height;
                            if (config.IsFullscreen)
                                data.MultiWindowCommand = MultiWindowCommand.OpenFullScreenWindow;
                            else
                                data.MultiWindowCommand = MultiWindowCommand.OpenFloatingWindow;


                            //data.X = 160;
                            //data.Y = 90;
                            //data.Height = 900;
                            //data.Width = 1600;
                            ////data.MultiWindowCommand = "OpenFloatingWindow";  //windowed
                            //data.MultiWindowCommand = "OpenFullScreenWindow";  //fullscreen
                            data.PlaybackSupportedInFloatingWindow = false;

                            ClientControl.Instance.CallOnUiThread((() =>
                            {
                                data.Window = (FQID)EnvironmentManager.Instance.SendMessage(new VideoOS.Platform.Messaging.Message(MessageId.SmartClient.MultiWindowCommand, data))[0];
                            }));

                            dataWindowFQID = data.Window;

                            return null;
                        }
                    }
                }
            }


            return null;
        }

        /// <summary>
        /// Define in what Environments the current background task should be started.
        /// </summary>
        public override List<EnvironmentType> TargetEnvironments
        {
            get { return new List<EnvironmentType>() { EnvironmentType.SmartClient }; } // Default will run in the Event Server
        }
    }
}
