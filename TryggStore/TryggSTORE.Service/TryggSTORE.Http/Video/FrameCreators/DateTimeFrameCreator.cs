using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryggSTORE.MJPEG.FrameCreators
{
    internal class DateTimeFrameCreator : FrameCreatorBase
    {
        Font font;
        Image baseImage;
        int lastRotaton = 0;

        internal override string Name => nameof(DateTimeFrameCreator);

        public DateTimeFrameCreator()
        {
            font = new Font("Arial", 72, FontStyle.Regular, GraphicsUnit.Pixel);
            baseImage = CreateImage(1920, 1080, 300.0F);
            this.UpdateInterval = 1000;
        }

        internal override Image CreateFrame()
        {
            //var image = CreateImage(1920, 1080, 300.0F);
            var image = new Bitmap(baseImage);

            var graphics = Graphics.FromImage(image);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            var imgRect = new Rectangle(0, 0, image.Width, image.Height);

            graphics.FillRectangle(new LinearGradientBrush(imgRect, Color.Red, Color.Blue, 60f), imgRect);
            graphics.TranslateTransform(image.Width / 2, image.Height / 2);

            graphics.RotateTransform(lastRotaton);

            lastRotaton += 1;
            if (lastRotaton >= 360)
                lastRotaton -= 360;

            var text = DateTime.Now.ToString();
            var stringSize = graphics.MeasureString(text, font);
            graphics.DrawString(text, font, Brushes.White, -(stringSize.Width/2), -(stringSize.Height/2));

            graphics.Dispose();

            return image;
        }
    }
}
