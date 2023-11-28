using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;
using VideoOS.Platform.Util;

namespace XProtectWebStreamPlugin.Client
{
    public class XProtectWebStreamPluginViewItemToolbarPluginInstance : ViewItemToolbarPluginInstance
    {
        private Item _viewItemInstance;
        private Item _window;
        private string cameraId;

        private ClientCommunication clientCommunication;

        public XProtectWebStreamPluginViewItemToolbarPluginInstance(ClientCommunication clientCommunication)
        {
            this.clientCommunication = clientCommunication;
        }

        public override void Init(Item viewItemInstance, Item window)
        {
            _viewItemInstance = viewItemInstance;
            _window = window;

            Title = "XProtectWebStreamPlugin";
            Tooltip = "Request webstream link";
            this.Icon = Properties.Resources.share;


        }

        public override void Activate()
        {
            cameraId = _viewItemInstance.Properties["CurrentCameraId"];

            // Here you should put whatever action that should be executed when the toolbar button is pressed
            if (clientCommunication.LastFeatureResponse.LicenseIsValid == false)
            {
                MessageBox.Show("This feature is not licensed. Try again or contact your reseller.", "No license");
                clientCommunication.RequestFeaturesFromServer();
                return;
            }

            var window = new Window();

            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ResizeMode = ResizeMode.NoResize;
            window.Title = "Request webstream link";
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Width = 1280;
            window.Height = 720;

            var cam = Configuration.Instance.GetItem(Guid.Parse(cameraId), Kind.Camera);

            if(cam == null)
            {
                MessageBox.Show("Failed to find camera in config");
                return;
            }

            var generateLinkDialog = new UI.GenerateLinkDialog(clientCommunication, cam, ClientControl.Instance.ShownWorkSpace.FQID.ObjectId == ClientControl.PlaybackBuildInWorkSpaceId); ;

            var interval = EnvironmentManager.Instance.SendMessage(new Message(MessageId.SmartClient.GetTimelineSelectedIntervalRequest)).FirstOrDefault();
            if(interval is TimeInterval)
            {
                var timeInterval = (TimeInterval)interval;

                if(timeInterval != TimeInterval.Zero)
                {
                    generateLinkDialog.fromDatePicker.SelectedDate = timeInterval.StartTime.ToLocalTime();
                    generateLinkDialog.toDatePicker.SelectedDate = timeInterval.EndTime.ToLocalTime();

                    generateLinkDialog.fromTimeTxtBox.Text = timeInterval.StartTime.ToLocalTime().TimeOfDay.ToString(@"hh\:mm\:ss");
                    generateLinkDialog.toTimeTxtBox.Text = timeInterval.EndTime.ToLocalTime().TimeOfDay.ToString(@"hh\:mm\:ss");
                }
            }

            window.Content = generateLinkDialog;

            window.ShowDialog();
        }

        public override void Close()
        {
        }
    }

    public class XProtectWebStreamPluginViewItemToolbarPlugin : ViewItemToolbarPlugin
    {
        ClientCommunication clientCommunication;

        public override Guid Id
        {
            get { return XProtectWebStreamPluginDefinition.XProtectWebStreamPluginViewItemToolbarPluginId; }
        }

        public override string Name
        {
            get { return "XProtectWebStreamPlugin"; }
        }

        public override ToolbarPluginOverflowMode ToolbarPluginOverflowMode
        {
            get { return ToolbarPluginOverflowMode.AsNeeded; }
        }


        public override void Init()
        {
            ViewItemToolbarPlaceDefinition.ViewItemIds = new List<Guid>() { ViewAndLayoutItem.CameraBuiltinId };
            //ViewItemToolbarPlaceDefinition.WorkSpaceIds = new List<Guid>() { ClientControl.LiveBuildInWorkSpaceId };
            ViewItemToolbarPlaceDefinition.WorkSpaceIds = new List<Guid>() { ClientControl.LiveBuildInWorkSpaceId, ClientControl.PlaybackBuildInWorkSpaceId };
            ViewItemToolbarPlaceDefinition.WorkSpaceStates = new List<WorkSpaceState>() { WorkSpaceState.Normal };

            clientCommunication = new ClientCommunication();
            clientCommunication.RequestFeaturesFromServer();
            clientCommunication.RequestAccessGroups();
        }

        public override void Close()
        {
            clientCommunication.Close();
        }

        public override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance()
        {
            return new XProtectWebStreamPluginViewItemToolbarPluginInstance(clientCommunication);
        }
    }
}
