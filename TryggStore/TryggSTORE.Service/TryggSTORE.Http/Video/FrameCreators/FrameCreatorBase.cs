using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TryggSTORE.MJPEG.FrameCreators
{
    internal abstract class FrameCreatorBase
    {
        internal event EventHandler OnNewImageAvailable;
        internal event EventHandler<string> OnError;

        protected byte[] latestFrameData;

        internal byte[] LatestFrame => latestFrameData;

        protected int Width { get; set; }
        protected int Height { get; set; }

        protected int UpdateInterval { get; set; } = 1000;

        private bool running = false;
        internal int CreateOffset { get; set; } = 0;

        internal abstract string Name { get; }

        protected virtual int WaitInterval { get; set; } = 100;

        internal FrameCreatorBase()
        {
        }

        internal void StartCreate()
        {
            running = true;

            //ThreadPool.QueueUserWorkItem(CreateThread);
            new Thread(CreateThread) { IsBackground = true, Name = Name + " create thread" }.Start();
        }

        protected void Error(string errorMessage)
        {
            OnError?.Invoke(this, errorMessage);
        }

        private void CreateThread(object state)
        {
            Thread.Sleep(CreateOffset);

            while (running)
            {
                try
                {
                    var newFrame = CreateFrame();
                    latestFrameData = ImageToBytesBetter(newFrame, 100);

                    OnNewImageAvailable?.Invoke(this, new EventArgs());
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, ex.Message);
                }

                var nextCreate = DateTime.Now.AddMilliseconds(UpdateInterval);

                while(DateTime.Now < nextCreate || !running)
                {
                    Thread.Sleep(WaitInterval);
                }
            }
        }

        protected Image CreateImage(int width, int height, Single dpi = 96.0F)
        {
            var image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            image.SetResolution(dpi, dpi);
            return image;
        }

        internal void Stop()
        {
            running = false;
        }

        private byte[] ImageToBytes(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private byte[] ImageToBytesBetter(Image image, long quality)
        {
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, jgpEncoder, myEncoderParameters);
                return ms.ToArray();
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        internal abstract Image CreateFrame();
    }
}
