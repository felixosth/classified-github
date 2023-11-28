using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VideoOS.Platform.Client;
using VideoOS.Platform;
using System.Linq;
using VideoOS.Platform.Messaging;
using System.Windows.Controls;
using System.Windows;
using Renci.SshNet;
using System.Xml;
using Newtonsoft.Json;
using System.Threading;

namespace BergendahlsPOS.Client
{
    /// <summary>
    /// Interaction logic for BergendahlsPOSWorkSpaceViewItemWpfUserControl.xaml
    /// </summary>
    public partial class BergendahlsPOSWorkSpaceViewItemWpfUserControl : ViewItemWpfUserControl
    {
        List<TransactionsTransaction> transactions;
        private PlaybackWpfUserControl _playbackWpfUserControl;
        private FQID playbackControllerFqid;

        public BergendahlsPOSWorkSpaceViewItemWpfUserControl()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            int i = 0;
            FQID[] usedCamerasList = new FQID[0];
            int storeId = -1;

            System.Xml.XmlNode result = VideoOS.Platform.Configuration.Instance.GetOptionsConfiguration(BergendahlsPOSDefinition.BergendahlsPOSSettingsPanel, false);

            if (result != null)
            {
                foreach (XmlNode node in result.ChildNodes)
                {
                    switch (node.Attributes["name"].Value)
                    {
                        case BergendahlsPOSSettingsPanelPlugin.CamerasSettingsKey:
                            string usedCamerasRaw = node.Attributes["value"].Value;
                            if (usedCamerasRaw != null)
                            {
                                var usedCamerasListStrings = JsonConvert.DeserializeObject<string[]>(usedCamerasRaw);
                                usedCamerasList = new FQID[usedCamerasListStrings.Length];
                                for (i = 0; i < usedCamerasListStrings.Length; i++)
                                {
                                    usedCamerasList[i] = new FQID(usedCamerasListStrings[i]);
                                }
                            }
                            break;
                        case BergendahlsPOSSettingsPanelPlugin.StoreIDSettingsKey:
                            if(!int.TryParse(node.Attributes["value"].Value, out storeId))
                            {
                                MessageBox.Show("Fel, kontakta InSupport. \r\n\r\nFelaktig konfiguration.", "Fel");
                                return;
                            }
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Fel, kontakta InSupport. \r\n\r\nSaknar konfiguration", "Fel");
                return;
            }

            List<Item> cameras = new List<Item>();

            foreach(var fqid in usedCamerasList)
            {
                var cameraItem = Configuration.Instance.GetItem(fqid);
                if (cameraItem != null)
                    cameras.Add(cameraItem);
            }

            playbackControllerFqid = ClientControl.Instance.GeneratePlaybackController();
            _playbackWpfUserControl = new PlaybackWpfUserControl();
            playbackControllerGrid.Children.Add(_playbackWpfUserControl);
            _playbackWpfUserControl.Init(playbackControllerFqid);
            _playbackWpfUserControl.SetCameras(cameras.Select(c => c.FQID).ToList());

            i = 0;
            foreach (var camera in cameras)
            {
                var column = new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                camerasGrid.ColumnDefinitions.Add(column);

                var videoFrame = new ImageViewerWpfControl();
                videoFrame.EnableMouseControlledPtz = true;
                videoFrame.EnableMousePtzEmbeddedHandler = true;
                videoFrame.EnableDigitalZoom = true;
                videoFrame.Selected = true;

                videoFrame.MouseDoubleClick += VideoFrame_SetFullscreen;

                videoFrame.CameraFQID = camera.FQID;
                videoFrame.PlaybackControllerFQID = playbackControllerFqid;
                videoFrame.Initialize();
                videoFrame.Connect();
                Grid.SetColumn(videoFrame, i);
                camerasGrid.Children.Add(videoFrame);

                i++;
            }

#if DEBUG
            new Thread(() => LoadFakedata()).Start();
#else
            new Thread(() => LoadXmlDocuments(storeId)).Start();
#endif
        }

        void LoadFakedata()
        {
            var random = new Random();
            var now = DateTime.Now.AddDays(-1);
            transactions = new List<TransactionsTransaction>();
            string[] tags = new string[]
            {
                "Sent Återköp",
                "Tidigt Återköp",
                "Bara ett återköp"
            };

            for (int i = 0; i < 80; i++)
            {
                var fakeTransaction = new TransactionsTransaction()
                {
                    Amount = random.Next(-1000, 0),
                    Date = now.AddHours(-i),
                    Operator = "Operator-" + random.Next(11),
                    Time = now.AddMinutes(-i),
                    Workstation = "Kassan",
                    TransNumber = i.ToString(),
                    RetailStore = "1",
                    RetailStoreName = "Affären",
                    Tag = tags[random.Next(tags.Length)]
                };
                transactions.Add(fakeTransaction);
            }


            ClientControl.Instance.CallOnUiThread(() =>
            {
                dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
                dataGrid.ItemsSource = transactions;
                loadingGrid.Visibility = Visibility.Collapsed;
                dataGrid.Visibility = Visibility.Visible;
            });
        }

        void LoadXmlDocuments(int storeId)
        {
            Dictionary<string, byte[]> fileBytes = null;
            try
            {
                fileBytes = GetXMLDocFromSFTP();

            }
            catch(Exception ex)
            {
                MessageBox.Show("Fel, kontakta InSupport. \r\n\r\nKunde ej hämta data från Bergendahls SFTP\r\n\r\n" + ex.Message, "Fel");
            }

            if (fileBytes == null)
            {
                MessageBox.Show("Fel, kontakta InSupport. \r\n\r\nKunde ej hämta data från Bergendahls SFTP", "Fel");
                return;
            }

            transactions = new List<TransactionsTransaction>();
            foreach (var kvp in fileBytes)
            {
                try
                {
                    var posData = new POSData(kvp.Value);
                    transactions.AddRange(posData.GetStronglyTypedTransactions().Transaction.Where(t => t.RetailStore == storeId.ToString()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fel i läsning av fil '" + kvp.Key.Replace(".xml", "") + "', kontakta InSupport\r\n\r\n" + ex.Message);
                }
            }

            ClientControl.Instance.CallOnUiThread(() =>
            {
                dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
                dataGrid.ItemsSource = transactions;
                loadingGrid.Visibility = Visibility.Collapsed;
                dataGrid.Visibility = Visibility.Visible;
            });
        }

        private void VideoFrame_SetFullscreen(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var fullCol = Grid.GetColumn(sender as ImageViewerWpfControl);
            foreach (ImageViewerWpfControl videoFrame in camerasGrid.Children)
            {
                var collapseCol = Grid.GetColumn(videoFrame);

                if(collapseCol != fullCol)
                {
                    videoFrame.Disconnect();
                    camerasGrid.ColumnDefinitions[collapseCol].Width = new GridLength(0);
                }
                else
                {
                    _playbackWpfUserControl.SetCameras(new List<FQID>() { videoFrame.CameraFQID });
                }
            }

            (sender as ImageViewerWpfControl).MouseDoubleClick -= VideoFrame_SetFullscreen;
            (sender as ImageViewerWpfControl).MouseDoubleClick += VideoFrame_UnsetFullscreen;
        }

        private void VideoFrame_UnsetFullscreen(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var fullCol = Grid.GetColumn(sender as UIElement);
            foreach (ImageViewerWpfControl videoFrame in camerasGrid.Children)
            {
                var showCol = Grid.GetColumn(videoFrame);

                if (showCol != fullCol)
                {
                    videoFrame.Connect();
                    camerasGrid.ColumnDefinitions[showCol].Width = new GridLength(1, GridUnitType.Star);
                }
            }
            _playbackWpfUserControl.SetCameras(camerasGrid.Children.Cast<ImageViewerWpfControl>().Select(iv => iv.CameraFQID).ToList());
            (sender as ImageViewerWpfControl).MouseDoubleClick -= VideoFrame_UnsetFullscreen;
            (sender as ImageViewerWpfControl).MouseDoubleClick += VideoFrame_SetFullscreen;
        }

        private void DataGrid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            switch(e.Column.Header)
            {
                case "Time":
                case "Date":
                case "RetailStore":
                case "RetailStoreName":
                    e.Cancel = true;
                    break;
                case "CombinedDateAndTime":
                    e.Column.Header = "Datum & tid";
                    break;
                case "Operator":
                    e.Column.Header = "PersonalNR";
                    break;
                case "Workstation":
                    e.Column.Header = "KassaNr";
                    break;
                case "TransNumber":
                    e.Column.Header = "KvittoNr";
                    break;
                case "Amount":
                    e.Column.Header = "Belopp";
                    break;
                case "Tag":
                    e.Column.Header = "Händelse";
                    break;
            }

            e.Column.MinWidth += 100;
        }

        public override void Close()
        {
            foreach(ImageViewerWpfControl videoFrame in camerasGrid.Children)
            {
                videoFrame.Disconnect();
                videoFrame.Close();
            }
            _playbackWpfUserControl?.Close();
            if(playbackControllerFqid != null)
                ClientControl.Instance.ReleasePlaybackController(playbackControllerFqid);
        }

        /// <summary>
        /// Do not show the sliding toolbar!
        /// </summary>
        public override bool ShowToolbar
        {
            get { return false; }
        }

        private void ViewItemWpfUserControl_ClickEvent(object sender, EventArgs e)
        {
            FireClickEvent();
        }

        private void ViewItemWpfUserControl_DoubleClickEvent(object sender, EventArgs e)
        {
            FireDoubleClickEvent();
        }

        public static List<Item> GetAllItems(List<Item> items)
        {
            List<Item> result = new List<Item>();
            foreach (var item in items)
            {
                if (item.FQID.FolderType == FolderType.No)
                    result.Add(item);
                else
                    result.AddRange(GetAllItems(item.GetChildren()));
            }
            return result;
        }

        private void dataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var row = dataGrid.SelectedItem as TransactionsTransaction;

            EnvironmentManager.Instance.SendMessage(new VideoOS.Platform.Messaging.Message(
                                MessageId.SmartClient.PlaybackCommand,
                                new PlaybackCommandData() { Command = PlaybackData.PlayStop, DateTime = row.CombinedDateAndTime.ToUniversalTime() }), playbackControllerFqid);

            EnvironmentManager.Instance.SendMessage(new VideoOS.Platform.Messaging.Message(
                                            MessageId.SmartClient.PlaybackCommand,
                                            new PlaybackCommandData() { Command = PlaybackData.Goto, DateTime = row.CombinedDateAndTime.ToUniversalTime() }), playbackControllerFqid);
        }

        Dictionary<string, byte[]> GetXMLDocFromSFTP()
        {
            Dictionary<string, byte[]> fileBytes = new Dictionary<string, byte[]>();
            using (SftpClient client = new SftpClient(new ConnectionInfo("file.bergendahls.se", "insupport", new PasswordAuthenticationMethod("insupport", "NX523MPk"))))
            {
                client.Connect();

                var files = client.ListDirectory("/");

                ClientControl.Instance.CallOnUiThread(() => loadingProgressBar.Maximum = files.Count());

                foreach (var xmlFile in files.Where(f => f.FullName.EndsWith(".xml")))
                {
                    using (var ms = new MemoryStream())
                    {
                        client.DownloadFile(xmlFile.FullName, ms);
                        fileBytes.Add(xmlFile.Name, ms.ToArray());
                    }
                    ClientControl.Instance.CallOnUiThread(() => loadingProgressBar.Value = loadingProgressBar.Value + 1);
                }

                client.Disconnect();
            }
            return fileBytes;
        }

        private void ViewItemWpfUserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
