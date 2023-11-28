using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WinServLib;
using WinServLite2.CloseableTab;

namespace WinServLite2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal const string CompanyName = "InSupport Nätverksvideo AB";
        internal const string Version = "2.7";

        internal static Dictionary<string, object> Settings { get; set; }

        public JobBrowser JobBrowser
        {
            get
            {
                return jobBrowser;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Settings = new Config(CompanyName).LoadConfig();

            WinServ.Initialize($"Data Source={Settings["server"]},{Settings["port"]};Initial Catalog={Settings["database"]};User ID={Settings["username"]};Password={Settings["password"]};");

            this.Title = $"WinServ Lite v{Version} [{Settings["user"]}]";

            jobBrowser.Init(this);

        }

        private void UpdateCheckThread()
        {
            var file = @"\\dc-01\InSupport_InstallationsService\WinServ Lite 2.0 Beta\WinServLite2.exe";


            var nextCheck = DateTime.Now;
            var myVer = new Version(Version);

            while (running)
            {
                if(DateTime.Now >= nextCheck)
                {
                    if(File.Exists(file))
                    {

                        FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(file);

                        var newVer = new Version(fileInfo.ProductVersion);

                        var comparison = newVer.CompareTo(myVer);

                        if(comparison > 0)
                        {
                            // update
                            if(MessageBox.Show("En ny uppdatering är tillgänglig. Uppdatera nu?", "Uppdatering", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                var dir = System.AppDomain.CurrentDomain.BaseDirectory;
                                var updateScript = System.IO.Path.Combine(dir, "update.bat");
                                if (File.Exists(updateScript))
                                    Process.Start(updateScript);
                                else
                                    MessageBox.Show("Kunde inte hitta uppdaterings-skript. Uppdatera manuellt.");
                            }
                            else
                            {
                                nextCheck.AddHours(12);
                            }
                        }
                    }

                    nextCheck = DateTime.Now.AddMinutes(15);
                }
                else
                {
                    Thread.Sleep(333);
                }
            }
        }

        public TabItem GetTabByTag(int tag)
        {
            foreach (TabItem tab in tabControl.Items)
            {
                if (tab.Tag == null || tab.Tag is int == false)
                    continue;

                if ((int)tab.Tag == tag)
                {
                    return tab;
                }
            }
            return null;
        }

        public void AddTab(TabItem tabItem)
        {
            tabControl.Items.Add(tabItem);
            tabItem.Focus();
        }

        public void OpenTab(DynamicUserControl content, int tag, string title, string tooltip = null, Action afterAdd = null)
        {
            var existingTab = GetTabByTag(tag);
            if (existingTab != null)
            {
                existingTab.Focus();
                return;
            }

            foreach (Window window in Application.Current.Windows)
            {
                if (window is ContainerWindow)
                {
                    if (window.Tag != null && window.Tag is int && (int)window.Tag == tag)
                    {
                        window.Focus();
                        return;
                    }
                }
            }

            CloseableTab.CloseableTab closeableTab = new CloseableTab.CloseableTab()
            {
                Tag = tag,
                Title = title,
                //Content = content,
                ToolTip = tooltip
            };
            closeableTab.SetContent(content);

            afterAdd?.Invoke();

            tabControl.Items.Add(closeableTab);
            closeableTab.Focus();
        }

        
        public List<T> GetUserControls<T>()
        {
            List<T> userControls = new List<T>();

            foreach(TabItem tab in tabControl.Items)
            {
                if(tab.Content is T)
                {
                    userControls.Add((T)tab.Content);
                }
            }


            foreach (Window window in Application.Current.Windows)
            {
                if (window is ContainerWindow)
                {
                    if(window.Content is T)
                    {
                        userControls.Add((T)window.Content);
                    }
                }
            }
            return userControls;
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var onlyLCtrl = e.KeyboardDevice.Modifiers == ModifierKeys.Control;
            var shiftAndCtrl = (e.KeyboardDevice.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == (ModifierKeys.Shift | ModifierKeys.Control);

            if(e.Key == Key.W && onlyLCtrl)
            {
                if(tabControl.SelectedItem is CloseableTab.CloseableTab)
                {
                    tabControl.Items.Remove(tabControl.SelectedItem);
                    e.Handled = true;
                }
            }
            else if (((int)e.Key >= 35 && (int)e.Key <= 43) && onlyLCtrl)
            {
                var targetIndex = (int)e.Key - 35;
                if (tabControl.Items.Count - 1 >= targetIndex)
                {
                    tabControl.SelectedIndex = targetIndex;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Tab && shiftAndCtrl)
            {
                if (tabControl.Items[tabControl.Items.Count - 1] != tabControl.Items[0])
                    ((TabItem)tabControl.Items[tabControl.SelectedIndex - 1]).Focus();
                else
                    ((TabItem)tabControl.Items[tabControl.Items.Count - 1]).Focus();

                e.Handled = true;
            }
            else if (e.Key == Key.Tab && onlyLCtrl)
            {
                var i = ++tabControl.SelectedIndex % tabControl.Items.Count;
                tabControl.SelectedIndex = ++tabControl.SelectedIndex % tabControl.Items.Count;

                //if (tabControl.Items[tabControl.Items.Count - 1] != tabControl.SelectedItem)
                //    ((TabItem)tabControl.Items[tabControl.SelectedIndex + 1]).Focus();
                //else
                //    ((TabItem)tabControl.Items[0]).Focus();

                e.Handled = true;
            }
        }

        bool running = true;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            running = false;
            jobBrowser.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(UpdateCheckThread).Start();
        }
    }
}
