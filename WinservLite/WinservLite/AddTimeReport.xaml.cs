using System;
using System.Collections.Generic;
using System.Data;
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

namespace WinservLite
{
    /// <summary>
    /// Interaction logic for AddTimeReport.xaml
    /// </summary>
    public partial class AddTimeReport : Window
    {
        readonly TimeReportEditType editType;

        int jobID;
        int recNr;

        public AddTimeReport(Job job)
        {
            editType = TimeReportEditType.AddNew;
            jobID = job.JobID;
            InitializeComponent();
            //foreach (ListBoxItem item in techBox.Items)
            //{
            //    if (item.Content as string == MainWindow.MyUser)
            //        techBox.SelectedItem = item;
            //}
            foreach (ListBoxItem item in jobType.Items)
            {
                if ((item.Content as string).StartsWith(job.JobType))
                {
                    jobType.SelectedItem = item;
                }
            }
            techBox.Items.Clear();
            foreach (Technician tech in SQLFunctions.GetTechnicians(MainWindow.SQLCONNSTRING))
            {
                techBox.Items.Add(tech);
                if (tech.UserName == MainWindow.MyUser)
                    techBox.SelectedItem = tech;
            }

            datePicker.SelectedDate = DateTime.Now;
            maxCharsDefaultBrush = maxCharsLabel.Foreground;
        }

        public AddTimeReport(TimeReport report)
        {
            InitializeComponent();
            editType = TimeReportEditType.Edit;
            recNr = report.UniqueID;

            techBox.Items.Clear();
            foreach (Technician tech in SQLFunctions.GetTechnicians(MainWindow.SQLCONNSTRING))
            {
                techBox.Items.Add(tech);
                if (tech.UserName == report.Technician)
                    techBox.SelectedItem = tech;
            }

            //foreach (ListBoxItem item in techBox.Items)
            //{
            //    if (item.Content as string == report.Technician)
            //        techBox.SelectedItem = item;
            //}
            foreach (ListBoxItem item in jobType.Items)
            {
                if ((item.Content as string).StartsWith(report.JobType))
                {
                    jobType.SelectedItem = item;
                }
            }

            datePicker.SelectedDate = report.StartDate;
            maxCharsDefaultBrush = maxCharsLabel.Foreground;

            traktChkBox.IsChecked = report.Traktamente;
            commentBox.Text = report.Comment;
            workBox.Text = report.WorkTime.ToString();
            travelBox.Text = report.TravelTime.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double workTime = 0, travelTime = 0;
            if(workBox.Text == "" || workBox.Text == "0")
            {
                MessageBox.Show("Please specify amount of worktime", "Error");
                return;
            }
            if (!double.TryParse(workBox.Text, out workTime))
            {
                MessageBox.Show("Unable to parse Arbtid", "Error");
                return;
            }

            if (travelBox.Text == "")
                travelTime = 0;
            else if(!double.TryParse(travelBox.Text, out travelTime))
            {
                MessageBox.Show("Unable to parse Restid", "Error");
                return;
            }
            if(techBox.SelectedIndex < 0)
            {
                MessageBox.Show("Select a technician", "Error");
                return;
            }

            try
            {
                using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
                {
                    sqlConn.Open();

                    if (editType == TimeReportEditType.AddNew)
                    {

                        string query = "INSERT into dbo.TEKNJOBB (TEKN, JOBBNR, JOBBTYP, PLANDAT, DELAYCODE, STARTDAT, SLUTDAT, ARBTID, RESTID, KOMMENTAR, TRAKTAMENTE) VALUES " +
                            "(@tekn, @jobbnr, @jobbtyp, @plandat, @delaycode, @startdat, @slutdat, @arbtid, @restid, @kommentar, @trakt)";
                        using (var cmd = new SqlCommand(query))
                        {
                            cmd.Connection = sqlConn;
                            cmd.Parameters.Add("@tekn", SqlDbType.NVarChar).Value = (techBox.SelectedItem as Technician).UserName;
                            cmd.Parameters.Add("@jobbnr", SqlDbType.Decimal).Value = (double)jobID;
                            cmd.Parameters.Add("@plandat", SqlDbType.DateTime).Value = datePicker.SelectedDate.ToString();
                            cmd.Parameters.Add("@delaycode", SqlDbType.NVarChar).Value = (jobType.SelectedIndex + 1).ToString();
                            cmd.Parameters.Add("@startdat", SqlDbType.DateTime).Value = datePicker.SelectedDate.ToString();
                            cmd.Parameters.Add("@slutdat", SqlDbType.DateTime).Value = datePicker.SelectedDate.ToString();
                            //var jt = (jobType.SelectedItem as ListBoxItem).Content as string;
                            cmd.Parameters.Add("@jobbtyp", SqlDbType.NVarChar).Value = (jobType.SelectedItem as ListBoxItem).Content.ToString()[0];
                            cmd.Parameters.Add("@arbtid", SqlDbType.Decimal).Value = workTime;
                            cmd.Parameters.Add("@restid", SqlDbType.Decimal).Value = travelTime;
                            cmd.Parameters.Add("@kommentar", SqlDbType.NVarChar).Value = commentBox.Text;
                            cmd.Parameters.Add("@trakt", SqlDbType.Bit).Value = traktChkBox.IsChecked;

                            int res = cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string query = "UPDATE dbo.TEKNJOBB SET TEKN=@tekn, JOBBTYP=@jobbtyp, PLANDAT=@plandat, DELAYCODE=@delaycode, STARTDAT=@plandat, " +
                            "SLUTDAT=@plandat, ARBTID=@arbtid, RESTID=@restid, KOMMENTAR=@kommentar, TRAKTAMENTE=@trakt WHERE RECNUM=@recnum";
                            //"(@tekn, @jobbnr, @jobbtyp, @plandat, @delaycode, @startdat, @slutdat, @arbtid, @restid, @kommentar) WHERE RECNUM='" + recNr + "'";
                        using (var cmd = new SqlCommand(query))
                        {
                            cmd.Connection = sqlConn;
                            cmd.Parameters.Add("@tekn", SqlDbType.NVarChar).Value = (techBox.SelectedItem as Technician).UserName;
                            //cmd.Parameters.Add("@jobbnr", SqlDbType.Decimal).Value = (double)jobID;
                            cmd.Parameters.Add("@plandat", SqlDbType.DateTime).Value = datePicker.SelectedDate.ToString();
                            cmd.Parameters.Add("@delaycode", SqlDbType.NVarChar).Value = (jobType.SelectedIndex + 1).ToString();
                            //cmd.Parameters.Add("@startdat", SqlDbType.DateTime).Value = datePicker.SelectedDate.ToString();
                            //cmd.Parameters.Add("@slutdat", SqlDbType.DateTime).Value = datePicker.SelectedDate.ToString();
                            //var jt = (jobType.SelectedItem as ListBoxItem).Content as string;
                            cmd.Parameters.Add("@jobbtyp", SqlDbType.NVarChar).Value = (jobType.SelectedItem as ListBoxItem).Content.ToString()[0];
                            cmd.Parameters.Add("@arbtid", SqlDbType.Decimal).Value = workTime;
                            cmd.Parameters.Add("@restid", SqlDbType.Decimal).Value = travelTime;
                            cmd.Parameters.Add("@kommentar", SqlDbType.NVarChar).Value = commentBox.Text;
                            cmd.Parameters.Add("@recnum", SqlDbType.BigInt).Value = recNr;
                            cmd.Parameters.Add("@trakt", SqlDbType.Bit).Value = traktChkBox.IsChecked;

                            cmd.ExecuteNonQuery();
                        }
                    }


                    sqlConn.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Very bad error");
            }
            DialogResult = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void timeBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Button_Click(this, null);
            }
        }

        private void timeBoxes_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (e.OriginalSource as TextBox);
            //tb.CaretIndex = tb.Text.Length;
            tb.SelectAll();
        }

