using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Live;
using VideoOS.Platform.UI;

namespace InSupport.Client
{
    public partial class GetCameraSnapshotForm : Form
    {
        JPEGLiveSource jpegSource;

        bool gotImage = false;

        public MemoryStream CameraStream;

        public GetCameraSnapshotForm(/*Bitmap bitmap,*/ Item camera)
        {
            InitializeComponent();
            //pictureBox1.Image = bitmap;

            jpegSource = new JPEGLiveSource(camera);
            jpegSource.LiveModeStart = true;
            jpegSource.SetKeepAspectRatio(true, false);
            jpegSource.KeyFramesOnly = false;

            jpegSource.Init();

            jpegSource.LiveContentEvent += JpegSource_LiveContentEvent;


            var tensec = DateTime.Now.TimeOfDay + new TimeSpan(0, 0, 5);
            var now = DateTime.Now.TimeOfDay;

            while (tensec > DateTime.Now.TimeOfDay)
            {
                if (gotImage)
                    break;
                Thread.Sleep(500);
            }

            if(!gotImage)
            {
                MessageBox.Show("Hittade ingen kamerabild. Kontakta oss.");
                DialogResult = DialogResult.Abort;
            }
        }

        private void JpegSource_LiveContentEvent(object sender, EventArgs e)
        {
            LiveContentEventArgs args = e as LiveContentEventArgs;
            if (args != null && args.LiveContent != null)
            {
                gotImage = true;

                //gotImage = true;
                CameraStream = new MemoryStream(args.LiveContent.Content);

                Bitmap img = new Bitmap(CameraStream);
                pictureBox1.Image = img;

                jpegSource.LiveContentEvent -= JpegSource_LiveContentEvent;
                jpegSource.Close();
                jpegSource = null;
            }
        }

        private void GetCameraSnapshotForm_Load(object sender, EventArgs e)
        {
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
