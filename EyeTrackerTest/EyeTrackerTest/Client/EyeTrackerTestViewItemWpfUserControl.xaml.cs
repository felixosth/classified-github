using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Tobii.Interaction;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace EyeTrackerTest.Client
{
    /// <summary>
    /// The ViewItemWpfUserControl is the WPF version of the ViewItemUserControl. It is instantiated for every position it is created on the current visible view. When a user select another View or ViewLayout, this class will be disposed.  No permanent settings can be saved in this class.
    /// The Init() method is called when the class is initiated and handle has been created for the UserControl. Please perform resource initialization in this method.
    /// <br>
    /// If Message communication is performed, register the MessageReceivers during the Init() method and UnRegister the receivers during the Close() method.
    /// <br>
    /// The Close() method can be used to Dispose resources in a controlled manor.
    /// <br>
    /// Mouse events not used by this control, should be passed on to the Smart Client by issuing the following methods:<br>
    /// FireClickEvent() for single click<br>
    ///	FireDoubleClickEvent() for double click<br>
    /// The single click will be interpreted by the Smart Client as a selection of the item, and the double click will be interpreted to expand the current viewitem to fill the entire View.
    /// </summary>
    public partial class EyeTrackerTestViewItemWpfUserControl : ViewItemWpfUserControl
    {
        private EyeTrackerTestViewItemManager _viewItemManager;
        ImageViewerWpfControl imageViewer;

        Host host;
        GazePointDataStream gazeStream;
        FixationDataStream fixationStream;

        Guid gotoPosGuid;
        Guid shapeGuid;

        const int defaultZoom = 60, maxZoom = 90;
        int zoom = defaultZoom;
        //int zoom = 0;

        //string gazeString;
        GazeLocation gaze;
        Point camLoc;
        bool closing = false;


        public EyeTrackerTestViewItemWpfUserControl(EyeTrackerTestViewItemManager viewItemManager)
        {
            _viewItemManager = viewItemManager;
            InitializeComponent();
            imageViewer = new ImageViewerWpfControl();
            imageViewer.MouseDown += ImageViewer_MouseDown;
            imageViewer.EnableVisibleCameraName = false;
            imageViewer.EnableVisibleHeader = false;
            imageViewer.EnableVisibleLiveIndicator = false;
            imageViewer.EnableVisibleTimestamp = false;
            imageViewer.MouseDoubleClick += ImageViewer_MouseDoubleClick;
            imageViewerGrid.Children.Add(imageViewer);
        }

        private void SetupTobii()
        {
            if (host != null)
                return;

            host = new Host();
            gazeStream = host.Streams.CreateGazePointDataStream();
            fixationStream = host.Streams.CreateFixationDataStream(Tobii.Interaction.Framework.FixationDataMode.Sensitive);

            //host.Streams.crea
            fixationStream.Next += FixationStream_Next;
            gazeStream.GazePoint(GazeHandler);

            camLoc = new Point(imageViewer.RenderSize.Width / 2, imageViewer.RenderSize.Height / 2);

            new Thread(() =>
            {
                while (!closing)
                {
                    GazeActions();
                    Thread.Sleep(1000 / 120);
                }
            }).Start();
        }

        DateTime startedLooking;
        int timer = 0;
        int every = 5;
        bool zoomIn = false;

        private void FixationStream_Next(object sender, StreamData<FixationData> e)
        {
            //int targetZoom = 0;
            int speed = 1;
            switch(e.Data.EventType)
            {
                case Tobii.Interaction.Framework.FixationDataEventType.Begin:
                    startedLooking = DateTime.Now;
                    break;
                case Tobii.Interaction.Framework.FixationDataEventType.Data:

                    if (DateTime.Now >= startedLooking.AddMilliseconds(1500))
                        zoomIn = true;

                    break;
                case Tobii.Interaction.Framework.FixationDataEventType.End:
                    zoomIn = false;
                    break;
            }

            speed = zoomIn ? 1 : -2;

            if (timer == every)
            {
                zoom += speed;
                timer = 0;
            }
            else
                timer++;

            if (zoom >= maxZoom)
                zoom = maxZoom;
            if (zoom <= defaultZoom)
                zoom = defaultZoom;
        }

        private void GazeActions()
        {
            if (gaze != null)
            {
                ClientControl.Instance.CallOnUiThread(() =>
                {
                    try
                    {
                        Color statusColor = Colors.Red;

                        var centerX = imageViewer.RenderSize.Width / 2;
                        var centerY = imageViewer.RenderSize.Height / 2;

                        var screenPos = imageViewer.PointToScreen(new Point(0d, 0d));
                        var rect = new Rect(screenPos, imageViewer.RenderSize);

                        //var distanceToCenter = Math.Sqrt(Math.Pow(centerX - camLoc.X, 2) + Math.Pow(centerY - camLoc.Y, 2));
                        var innerPos = new Point(gaze.X - screenPos.X, gaze.Y - screenPos.Y);


                        var distX = (centerX - innerPos.X);
                        var distY = (centerY - innerPos.Y);

                        //var gotoPos = new Point(innerPos.X, innerPos.Y);
                        var gotoPos = new Point(innerPos.X - (distX / 2), innerPos.Y - (distY / 2));
                        //var gotoPos = new Point(innerPos.X * 0.79f, innerPos.Y * 0.79f);


                        if (rect.IsWithin(gaze.Location))
                        {
                            statusColor = Colors.Green;

                            //var distanceToGaze = Math.Sqrt(Math.Pow(innerPos.X - camLoc.X, 2) + Math.Pow(innerPos.Y - camLoc.Y, 2));
                            var distanceToGaze = Math.Sqrt(Math.Pow(gotoPos.X - camLoc.X, 2) + Math.Pow(gotoPos.Y - camLoc.Y, 2));

                            if (distanceToGaze > 0)
                            {
                                //var direction = (innerPos - camLoc);e
                                var direction = (gotoPos - camLoc);
                                direction.Normalize();

                                var smoothing = 200;

                                camLoc = new Point(camLoc.X + direction.X * (distanceToGaze / smoothing),
                                    camLoc.Y + direction.Y * (distanceToGaze / smoothing));

                                imageViewer.PtzCenter((int)imageViewer.RenderSize.Width, (int)imageViewer.RenderSize.Height, (int)camLoc.X, (int)camLoc.Y, zoom);
                            }
                        }

                        var gazeString = string.Format("{0:0.0}, {1:0.0} / {2:0.0}, {3:0.0}", innerPos.X, innerPos.Y, rect.Size.Width, rect.Size.Height);
                        var shapes = new List<Shape>();
                        shapes.Add(ShapeHelper.CreateTextShape(gazeString, innerPos.X - 140, innerPos.Y - 40, 24, Colors.Red));
                        shapes.Add(ShapeHelper.CreateTextShape("O", innerPos.X - 6, innerPos.Y - 6, 12, Colors.LimeGreen));

                        var shape2 = new List<Shape>();
                        shape2.Add(ShapeHelper.CreateTextShape("X", gotoPos.X - 5, gotoPos.Y - 5, 12, Colors.Orange));

                        if (shapeGuid == Guid.Empty)
                        {
                            shapeGuid = imageViewer.ShapesOverlayAdd(shapes, new ShapesOverlayRenderParameters() { FollowDigitalZoom = false });
                            gotoPosGuid = imageViewer.ShapesOverlayAdd(shape2, new ShapesOverlayRenderParameters() { FollowDigitalZoom = true });
                        }
                        else
                        {
                            imageViewer.ShapesOverlayUpdate(gotoPosGuid, shape2);
                            imageViewer.ShapesOverlayUpdate(shapeGuid, shapes);
                        }
                    }
                    catch { }
                });
            }
        }

        private void GazeHandler(double x, double y, double ts)
        {
            if (gaze == null)
                gaze = new GazeLocation(x, y);
            else
            {
                gaze.Location = new Point(x, y);
            }
        }

        private void ImageViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FireDoubleClickEvent();
            //imageViewer.PtzCenter(1000, 1000, 500, 500, 100);
        }

        private void ImageViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FireClickEvent();
        }

        public void LoadCamera(FQID camera)
        {
            if (imageViewer.CameraFQID != null)
                imageViewer.Disconnect();

            imageViewer.EnableDigitalZoom = !_viewItemManager.IsPTZ;
            
            imageViewer.CameraFQID = camera;
            imageViewer.Initialize();
            imageViewer.Connect();

            SetupTobii();
        }

        public override void Init()
        {
            if (_viewItemManager.CameraFQID != null)
                LoadCamera(_viewItemManager.CameraFQID);
            
        }

        public override void Close()
        {
            closing = true;
            imageViewer.Disconnect();
        }


        public override void Print()
        {
            Print("Tobii Eye Tracker Test", "By Felix Östh");
        }

        public override bool Maximizable
        {
            get { return true; }
        }

        public override bool Selectable
        {
            get { return true; }
        }

        public override bool ShowToolbar => false;


    }

    class GazeLocation
    {
        public Point Location { get; set; }

        public GazeLocation(double x, double y)
        {
            Location = new Point(x, y);
        }

        public GazeLocation(Point loc)
        {
            Location = loc;
        }

        public double X
        {
            get { return Location.X; }
        }

        public double Y
        {
            get { return Location.Y; }
        }
    }

    class Rect
    {
        public Point Postition { get; set; }
        public System.Windows.Size Size { get; set; }

        public Rect(Point pos, System.Windows.Size size)
        {
            this.Postition = pos;
            this.Size = size;
        }


        public bool IsWithin(Point otherPos)
        {
            return otherPos.X >= Postition.X && otherPos.X <= Postition.X + Size.Width && otherPos.Y >= Postition.Y && otherPos.Y <= Postition.Y + Size.Height;
        }
    }

    public static class Extensions
    {
        public static Point Normalize(this Point A)
        {
            double distance = Math.Sqrt(A.X * A.X + A.Y * A.Y);
            return new Point(A.X / distance, A.Y / distance);
        }
    }
}
