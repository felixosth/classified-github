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
using VideoOS.Platform.Client;

namespace TryggSync.Client
{
    /// <summary>
    /// Interaction logic for CustomPlaybackUserControl.xaml
    /// </summary>
    public partial class CustomPlaybackUserControl : UserControl
    {
        public event EventHandler<PlaybackChangedEventArgs> OnPlaybackChanged;

        public event EventHandler<PlaybackChangedEventArgs> OnLiveButtonClicked;

        public CustomPlaybackUserControl()
        {
            InitializeComponent();
        }

        private void liveBtn_Click(object sender, RoutedEventArgs e)
        {
            OnLiveButtonClicked(this, null);
            //OnPlaybackChanged(this, new PlaybackChangedEventArgs(PlaybackController.PlaybackModeType.Forward));
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            OnPlaybackChanged(this, new PlaybackChangedEventArgs( PlaybackController.PlaybackModeType.Forward));
        }

        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            OnPlaybackChanged(this, new PlaybackChangedEventArgs( PlaybackController.PlaybackModeType.Stop));
        }
    }

    public class PlaybackChangedEventArgs : EventArgs
    {
        public PlaybackController.PlaybackModeType PlaybackMode { get; set; }
        public PlaybackChangedEventArgs(PlaybackController.PlaybackModeType mode)
        {
            this.PlaybackMode = mode;
        }
    }

}
