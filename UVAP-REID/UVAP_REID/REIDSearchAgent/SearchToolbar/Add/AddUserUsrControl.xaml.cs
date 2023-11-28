using REIDShared.NodeRED;
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
using VideoOS.Platform.Messaging;

namespace REIDSearchAgent.SearchToolbar.Add
{
    /// <summary>
    /// Interaction logic for AddUserUsrControl.xaml
    /// </summary>
    public partial class AddUserUsrControl : UserControl
    {
        private Item camera;
        private DateTime triggerTime;
        private FQID playbackFQID;
        NodeREDPerson person;


        public string KeyTxtBox
        {
            get => keyTxtBox.Text;
            set => keyTxtBox.Text = value;
        }

        public string NameTxtBox
        {
            get => nameTxtBox.Text;
            set => nameTxtBox.Text = value;
        }

        public NodeREDPerson GetPerson()
        {
            if (person == null)
                return new NodeREDPerson()
                {
                    key = KeyTxtBox,
                    personName = NameTxtBox,
                    category = (categoryComboBox.SelectedItem as NodeREDCategory).id ?? default
                };
            else
            {
                person.personName = NameTxtBox;
                person.category = (categoryComboBox.SelectedItem as NodeREDCategory).id ?? default;
                return person;
            }
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="person"></param>
        public AddUserUsrControl(NodeREDPerson person, Item camera, DateTime triggerTime)
        {
            InitializeComponent();
            KeyTxtBox = person.key;
            NameTxtBox = person.personName;
            this.person = person;

            this.camera = camera;
            this.triggerTime = triggerTime;
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="triggerTime"></param>
        public AddUserUsrControl(Item camera, DateTime triggerTime)
        {
            InitializeComponent();

            this.camera = camera;
            this.triggerTime = triggerTime;
        }

        public AddUserUsrControl()
        {
            InitializeComponent();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Window).DialogResult = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Window).DialogResult = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (camera == null)
                return;

            imageViewerControl.EnableDigitalZoom = true;
            imageViewerControl.EnableMouseControlledPtz = true;
            imageViewerControl.EnableMousePtzEmbeddedHandler = true;
            imageViewerControl.MaintainImageAspectRatio = true;
            imageViewerControl.EnableSetupMode = false;
            imageViewerControl.SetImageQuality(101);

            playbackFQID = ClientControl.Instance.GeneratePlaybackController();
            playbackControl.Init(playbackFQID);

            playbackControl.SetCameras(new List<FQID>() { camera.FQID });
            playbackControl.TimeSpan = TimeSpan.FromMinutes(1);

            imageViewerControl.CameraFQID = camera.FQID;
            imageViewerControl.PlaybackControllerFQID = playbackFQID;

            imageViewerControl.Initialize();
            imageViewerControl.Connect();
            imageViewerControl.UpdateStates();

            imageViewerControl.StartBrowse();


            EnvironmentManager.Instance.SendMessage(new VideoOS.Platform.Messaging.Message(
                                            VideoOS.Platform.Messaging.MessageId.System.ModeChangeCommand,
                                            Mode.ClientPlayback), playbackFQID);

            EnvironmentManager.Instance.SendMessage(new VideoOS.Platform.Messaging.Message(
                                MessageId.SmartClient.PlaybackCommand,
                                new PlaybackCommandData() { Command = PlaybackData.PlayStop }), playbackFQID);

            EnvironmentManager.Instance.SendMessage(new VideoOS.Platform.Messaging.Message(
                                            MessageId.SmartClient.PlaybackCommand,
                                            new PlaybackCommandData() { Command = PlaybackData.Goto, DateTime = triggerTime.ToUniversalTime() }), playbackFQID);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (playbackFQID == null)
                return;
            imageViewerControl.Disconnect();
            imageViewerControl.Close();
            playbackControl.Close();
            ClientControl.Instance.ReleasePlaybackController(playbackFQID);
        }
    }
}