        Brush maxCharsDefaultBrush;
        private void commentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            maxCharsLabel.Content = commentBox.Text.Length + "/" + commentBox.MaxLength;

            if (commentBox.Text.Length == commentBox.MaxLength)
                maxCharsLabel.Foreground = Brushes.Red;
            else
                maxCharsLabel.Foreground = maxCharsDefaultBrush;
        }

        private void commentBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Button_Click(sender, e);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                DialogResult = false;
        }

        private void workBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = (sender as TextBox);
            if(textBox != null)
            {

                if(!textBox.IsKeyboardFocused && textBox.Text == "0")
                {
                    e.Handled = true;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ThemeManager.RemoveWindow(this);
        }

        private void FinishDayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var totalTime = GetTotalTimeToday();

            if(totalTime < 8)
            {
                workBox.Text = (8 - totalTime).ToString();
            }
            else
            {
                MessageBox.Show("You're stacked!\r\n\r\nYou've reported a total of " + totalTime + "h this day.");
            }
        }

        decimal GetTotalTimeToday()
        {
            var day = datePicker.SelectedDate.ToString();
            string query = string.Format("SELECT ARBTID, RESTID FROM dbo.TEKNJOBB WHERE TEKN='{0}' AND STARTDAT='{1}'",
                (techBox.SelectedItem as Technician).UserName, day);

            decimal totalTime = 0;

            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();
                using (var reader = new SqlCommand(query, sqlConn).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var arb = reader.GetDecimal(0);
                        var res = reader.GetDecimal(1);

                        totalTime += arb + res;
                    }
                }
            }
            return totalTime;
        }
    }

    enum TimeReportEditType
    {
        Edit,
        AddNew
    }
}
