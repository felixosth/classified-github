using Newtonsoft.Json;
using REIDShared.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace REIDOverlay.Background.Visualizers
{
    class HeadVisualizer : BaseVisualizer
    {
        public HeadVisualizer(ImageViewerAddOn imageViewerAddOn, Item metadataItem, Item cameraItem) : base(imageViewerAddOn, metadataItem, cameraItem)
        {
            //imageViewerAddOn.mo

        }

        // Try to deserialize the string of data, if we succeed we visualize the data
        public override void OnMetadataRecieved(string data)
        {
            var container = JsonConvert.DeserializeObject<MetadataContainer>(data);
            var shapes = new List<Shape>();

            if (container.Detections.Length > 0)
            {
                float scaleX = ImageViewer.PaintSize.Width / (float)Resolution.Width;
                float scaleY = ImageViewer.PaintSize.Height / (float)Resolution.Height;

                foreach (var detection in container.Detections)
                {
                    var rect = detection.Detection.bounding_box.ToRect(scaleX, scaleY);

                    Path path = new Path();
                    path.Data = new RectangleGeometry(rect);

                    SolidColorBrush brush = Brushes.Gray;

                    path.StrokeThickness = 2;

                    if(REIDOverlayDefinition.NodeRED == null)
                        shapes.Add(CreateTextShape("NodeRED offline", 20, 20, Brushes.Red, 300, out _, above: false));

                    if (detection.Reid != null && detection.Reid.reid_event != null)
                    {
                        string text = detection.Reid.reid_event.first_key;

                        if (REIDOverlayDefinition.NodeRED != null)
                        {
                            var person = REIDOverlayDefinition.NodeRED.Persons.FirstOrDefault(p => p.key == detection.Reid.reid_event.first_key);
                            if (person != null)
                            {
                                text = person.personName;
                                brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(person.categoryColor));
                            }
                            else
                                brush = Brushes.Orange;
                        }
                        shapes.AddRange(CreateRectWithText(new Rect(rect.X, rect.Y - 4, rect.Width, rect.Height), brush, Brushes.Black, new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)), text, true));
                    }

                    if(detection.Age != null && detection.Gender != null)
                    {
                        string text = $"{detection.Age.age}, {detection.Gender.gender}";

                        shapes.AddRange(CreateRectWithText(new Rect(rect.X, rect.Y + rect.Height + 4, rect.Width, rect.Height), brush, Brushes.Black, new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)), text, false));
                    }

                    path.Stroke = brush;

                    shapes.Add(path);
                }
            }

            DrawOverlay(shapes, z: 101);
        }

        private Shape[] CreateRectWithText(Rect rect, SolidColorBrush textColor, SolidColorBrush shadowColor, SolidColorBrush rectColor, string text, bool above)
        {
            Shape[] shapes = new Shape[3];

            double fontHeight;
            int space = 1;
            shapes[1] = CreateTextShape(text, rect.X - space, rect.Y + space, shadowColor, rect.Width, out _, above);
            shapes[2] = CreateTextShape(text, rect.X, rect.Y, textColor, rect.Width, out fontHeight, above);
            shapes[0] = CreateRect(new Rect(rect.X, rect.Y - (above ? fontHeight : 0), rect.Width, fontHeight), 0, null, rectColor);
            return shapes;
        }

        private Shape CreateRect(Rect rect, double strokeThickness, SolidColorBrush stroke, SolidColorBrush fill)
        {
            Path path = new Path();
            path.Data = new RectangleGeometry(rect);
            path.StrokeThickness = strokeThickness;
            path.Stroke = stroke;
            path.Fill = fill;
            return path;
        }

        private Shape CreateTextShape(string text, double placeX, double placeY, SolidColorBrush brush, double width, out double fontHeight, bool above = true)
        {
            Shape textShape;

            double fontSize = 8;
            FontFamily fontFamily = new FontFamily("Arial");


            Typeface typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, new FontStretch());

            FormattedText fText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, 1.25);
            double scale = width / (fText.Width);
            fontHeight = Math.Ceiling(fontSize * fontFamily.LineSpacing) * scale;

            Path path = new Path();
            path.Data = fText.BuildGeometry(new Point());
            ScaleTransform transform = new ScaleTransform(scale, scale);
            path.Data.Transform = transform;
            Canvas.SetLeft(path, placeX);
            Canvas.SetTop(path, placeY + (above ?  -fontHeight : 0));
            path.Fill = brush;
            textShape = path;
            return textShape;
        }
    }
}
