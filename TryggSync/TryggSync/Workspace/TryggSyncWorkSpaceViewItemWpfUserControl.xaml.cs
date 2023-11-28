using System;
using System.Collections.Generic;
using System.Windows.Media;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Login;
using VideoOS.Platform.Messaging;
using VideoOS.Platform.UI;
using TryggSync.Server;
using TryggSync.Client;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Ink;
using System.IO;

namespace TryggSync.Workspace
{
    /// <summary>
    /// Interaction logic for TryggSyncWorkSpaceViewItemWpfUserControl.xaml
    /// </summary>
    public partial class TryggSyncWorkSpaceViewItemWpfUserControl : ViewItemWpfUserControl
    {
        //ImageViewerWpfControl imageViewer;
        PlaybackController playbackController;
        PlaybackWpfUserControl pbUserControl;
        FQID playbackFqid;

        PlaybackControllerMessages playbackMessages; 

        MessageCommunication messageCommunication;

        //CustomInkCanvas customInkCanvas;
        Item currentCam;

        MessagingManager msgManager;
        OperatorUser myUser;

        public TryggSyncWorkSpaceViewItemWpfUserControl()
        {
            InitializeComponent();

            var ls = LoginSettingsCache.GetLoginSettings(EnvironmentManager.Instance.MasterSite);
            myUser = new OperatorUser(ls.UserName, ls.Guid);

            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            msgManager = new MessagingManager(myUser, messageCommunication);
            msgManager.OnOwnershipChanged += MsgMng_OnOwnershipChanged;
            msgManager.OnViewersChanged += MsgMng_OnViewersChanged;
            msgManager.OnCameraChanged += MsgManager_OnCameraChanged;
            msgManager.OnNewPaintStroke += MsgManager_OnNewPaintStroke;

            msgManager.OnNewUserConnected += MsgManager_OnUserRequestCameraInfo;

            new MessageTester().Show();


            //var s = System.Windows.Input.Mouse.GetPosition(this);

        }

        CameraPlaybackInfo GeneratePlaybackInfo()
        {
            if(currentCam != null)
                return new CameraPlaybackInfo(currentCam.Serialize(), playbackController.PlaybackTime, 1, playbackController.PlaybackMode);
            else
                return null;
        }
        //=> new CameraPlaybackInfo(currentCam.Serialize(), playbackController.PlaybackTime, 1, playbackController.PlaybackMode);

        private void MsgManager_OnUserRequestCameraInfo(object sender, MessageEventArgs e)
        {
            ClientControl.Instance.CallOnUiThread(() =>
            {
                msgManager.SendPlaybackInfo(GeneratePlaybackInfo(), specialEndPoint: e.MessageData as FQID);
                msgManager.SendPaintStrokes(customInkCanvas.GetStrokesStream());
            });
        }

        private void MsgManager_OnNewPaintStroke(object sender, MessageEventArgs e)
        {
            if (IsOperator) return;

            var strokeStream = e.MessageData as MemoryStream;

            customInkCanvas.UpdateStrokes(strokeStream);
        }

        private void MsgManager_OnCameraChanged(object sender, MessageEventArgs e)
        {
            if (IsOperator)
            {
                //ConnectToCamera(cam);
                return;
            }

            var info = e.MessageData as CameraPlaybackInfo;
            if (info == null)
                return;

            var cam = Item.Deserialize(info.SerializedCamera);


            ClientControl.Instance.CallOnUiThread((() =>
            {
                ConnectToCamera(cam);
                playbackController.PlaybackTime = info.PlaybackTime;
                playbackController.PlaybackMode = info.PlaybackMode;
                playbackController.PlaybackSpeed = info.PlaybackSpeed;
            }));
        }

        private bool _isOp = false;
        bool IsOperator
        {
            get { return _isOp; }
            set
            {
                controlButton.Content = value ? "Leave Control" : "Take Control";

                pickCamBtn.IsEnabled = value;
                controlButton.IsEnabled = value;
                pbUserControl.IsEnabled = value;
                //customPlaybackUserControl.IsEnabled = value;
                clearDrawingBtn.IsEnabled = value;
                customInkCanvas.IsEnabled = value;
                //clearStrokesBtn.IsEnabled = value;

                _isOp = value;
            }
        }

