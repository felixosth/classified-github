using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Data;
using VideoOS.Platform;
using System.IO;

namespace TryggLarm.Background
{
    public class ImageGetter
    {

        public MemoryStream GetImage(Item camera, DateTime alarmDate)
        {
            JPEGVideoSource videoSource = new JPEGVideoSource(camera);
            videoSource.AllowUpscaling = true;
            videoSource.Init();

            JPEGData data = videoSource.GetNearest(alarmDate) as JPEGData;

            MemoryStream ms = new MemoryStream(data.Bytes);
            ms.Position = 0;
            return ms;
        }
    }
}
