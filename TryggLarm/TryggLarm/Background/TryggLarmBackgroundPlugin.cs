using InSupport_LicenseSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;
using TryggLarm.Nodes;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Data;
using VideoOS.Platform.Messaging;

namespace TryggLarm.Background
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
    public class TryggLarmBackgroundPlugin : BackgroundPlugin
    {
        private object _alarmComObj, _licComObject;
        public static MessageCommunication _messageCommunication;

        NotificationSender notSender = new NotificationSender();

        LicenseWrapper licenseWrapper;
        string myMguid;
        string myLicense;

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get { return TryggLarmDefinition.TryggLarmBackgroundPlugin; }
        }

        /// <summary>
        /// The name of this background plugin
        /// </summary>
        public override String Name
        {
            get { return "TryggLarm BackgroundPlugin"; }
        }

        /// <summary>
        /// Called by the Environment when the user has logged in.
        /// </summary>
        public override void Init()
        {
            myMguid = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null) as string;
            if (string.IsNullOrEmpty(myMguid))
            {
                EnvironmentManager.Instance.Log(true, "TryggLarm", "No MGUID found.");
            }
            else
            {
                MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
                _messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

                int tries = 0;
                while(!_messageCommunication.IsConnected && tries < 5)
                {
                    EnvironmentManager.Instance.Log(false, "TryggLarm", "MsgCom is not connected, waiting...");
                    Thread.Sleep(3000);
                    tries++;
                }

                if (!_messageCommunication.IsConnected)
                    EnvironmentManager.Instance.Log(true, "TryggLarm", "MsgCom is still not connected!");

                EnvironmentManager.Instance.Log(false, "TryggLarm", "Checking license (" + myMguid + ")");
                licenseWrapper = new LicenseWrapper(myMguid, _messageCommunication);
                licenseWrapper.OnLicenseCheck += LicenseWrapper_OnLicenseCheck;
                licenseWrapper.OnLicenseExpired += LicenseWrapper_OnLicenseExpired;
                _licComObject = _messageCommunication.RegisterCommunicationFilter(licenseWrapper.LicenseCommunicationObj, new CommunicationIdFilter(TryggLarmDefinition.LicenseCommunicationFilter));

                licenseWrapper.Init();
                myLicense = licenseWrapper.LicenseKey;
            }
        }

        private void LicenseWrapper_OnLicenseExpired(object sender, LicenseCheckResult e)
        {
            StopServices();
        }

        private void LicenseWrapper_OnLicenseCheck(object sender, LicenseCheckResult e)
        {
            EnvironmentManager.Instance.Log(false, "TryggLarm", "License status: " + e.ToString(), null);
            if (e != LicenseCheckResult.Valid)
                StopServices();
            else
                StartServices();
        }

        bool isRunning = false;
        void StartServices()
        {
            if (isRunning) return;
            isRunning = true;



            _alarmComObj = _messageCommunication.RegisterCommunicationFilter(NewAlarmMessageHandler,
                new CommunicationIdFilter(MessageId.Server.NewAlarmIndication));



            EnvironmentManager.Instance.Log(false, "TryggLarm", "Services Started");
        }

        void StopServices()
        {
            if (!isRunning) return;
            isRunning = false;

            _messageCommunication.UnRegisterCommunicationFilter(_alarmComObj);
            //MessageCommunicationManager.Stop(EnvironmentManager.Instance.MasterSite.ServerId);
            //_messageCommunication.Dispose();
            EnvironmentManager.Instance.Log(false, "TryggLarm", "Services Stopped");

        }

        private object NewAlarmMessageHandler(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            Alarm alarm = message.Data as Alarm;

            var items = Configuration.Instance.GetItemsByKind(TryggLarmDefinition.TryggLarmKind);

            //prefetch images
            //var relatedImages = notSender.CreateImages(alarm, )

            foreach (var item in items) // loop through all plugin items
            {
                //var rootNodes = new List<CustomNode>();
                RootNode rootNode;
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(item.Properties["treeNodes"])))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    rootNode = (RootNode)bf.Deserialize(ms);
                }
                rootNode.AddChildNodes(null);

                foreach (CustomNode node in rootNode.Nodes) // loop through alarmObjects
                {
                    if(node is AlarmObjectNode) // ignore root node
                    {
                        //(node as AlarmObjectNode).DisplayName = "Hahahahhaha"; // wtf why did I do this
                        bool foundAlarm = false;
                        foreach(CustomNode grpNode in node.Nodes) // make sure we check the alarms first
                        {
                            if (grpNode is AlarmGroupNode)
                            {
                                foreach (CustomNode alarmNode in grpNode.Nodes)
                                {
                                    if (alarmNode.DisplayName == alarm.EventHeader.Name)
                                    {
                                        EnvironmentManager.Instance.Log(false, "TryggLarm", "Found match at " + rootNode.DisplayName, null);

                                        foundAlarm = true;
                                        break;
                                    }
                                }
                                if (foundAlarm)
                                    break;
                            }
                        }

                        if (!foundAlarm) //keep checking objects if we didnt find a match
                            continue;

                        foreach (CustomNode grpNode in node.Nodes) // then check other stuff if we found a matching alarm
                        {
                            if (grpNode is SMSRecipientGroupNode)
                            {
                                foreach(SMSRecipientNode smsNode in grpNode.Nodes)
                                {
                                    if(smsNode.LastActiveDate.AddMinutes(smsNode.CooldownTime) < DateTime.Now && smsNode.IsEnabled)
                                    {
                                        EnvironmentManager.Instance.Log(false, "TryggLarm", "Sending notification to " + smsNode.TelephoneNumber, null);
                                        smsNode.LastActiveDate = DateTime.Now;
                                        var result = notSender.SendSMS(smsNode, alarm, myMguid, myLicense);
                                        EnvironmentManager.Instance.Log(false, "TryggLarm", "Result: " + result, null);

                                    }
                                    //else
                                    //    EnvironmentManager.Instance.Log(false, "TryggLarm", "Cooldown still active or disabled, aborting SMS.", null);
                                }
                            }
                            else if(grpNode is EmailRecipientGroupNode)
                            {
                                foreach(EmailRecipientNode emailNode in grpNode.Nodes)
                                {
                                    if (emailNode.LastActiveDate.AddMinutes(emailNode.CooldownTime) < DateTime.Now && emailNode.IsEnabled)
                                    {
                                        emailNode.LastActiveDate = DateTime.Now;
                                        EnvironmentManager.Instance.Log(false, "TryggLarm", "Sending notification to " + emailNode.EmailAddress, null);
                                        notSender.SendEmail(emailNode, alarm);
                                    }
                                    //else
                                    //    EnvironmentManager.Instance.Log(false, "TryggLarm", "Cooldown still active  or disabled, aborting email.", null);
                                }
                            }
                            else if(grpNode is NokasGroupNode)
                            {
                                foreach(NokasNode nokasNode in grpNode.Nodes)
                                {
                                    var connector = new NokasConnector(nokasNode.Code, nokasNode.Pin);
                                    var result = connector.SendAlarm(nokasNode.Type, nokasNode.Info);
                                    EnvironmentManager.Instance.Log(false, "TryggLarm", "Nokas Alarm Result: " + result.ToString());
                                }
                            }
                        }
                    }
                }

                // save item bcuz cooldowns
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    BinaryFormatter bf = new BinaryFormatter();
                //    bf.Serialize(ms, rootNode);
                //    ms.Position = 0;
                //    byte[] buffer = new byte[(int)ms.Length];
                //    ms.Read(buffer, 0, buffer.Length);
                //    item.Properties["treeNodes"] = Convert.ToBase64String(buffer);
                //}
                item.Properties["treeNodes"] = Packer.Serialize(rootNode);
                Configuration.Instance.SaveItemConfiguration(TryggLarmDefinition.TryggLarmPluginId, item);
            }

            return null;
        }

        public override void Close()
        {
            _messageCommunication.UnRegisterCommunicationFilter(_licComObject);
            //EnvironmentManager.Instance.UnRegisterReceiver(_alarmComObj);
            StopServices();
            EnvironmentManager.Instance.Log(false, "TryggLarm", "Closed", null);
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
