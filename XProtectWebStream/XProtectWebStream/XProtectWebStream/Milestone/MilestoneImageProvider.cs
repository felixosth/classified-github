using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Live;
using VideoOS.Platform;
using XProtectWebStream.Global;

namespace XProtectWebStream.Milestone
{
    internal class MilestoneImageProvider
    {
        private int _imgHash;
        internal int ImageHash
        {
            get
            {
                lastPoll = DateTime.Now;
                return _imgHash;
            }
            set => _imgHash = value;
        }

        internal event EventHandler<string> OnLog;
        internal bool IsClosed { get; set; }

        private DateTime lastPoll;

        internal event EventHandler OnClose;

        internal byte[] ImageBytes { get; set; }
        internal string CameraId { get; set; }
        private readonly JPEGLiveSource liveSource;

        public MilestoneImageProvider(Item item, string cameraId)
        {
            CameraId = cameraId;
            liveSource = new JPEGLiveSource(item);
            lastPoll = DateTime.Now;

            liveSource.LiveContentEvent += LiveSource_LiveContentEvent;
            liveSource.LiveStatusEvent += LiveSource_LiveStatusEvent;


            liveSource.Width = Config.Instance.Camera.ResWidth;
            liveSource.Height = Config.Instance.Camera.ResHeight;
            liveSource.Compression = Config.Instance.Camera.Compression;
            liveSource.FPS = Config.Instance.Camera.FPS;

            liveSource.SetKeepAspectRatio(true, true);
            liveSource.Init();

            liveSource.LiveModeStart = true;
        }

        private void LiveSource_LiveContentEvent(object sender, EventArgs e)
        {
            try
            {
                LiveContentEventArgs args = e as LiveContentEventArgs;
                if (args != null && args.LiveContent != null)
                {
                    ImageBytes = args.LiveContent.Content;
                    ImageHash = ImageBytes.GetHashCode();
                }
                else if (args.Exception != null)
                {
                    throw args.Exception;
                }

                if (lastPoll.AddSeconds(10) < DateTime.Now) // Close if nobody wants us :'(
                {
                    Close();
                }
            }
            catch(Exception ex)
            {
                OnLog?.Invoke(this, ex.Message);
            }
        }

        private void LiveSource_LiveStatusEvent(object sender, EventArgs e)
        {
        }

        internal void Close()
        {
            liveSource.LiveModeStart = false;
            liveSource.LiveContentEvent -= LiveSource_LiveContentEvent;
            liveSource.LiveStatusEvent -= LiveSource_LiveStatusEvent;
            liveSource.Close();
            IsClosed = true;
            OnClose?.Invoke(this, new EventArgs());
        }


    }
}
