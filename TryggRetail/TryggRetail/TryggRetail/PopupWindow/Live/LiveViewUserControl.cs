using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform.Client;
using VideoOS.Platform;
using System.Windows.Shapes;

namespace TryggRetail.PopupWindow.Live
{
    public partial class LiveViewUserControl : ViewItemUserControl
    {
        private ImageViewerControl imageViewerControl;


        public LiveViewUserControl(Background.TryggRetailBackgroundPlugin instance)
        {
            InitializeComponent();

            var alarm = instance.CurrentAlarm;

            if (alarm.ReferenceList.Count < 1)
                return;

            FQID camera = alarm.ReferenceList[0].FQID;


            imageViewerControl = ClientControl.Instance.GenerateImageViewerControl(this.WindowInformation);
            imageViewerControl.Dock = DockStyle.Fill;
            //_imageViewerControl.AllowDrop = true;
            imageViewerControl.Selected = true;

            this.Controls.Add(imageViewerControl);

            imageViewerControl.CameraFQID = camera;
        }

        private void LiveViewUserControl_Load(object sender, EventArgs e)
        {
            imageViewerControl.Initialize();
            imageViewerControl.Connect();

            var shapes = new List<Shape>();
            shapes.Add(ShapeHelper.CreateTextShape("Live", 12, 12, 30, System.Windows.Media.Colors.Black));
            shapes.Add(ShapeHelper.CreateTextShape("Live", 10, 10, 30, System.Windows.Media.Colors.Green));

            imageViewerControl.ShapesOverlayAdd(shapes, new ShapesOverlayRenderParameters());
        }

        public override void Close()
        {
            imageViewerControl.Disconnect();
            imageViewerControl.Close();
            imageViewerControl.Dispose();
        }

        public override bool ShowToolbar => false;
    }
}
