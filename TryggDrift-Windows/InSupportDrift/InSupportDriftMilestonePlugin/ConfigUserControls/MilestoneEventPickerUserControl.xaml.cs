using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Messaging;

namespace InSupportDriftMilestonePlugin
{
    /// <summary>
    /// Interaction logic for MilestoneEventPickerUserControl.xaml
    /// </summary>
    public partial class MilestoneEventPickerUserControl : UserControl
    {
        private readonly MessageCommunication msgCom;

        private readonly Dictionary<string, CustomMenuItem> treeViewItems = new Dictionary<string, CustomMenuItem>();

        private readonly object msgComObj;

        public MilestoneEventConfig EventConfig { get; set; }

        public MilestoneEventPickerUserControl(MilestoneEventConfig existingConfig = null)
        {
            InitializeComponent();

            var cameras = GetAllMilestoneItems(Configuration.Instance.GetItemsByKind(Kind.Camera)).Where(camItem => camItem.Enabled).OrderBy(camItem => camItem.Name);

            foreach (var camItem in cameras)
            {
                var camMenuItem = new CustomMenuItem(camItem.Name);
                treeViewItems[camItem.FQID.ObjectId.ToString()] = camMenuItem;
                eventsTreeView.Items.Add(camMenuItem);
            }

            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            msgCom = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            msgComObj = msgCom.RegisterCommunicationFilter(NewEventHandler, new CommunicationIdFilter(MessageId.Server.NewEventIndication));

            if (existingConfig != null)
            {
                inactivityTxtBox.Text = existingConfig.MaxAllowedInactivityHours.ToString();
                foreach (var cam in existingConfig.CamerasAndEvents)
                {
                    if (treeViewItems.ContainsKey(cam.CameraId))
                    {
                        foreach (var camEvent in cam.Events)
                        {
                            var menuItem = treeViewItems[cam.CameraId];
                            menuItem.Items.Add(new CustomMenuItemEvent(menuItem, camEvent.Name, camEvent.MaxAllowedInactivityHours.ToString()) { IsChecked = true });
                        }
                    }
                    else
                    {
                        var camMenuItem = new CustomMenuItem("N/A");

                        foreach (var camEvent in cam.Events)
                        {
                            camMenuItem.Items.Add(new CustomMenuItemEvent(camMenuItem, camEvent.Name, camEvent.MaxAllowedInactivityHours.ToString()) { IsChecked = true });
                        }
                        treeViewItems[cam.CameraId] = camMenuItem;
                        eventsTreeView.Items.Add(camMenuItem);
                    }
                }
            }
            foreach (var menuItem in treeViewItems.Values)
                menuItem.UpdateCheckedState();
        }

        private object NewEventHandler(Message message, FQID f1, FQID f2)
        {
            if (message.Data is BaseEvent baseEventData)
            {
                Dispatcher.Invoke(() => AddEventToTreeView(baseEventData));
            }

            return null;
        }

        private void AddEventToTreeView(BaseEvent baseEventData)
        {
            var sourceObjectId = baseEventData.EventHeader.Source.FQID.ObjectId.ToString();
            var sourceName = baseEventData.EventHeader.Source.Name;
            var eventName = baseEventData.EventHeader.Name;

            if (treeViewItems.ContainsKey(sourceObjectId))
            {
                AddEventToCamera(treeViewItems[sourceObjectId], eventName, false);
            }
            else
            {
                var menuItem = new CustomMenuItem(sourceName);
                AddEventToCamera(menuItem, eventName, false);
                treeViewItems[sourceObjectId] = menuItem;
                eventsTreeView.Items.Add(menuItem);
            }
        }

        private void AddEventToCamera(CustomMenuItem menuItem, string eventName, bool setChecked)
        {
            CustomMenuItemEvent eventItem = menuItem.Items.FirstOrDefault(item => item.Title == eventName);
            if (eventItem == null)
            {
                eventItem = new CustomMenuItemEvent(menuItem, eventName, "");
                menuItem.Items.Add(eventItem);
                eventItem.IsChecked = setChecked;
            }
            else if (setChecked)
                eventItem.IsChecked = true;
        }

        private void LogoutFromMilestone()
        {
            msgCom.UnRegisterCommunicationFilter(msgComObj);
            msgCom.Dispose();
            MessageCommunicationManager.Stop(EnvironmentManager.Instance.MasterSite.ServerId);
            VideoOS.Platform.SDK.Environment.Logout();
            VideoOS.Platform.SDK.Environment.RemoveAllServers();
        }

