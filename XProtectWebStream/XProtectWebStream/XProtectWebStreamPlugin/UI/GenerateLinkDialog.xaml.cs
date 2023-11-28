using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using VideoOS.Platform.Data;
using VideoOS.Platform.Live;
using XProtectWebStream.Shared;

namespace XProtectWebStreamPlugin.UI
{
    /// <summary>
    /// Interaction logic for GenerateLinkDialog.xaml
    /// </summary>
    public partial class GenerateLinkDialog : UserControl
    {
        List<SharedAccessGroup> accessGroups;
        SharedAccessGroup anyoneGroup;

        ClientCommunication clientCommunication;
        string cameraId;

        public GenerateLinkDialog()
        {
            InitializeComponent();
        }

        internal GenerateLinkDialog(ClientCommunication clientCommunication, Item camera, bool startAtExport = false)
        {
            InitializeComponent();

            liveRadio.IsEnabled = clientCommunication.LastFeatureResponse.CanShareLive == true;
            exportRadio.IsEnabled = clientCommunication.LastFeatureResponse.CanShareRecorded == true;

            requestLinkBtn.IsEnabled = clientCommunication.LastFeatureResponse.LicenseIsValid == true &&
                (clientCommunication.LastFeatureResponse.CanShareLive == true ||
                clientCommunication.LastFeatureResponse.CanShareRecorded == true);

            validTimeTxtBox.Text = clientCommunication.LastFeatureResponse.DefaultValidMinutes.ToString();

            requireBankIdChkBox.IsEnabled = clientCommunication.LastFeatureResponse.CanUseBankID == true;

            cameraNameTxtBox.Text = "Camera: " + camera.Name;
            this.clientCommunication = clientCommunication;
            this.cameraId = camera.FQID.ObjectId.ToString();

            fromDatePicker.SelectedDate = DateTime.Now;
            toDatePicker.SelectedDate = DateTime.Now;

            fromTimeTxtBox.Text = DateTime.Now.AddMinutes(-1).TimeOfDay.ToString(@"hh\:mm\:ss");
            toTimeTxtBox.Text = DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss");


            accessGroups = new List<SharedAccessGroup>(clientCommunication.AccessGroups.OrderBy(grp => grp.Name));
            accessGroups.ForEach(grp => grp.IsChecked = false);
            anyoneGroup = new SharedAccessGroup(-1, "Anyone") { IsChecked = true, CanCheck = false };
            accessGroups.Insert(0, anyoneGroup);
            accessGroupsListBox.ItemsSource = accessGroups;
            accessGroupsExpander.Header = string.Join(", ", accessGroups.Where(grp => grp.IsChecked).Select(grp => grp.Name));


            if (startAtExport)
                exportRadio.IsChecked = true;
            else
                liveRadio.IsChecked = true;

            SetCamImgPreview(camera);
        }

