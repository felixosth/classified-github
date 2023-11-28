using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TryggRetail.PopupWindow
{
    public static class ShapeHelper
    {
        public static Shape CreateTextShape(System.Drawing.Size size, string text, double scaleX, double scaleY, double scaleFontSize, System.Windows.Media.Color color)
        {
            //Debug.WriteLine(text + " paint size (" + size.Height + "," + size.Width + ")");
            double x = (size.Width * scaleX) / 1000;
            double y = (size.Height * scaleY) / 1000;
            double fontsize = (size.Height * scaleFontSize) / 1000;
            if (fontsize < 7) fontsize = 12;

            return CreateTextShape(text, x, y, fontsize, color);
        }

        public static Shape CreateTextShape(string text, double placeX, double placeY, double fontSize, System.Windows.Media.Color color)
        {
            Shape textShape;
            System.Windows.Media.FontFamily fontFamily = new System.Windows.Media.FontFamily("Arial");
            Typeface typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Bold, new FontStretch());

            FormattedText fText = new FormattedText(text, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, typeface, fontSize, System.Windows.Media.Brushes.Black);

            System.Windows.Point textPosition1;
            textPosition1 = new System.Windows.Point(placeX, placeY);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = fText.BuildGeometry(textPosition1);
            path.Fill = new SolidColorBrush(color);
            textShape = path;
            return textShape;
        }
    }
}
