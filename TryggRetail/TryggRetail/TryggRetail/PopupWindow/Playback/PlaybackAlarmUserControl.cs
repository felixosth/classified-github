using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform.Client;
using TryggRetail.Background;
//using VideoOS.Platform.Metadata;
using System.Windows.Media;
using System.Threading;
using VideoOS.Platform;
//using TryggRetail.Background;
using VideoOS.Platform.Data;
using System.Windows;
using System.Globalization;
using System.Windows.Shapes;
using TryggRetail.Admin;
using TryggRetail.PopupWindow;

namespace TryggRetail.Playback
{
    public partial class PlaybackAlarmUserControl : ViewItemUserControl
    {

        private PlaybackUserControl playbackUserControl;
        private ImageViewerControl imageViewerControl;
        PlaybackController pc;
        private FQID playbackFQID;
        Alarm alarm;

        PopupConfig popupConfig;

        bool running = true;

        public bool LoopPlayback;

        TryggRetailBackgroundPlugin instance;

        public PlaybackAlarmUserControl(TryggRetailBackgroundPlugin _instance, bool liveMode)
        {
            InitializeComponent();
            this.instance = _instance;
            popupConfig = instance.CurrentPopupConfig;

            instance.CurrentPlaybacker = this;

            alarm = instance.CurrentAlarm;

            if (alarm.ReferenceList.Count < 1)
                return;

            FQID camera = alarm.ReferenceList[0].FQID;

            playbackUserControl = ClientControl.Instance.GeneratePlaybackUserControl(this.WindowInformation);
            playbackUserControl.Dock = DockStyle.Fill;
            panelPlaybackControl.Controls.Add(playbackUserControl);
            //_audioPlayerControl = ClientControl.Instance.GenerateAudioPlayerControl(WindowInformation);
            //panelVideo.Controls.Add(_audioPlayerControl);

            imageViewerControl = ClientControl.Instance.GenerateImageViewerControl(this.WindowInformation);
            imageViewerControl.Dock = DockStyle.Fill;
            //_imageViewerControl.AllowDrop = true;
            imageViewerControl.Selected = true;

            panelVideo.Controls.Add(imageViewerControl);

            playbackFQID = ClientControl.Instance.GeneratePlaybackController();
            playbackUserControl.Init(playbackFQID);
            imageViewerControl.PlaybackControllerFQID = playbackFQID;

            pc = ClientControl.Instance.GetPlaybackController(playbackFQID);
            pc.PlaybackTime = alarm.EventHeader.Timestamp.AddSeconds(-popupConfig.AlarmPlaybackOffsetBefore);
            pc.SkipGaps = false;
            pc.PlaybackMode = PlaybackController.PlaybackModeType.Forward;
            pc.PlaybackSpeed = 1;

            imageViewerControl.CameraFQID = camera;
            //imageViewerControl.EnableVisibleTimeStamp = true;
            imageViewerControl.Initialize();
            imageViewerControl.Connect();

            var shapes = new List<Shape>();
            shapes.Add(ShapeHelper.CreateTextShape("Playback", 12, 12, 30, System.Windows.Media.Colors.Black));
            shapes.Add(ShapeHelper.CreateTextShape("Playback", 10, 10, 30, System.Windows.Media.Colors.Red));
            imageViewerControl.ShapesOverlayAdd(shapes, new ShapesOverlayRenderParameters());


            playbackUserControl.SetCameras(new List<FQID>() { camera });

            backgroundWorker1.RunWorkerAsync();

            //ClientControl.Instance.RegisterUIControlForAutoTheming(this);

            LoopPlayback = popupConfig.LoopByDefault;   
        }

        public override void Close()
        {
            playbackUserControl.Close();
            imageViewerControl.Disconnect();
            imageViewerControl.Close();
            imageViewerControl.Dispose();

            ClientControl.Instance.ReleasePlaybackController(playbackFQID);
            running = false;
            base.Close();
        }

        public void RewindAlarm()
        {
            pc.PlaybackMode = PlaybackController.PlaybackModeType.Forward;
            pc.PlaybackSpeed = 1;
            pc.PlaybackTime = alarm.EventHeader.Timestamp.AddSeconds(-popupConfig.AlarmPlaybackOffsetBefore);
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (running)
            {
                if (pc.PlaybackTime > alarm.EventHeader.Timestamp.AddSeconds(popupConfig.AlarmPlaybackOffsetAfter))
                {
                    ClientControl.Instance.CallOnUiThread(() =>
                    {
                        if (LoopPlayback)
                        {
                            pc.PlaybackTime = alarm.EventHeader.Timestamp.AddSeconds(-popupConfig.AlarmPlaybackOffsetBefore);
                            pc.PlaybackMode = PlaybackController.PlaybackModeType.Forward;
                            pc.PlaybackSpeed = 1;
                        }
                        else
                        {
                            pc.PlaybackMode = PlaybackController.PlaybackModeType.Stop;
                            pc.PlaybackSpeed = 0;
                        }

                    });
                }
                Thread.Sleep(500);
            }
        }

        //private static Shape CreateTextShape(System.Drawing.Size size, string text, double scaleX, double scaleY, double scaleFontSize, System.Windows.Media.Color color)
        //{
        //    //Debug.WriteLine(text + " paint size (" + size.Height + "," + size.Width + ")");
        //    double x = (size.Width * scaleX) / 1000;
        //    double y = (size.Height * scaleY) / 1000;
        //    double fontsize = (size.Height * scaleFontSize) / 1000;
        //    if (fontsize < 7) fontsize = 12;

        //    return CreateTextShape(text, x, y, fontsize, color);
        //}

        //private static Shape CreateTextShape(string text, double placeX, double placeY, double fontSize, System.Windows.Media.Color color)
        //{
        //    Shape textShape;
        //    System.Windows.Media.FontFamily fontFamily = new System.Windows.Media.FontFamily("Arial");
        //    Typeface typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Bold, new FontStretch());

        //    FormattedText fText = new FormattedText(text, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, typeface, fontSize, System.Windows.Media.Brushes.Black);

        //    System.Windows.Point textPosition1;
        //    textPosition1 = new System.Windows.Point(placeX, placeY);
        //    System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
        //    path.Data = fText.BuildGeometry(textPosition1);
        //    path.Fill = new SolidColorBrush(color);
        //    textShape = path;
        //    return textShape;
        //}

        private void panelVideo_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelVideo_Paint_1(object sender, PaintEventArgs e)
        {

        }

        public override bool ShowToolbar => false;
    }
}

