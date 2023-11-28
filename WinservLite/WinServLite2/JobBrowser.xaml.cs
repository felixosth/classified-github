using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
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
using WinServLib.Objects;
using WinServLite2.CloseableTab;
using WinServLite2.Jobs;

namespace WinServLite2
{
    /// <summary>
    /// Interaction logic for JobBrowser.xaml
    /// </summary>
    public partial class JobBrowser : UserControl
    {
        List<Job> jobList = new List<Job>();
        MainWindow mw;
        JobChecker jobChecker;

        public List<Job> JobList => jobList;

        List<string> statusFilterList = new List<string>();

        public JobBrowser()
        {
            InitializeComponent();
        }

        public void Init(MainWindow mw)
        {
            this.mw = mw;
            FocusManager.SetIsFocusScope(this, true);

            if ((bool)MainWindow.Settings["autosearch"] == true)
            {
                searchBox.TextChanged += searchBox_TextChanged;
                searchBox.KeyDown -= searchBox_KeyDown;
            }
            else
            {
                searchBox.TextChanged -= searchBox_TextChanged;
                searchBox.KeyDown += searchBox_KeyDown;
            }

            jobStatusesListBox.ItemsSource = WinServ.JobStatus.OrderBy(s => s.Name);

            RefreshJobList();

            jobChecker = new JobChecker(this);
            jobChecker.OnJobsModified += JobChecker_OnJobsModified;
            jobChecker.OnNewJobsFound += JobChecker_OnNewJobsFound;
            jobChecker.OnError += (s, e) =>
            {
                MessageBox.Show(e.ToString(), "JobChecker");
            };
        }

        private void JobChecker_OnNewJobsFound(object sender, IEnumerable<Job> e)
        {
            jobList.AddRange(e);
            RebuildList();
        }

        private void JobChecker_OnJobsModified(object sender, IEnumerable<Job> e)
        {
            for (int i = 0; i < jobList.Count; i++)
            {
                foreach(var newJob in e)
                {
                    var job = JobList[i];
                    if(job.JobID == newJob.JobID)
                    {
                        var refreshedJob = job.RefreshFromDB();
                        jobList[i] = refreshedJob;
                        continue;
                    }
                }
            }

            RebuildList();
        }

        private void RefreshJobList(bool archivedJobs = false)
        {
            jobList = WinServ.JobManager.GetActiveJobs();

            if(archivedJobs)
                jobList.AddRange(WinServ.JobManager.GetArchivedJobs());

            jobListView.ItemsSource = jobList;
            RebuildList();
        }

        private void RebuildList()
        {
            if (!Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new Action(() => { RebuildList(); }));
            }
            else
            {
                //jobListView.ItemsSource = jobList;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(jobListView.ItemsSource);

                view.GroupDescriptions.Clear();
                view.SortDescriptions.Clear();

                view.GroupDescriptions.Add(new PropertyGroupDescription("SiteName"));
                view.SortDescriptions.Add(new SortDescription("SiteName", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("JobID", ListSortDirection.Descending));
                view.Filter = UserFilter;
            }
        }

        private void SortBy(string sort, ListSortDirection direction)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(jobListView.ItemsSource);

            view.GroupDescriptions.Clear();
            view.SortDescriptions.Clear();

            view.SortDescriptions.Add(new SortDescription(sort, direction));
            view.Filter = UserFilter;
            view.Refresh();
        }

        private bool UserFilter(object item)
        {
            var job = item as Job;

            bool dateCondition = false;
            foreach (DateTime date in sortByDatePicker.SelectedDates)
            {
                if (date.Date == job.DateAdded)
                {
                    dateCondition = true;
                    break;
                }
            }
            
            if(sortByDatePicker.SelectedDates.Count > 0 && !string.IsNullOrEmpty(searchBox.Text))
            {
                return ((job.CompleteJobDescription.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (job.SiteName.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (job.RefName.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (job.Technician.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (job.JobID.ToString().IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)) && dateCondition &&
                        (statusFilterList.Count > 0 ? statusFilterList.Contains(job.Status) : true);
            }
            else if (sortByDatePicker.SelectedDates.Count > 0)
            {
                return dateCondition;
            }
            else
                return ((job.CompleteJobDescription.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.SiteName.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.RefName.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.Technician.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (job.JobID.ToString().IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                    (statusFilterList.Count > 0 ? statusFilterList.Contains(job.Status) : true);
        }

        private void clearSearchBtn_Click(object s, RoutedEventArgs e)
        {
            searchBox.Text = "";
            if ((bool)MainWindow.Settings["autosearch"] == false)
                searchBox_TextChanged(s, null);
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(jobListView.ItemsSource).Refresh();
        }

        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchBox_TextChanged(sender, null);
            }
        }

        private void ListOldChckBox_Checked(object sender, RoutedEventArgs e)
        {
            RefreshJobList(archivedJobs: true);
        }

        private void ListOldChckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RefreshJobList();
        }

        public void OpenJob(Job job)
        {
            string tooltip = null;
            string title = "[#" + job.JobID + "] " + job.SiteName;
            if (title.Length > 45)
            {
                tooltip = title;
                title = title.Remove(42, title.Length - 42);
                title += "...";
            }

            JobViewer jobViewer = new JobViewer();
            jobViewer.OnJobChanged += JobViewer_OnJobChanged;
            jobViewer.OnJobIDChanged += JobViewer_OnJobIDChanged;
            
            mw.OpenTab(jobViewer, job.JobID, title, tooltip, () =>
            {
                jobViewer.Init(job);
            });
        }

        private void JobViewer_OnJobIDChanged(object sender, int e)
        {
            var openUserControls = mw.GetUserControls<JobViewer>();

            foreach(var userControl in openUserControls)
            {
                if(userControl.OpenJobID == e)
                {
                    userControl.RefreshList();
                }
            }
        }

        private void JobViewer_OnJobChanged(object sender, Job e)
        {
            for (int i = 0; i < jobList.Count; i++)
            {
                if(jobList[i].JobID == e.JobID)
                {
                    jobList[i] = jobList[i].RefreshFromDB();
                    break;
                }
            }

            CollectionViewSource.GetDefaultView(jobListView.ItemsSource).Refresh();
        }


        public void AddJobToList(Job job)
        {
            jobList.Add(job);
            CollectionViewSource.GetDefaultView(jobListView.ItemsSource).Refresh();
        }

        public void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var job = ((ListViewItem)sender).Content as Job;
            if (job == null)
                return;

            e.Handled = true;
            OpenJob(job);
        }

        private void OpenCalendarBtn_Click(object sender, RoutedEventArgs e)
        {
            calendarBorder.Visibility = Visibility.Visible;
        }

        private void ApplyCalendarBtn_Click(object sender, RoutedEventArgs e)
        {
            calendarBorder.Visibility = Visibility.Collapsed;
            CollectionViewSource.GetDefaultView(jobListView.ItemsSource).Refresh();
        }

        private void ClearCalendarFilter_Click(object sender, RoutedEventArgs e)
        {
            sortByDatePicker.SelectedDates.Clear();
            CollectionViewSource.GetDefaultView(jobListView.ItemsSource).Refresh();
        }

        private void CloseCalendarBtn_Click(object sender, RoutedEventArgs e)
        {
            calendarBorder.Visibility = Visibility.Collapsed;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        public void RefreshList()
        {
            RefreshJobList(archivedJobs: listOldChckBox.IsChecked == true);
        }

        private void ListViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key != Key.Enter)
            {
                return;
            }

            var job = ((ListViewItem)sender).Content as Job;
            if (job == null)
                return;

            e.Handled = true;
            OpenJob(job);
        }

        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private void SortByColumns(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);

            if(listViewSortAdorner != null && listViewSortAdorner.Direction == ListSortDirection.Descending && listViewSortCol == column)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                listViewSortAdorner = null;
                listViewSortCol = null;

                RebuildList();
                return;
            }

            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);

