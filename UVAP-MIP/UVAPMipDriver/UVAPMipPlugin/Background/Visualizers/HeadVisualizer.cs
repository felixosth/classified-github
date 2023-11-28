using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
    class HeadVisualizer : BaseVisualizer
    {
        public HeadVisualizer(ImageViewerAddOn imageViewerAddOn, Item metadataItem, Item cameraItem) : base(imageViewerAddOn, metadataItem, cameraItem)
        {
        }

        // Try to deserialize the string of data, if we succeed we visualize the data
        public override void OnMetadataRecieved(string data)
        {
            var heads = JsonConvert.DeserializeObject<SlimHeadDetection[]>(data);
            var shapes = new List<Shape>();

            if (heads.Length > 0)
            {

                float scaleX = ImageViewer.PaintSize.Width / (float)Resolution.Width;
                float scaleY = ImageViewer.PaintSize.Height / (float)Resolution.Height;

                foreach (var head in heads)
                {
                    Path path = new Path();
                    path.Data = new RectangleGeometry(head.BoundingBox.ToRect(scaleX, scaleY));
                    path.Stroke = Brushes.HotPink;
                    path.StrokeThickness = 2;
                    shapes.Add(path);
                }
            }

            if (shapes.Count > 0)
                DrawOverlay(shapes, z: 101);
        }
    }
}
