using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Data;

namespace ImageRetrievalService
{
    public partial class ImageRetrievalService : ServiceBase
    {

        public ImageRetrievalService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
#if DEBUG
            base.RequestAdditionalTime(600000); // 600*1000ms = 10 minutes timeout
            Debugger.Launch(); // launch and attach debugger
#endif


            if (args.Length < 1)
                return;

            Uri uri = new Uri("localhost");

            VideoOS.Platform.SDK.Export.Environment.Initialize();
            VideoOS.Platform.SDK.Environment.AddServer(new Uri("localhost"), CredentialCache.DefaultNetworkCredentials);


            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string myFolder = Path.Combine(appData, "TryggLarm.ImageGetter");
            Directory.CreateDirectory(myFolder);

            var item = Item.Deserialize(args[0]);
            Image.FromStream(GetImage(item, DateTime.Parse(args[1]))).Save(Path.Combine(myFolder, "tmpImage.jpeg"));

            this.Stop();

            base.OnStart(args);
        }

        protected override void OnStop()
        {
        }


        MemoryStream GetImage(Item camera, DateTime alarmDate)
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
