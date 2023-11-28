using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryggSTORE.MJPEG.FrameCreators
{
    internal class NotFoundFrameCreator : FrameCreatorBase
    {
        Image baseImage;
        internal override string Name => nameof(NotFoundFrameCreator);

        public NotFoundFrameCreator()
        {
            this.UpdateInterval = 10000;
            baseImage = CreateImage(1280, 720);

            var graphics = Graphics.FromImage(baseImage);

            graphics.Clear(Color.Black);

            graphics.DrawString("Stream not found :(", SystemFonts.DefaultFont, Brushes.White, 20, 20);
            graphics.Dispose();
        }

        internal override Image CreateFrame()
        {
            return new Bitmap(baseImage);
        }
    }
}
