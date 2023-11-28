using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using UvapShared.Objects;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace UVAPMipPlugin.Background.Visualizers
{
    class SkeletonVisualizer : BaseVisualizer
    {
        public SkeletonVisualizer(ImageViewerAddOn imageViewerAddOn, Item metadataItem, Item cameraItem) : base(imageViewerAddOn, metadataItem, cameraItem)
        {
            
        }

        // Try to deserialize the string of data, if we succeed we visualize the data
        public override void OnMetadataRecieved(string data)
        {
            if (ImageViewer == null || ImageViewer.PaintSize.IsEmpty)
                return;

            var skeletonBodies = JsonConvert.DeserializeObject<SlimSkeleton[]>(data);

            float scaleX = ImageViewer.PaintSize.Width / (float)Resolution.Width;
            float scaleY = ImageViewer.PaintSize.Height / (float)Resolution.Height;
            List<Shape> shapes = new List<Shape>();

            foreach (var body in skeletonBodies)
            {
                ConnectPoints("NECK", "NOSE", body, shapes, scaleX, scaleY);
                ConnectPoints("NOSE", "RIGHT_EYE", body, shapes, scaleX, scaleY);
                ConnectPoints("NOSE", "LEFT_EYE", body, shapes, scaleX, scaleY);
                ConnectPoints("NECK", "RIGHT_SHOULDER", body, shapes, scaleX, scaleY);
                ConnectPoints("NECK", "LEFT_SHOULDER", body, shapes, scaleX, scaleY);
                ConnectPoints("NECK", "RIGHT_HIP", body, shapes, scaleX, scaleY, "LEFT_HIP");
                ConnectPoints("NECK", "LEFT_HIP", body, shapes, scaleX, scaleY, "RIGHT_HIP");
                ConnectPoints("RIGHT_SHOULDER", "RIGHT_ELBOW", body, shapes, scaleX, scaleY);
                ConnectPoints("RIGHT_ELBOW", "RIGHT_WRIST", body, shapes, scaleX, scaleY);
                ConnectPoints("LEFT_SHOULDER", "LEFT_ELBOW", body, shapes, scaleX, scaleY);
                ConnectPoints("LEFT_ELBOW", "LEFT_WRIST", body, shapes, scaleX, scaleY);
                ConnectPoints("RIGHT_HIP", "RIGHT_KNEE", body, shapes, scaleX, scaleY);
                ConnectPoints("RIGHT_KNEE", "RIGHT_ANKLE", body, shapes, scaleX, scaleY);
                ConnectPoints("LEFT_HIP", "LEFT_KNEE", body, shapes, scaleX, scaleY);
                ConnectPoints("LEFT_KNEE", "LEFT_ANKLE", body, shapes, scaleX, scaleY);
                ConnectPoints("RIGHT_EYE", "RIGHT_EAR", body, shapes, scaleX, scaleY);
                ConnectPoints("LEFT_EYE", "LEFT_EAR", body, shapes, scaleX, scaleY);

                foreach (var point in body.Points)
                    AddPoint(point, shapes, scaleX, scaleY, 4);
            }

            DrawOverlay(shapes);
        }

        // Create a line between to types of skeleton points and add it to the shapes list
        void ConnectPoints(string typeA, string typeB, SlimSkeleton body, List<Shape> shapes, float scaleX, float scaleY, string middleType = null)
        {
            var pointA = body.Points.SingleOrDefault(p => p.Type == typeA);
            var pointB = body.Points.SingleOrDefault(p => p.Type == typeB);

            if (pointA == null || pointB == null)
                return;

            SlimSkeleton.Point middlePoint = null;
            float middleX = 0, middleY = 0;

            if (middleType != null)
            {
                middlePoint = body.Points.SingleOrDefault(p => p.Type == middleType);
                if (middlePoint != null)
                {
                    middleX = (pointB.X + middlePoint.X) / 2f;
                    middleY = (pointB.Y + middlePoint.Y) / 2f;
                }
            }

            if (middlePoint != null)
            {
                shapes.Add(new Path()
                {
                    Data = new LineGeometry(new Point(pointA.X * scaleX, pointA.Y * scaleY),
                    new Point(middleX * scaleX, middleY * scaleY)),
                    Stroke = new SolidColorBrush(Skeleton.PointColors.GetColor(typeB + middleType)),
                    StrokeThickness = 2,
                });

                shapes.Add(new Path()
                {
                    Data = new LineGeometry(new Point(middleX * scaleX, middleY * scaleY),
                    new Point(pointB.X * scaleX, pointB.Y * scaleY)),
                    Stroke = new SolidColorBrush(Skeleton.PointColors.GetColor(typeA + typeB)),
                    StrokeThickness = 2
                });
            }
            else
            {
                shapes.Add(new Path()
                {
                    Data = new LineGeometry(new Point(pointA.X * scaleX, pointA.Y * scaleY),
                    new Point(pointB.X * scaleX, pointB.Y * scaleY)),
                    Stroke = new SolidColorBrush(Skeleton.PointColors.GetColor(typeA + typeB)),
                    StrokeThickness = 2
                });
            }
        }

        // Add a ellipse at the designated point to visualize the skeleton point position
        void AddPoint(SlimSkeleton.Point point, List<Shape> shapes, float scaleX, float scaleY, double radious)
        {
            Path path = new Path();
            path.Data = new EllipseGeometry(new System.Windows.Point(point.X * scaleX, point.Y * scaleY), radious, radious);
            path.Fill = new SolidColorBrush(Skeleton.PointColors.GetColor(point.Type)) { Opacity = 0.5 };
            shapes.Add(path);
        }
    }
}