        public override void Init()
        {
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            
            //imageViewer = new ImageViewerWpfControl();
            imageViewer.EnableVisibleCameraName = true;
            imageViewer.EnableVisibleHeader = true;
            imageViewer.EnableVisibleLiveIndicator = true;
            imageViewer.EnableVisibleTimestamp = true;
            imageViewer.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            imageViewer.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            //imageViewerContainer.Children.Add(imageViewer);

            //customInkCanvas = new CustomInkCanvas();
            customInkCanvas.OnStrokeCollected += CustomInkCanvas_OnStrokeCollected;
            //imageViewerContainer.Children.Add(customInkCanvas);

            //imageViewerDrawer = new ImageViewerDrawer(imageViewer, Brushes.Red);

            playbackMessages = new PlaybackControllerMessages();
            playbackMessages.OnPlaybackCurrentTimeChanged += PlaybackMessages_OnPlaybackCurrentTimeChanged;
            playbackMessages.OnPlaybackIndicationChanged += PlaybackMessages_OnPlaybackIndicationChanged;

            pbUserControl = new PlaybackWpfUserControl();
            //pbUserControl.SetEnabled(false);
            pbUserControl.IsEnabled = false;
            pbUserControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            pbUserControl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            imageControllerContianer.Children.Add(pbUserControl);

            playbackFqid = ClientControl.Instance.GeneratePlaybackController();
            //playbackController.PlaybackMode
            pbUserControl.Init(playbackFqid);

            imageViewer.PlaybackControllerFQID = playbackFqid;

            //var browseTimeEvent = new PlaybackController.BrowseTimeChangedEventHandler(OnTimeChanged);
            //PlaybackController.BrowseTimeChangedEventHandler brrr = OnTimeChanged;
            //PlaybackController.BrowseTimeChangedEventHandler = OnTimeChanged;
            playbackController = ClientControl.Instance.GetPlaybackController(playbackFqid);
            //var baseController = playbackController as PlaybackControllerBase;
            //(playbackController as PlaybackControllerBase).BrowseEndEvent += BaseController_BrowseEndEvent;
            //playbackController = new CustomPlaybackController();
            playbackController.FQID = playbackFqid;

            playbackController.Init();

            //customPlaybackUserControl.OnPlaybackChanged += CustomPlaybackUserControl_OnPlaybackChanged;
            //customPlaybackUserControl.OnLiveButtonClicked += CustomPlaybackUserControl_OnLiveButtonClicked;

            //new MessageTester().Show();

            //new TryggSyncMsgCommandRequest(MsgCommandTypes.OwnershipStatus).SendToEventServer(messageCommunication);
            //new TryggSyncMsgCommandRequest(MsgCommandTypes.UpdateViewList, Packer.SerializeObject(myUser), true).SendToEventServer(messageCommunication);
        }

        private void PlaybackMessages_OnPlaybackIndicationChanged(object sender, PlaybackController.PlaybackModeType e)
        {
            if (currentCam == null)
                return;

            var info = new CameraPlaybackInfo(currentCam.Serialize(), playbackController.PlaybackTime, 1, e);
            msgManager.SendPlaybackInfo(info);
        }

        private void PlaybackMessages_OnPlaybackCurrentTimeChanged(object sender, DateTime e)
        {
            if (currentCam == null || playbackController.PlaybackMode == PlaybackController.PlaybackModeType.Forward)
                return;

            var info = new CameraPlaybackInfo(currentCam.Serialize(), e, 1, PlaybackController.PlaybackModeType.Stop);
            msgManager.SendPlaybackInfo(info);

        }

        private void CustomInkCanvas_OnStrokeCollected(object sender, System.Windows.Controls.InkCanvasStrokeCollectedEventArgs e)
        {
            msgManager.SendPaintStrokes(customInkCanvas.GetStrokesStream());
        }


        //private void CustomPlaybackUserControl_OnLiveButtonClicked(object sender, PlaybackChangedEventArgs e)
        //{
        //    if (currentCam == null)
        //        return;
        //    var info = new CameraPlaybackInfo(currentCam.Serialize(), playbackController.PlaybackTime, 
        //        1, e.PlaybackMode);
        //    msgManager.SendPlaybackInfo(info);
        //}

        //private void CustomPlaybackUserControl_OnPlaybackChanged(object sender, PlaybackChangedEventArgs e)
        //{
        //    if (currentCam == null)
        //        return;
        //    var info = new CameraPlaybackInfo(currentCam.Serialize(), playbackController.PlaybackTime, 1, e.PlaybackMode);
        //    msgManager.SendPlaybackInfo(info);
        //}

