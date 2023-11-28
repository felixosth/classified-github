using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Deployment.Application;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinservLite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string SQLCONNSTRING = "";

        public static string MyUser;

        bool sortWithDates = false;

        public static List<Job> JobBookmarks = new List<Job>();

        private bool AutoSearch
        {
            set
            {
                if(value)
                {
                    searchBox.TextChanged += searchBox_TextChanged;
                    searchBox.KeyDown -= searchBox_KeyDown;
                }
                else
                {
                    searchBox.TextChanged -= searchBox_TextChanged;
                    searchBox.KeyDown += searchBox_KeyDown;
                }
            }
        }

        NewJobChecker jobChecker;

        string currentDB = "wsdb";
        public static bool IsDebug = false;

        List<JobDescription> jobDescriptions = new List<JobDescription>();
        List<Job> jobList = new List<Job>();

        bool first = true;

        public List<Job> GetJobList() => jobList;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var args = Environment.GetCommandLineArgs();
                if (args.Length > 0)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        //MessageBox.Show(args[i]);
                        //if(args[i].StartsWith("wslite:"))
                        //{
                        //    Debugger.Launch();
                        //    RefreshAndOpenJob(args[i].Split(':')[1]);
                        //    return;
                        //}

                        if (args[i] == "-debug")
                        {
                            currentDB = "wsdb_dev2";
                            IsDebug = true;
                        }
                    }
                }
                //ThemeManager.AddWindow(this);

                try
                {
                    if (!Debugger.IsAttached)
                        Title += " [v" + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() + "]";
                }
                catch { }

                if (Properties.Settings.Default.WindowPosition.X != 0 && Properties.Settings.Default.WindowPosition.Y != 0)
                {
                    var scrPoint = Properties.Settings.Default.WindowPosition;
                    this.Left = scrPoint.X;
                    this.Top = scrPoint.Y;

                    this.Width = Properties.Settings.Default.WindowSize.X;
                    this.Height = Properties.Settings.Default.WindowSize.Y;
                }

                var winUsr = Environment.UserName;
                switch (winUsr)
                {
                    case "dennis.ahlzen":
                        MyUser = "DEAH";
                        break;
                    case "felix.osth":
                        MyUser = "FEOS";
                        break;
                    case "jens.lovgren":
                        MyUser = "JELO";
                        break;
                    case "kristoffer.heleander":
                        MyUser = "KRHE";
                        break;
                    case "linus.olsson": // dead
                        MyUser = "LIOL";
                        break;
                    case "markus.torgen":
                        MyUser = "MATO";
                        break;
                    case "viktor.lagebro":
                        MyUser = "VIKLA";
                        break;
                    case "niclas.nordberg":
                        MyUser = "NINO";
                        break;
                    case "fredrik.westin":
                        MyUser = "FRWE";
                        break;
                    default:
                        MyUser = "";
                        break;
                }

                if (MyUser == "")
                {
                    if (string.IsNullOrEmpty(Properties.Settings.Default.CustomUser))
                    {
                        MessageBox.Show("Please input your WinServ username manually.", "No WS user found");
                    }
                    else
                    {
                        usrBox.Text = Properties.Settings.Default.CustomUser;
                        MyUser = usrBox.Text;
                    }
                }
                else
                    usrBox.Text = MyUser;


                var dbAddr = Properties.Settings.Default.dbaddress;

                SQLCONNSTRING = $"Data Source={dbAddr}\\WINSERV,1436;Initial Catalog=" + currentDB + ";User ID=wslite;Password=wslite;";
                //SQLCONNSTRING = "Data Source=VCMBS01\\WINSERV,1436;Initial Catalog=" + currentDB + ";User ID=wslite;Password=wslite;";

                listOldChckBox.IsChecked = Properties.Settings.Default.ListArchivedOrders;
                listOldChckBox.Checked += listOldChckBox_Checked;
                listOldChckBox.Unchecked += ListOldChckBox_Unchecked;

                //try
                //{
                RefreshJobList();

                if (listOldChckBox.IsChecked == true)
                    RefreshJobList(true);

                try
                {
                    LoadJobBookmarks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load bookmarks\r\n" + ex.ToString(), "Bookmark error");
                }

                BookmarksListView.ItemsSource = JobBookmarks;

                var checkInterval = new TimeSpan(0, 10, 0);

                if (IsDebug)
                    checkInterval = new TimeSpan(0, 0, 10);



                //< MenuItem x: Name = "autoSearchContextMenuItem" Checked = "AutoSearchContextMenuItem_Checked" Unchecked = "AutoSearchContextMenuItem_Unchecked" IsCheckable = "True" Header = "AutoSearch" />

                var contexMenu = GetDefaultContextMenu(searchBox);

                MenuItem autoSearchContextMenuItem = new MenuItem();
                autoSearchContextMenuItem.IsCheckable = true;
                autoSearchContextMenuItem.Header = "AutoSearch";
                autoSearchContextMenuItem.Checked += AutoSearchContextMenuItem_Checked;
                autoSearchContextMenuItem.Unchecked += AutoSearchContextMenuItem_Unchecked;
                autoSearchContextMenuItem.IsChecked = Properties.Settings.Default.AutoSearch;

                contexMenu.Items.Add(autoSearchContextMenuItem);
                searchBox.ContextMenu = contexMenu;

                jobChecker = new NewJobChecker(this, checkInterval);
                jobChecker.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        bool stopAnim = false;
        public void NotifyNewJobs()
        {
            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(NotifyNewJobs);
            }
            else
            {
                stopAnim = false;

                var back = refreshBtn.Background;
                if (back.IsFrozen)
                    back = back.CloneCurrentValue();

                refreshBtn.Background = back;

                //refreshBtn.Background = Brushes.Orange;

                var dur = new Duration(new TimeSpan(0, 0, 0, 0, 666));
                ColorAnimation toRed = new ColorAnimation(Colors.Red, dur);
                ColorAnimation toOrange = new ColorAnimation(Colors.Orange, dur);

                toOrange.Completed += (s, e) =>
                {

                    if(stopAnim)
                        refreshBtn.ClearValue(Button.BackgroundProperty);
                    else
                        back.BeginAnimation(SolidColorBrush.ColorProperty, toRed);
                };

                toRed.Completed += (s, e) =>
                {
                    if(stopAnim)
                        refreshBtn.ClearValue(Button.BackgroundProperty);
                    else
                        back.BeginAnimation(SolidColorBrush.ColorProperty, toOrange);
                };

                back.BeginAnimation(SolidColorBrush.ColorProperty, toRed);

            }
        }

        private void UpdateChecker_OnUpdateComplete(object sender, UpdateCheckEventArgs e)
        {
            MessageBox.Show("A new update have been installed.\r\nRestart to access the new features & fixes." , "New Update: " + e.Version.ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateChecker_OnUpdateError(object sender, UpdateCheckEventArgs e)
        {
            MessageBox.Show(e.Error.ToString(), "Update Checker Error");
        }

        private void Shutdown()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => Shutdown()));
            }
            else
                Application.Current.Shutdown();
        }

        public Job GetJobFromID(int id)
        {
            foreach(var job in jobList)
            {
                if (job.JobID == id)
                    return job;
            }

            return null;
        }

        public static void LoadJobBookmarks()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.BookmarkList))
                return;
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.BookmarkList)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                JobBookmarks = (List<Job>)bf.Deserialize(ms);
            }
        }

        public static void SaveJobBookmarks()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, JobBookmarks);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.BookmarkList = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }

        public void RefreshJobList(bool archived = false, string specific = "", bool updateGui = true)
        {
            if(!archived)
            {
                jobDescriptions.Clear();
                jobList.Clear();
            }

            using (var sqlConn = new SqlConnection(SQLCONNSTRING))
            {
                sqlConn.Open();

                string query = "SELECT JOBBNR, TEXTRAD, RAD FROM dbo.JOBBFT";

                if (archived)
                    query += "H";

                if(specific != "")
                {
                    query += " WHERE JOBBNR=" + specific;
                }

                try
                {
                    SqlDataReader reader = new SqlCommand(query, sqlConn).ExecuteReader();

                    while (reader.Read())
                    {
                        var desc = reader.GetString(1);
                        jobDescriptions.Add(new JobDescription(reader.GetDecimal(0), desc, (int)reader.GetDecimal(2)));
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                query = "SELECT JOBBNR, KUNDNR, INSTALL, MNR, REF, TEL, EMAIL, TEKN, JOBBTYP, INDAT, GATA, ORT, PNR, STATUS FROM dbo.JOBB";
                if (archived)
                    query += "HIST";

                try
                {
                    SqlDataReader reader = new SqlCommand(query, sqlConn).ExecuteReader();

                    while (reader.Read())
                    {
                        var job = new Job(reader.GetDecimal(0), "",
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4),
                            reader.GetString(5),
                            reader.GetString(6),
                            reader.GetString(7),
                            archived,
                            reader.GetString(8),
                            reader.GetDateTime(9),
                            reader.GetString(13))
                        { Address = reader.GetString(10), City = reader.GetString(11), PostNumber = reader.GetString(12) };
                        jobList.Add(job);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                sqlConn.Close();
            }

            PairDescriptions();
            PairDescriptionsWithJobs();

            if (updateGui)
            {
                if (first)
                {
                    first = false;
                    AddObjectsToList(isFirst: true);
                }
                else
                    AddObjectsToList();
            }
        }

        private void AddObjectsToList(string searchTag = "", bool isFirst = false)
        {
            var visibleJobs = new ObservableCollection<Job>(jobList);
            jobListView.ItemsSource = visibleJobs;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(jobListView.ItemsSource);

            if (isFirst)
            {
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("SiteName");
                view.GroupDescriptions.Add(groupDescription);
                view.SortDescriptions.Add(new SortDescription("SiteName", ListSortDirection.Ascending));
            }

            FilterList(searchBox.Text, sortByDatePicker.SelectedDates);

            //countLabel.Text = jobListView.Items.Count + " items | Database: " + currentDB;
        }

        private bool UserFilter(object item)
        {
            var job = item as Job;

            if (searchBox.Text.ToLower() == "status:u")
                return job.Status == "U";
            else if (searchBox.Text.ToLower() == "status:a")
                return job.Status == "A";

            bool dateCondition = true;
            if(sortWithDates)
                if (sortByDatePicker.SelectedDates.Count > 1)
                    foreach(DateTime date in sortByDatePicker.SelectedDates)
                    {
                        if(date.Date == job.DateAdded)
                        {
                            dateCondition = true;
                            break;
                        }
                        dateCondition = false;
                    }
            else if(sortByDatePicker.SelectedDates.Count > 0)
                dateCondition = sortByDatePicker.SelectedDates[0].Date == job.DateAdded.Date;

            if (string.IsNullOrEmpty(searchBox.Text))
            {
                return dateCondition;
            }
            else
                return (job.CompleteJobDescription.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.SiteName.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.RefName.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.Technician.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.JobID.ToString().IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    

        private void PairDescriptions()
        {
            for (int i = 0; i < jobDescriptions.Count; i++)
            {
                //while (jobDescriptions[i].JobID == jobDescriptions[i + 1].JobID)

                if (i + 1 != jobDescriptions.Count)
                {
                    if (jobDescriptions[i + 1].Row == 1)
                    {
                        jobDescriptions[i].IsPrimary = true;
                        //jobDescriptions[i].JobText += "\r\n";
                    }
                    else if (!jobDescriptions[i].IsProcessed)
                    {
                        int wIndex = 1;

                        try
                        {

                            while (jobDescriptions[i + wIndex].Row != 1)
                            {
                                //if (jobDescriptions[i + wIndex].JobText == "")
                                //    jobDescriptions[i].CompleteJobText += "\r\n";
                                //else
                                jobDescriptions[i].CompleteJobText += /*" " +*/ jobDescriptions[i + wIndex].JobText /* + " "*/;

                                //if (jobDescriptions[i + wIndex + 1].Row != 1 && jobDescriptions[i + wIndex].JobText != "")
                                //    jobDescriptions[i].CompleteJobText += "\r\n";

                                jobDescriptions[i].Children.Add(jobDescriptions[i + wIndex]);
                                //jobDescriptions.RemoveAt(i + 1);
                                jobDescriptions[i + wIndex].IsProcessed = true;
                                if (i + wIndex == jobDescriptions.Count)
                                    break;

                                wIndex++;
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private void PairDescriptionsWithJobs()
        {
            for (int i = jobList.Count; i-- > 0;)
            {
                for (int x = jobDescriptions.Count; x-- > 0;)
                {
                    if (jobList[i].JobID == jobDescriptions[x].JobID && jobDescriptions[x].Row == 1)
                    {
                        jobList[i].CompleteJobDescription = jobDescriptions[x].CompleteJobText;
                        jobList[i].JobDescriptions.Add(jobDescriptions[x]);
                        //jobList[i].JobDescriptions.Add(jobDescriptions[x].Children);
                        foreach (JobDescription jobDesc in jobDescriptions[x].Children)
                        {
                            jobList[i].JobDescriptions.Add(jobDesc);
                        }
                        jobDescriptions.RemoveAt(x);
                        break;
                    }
                }
            }
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            //searchBox.Text = "";
            RefreshJobList();

            if (listOldChckBox.IsChecked == true)
                RefreshJobList(archived: true);


            stopAnim = true;
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = searchBox.Text.ToLower();
            new Thread(() => FilterList(filter, sortByDatePicker.SelectedDates)).Start();
        }

        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchBox_TextChanged(sender, null);
            }
        }

        private void FilterList(string filter, SelectedDatesCollection dates)
        {
            lock (jobListView)
            {
                var newVisibleJobs = new ObservableCollection<Job>();

                foreach (var job in jobList)
                {
                    bool filterByDates = dates.Count > 0;
                    bool foundByDate = false;
                    foreach (var date in dates)
                    {
                        if (date.Date == job.DateAdded)
                            foundByDate = true;
                    }

                    if (((job.CompleteJobDescription.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.SiteName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.RefName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.Technician.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.JobID.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)) && (filterByDates ? foundByDate : true))
                    {
                        newVisibleJobs.Add(job);
                    }



                }

                this.Dispatcher.Invoke(() =>
                {
                    jobListView.ItemsSource = newVisibleJobs;
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(jobListView.ItemsSource);
                    PropertyGroupDescription groupDescription = new PropertyGroupDescription("SiteName");
                    view.GroupDescriptions.Add(groupDescription);
                    view.SortDescriptions.Add(new SortDescription("SiteName", ListSortDirection.Ascending));
                    countLabel.Text = jobListView.Items.Count + " items | Database: " + currentDB;
                });
            }
        }

        private void jobListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = jobListView.SelectedItem as Job;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Text = "";
        }

        private void listOldChckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListArchivedOrders = (bool)listOldChckBox.IsChecked;
            Properties.Settings.Default.Save();

            RefreshJobList(archived:true);
        }

        private void ListOldChckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListArchivedOrders = (bool)listOldChckBox.IsChecked;
            Properties.Settings.Default.Save();

            RefreshJobList();
        }

        public void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var job = ((ListViewItem)sender).Content as Job;
            if (job == null)
                return;

            var dj = new DisplayJob(job, this);
            //dj.Owner = this;
            //ThemeManager.AddWindow(dj);
            try
            {
                e.Handled = true;
                dj.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //if(dj.ModifiedJob)
            //{
            //    RefreshJobList();
            //    if ((bool)listOldChckBox.IsChecked)
            //        RefreshJobList(archived: true);
            //}
        }

        private void addJobBtn_Click(object sender, RoutedEventArgs e)
        {
            var newJob = new AddJob();
            newJob.Owner = this;
            //ThemeManager.AddWindow(newJob);
            if(newJob.ShowDialog() == true)
            {
                RefreshJobList();
                jobChecker.SetJoblistCount(jobList.Count);

                if (listOldChckBox.IsChecked == true)
                    RefreshJobList(archived: true);

                foreach(Job job in jobList)
                {
                    if (job.JobID == newJob.NewJobID)
                    {
                        new DisplayJob(job, this)
                        {
                            Owner = this
                        }.Show();
                        break;
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CustomUser = usrBox.Text;
            Properties.Settings.Default.Save();
            MyUser = usrBox.Text;
        }

        private void closeCalendarBtn_Click(object sender, RoutedEventArgs e)
        {
            calendarBorder.Visibility = Visibility.Collapsed;
            sortWithDates = true;
            //CollectionViewSource.GetDefaultView(jobListView.ItemsSource).Refresh();
            FilterList(searchBox.Text, sortByDatePicker.SelectedDates);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            calendarBorder.Visibility = Visibility.Visible;
        }

        private void clearCalendarFilter_Click(object sender, RoutedEventArgs e)
        {
            sortWithDates = false;
            //CollectionViewSource.GetDefaultView(jobListView.ItemsSource).Refresh();
            sortByDatePicker.SelectedDates.Clear();
            FilterList(searchBox.Text, sortByDatePicker.SelectedDates);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            calendarBorder.Visibility = Visibility.Collapsed;
        }

        GetTimeReports timeReportGetter = null;
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if(timeReportGetter == null)
            {
                getTimeReportsButton.IsEnabled = false;
                timeReportGetter = new GetTimeReports(this);
                timeReportGetter.Closed += TimeReportGetter_Closed;
                //timeReportGetter.Owner = this;
                e.Handled = true;
                //ThemeManager.AddWindow(timeReportGetter);
                timeReportGetter.Show();
            }
        }

        private void TimeReportGetter_Closed(object sender, EventArgs e)
        {
            getTimeReportsButton.IsEnabled = true;
            timeReportGetter.Close();
            timeReportGetter = null;
        }

        public void bookmarkList_ItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            var clickedJob = ((ListViewItem)sender).Content as Job;
            if (clickedJob == null)
                return;
            bookmarksExpander.IsExpanded = false;
            bool found = false;
            foreach (Job job in jobList)
            {
                if (job.JobID == clickedJob.JobID)
                {
                    found = true;
                    new DisplayJob(job, this)
                    {
                        //Owner = this
                    }.Show();
                }
            }

            if (!found)
                if (MessageBox.Show("Job not found, it's probably archived or removed. Remove bookmark?", "Job not found", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    JobBookmarks.Remove(clickedJob);
                    SaveJobBookmarks();
                    CollectionViewSource.GetDefaultView(BookmarksListView.ItemsSource).Refresh();
                }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //updateChecker.Stop();
            jobChecker?.Stop();

            Properties.Settings.Default.WindowPosition = new Point(this.Left, this.Top);
            Properties.Settings.Default.WindowSize = new Point(this.Width, this.Height);
            Properties.Settings.Default.WindowState = this.WindowState;
            Properties.Settings.Default.Save();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.RightButton == MouseButtonState.Pressed)
            {
                Debugger.Launch();
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(Theme.Default);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(Theme.Dark);
        }

        private void themeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.AddedItems[0] as ListBoxItem;

            if ((string)selected.Content == "Default")
                ThemeManager.ChangeTheme(Theme.Default);
            else
                ThemeManager.ChangeTheme(Theme.Dark);
        }

        private void TimeReport_SearchButton(object sender, RoutedEventArgs e)
        {
            new TimeReportSearch(this).Show();
        }

        private void copyrightText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.RightButton == MouseButtonState.Pressed)
            {
                var path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(path));
            }
        }

        private void AutoSearchContextMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            AutoSearch = true;
            Properties.Settings.Default.AutoSearch = true;
        }

        private void AutoSearchContextMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            AutoSearch = false;
            Properties.Settings.Default.AutoSearch = false;
        }

        private ContextMenu GetDefaultContextMenu(TextBox txtBox)
        {
            var cm = new ContextMenu();

            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Undo";
            menuItem.Click += (s, e) => txtBox.Undo();
            cm.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Cut";
            menuItem.Click += (s, e) => txtBox.Cut();
            cm.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Copy";
            menuItem.Click += (s, e) => txtBox.Copy();
            cm.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Paste";
            menuItem.Click += (s, e) => txtBox.Paste();
            cm.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Select All";
            menuItem.Click += (s, e) => txtBox.SelectAll();
            cm.Items.Add(menuItem);

            return cm;
        }
    }

    [Serializable]
    public class JobDescription
    {
        public JobDescription(decimal id, string text, int row)
        {
            this.Row = row;
            this.JobID = (int)id;
            this.JobText = text;
            CompleteJobText = JobText;
        }

        public bool IsPrimary { get; set; }
        public bool IsProcessed { get; set; }
        public List<JobDescription> Children = new List<JobDescription>();

        public int Row { get; set; }
        public int JobID { get; set; }
        public string CompleteJobText { get; set; }
        public string JobText { get; set; }
    }

    public enum JobStatusType { Archived, Active }

    [Serializable]
    public class Job
    {
        public Job(decimal id, string desc, string cId, string siteName, string siteid, string refname, string reftel, string refemail, string tekn, bool archived, string jobtype,
            DateTime date, string status)
        {
            Status = status;
            JobID = (int)id;
            CompleteJobDescription = desc;
            CustomerID = cId;
            SiteName = siteName;
            SiteID = siteid;
            RefName = refname;
            RefPhoneNumber = reftel;
            RefEmailAddress = refemail;
            Technician = tekn;
            //IsArchived = archived;
            if (archived)
                JobStatus = JobStatusType.Archived;
            else
                JobStatus = JobStatusType.Active;
            JobType = jobtype;
            DateAdded = date;
        }

        public Job()
        {

        }
        public string Status { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostNumber { get; set; }

        public string Technician { get; set; }

        public JobStatusType JobStatus { get; set; }

        public int JobID { get; set; }

        public List<JobDescription> JobDescriptions = new List<JobDescription>();
        public string CompleteJobDescription { get; set; }

        public string CustomerID { get; set; }

        public string SiteName { get; set; }
        public string SiteID { get; set; }

        public string RefName { get; set; }
        public string RefPhoneNumber { get; set; }
        public string RefEmailAddress { get; set; }

        public string JobType { get; set; }

        public DateTime DateAdded { get; set; }
    }

    public class JobConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] parts = ((string)value).Split(new char[] { ',' });
                Job job = new Job(decimal.Parse(parts[0]), "", "", parts[1], "", "", "", "", "", false, "", DateTime.Parse(parts[2]), "");
                //room.RoomNumber = Convert.ToInt32(parts[0]);
                //room.Location = parts.Length > 1 ? parts[1] : null;
                return job;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                Job job = value as Job;
                return string.Format("{0},{1},{2}", job.JobID, job.SiteName, job.DateAdded);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
