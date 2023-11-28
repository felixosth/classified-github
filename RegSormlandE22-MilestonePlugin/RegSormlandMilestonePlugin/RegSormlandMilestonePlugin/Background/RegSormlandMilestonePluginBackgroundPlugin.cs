using RegSormlandMilestonePlugin.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace RegSormlandMilestonePlugin.Background
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
    public class RegSormlandMilestonePluginBackgroundPlugin : BackgroundPlugin
    {
        Dictionary<string, CameraEvent> camerasAndEvents;

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get { return RegSormlandMilestonePluginDefinition.RegSormlandMilestonePluginBackgroundPlugin; }
        }

        /// <summary>
        /// The name of this background plugin
        /// </summary>
        public override String Name
        {
            get { return "RegSormlandMilestonePlugin BackgroundPlugin"; }
        }

        /// <summary>
        /// Called by the Environment when the user has logged in.
        /// </summary>
        public override void Init()
        {
            Configuration.Instance.RefreshConfiguration(RegSormlandMilestonePluginDefinition.RegSormlandMilestonePluginKind);
            var configItem = Configuration.Instance.GetItem(RegSormlandMilestonePluginDefinition.ConfigItemGuid, RegSormlandMilestonePluginDefinition.RegSormlandMilestonePluginKind);


            if (configItem != null && configItem.Properties.ContainsKey(RegSormlandConfig._CFG_KEY))
            {
                var config = RegSormlandConfig.Deserialize(configItem.Properties[RegSormlandConfig._CFG_KEY]);
                camerasAndEvents = config?.CamerasEventsDictionary;
                if(camerasAndEvents != null && camerasAndEvents.Count > 0)
                    ClientControl.Instance.NewImageViewerControlEvent += NewImageViewerControlEvent;
            }
            else
            {
                EnvironmentManager.Instance.Log(true, Name, "Found no config");
            }

        }

        private void NewImageViewerControlEvent(ImageViewerAddOn imageViewerAddOn)
        {
            var fqidXml = imageViewerAddOn.CameraFQID.SerializeXml();
            if (camerasAndEvents.ContainsKey(fqidXml))
            {
                var camEvent = camerasAndEvents[fqidXml];
                if (camEvent.ButtonEventFQIDXml != null)
                {
                    var eventItem = Configuration.Instance.GetItem(new FQID(camEvent.ButtonEventFQIDXml));
                    if (eventItem != null)
                    {
                        var button = new Button()
                        {
                            Content = new Viewbox()
                            {
                                Child = new TextBlock()
                                {
                                    Text = !string.IsNullOrWhiteSpace(camEvent.ButtonEventDisplayText) ? camEvent.ButtonEventDisplayText : eventItem.Name,
                                    Effect = new DropShadowEffect()
                                    {
                                        ShadowDepth = 0,
                                        BlurRadius = 1,
                                        Color = Colors.Black
                                    }
                                }
                            },
                            Width = 400,
                            Height = 100,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(20, 20, 0, 0)
                        };

                        button.Click += (s, e) =>
                        {
                            button.IsEnabled = false;
                            FireEvent(camEvent.ButtonEventToFQID());
                            Task.Run(() =>
                            {
                                Thread.Sleep(1000);
                                ClientControl.Instance.CallOnUiThread(() => button.IsEnabled = true);

                            });
                        };
                        imageViewerAddOn.ActiveElementsOverlayAdd(new List<FrameworkElement> { button }, new ActiveElementsOverlayRenderParameters() { FollowDigitalZoom = false, ShowAlways = true, ZOrder = 1 });
                    }

                }

            }
        }

        /// <summary>
        /// Called by the Environment when the user log's out.
        /// You should close all remote sessions and flush cache information, as the
        /// user might logon to another server next time.
        /// </summary>
        public override void Close()
        {
            ClientControl.Instance.NewImageViewerControlEvent -= NewImageViewerControlEvent;
        }

        /// <summary>
        /// Define in what Environments the current background task should be started.
        /// </summary>
        public override List<EnvironmentType> TargetEnvironments
        {
            get { return new List<EnvironmentType>() { EnvironmentType.SmartClient }; } // Default will run in the Event Server
        }


        private void FireEvent(FQID evtFqid)
        {
            EnvironmentManager.Instance.PostMessage(new Message(MessageId.Control.TriggerCommand), evtFqid);



        }
    }
}