        private void MsgMng_OnViewersChanged(object sender, MessageEventArgs e)
        {
            ClientControl.Instance.CallOnUiThread(() =>
            {
                var viewers = e.MessageData as List<OperatorUser>;

                viewersListBox.Items.Clear();
                foreach (var viewer in viewers)
                {
                    var viewerOperator = viewer as OperatorUser;

                    viewersListBox.Items.Add(viewerOperator);
                }
            });
        }

        private void MsgMng_OnOwnershipChanged(object sender, MessageEventArgs e)
        {
            ClientControl.Instance.CallOnUiThread(() =>
            {
                var op = e.MessageData as OperatorUser;
                SetOperator(op);
                IsOperator = op != null ? op.Equals(myUser) : false;

                if (op == null)  // when operator left
                {
                    controlButton.IsEnabled = true;
                    imageViewer.Disconnect();
                    imageViewer.CameraFQID = new FQID();
                    customInkCanvas.ClearStrokes();
                    currentCam = null;
                }
                else if(!op.Equals(myUser))
                {
                    msgManager.JustConnectedInfo();
                }
            });
        }

        public void SetOperator(OperatorUser _operator)
        {
            operatorNameLabel.Content = _operator != null ? _operator.DisplayName : "No operator";
        }

        public override void Close()
        {
            msgManager.SendToEventServer(MsgCommandTypes.UpdateViewList, myUser, false);
            //new TryggSyncMsgCommandRequest(MsgCommandTypes.UpdateViewList, Packer.SerializeObject(myUser), false).SendToEventServer(messageCommunication);

            msgManager.Close();

            messageCommunication.Dispose();
            MessageCommunicationManager.Stop(EnvironmentManager.Instance.MasterSite.ServerId);

            if (imageViewer.CameraFQID != null)
                imageViewer.Disconnect();

            imageViewer.Dispose();
        }

        void ConnectToCamera(Item camera)
        {
            if(imageViewer.CameraFQID != null)
                if (imageViewer.CameraFQID.ObjectId == camera.FQID.ObjectId)
                    return;

            if (imageViewer.CameraFQID != null)
            {
                imageViewer.Disconnect();
            }



            currentCam = camera;
            pbUserControl.SetCameras(new List<FQID>() { camera.FQID});
            pbUserControl.TimeSpan = new TimeSpan(0, 15, 0);
            imageViewer.CameraFQID = camera.FQID;
            imageViewer.Initialize();
            imageViewer.Connect();

        }

        private void PickCameraButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var itemPicker = new ItemPickerForm();
            itemPicker.KindFilter = Kind.Camera;
            itemPicker.ShowDisabledItems = false;
            itemPicker.Init();

            if(itemPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentCam = itemPicker.SelectedItem;
                msgManager.SendPlaybackInfo(new CameraPlaybackInfo(currentCam.Serialize(), DateTime.Now, 1, PlaybackController.PlaybackModeType.Forward));

                if (IsOperator)
                {
                    ConnectToCamera(currentCam);
                    playbackController.PlaybackTime = DateTime.Now;
                    playbackController.PlaybackMode = PlaybackController.PlaybackModeType.Forward;
                    playbackController.PlaybackSpeed = 1;
                }
                //msgManager.SendToEventServer(MsgCommandTypes.CameraPlayback, myUser, itemPicker.SelectedItem.Serialize());
            }
        }

        public override bool ShowToolbar => false;

        private void controlButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsOperator)
                msgManager.SendToEventServer(MsgCommandTypes.LeaveOwnership, myUser);
                //new TryggSyncMsgCommandRequest(MsgCommandTypes.LeaveOwnership, Packer.SerializeObject(myUser)).SendToEventServer(messageCommunication);
            else
                msgManager.SendToEventServer(MsgCommandTypes.TakeOwnership, myUser);
                //new TryggSyncMsgCommandRequest(MsgCommandTypes.TakeOwnership, Packer.SerializeObject(myUser)).SendToEventServer(messageCommunication);
        }

        private void ViewItemWpfUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            msgManager.SendToEventServer(MsgCommandTypes.UpdateViewList, myUser, true);
            msgManager.SendToEventServer(MsgCommandTypes.OwnershipStatus);
        }

        private void clearDrawingBtn_Click(object sender, RoutedEventArgs e)
        {
            customInkCanvas.ClearStrokes();
            msgManager.SendPaintStrokes(customInkCanvas.GetStrokesStream());
        }
    }
}
