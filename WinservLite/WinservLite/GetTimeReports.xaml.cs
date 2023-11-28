using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace WinservLite
{
    /// <summary>
    /// Interaction logic for GetTimeReports.xaml
    /// </summary>
    public partial class GetTimeReports : Window
    {
        List<TimeReport> timeReports = new List<TimeReport>();

        bool refresh = false;

        MainWindow mainWindow;

        public GetTimeReports(MainWindow window)
        {
            InitializeComponent();
            this.mainWindow = window;
            techBox.Items.Clear();
            //techBox.Items = SQLFunctions.GetTechnicians(MainWindow.SQLCONNSTRING);
            foreach(Technician tech in SQLFunctions.GetTechnicians(MainWindow.SQLCONNSTRING))
            {
                techBox.Items.Add(tech);
                if (tech.UserName == MainWindow.MyUser)
                    techBox.SelectedItem = tech;
            }

            //foreach (Technician tech in techBox.Items)
            //{
            //    if (item.Content as string == MainWindow.MyUser)
            //        techBox.SelectedItem = item;
            //}

            calendar.SelectedDate = DateTime.Now;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();

                string query = string.Format("SELECT TEKN, JOBBNR, STARTDAT, ARBTID, RESTID, KOMMENTAR, TRAKTAMENTE FROM dbo.JOBBTEKN WHERE TEKN='{0}'",
                    (techBox.SelectedItem as Technician).UserName);
                for (int i = 0; i < 2; i++)
                {

                    using (var reader = new SqlCommand(query, sqlConn).ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bool trakt = false;
                            int boolean = reader.GetOrdinal("TRAKTAMENTE");
                            if (!reader.IsDBNull(boolean))
                                trakt = reader.GetBoolean(boolean);
                            //bool trakt = reader.IsDBNull(6) ? false : reader.GetBoolean(6);
                            timeReports.Add(new TimeReport()
                            {
                                Technician = reader.GetString(0),
                                JobID = (int)reader.GetDecimal(1),
                                StartDate = reader.GetDateTime(2),
                                WorkTime = (float)reader.GetDecimal(3),
                                TravelTime = (float)reader.GetDecimal(4),
                                Comment = reader.GetString(5),
                                Traktamente = trakt
                            });
                        }
                        reader.Close();
                    }

                    query = query.Replace("dbo.JOBBTEKN", "dbo.TEKNJOBB");
                    //query = string.Format("SELECT TEKN, JOBBNR, STARTDAT, ARBTID, RESTID FROM dbo.TEKNJOBB WHERE TEKN='{0}'",
                    //    (techBox.SelectedItem as ListBoxItem).Content as string);
                }


                TimeReport[] tempTR = new TimeReport[timeReports.Count];
                timeReports.CopyTo(tempTR);

                float arbtid = 0, restid = 0;

                foreach (var timeReport in tempTR)
                {
                    bool found = false;
                    foreach (DateTime selectedDate in calendar.SelectedDates)
                    {
                        if (timeReport.StartDate.Date == selectedDate.Date)
                        {
                            found = true;
                            arbtid += timeReport.WorkTime;
                            restid += timeReport.TravelTime;

                            using(var reader = new SqlCommand("SELECT INSTALL FROM dbo.JOBB WHERE JOBBNR='" + (double)timeReport.JobID + "'", sqlConn).ExecuteReader())
                            {
                                while(reader.Read())
                                {
                                    timeReport.SiteName = reader.GetString(0);
                                }
                                reader.Close();
                            }
                            if(string.IsNullOrEmpty(timeReport.SiteName))
                            {
                                using (var reader = new SqlCommand("SELECT INSTALL FROM dbo.JOBBHIST WHERE JOBBNR='" + (double)timeReport.JobID + "'", sqlConn).ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        timeReport.SiteName = reader.GetString(0);
                                    }
                                    reader.Close();
                                }
                            }
                            break;
                        }
                    }
                    if (!found)
                        timeReports.Remove(timeReport);
                }
                sqlConn.Close();

                totalArbLabel.Content = "Arbtid: " + arbtid + "h";
                totalResLabel.Content = "Restid: " + restid + "h";
            }


            timeTableView.ItemsSource = timeReports;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(timeTableView.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("StartDate", ListSortDirection.Ascending));


            if (refresh)
                view.GroupDescriptions.Clear();
            else
                refresh = true;

            //PropertyGroupDescription groupDescription = new PropertyGroupDescription("SiteName");

            PropertyGroupDescription groupDescription = new PropertyGroupDescription("StartDate");

            view.GroupDescriptions.Add(groupDescription);
            //view.SortDescriptions.Add(new SortDescription("SiteName", ListSortDirection.Ascending));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
                //DialogResult = true;
        }

        private void timeTableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var report = ((ListViewItem)sender).Content as TimeReport;

            //var dj = new DisplayJob()
            var job = mainWindow.GetJobFromID(report.JobID);

            if(job != null)
            {
                e.Handled = true;
                var dj = new DisplayJob(job, mainWindow);
                dj.Show();
                //dj.BringIntoView();
                //dj.Focus();
            }
        }

        private void calendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ThemeManager.RemoveWindow(this);
        }
    }

    public class GroupHoursConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return "null";

            ReadOnlyObservableCollection<object> items =
                  (ReadOnlyObservableCollection<object>)value;

            var hours = (from i in items
                         select (((TimeReport)i).WorkTime + ((TimeReport)i).TravelTime)).Sum();

            return "Total Time: " + hours.ToString() + "h";
        }

        public object ConvertBack(object value, System.Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
