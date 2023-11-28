using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using WinServLite2.CloseableTab;
using WinServLib;
using WinServLib.Jobs;
using WinServLib.Objects;

namespace WinServLite2.Jobs
{
    /// <summary>
    /// Interaction logic for TimeReportSearch.xaml
    /// </summary>
    public partial class TimeReportSearch : DynamicUserControl
    {
        MainWindow mw;

        public TimeReportSearch(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var reportResults = WinServ.JobManager.SearchTimeReports(searchTxtBox.Text).OrderByDescending(tr => tr.Date);

            timeReportsListView.ItemsSource = null;
            timeReportsListView.ItemsSource = reportResults;
        }

        private void searchTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && !string.IsNullOrWhiteSpace(searchTxtBox.Text))
            {
                e.Handled = true;
                SearchButton_Click(sender, new RoutedEventArgs());
            }
        }

        private void ReportViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var report = ((ListViewItem)sender).Content as TimeReport;

            var job = WinServLib.WinServ.JobManager.GetJob(report.JobID, report.ArchivedJob);

            mw.JobBrowser.OpenJob(job);

            //var job = GetJobById(report.JobID, report.ArchivedJob);
            //if(job != null)
            //    new DisplayJob(job, mw).ShowDialog();
        }

        //Job GetJobById(int jobid, bool archived)
        //{
        //    string jobsTable = archived ? "JOBBHIST" : "JOBB";
        //    string jobFtsTable = archived ? "JOBBFTH" : "JOBBFT";

        //    using (SqlConnection sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
        //    {
        //        sqlConn.Open();

        //        List<JobDescription> jobDescriptionRows = new List<JobDescription>();
        //        using (var sqlCmd = new SqlCommand("SELECT TEXTRAD, RAD FROM dbo." + jobFtsTable + " WHERE JOBBNR=@jobid;", sqlConn))
        //        {
        //            sqlCmd.Parameters.AddWithValue("jobid", (decimal)jobid);
        //            using (var reader = sqlCmd.ExecuteReader())
        //            {
        //                while(reader.Read())
        //                {
        //                    jobDescriptionRows.Add(new JobDescription(jobid, reader["TEXTRAD"] as string, (int)(decimal)reader["RAD"]));
        //                }
        //            }
        //        }

        //        using (var sqlCommand = new SqlCommand($"SELECT JOBBNR, KUNDNR, INSTALL, MNR, REF, TEL, EMAIL, TEKN, JOBBTYP, INDAT, GATA, ORT, PNR, STATUS FROM dbo.{jobsTable} WHERE JOBBNR=@jobid", sqlConn))
        //        {
        //            sqlCommand.Parameters.AddWithValue("@jobid", (decimal)jobid);
        //            SqlDataReader reader = sqlCommand.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                var job = new Job(reader.GetDecimal(0), "",
        //                    reader.GetString(1),
        //                    reader.GetString(2),
        //                    reader.GetString(3),
        //                    reader.GetString(4),
        //                    reader.GetString(5),
        //                    reader.GetString(6),
        //                    reader.GetString(7),
        //                    archived,
        //                    reader.GetString(8),
        //                    reader.GetDateTime(9),
        //                    reader.GetString(13))
        //                { Address = reader.GetString(10), City = reader.GetString(11), PostNumber = reader.GetString(12), JobDescriptions = jobDescriptionRows };

        //                return job;
        //            }
        //        }
        //    }
        //    return null;

        //}
    }
}
