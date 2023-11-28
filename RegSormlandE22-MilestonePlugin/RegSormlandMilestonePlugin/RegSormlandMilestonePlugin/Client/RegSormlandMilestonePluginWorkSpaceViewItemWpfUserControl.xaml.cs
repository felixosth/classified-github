using RegSormlandMilestonePlugin.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace RegSormlandMilestonePlugin.Client
{
    /// <summary>
    /// Interaction logic for RegSormlandMilestonePluginWorkSpaceViewItemWpfUserControl.xaml
    /// </summary>
    public partial class RegSormlandMilestonePluginWorkSpaceViewItemWpfUserControl : ViewItemWpfUserControl
    {
        RegSormlandMilestonePluginWorkSpaceViewItemManager viewItemManager;

        const double screenSize = 0.9;

        Dictionary<string, MultiWindowCommandData> cachedMultiWindowCommandData = new Dictionary<string, MultiWindowCommandData>();

        int screenPosX, screenPosY, windowWidth, windowHeight;

        public RegSormlandMilestonePluginWorkSpaceViewItemWpfUserControl(RegSormlandMilestonePluginWorkSpaceViewItemManager viewItemManager)
        {
            this.viewItemManager = viewItemManager;
            InitializeComponent();
        }
        
        private void SetScreenProperties()
        {
            var window = Application.Current.MainWindow;
            var screen = window.GetScreen();

            screenPosX = screen.Bounds.X + (int)(screen.Bounds.Width * (1- screenSize)) / 2;
            screenPosY = screen.Bounds.Y + (int)(screen.Bounds.Height * (1 - screenSize)) / 2;
            windowWidth = (int)(screen.Bounds.Width * screenSize);
            windowHeight = (int)(screen.Bounds.Height * screenSize);
        }

        public override void Init()
        {
            Configuration.Instance.RefreshConfiguration(RegSormlandMilestonePluginDefinition.RegSormlandMilestonePluginKind);
            var configItem = Configuration.Instance.GetItem(RegSormlandMilestonePluginDefinition.ConfigItemGuid, RegSormlandMilestonePluginDefinition.RegSormlandMilestonePluginKind);

            Dictionary<string, CameraEvent> camerasAndEvents = null;

            if (configItem != null && configItem.Properties.ContainsKey(RegSormlandConfig._CFG_KEY))
            {
                var config = RegSormlandConfig.Deserialize(configItem.Properties[RegSormlandConfig._CFG_KEY]);
                camerasAndEvents = config.CamerasEventsDictionary;
            }
            else
            {
                MessageBox.Show("Found no configuration");
                return;
            }

            Helper.ResetDistinctColorIndex();

            foreach(var kvp in camerasAndEvents)
            {
                if (string.IsNullOrEmpty(kvp.Key) || string.IsNullOrEmpty(kvp.Value?.EventFQIDXml))
                    continue;

                var cam = Configuration.Instance.GetItem(new FQID(kvp.Key));

                if(cam != null)
                {
                    var evt = Configuration.Instance.GetItem(new FQID(kvp.Value.EventFQIDXml));
                    if (evt != null)
                    {

                        var background = Helper.GetDistinctColor();
                        var button = new Button()
                        {
                            Margin = new System.Windows.Thickness(25),
                            Width = 400,
                            Height = 120,
                            Background = new SolidColorBrush(background),
                            Tag = new CameraAndEvent()
                            {
                                Event = evt,
                                Camera = cam,
                                TimeToDisplay = kvp.Value.TimeToDisplay
                            }
                        };

                        button.Click += CameraClick;
                        
                        button.Content = new Viewbox() { Child = new TextBlock() { Text = cam.Name, Foreground = new SolidColorBrush(background.Invert()) } };

                        stackPanel.Children.Add(button);
                    }
                }
            }
        }

        private void CameraClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var camAndEvent = (sender as Button).Tag as CameraAndEvent;

            FireEvent(camAndEvent.Event.FQID);

            OpenFloatingWindow(camAndEvent);
        }

        private void FireEvent(FQID evtFqid)
        {
            EnvironmentManager.Instance.PostMessage(new Message(MessageId.Control.TriggerCommand), evtFqid);
        }

        int viewCounter = 0;
        private void OpenFloatingWindow(CameraAndEvent cameraAndEvent)
        {
            SetScreenProperties();
            Item screen = Configuration.Instance.GetItemsByKind(Kind.Screen).FirstOrDefault();
            Item window = Configuration.Instance.GetItemsByKind(Kind.Window).FirstOrDefault();

            MultiWindowCommandData data = new MultiWindowCommandData();
            if (cachedMultiWindowCommandData.ContainsKey(cameraAndEvent.Camera.FQID.ObjectId.ToString()))
            {

                data = cachedMultiWindowCommandData[cameraAndEvent.Camera.FQID.ObjectId.ToString()];


                data.Screen = screen.FQID;
                data.Window = window.FQID;
                data.X = screenPosX;
                data.Y = screenPosY;

                data.Height = windowHeight;
                data.Width = windowWidth;
            }
            else
            {

                var viewNumber = viewCounter++;

                ConfigItem tempGroupItem = (ConfigItem)ClientControl.Instance.CreateTemporaryGroupItem("Temporary" + viewNumber);
                tempGroupItem.Properties["Visible"] = "False";
                // Make a group 
                ConfigItem groupItem = tempGroupItem.AddChild("DynamicViewGroup" + viewNumber, Kind.View, FolderType.UserDefined);
                // Build a layout with wide ViewItems at the top and buttom, and 5 small ones in the middle
                Rectangle[] rect = new Rectangle[]
                {
                new Rectangle(000, 000, 1000, 1000)
                };
                //string viewName = "DynamicView" + viewNumber;
                string viewName = cameraAndEvent.Camera.Name;


                ViewAndLayoutItem viewAndLayoutItem = groupItem.AddChild(viewName, Kind.View, FolderType.No) as ViewAndLayoutItem;
                viewAndLayoutItem.Layout = rect;

                viewAndLayoutItem.InsertBuiltinViewItem(0,
                                            ViewAndLayoutItem.CameraBuiltinId,
                                            new Dictionary<string, string>()
                                            {
                                            { "CameraId", cameraAndEvent.Camera.FQID.ObjectId.ToString() }
                                            });


                viewAndLayoutItem.Save();
                tempGroupItem.PropertiesModified();


                Item view = groupItem.GetChildren().FirstOrDefault(v => v.Name.Equals(viewName));
                if (screen == null || window == null || view == null)
                {
                    return;
                }


                // Create floating window
                data.Screen = screen.FQID;
                data.Window = window.FQID;
                data.View = view.FQID;
                data.X = screenPosX;
                data.Y = screenPosY;

                data.Height = windowHeight;
                data.Width = windowWidth;
                data.MultiWindowCommand = "OpenFloatingWindow";
                data.PlaybackSupportedInFloatingWindow = true;

                cachedMultiWindowCommandData[cameraAndEvent.Camera.FQID.ObjectId.ToString()] = data;
            }


            var res = EnvironmentManager.Instance.SendMessage(new VideoOS.Platform.Messaging.Message(MessageId.SmartClient.MultiWindowCommand, data), null, null);

            ThreadPool.QueueUserWorkItem((state) =>
            {
                var floatingWindow = res.FirstOrDefault();

                if(floatingWindow != null)
                {
                    var closeAt = DateTime.Now.AddSeconds(cameraAndEvent.TimeToDisplay);

                    while(DateTime.Now < closeAt)
                    {
                        Thread.Sleep(100);
                    }
                    CloseWindow(floatingWindow as FQID);
                }
            });
        }

        private void CloseWindow(FQID window)
        {
            var data = new MultiWindowCommandData();
            data.Window = window;
            data.MultiWindowCommand = MultiWindowCommand.CloseSelectedWindow;

            ClientControl.Instance.CallOnUiThread(() =>
            {
                EnvironmentManager.Instance.SendMessage(new VideoOS.Platform.Messaging.Message(MessageId.SmartClient.MultiWindowCommand, data), null, null);
            });
        }

        public override void Close()
        {
        }

        /// <summary>
        /// Do not show the sliding toolbar!
        /// </summary>
        public override bool ShowToolbar
        {
            get { return false; }
        }

        private void ViewItemWpfUserControl_ClickEvent(object sender, EventArgs e)
        {
            FireClickEvent();
        }

        private void ViewItemWpfUserControl_DoubleClickEvent(object sender, EventArgs e)
        {
            FireDoubleClickEvent();
        }


    }

    internal class CameraAndEvent
    {
        internal Item Event { get; set; }
        internal Item Camera { get; set; }
        internal int TimeToDisplay { get; set; }
    }
}