        private void CreateConfig()
        {
            MilestoneEventConfig config = new MilestoneEventConfig();
            List<MilestoneEventCamera> cameras = new List<MilestoneEventCamera>();

            if (!float.TryParse(inactivityTxtBox.Text, out float hours))
                throw new FormatException($"Cannot parse default inactivity limit \"{inactivityTxtBox.Text}\"");
            config.MaxAllowedInactivityHours = hours;
            foreach (var cameraMenuItem in treeViewItems)
            {
                var cam = new MilestoneEventCamera();
                cam.CameraId = cameraMenuItem.Key;
                cam.CameraName = cameraMenuItem.Value.Title;
                cam.Events = new List<MilestoneEventCameraEvent>();
                foreach (var eventMenuItem in cameraMenuItem.Value.Items.Where(item => item.IsChecked))
                {
                    float? maxAllowedInactivity = null;
                    if (!string.IsNullOrEmpty(eventMenuItem.MaxAllowedInactivityHours))
                    {
                        if (!float.TryParse(eventMenuItem.MaxAllowedInactivityHours, out hours))
                            throw new FormatException($"Cannot parse \"{eventMenuItem.MaxAllowedInactivityHours}\" at {cameraMenuItem.Value.Title} -> {eventMenuItem.Title}");
                        maxAllowedInactivity = hours;
                    }

                    cam.Events.Add(new MilestoneEventCameraEvent()
                    {
                        Name = eventMenuItem.Title,
                        MaxAllowedInactivityHours = maxAllowedInactivity
                    });
                }
                if (cam.Events.Count > 0)
                    cameras.Add(cam);
            }

            config.CamerasAndEvents = cameras.ToArray();
            EventConfig = config;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateConfig();
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LogoutFromMilestone();
            Window.GetWindow(this).DialogResult = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            LogoutFromMilestone();
            Window.GetWindow(this).DialogResult = false;
        }

        private void addManualEventBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new Window()
            {
                Width = 300,
                Height = 200
            };
            var eventNameInput = new InputEventNameUserControl();
            dialogWindow.Content = eventNameInput;
            if (dialogWindow.ShowDialog() == true)
            {
                var eventName = eventNameInput.eventNameTxtBox.Text;
                var applyToAll = eventNameInput.applyToAllChkBox.IsChecked == true;

                foreach (CustomMenuItem camMenuItem in eventsTreeView.Items)
                {
                    var foundEvent = camMenuItem.Items.FirstOrDefault(eventMenuItem => eventMenuItem.Title.ToLower() == eventName.ToLower());
                    if (foundEvent == null)
                        camMenuItem.Items.Add(new CustomMenuItemEvent(camMenuItem, eventName, "") { IsChecked = applyToAll });
                    else if (applyToAll)
                        foundEvent.IsChecked = true;
                }
            }
        }

        protected List<Item> GetAllMilestoneItems(List<Item> items)
        {
            List<Item> result = new List<Item>();
            foreach (var item in items)
            {
                if (item.FQID.FolderType == FolderType.No)
                    result.Add(item);
                else
                    result.AddRange(GetAllMilestoneItems(item.GetChildren()));
            }
            return result;
        }

        private void inactivityTextBoxes_Validation(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            bool isValid = float.TryParse(textBox.Text, out _);
            textBox.Foreground = isValid ? Brushes.Black : Brushes.Red;
        }

        private void copyEventContext_Click(object sender, RoutedEventArgs e)
        {
            var eventName = ((CustomMenuItemEvent)((MenuItem)sender).DataContext).Title;
            try
            {
                Clipboard.SetText(eventName);
            }
            catch (COMException)
            {
                //This exception gets thrown on demo machine with Teamviewer,
                //but the clipboard is still updated successfully
            }
        }

        private void pasteEventContext_Click(object sender, RoutedEventArgs e)
        {
            var cameraName = ((CustomMenuItem)((MenuItem)sender).DataContext).Title;
            // We only have camera name here, which is used to look up treeview item
            // This assumes every camera name is unique
            var menuItem = treeViewItems.First(item => item.Value.Title == cameraName).Value;
            var eventName = (string)Clipboard.GetText();

            // Only add event if it has a non-empty name
            if (!string.IsNullOrWhiteSpace(eventName))
                AddEventToCamera(menuItem, eventName, true);
        }
    }

    public class CustomMenuItem : INotifyPropertyChanged
    {
        public CustomMenuItem(string title)
        {
            Title = title;
            this.Items = new ObservableCollection<CustomMenuItemEvent>();
        }

        public string Title { get; set; }

        private bool? _isChecked = false;
        public bool? IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    NotifyPropertyChanged();
                    if (IsChecked != null)
                    {
                        //IsChecked will be updated when children are updated, so save state first
                        bool currentState = (bool)IsChecked;
                        foreach (var item in Items)
                            item.IsChecked = currentState;
                    }
                }
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateCheckedState()
        {
            IsEnabled = true;
            if (!Items.Any(item => item.IsChecked)) // No items checked
                IsChecked = false;
            else if (Items.All(item => item.IsChecked)) // All items checked
                IsChecked = true;
            else // Some items checked
                IsChecked = null;
        }

        public ObservableCollection<CustomMenuItemEvent> Items { get; set; }
    }

    public class CustomMenuItemEvent : INotifyPropertyChanged
    {
        public CustomMenuItemEvent(CustomMenuItem parent, string title, string maxAllowedInactivity)
        {
            this._parent = parent;
            parent.UpdateCheckedState();
            this.Title = title;
            this.MaxAllowedInactivityHours = maxAllowedInactivity;
        }

        private readonly CustomMenuItem _parent;

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _maxAllowedInactivity;
        public string MaxAllowedInactivityHours
        {
            get => _maxAllowedInactivity;
            set
            {
                if (_maxAllowedInactivity != value)
                {
                    _maxAllowedInactivity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    NotifyPropertyChanged();
                    _parent.UpdateCheckedState();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
