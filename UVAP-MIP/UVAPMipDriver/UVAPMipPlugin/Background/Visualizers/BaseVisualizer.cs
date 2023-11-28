using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Client;
using VideoOS.Platform.Data;
using VideoOS.Platform.Live;
using VideoOS.Platform;
using System.Threading;
using System.Windows;
using System.Windows.Shapes;

namespace UVAPMipPlugin.Background.Visualizers
{
    // Base class for the overlay visualizers.
    // This class listens to the imageViewerAddOn for changes in the UI, e.g. when the user switch between playback and live mode.
    // Classes that derive from this class will be fed metadata to display and doesn't have to worry about live/playback mode.
    internal abstract class BaseVisualizer
    {
        public ImageViewerAddOn ImageViewer { get; private set; }
        private MetadataPlaybackSource MetadataPlayback { get; set; }
        private MetadataLiveSource MetadataLive { get; set; }
        private Guid OverlayGuid { get; set; } = Guid.Empty;
        private Item MetadataItem { get; set; }
        private Item CameraItem { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
        private List<DateTime> TimestampQueue { get; set; } = new List<DateTime>();
        private Mode ClientMode { get; set; } = Mode.StandAlone;
        public bool Closed { get; private set; } = false;

        private Size _res;
        protected Size Resolution
        {
            get
            {
                if(_res == default)
                {
                    _res = GetResolution(CameraItem);
                }
                return _res;
            }
        }

        public BaseVisualizer(ImageViewerAddOn imageViewerAddOn, Item metadataItem, Item cameraItem)
        {
            ImageViewer = imageViewerAddOn;
            MetadataItem = metadataItem;
            CameraItem = cameraItem;

            Init();
        }

        private void Init()
        {
            if (MetadataItem == null)
                return;

            // Subscribe to relevant events
            ImageViewer.StartLiveEvent += ImageViewerAddOn_StartStopLiveEvent;
            ImageViewer.StopLiveEvent += ImageViewerAddOn_StartStopLiveEvent;
            ImageViewer.RecordedImageReceivedEvent += ImageViewerAddOn_RecordedImageReceivedEvent;

            // Initialize the provider of live data
            MetadataLive = new MetadataLiveSource(MetadataItem);
            MetadataLive.LiveContentEvent += MetadataLive_LiveContentEvent;
            MetadataLive.Init();

            // Initialize the provider of recorded data
            MetadataPlayback = new MetadataPlaybackSource(MetadataItem);
            MetadataPlayback.Init();

            ToggleLivePlayback();
        }

        public abstract void OnMetadataRecieved(string data);

        // Draw the shapes provided using the api method in the ImageViewerAddOn
        // This must be called on the UI thread (API method ClientControl.Instance.CallOnUiThread)
        protected void DrawOverlay(List<Shape> shapes, int z = 100)
        {
            if(OverlayGuid == Guid.Empty)
            {
                OverlayGuid = ImageViewer.ShapesOverlayAdd(shapes, new ShapesOverlayRenderParameters() { ZOrder = z });
            }
            else
            {
                ImageViewer.ShapesOverlayUpdate(OverlayGuid, shapes, new ShapesOverlayRenderParameters() { ZOrder = z });
            }
        }

        // Check if the Smart Client is in live or playback mode and switch metadata providers
        private void ToggleLivePlayback()
        {
            if (ImageViewer.WindowInformation.Mode != ClientMode)
            {
                ClientMode = ImageViewer.WindowInformation.Mode;

                switch (ImageViewer.WindowInformation.Mode)
                {
                    case Mode.ClientLive:
                        CancellationTokenSource.Cancel();
                        MetadataLive.LiveModeStart = true;
                        break;
                    case Mode.ClientPlayback:
                        CancellationTokenSource = new CancellationTokenSource();
                        MetadataLive.LiveModeStart = false;
                        new Thread(() => PlaybackMetadata()).Start();
                        break;
                    default:
                        if (!CancellationTokenSource.IsCancellationRequested)
                            CancellationTokenSource.Cancel();
                        MetadataLive.LiveModeStart = false;
                        break;
                }
            }
        }

        // Thread running in the background used to fetch recorded metadata when the Smart Client displays a new frame of video data
        private void PlaybackMetadata()
        {
            while (!CancellationTokenSource.IsCancellationRequested && ImageViewer.WindowInformation.ViewAndLayoutItem != null)
            {
                var timestamp = TimestampQueue.FirstOrDefault(); // Check the timestamp queue for a new entry and fetch the metadata frame from the timestamp
                if (timestamp != default)
                {
                    TimestampQueue.Remove(timestamp);
                    var content = MetadataPlayback.GetAtOrBefore(timestamp);

                    if (content != null)
                    {
                        try
                        {
                            OnMetadata(content.Content.GetMetadataString());
                        }
                        catch
                        {
                            Close();
                        }
                    }
                }
            }
        }

        // Add the frame data timestamp in the queue
        private void ImageViewerAddOn_RecordedImageReceivedEvent(object sender, RecordedImageReceivedEventArgs e)
        {
            TimestampQueue.Add(e.DateTime);
        }

        // Call the abstract method OnMetadataRecieved on the UI thread so the classes that derive from this doesn't have to worry about UI/background threads.
        void OnMetadata(string metadata)
        {
            ClientControl.Instance.CallOnUiThread(() => OnMetadataRecieved(metadata));
        }

        // Fired by the MetadataLive provider
        private void MetadataLive_LiveContentEvent(MetadataLiveSource metadatLiveSource, MetadataLiveContent metadataLiveContent)
        {
            try
            {
                OnMetadata(metadataLiveContent.Content.GetMetadataString());
            }
            catch
            {
                Close();
            }
        }

        private void ImageViewerAddOn_StartStopLiveEvent(object sender, PassRequestEventArgs e)
        {
            ToggleLivePlayback();
        }

        public void Close()
        {
            CancellationTokenSource.Cancel();

            ImageViewer.StartLiveEvent -= ImageViewerAddOn_StartStopLiveEvent;
            ImageViewer.StopLiveEvent -= ImageViewerAddOn_StartStopLiveEvent;
            ImageViewer.RecordedImageReceivedEvent -= ImageViewerAddOn_RecordedImageReceivedEvent;

            TimestampQueue.Clear();

            MetadataLive.LiveContentEvent -= MetadataLive_LiveContentEvent;
            MetadataLive.Close();
            MetadataLive = null;

            MetadataPlayback.Close();
            MetadataPlayback = null;
            Closed = true;
        }

        // Get the resolution of the camera based on the latest recorded video frame
        private Size GetResolution(Item item)
        {
            JPEGVideoSource source = new JPEGVideoSource(item);
            source.Init();
            JPEGData frame = null;
            DateTime start = DateTime.Now;
            while(frame == null)
            {
                frame = source.GetNearest(DateTime.Now) as JPEGData;

                if (start.AddSeconds(5) < DateTime.Now)
                    return default;
            }
            source.Close();
            return new Size() { Width = frame.Width, Height = frame.Height };
        }
    }
}
