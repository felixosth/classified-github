using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace WpfTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Uri server = new Uri("http://172.16.100.222");

        public MainWindow()
        {
            VideoOS.Platform.SDK.Environment.Initialize();
            VideoOS.Platform.SDK.Environment.AddServer(server, new System.Net.NetworkCredential("administrator", "Camera19!"));
            VideoOS.Platform.SDK.UI.Environment.Initialize();
            VideoOS.Platform.SDK.Environment.Login(server);

            InitializeComponent();

            var camera = GetChildren(Configuration.Instance.GetItems(ItemHierarchy.SystemDefined)[0]).Where(c => c.FQID.Kind == Kind.Camera && c.Enabled == true && c.HasChildren == HasChildren.No).First();
            EnvironmentManager.Instance.Mode = Mode.ClientLive;


            for (int i = 0; i < 4; i++)
            {
                var videoFrame = new ImageViewerWpfControl();

                videoFrame.CameraFQID = camera.FQID;
                videoFrame.Initialize();
                videoFrame.Connect();
                Grid.SetColumn(videoFrame, i);
                videoGrid.Children.Add(videoFrame);
            }

            foreach (ImageViewerWpfControl videoFrame in videoGrid.Children)
            {
                //videoFrame = new ImageViewerWpfControl();
                //_imageViewerControl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                //_imageViewerControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                //videoFrame.EnableMouseControlledPtz = true;

                //this.Content = videoFrame;
                //myGrid.Children.Add(_imageViewerControl);

                //_imageViewerControl.PlaybackControllerFQID = _playbackFQID;
                videoFrame.CameraFQID = camera.FQID;
                videoFrame.Initialize();
                videoFrame.Connect();
                //videoFrame.StartLive();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            VideoOS.Platform.SDK.Environment.Logout();
            base.OnClosed(e);
        }

        static List<Item> GetChildren(Item parent)
        {
            List<Item> items = new List<Item>();
            var children = parent.GetChildren();
            foreach (var child in children)
            {
                items.Add(child);
                items.AddRange(GetChildren(child));
            }
            return items;
        }
    }
}
