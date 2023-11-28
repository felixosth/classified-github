using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinServLib.Objects;
using WinServLite2.CloseableTab;

namespace WinServLite2.Jobs
{
    /// <summary>
    /// Interaction logic for GetTimeReports.xaml
    /// </summary>
    public partial class GetTimeReports : DynamicUserControl
    {
        List<TimeReport> timeReports = new List<TimeReport>();

        MainWindow mw;

        public GetTimeReports(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();

            foreach (Technician tech in WinServLib.WinServ.GetTechnicians())
            {
                techBox.Items.Add(tech);
                if (tech.UserName == (string)MainWindow.Settings["user"])
                    techBox.SelectedItem = tech;
            }

            calendar.SelectedDate = DateTime.Now;
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            timeReports.Clear();

            if (techBox.SelectedItem == null)
            {
                MessageBox.Show("Select a technician", "Error");
                return;
            }
            
            if (calendar.SelectedDates.Count < 1)
            {
                MessageBox.Show("Select 1 date or more", "Error");
                return;
            }

            var dates = calendar.SelectedDates.OrderBy(d => d.Date);
            timeReports = WinServLib.WinServ.JobManager.GetTimeReportsReport((techBox.SelectedItem as Technician).UserName, dates.First().Date, dates.Last().Date);

            timeTableView.ItemsSource = timeReports;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(timeTableView.ItemsSource);
            view.SortDescriptions.Clear();
            view.GroupDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription("SiteName", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("UniqueID", ListSortDirection.Ascending));
            view.GroupDescriptions.Add(new PropertyGroupDescription("DateOnly"));

            view.Refresh();

            float arbtid = 0, restid = 0;

            foreach(var timeReport in timeReports)
            {
                arbtid += timeReport.WorkTime;
                restid += timeReport.TravelTime;
            }

            totalArbLabel.Content = "Arbtid: " + arbtid + "h";
            totalResLabel.Content = "Restid: " + restid + "h";
        }

        private void timeTableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var report = ((ListViewItem)sender).Content as TimeReport;

            var job = WinServLib.WinServ.JobManager.GetJob(report.JobID, report.ArchivedJob);

            mw.JobBrowser.OpenJob(job);
        }

        private void calendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }
    }
}