            SortBy(column.Tag.ToString(), newDir);
        }

        private void AddJobBtn_Click(object sender, RoutedEventArgs e)
        {
            var newJob = new AddJob(this);
            mw.OpenTab(newJob, -1234112, "Lägg till ärende");
        }

        public void Close()
        {
            jobChecker.Stop();
        }

        private void JobStatusCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var status = (Status)checkBox.Tag;
            if (checkBox.IsChecked == true)
                statusFilterList.Add(status.Nr);
            else
                statusFilterList.Remove(status.Nr);

            CollectionViewSource.GetDefaultView(jobListView.ItemsSource).Refresh();
        }

        private void JobStatusFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if(jobStatusFilterBorder.Visibility == Visibility.Collapsed)
            {
                jobStatusFilterBorder.Visibility = Visibility.Visible;
                jobStatusesListBox.Focus();
                jobStatusesListBox.LostFocus += JobStatusFilterBorder_LostFocus;
            }
            else
            {
                jobStatusFilterBorder.Visibility = Visibility.Collapsed;
                jobStatusesListBox.LostFocus -= JobStatusFilterBorder_LostFocus;
            }
            //jobStatusFilterBorder.Visibility = jobStatusFilterBorder.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void JobStatusFilterBorder_LostFocus(object sender, RoutedEventArgs e)
        {
            Status status = default;
            var focused = FocusManager.GetFocusedElement(this);
            if (focused == jobStatusFilterButton)
                return;

            if(focused is CheckBox)
            {
                status = (Status)(focused as CheckBox).Tag;
            }
            //else if(focused is ListBoxItem)
            //{
            //    try
            //    {
            //        kvp = (KeyValuePair<string, string>)(focused as ListBoxItem).DataContext;
            //    }
            //    catch { }
            //}

            if (!jobStatusesListBox.Items.Contains(status))
                JobStatusFilterButton_Click(sender, e);
        }

        private void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dir = System.AppDomain.CurrentDomain.BaseDirectory;
            var updateScript = System.IO.Path.Combine(dir, "update.bat");
            if (File.Exists(updateScript))
                Process.Start(updateScript);
            else
                MessageBox.Show("Kunde inte hitta uppdaterings-skript. Uppdatera manuellt.");
        }

        private void AppdataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var path = new Config(MainWindow.CompanyName).ConfigPath;
            if (File.Exists(path))
                Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(path));
        }

        private void ShowTimeReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            GetTimeReports getTimeReports = new GetTimeReports(mw);
            mw.OpenTab(getTimeReports, -23123, "Tidsrapporter");
        }

        private void searchTimeReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            TimeReportSearch timeReportSearch = new TimeReportSearch(mw);
            mw.OpenTab(timeReportSearch, -95219, "Tidsrapportssökning");
        }
    }

    public class SortAdorner : Adorner
    {
        private static Geometry ascGeometry =
                Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

        private static Geometry descGeometry =
                Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        public ListSortDirection Direction { get; private set; }

        public SortAdorner(UIElement element, ListSortDirection dir)
                : base(element)
        {
            this.Direction = dir;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            TranslateTransform transform = new TranslateTransform
                    (
                            AdornedElement.RenderSize.Width - 15,
                            (AdornedElement.RenderSize.Height - 5) / 2
                    );
            drawingContext.PushTransform(transform);

            Geometry geometry = ascGeometry;
            if (this.Direction == ListSortDirection.Descending)
                geometry = descGeometry;
            drawingContext.DrawGeometry(Brushes.Black, null, geometry);

            drawingContext.Pop();
        }
    }
}