        private void SetCamImgPreview(Item camera)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
            try
            {
                    DateTime start = DateTime.Now;
                    JPEGLiveSource liveSource = new JPEGLiveSource(camera);
                    liveSource.Init();
                    ImageSource img = null;

                    EventHandler eventHandler = (s, e) => 
                    {
                        var contentEventArgs = e as LiveContentEventArgs;
                        if(img == null && contentEventArgs != null && contentEventArgs.LiveContent != null && contentEventArgs.LiveContent.Content != null && contentEventArgs.LiveContent.Content.Length > 0)
                        {
                            img = LoadImage(contentEventArgs.LiveContent.Content);
                        }
                    };
                    liveSource.LiveContentEvent += eventHandler;
                    liveSource.LiveModeStart = true;

                    while(img == null || DateTime.Now > start.AddSeconds(20))
                    {
                        Thread.Sleep(10);
                    }
                    liveSource.LiveModeStart = false;
                    liveSource.LiveContentEvent -= eventHandler;
                    liveSource.Close();

                    ClientControl.Instance.CallOnUiThread(() =>
                    {
                        camImg.Tag = camera.Name;
                        camImg.Source = img;
                        camImgPreviewTextBlock.Visibility = Visibility.Collapsed;
                    });

                }
                catch(Exception ex)
                {
                    EnvironmentManager.Instance.Log(true, nameof(GenerateLinkDialog), ex.ToString());
                }
            });
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void RequestLinkBtn_Click(object sender, RoutedEventArgs e)
        {
            int minutes;
            string password = null;

            TimeSpan fromTimeSpan, toTimeSpan;

            if(!int.TryParse(validTimeTxtBox.Text, out minutes))
            {
                MessageBox.Show("Invalid valid time input");
                validTimeTxtBox.Focus();
                return;
            }
            else if(minutes > clientCommunication.LastFeatureResponse.MaxValidMinutes)
            {
                MessageBox.Show("Max valid time is above the allowed threshold. Max is " + clientCommunication.LastFeatureResponse.MaxValidMinutes + " minutes.");
                validTimeTxtBox.Focus();
                validTimeTxtBox.Text = clientCommunication.LastFeatureResponse.MaxValidMinutes.ToString();

                return;
            }
            else if(minutes < 1)
            {
                MessageBox.Show("Max valid time is too low.");
                validTimeTxtBox.Text = "1";
                validTimeTxtBox.Focus();
                return;
            }

            if(DateTime.TryParse(fromTimeTxtBox.Text, out DateTime fromTimeDateTime))
            {
                fromTimeSpan = fromTimeDateTime.TimeOfDay;
            }
            else
            {
                MessageBox.Show("Invalid from-time");
                fromTimeTxtBox.Focus();
                return;
            }

            if (DateTime.TryParse(toTimeTxtBox.Text, out DateTime toTimeDateTime))
            {
                toTimeSpan = toTimeDateTime.TimeOfDay;
            }
            else
            {
                MessageBox.Show("Invalid to-time");
                toTimeTxtBox.Focus();
                return;
            }

            if (passwordTxtBox.Password != "" && passwordTxtBox.Password != confirmPasswordTxtBox.Password)
            {
                MessageBox.Show("Please confirm your password");
                confirmPasswordTxtBox.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(passwordTxtBox.Password) == false)
                password = passwordTxtBox.Password;


            requestLinkBtn.IsEnabled = false;

            var selectedAccessGroups = accessGroups.Where(grp => grp.IsChecked).Select(grp => grp.Id).Where(id => id > 0).ToArray();

            if (liveRadio.IsChecked == true)
            {
                clientCommunication.RequestLiveToken(cameraId, TimeSpan.FromMinutes(minutes), password, commentTxtBox.Text, requireBankIdChkBox.IsChecked == true, selectedAccessGroups);
            }
            else if(exportRadio.IsChecked == true)
            {
                var from = fromDatePicker.SelectedDate.Value.Date + fromTimeSpan;
                var to = toDatePicker.SelectedDate.Value.Date + toTimeSpan;
                clientCommunication.RequestExportToken(cameraId, TimeSpan.FromMinutes(minutes), from, to, password, commentTxtBox.Text, requireBankIdChkBox.IsChecked == true, selectedAccessGroups);
            }

            Window.GetWindow(this).Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void passwordTxtBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(passwordTxtBox.Password.Length > 0)
            {
                passwordTextBlock.Visibility = Visibility.Collapsed;
                confirmPasswordTxtBox.IsEnabled = true;
            }
            else
            {
                passwordTextBlock.Visibility = Visibility.Visible;
                confirmPasswordTxtBox.IsEnabled = false;
                confirmPasswordTxtBox.Password = string.Empty;
            }
        }

        private void confirmPasswordTxtBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            confirmPasswordTextBlock.Visibility = confirmPasswordTxtBox.Password.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void historyBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Content = new LinkHistoryDialog(clientCommunication);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkedGroups = accessGroups.Count(grp => grp.IsChecked);
            if (anyoneGroup.IsChecked && checkedGroups >= 2)
            {
                anyoneGroup.IsChecked = false;
            }
            else if(accessGroups.Count(grp => grp.IsChecked) == 0)
            {
                anyoneGroup.IsChecked = true;
            }

            accessGroupsExpander.Header = string.Join(", ", accessGroups.Where(grp => grp.IsChecked).Select(grp => grp.Name));
        }

        private void requireBankIdChkBox_Checked(object sender, RoutedEventArgs e)
        {
            passwordTxtBox.IsEnabled = requireBankIdChkBox.IsChecked == false;
            if (passwordTxtBox.IsEnabled == false)
            {
                passwordTxtBox.Password = "";
                confirmPasswordTxtBox.Password = "";
            }
        }
    }
}
