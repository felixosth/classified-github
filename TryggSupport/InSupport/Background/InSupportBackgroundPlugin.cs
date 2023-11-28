using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace InSupport.Background
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
    public class InSupportBackgroundPlugin : BackgroundPlugin
    {
        private bool _stop = false;
        private Thread _thread;

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get { return InSupportDefinition.InSupportBackgroundPlugin; }
        }

        /// <summary>
        /// The name of this background plugin
        /// </summary>
        public override String Name
        {
            get { return "InSupport BackgroundPlugin"; }
        }

        /// <summary>
        /// Called by the Environment when the user has logged in.
        /// </summary>
        public override void Init()
        {
            _stop = false;
            _thread = new Thread(new ThreadStart(Run));
            _thread.Name = "InSupport Background Thread";
            _thread.Start();

        }

        /// <summary>
        /// Called by the Environment when the user log's out.
        /// You should close all remote sessions and flush cache information, as the
        /// user might logon to another server next time.
        /// </summary>
        public override void Close()
        {
            _stop = true;
        }

        /// <summary>
        /// Define in what Environments the current background task should be started.
        /// </summary>
        public override List<EnvironmentType> TargetEnvironments
        {
            get { return new List<EnvironmentType>() { EnvironmentType.SmartClient }; } // Default will run in the Event Server
        }


        /// <summary>
        /// the thread doing the work
        /// </summary>
        private void Run()
        {
            EnvironmentManager.Instance.Log(false, "InSupport background thread", "Now starting...", null);


            SmartClientMessageData messageData = new SmartClientMessageData();
            messageData.MessageId = new object();

            string lastMessage = "";
            int ErrorTries = 0;

            using (WebClient webClient = new WebClient())
            {
                while (!_stop)
                {

                    try
                    {
                        string criticalInfo = webClient.DownloadString(new Uri("http://83.233.164.117/pluginhttp/criticalInfo.txt"));

                        if (criticalInfo != "" && criticalInfo != string.Empty)
                        {
                            if (lastMessage == criticalInfo)
                                continue;

                            lastMessage = criticalInfo;
                            messageData.Message = "Meddelande från InSupport: " + criticalInfo;
                            messageData.MessageType = SmartClientMessageDataType.Warning;
                            messageData.Priority = SmartClientMessageDataPriority.High;
                            messageData.IsClosable = true;
                            

                            VideoOS.Platform.Messaging.Message message = new VideoOS.Platform.Messaging.Message(MessageId.SmartClient.SmartClientMessageCommand, messageData);
                            EnvironmentManager.Instance.SendMessage(message);
                        }
                    }
                    catch
                    {
                        ErrorTries++;
                        if (ErrorTries >= 3)
                            break;

                        if (Properties.Settings.Default.Debug)
                            throw;
                    }

                    Thread.Sleep(15.ToSeconds());
                }
            }
            EnvironmentManager.Instance.Log(false, "InSupport background thread", "Now stopping...", null);
            _thread = null;
        }
    }

    public static class Extensions
    {
        public static int ToSeconds(this int milisec)
        {
            return milisec * 1000;
        }

        public static int ToMinutes(this int seconds)
        {
            return seconds * 60;
        }
    }
}
