using System;
using System.Collections.Generic;
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
using WinServLib.Objects;

namespace WinServLite2.Jobs
{
    /// <summary>
    /// Interaction logic for TimeReportWindow.xaml
    /// </summary>
    public partial class TimeReportWindow : Window
    {
        TimeReportEditType editType;

        TimeReport relatedReport;
        Job relatedJob;

        //Brush maxCharsDefaultBrush;

        public event EventHandler<bool> OnDialogResult;
        public event EventHandler<int> OnJobIDChanged;

        // for new
        public TimeReportWindow(Job job)
        {
            InitializeComponent();
            SetJobTypes();
            editType = TimeReportEditType.AddNew;
            relatedJob = job;

            foreach (JobTimeType item in jobType.Items)
            {
                if (item.Code.ToString() == job.JobType)
                {
                    jobType.SelectedItem = item;
                    break;
                }
            }
            techBox.Items.Clear();
            foreach (Technician tech in WinServLib.WinServ.GetTechnicians())
            {
                techBox.Items.Add(tech);
                if (tech.UserName == (string)MainWindow.Settings["user"])
                    techBox.SelectedItem = tech;
            }

            datePicker.SelectedDate = DateTime.Now;
            jobIdTxtBox.Text = job.JobID.ToString();

            //maxCharsDefaultBrush = maxCharsLabel.Foreground;
        }

        // for edit
        public TimeReportWindow(TimeReport report, Job job)
        {
            InitializeComponent();
            SetJobTypes();
            editType = TimeReportEditType.Edit;
            relatedReport = report;
            relatedJob = job;
            commentBox.MaxLength = 44;

            var technicians = WinServLib.WinServ.GetTechnicians();
            techBox.ItemsSource = technicians;
            techBox.SelectedIndex = technicians.IndexOf(technicians.First(t => t.UserName == report.Technician));

            foreach (JobTimeType item in jobType.Items)
            {
                if (item.Code.ToString() == report.DelayCode)
                {
                    jobType.SelectedItem = item;
                    break;
                }
            }

            datePicker.SelectedDate = report.Date;

            traktChkBox.IsChecked = report.Traktamente;
            commentBox.Text = report.Comment;


            var workTime = TimeSpan.FromHours(report.WorkTime);
            workBox.Text = workTime.Hours.ToString();
            workBoxMin.Text = workTime.Minutes.ToString();

            //workBox.Text = report.WorkTime.ToString();
            travelBox.Text = report.TravelTime.ToString();
            jobIdTxtBox.Text = report.JobID.ToString();
        }

        void SetJobTypes()
        {
            var jobTypes = WinServLib.WinServ.GetJobTimeTypes().OrderBy(jt => jt.Code);
            jobType.ItemsSource = jobTypes;

            jobType.SelectedItem = jobTypes.FirstOrDefault(jobType => jobType.Code == WinServLib.WinServ.NotSpecifiedJobTimeTypeCode);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            double workTimeHours = 0, workTimeMinutes = 0, travelTime = 0;
            //if (workBox.Text == "" || workBox.Text == "0")
            //{
            //    MessageBox.Show("Please specify amount of worktime", "Error");
            //    return;
            //}

            if((jobType.SelectedItem as JobTimeType).Code == WinServLib.WinServ.NotSpecifiedJobTimeTypeCode)
            {
                MessageBox.Show("Please specify job time type.", "Error");
                jobType.Focus();
                return;
            }

            if(commentBox.Text == string.Empty)
            {
                MessageBox.Show("Please write a comment", "Error");
                commentBox.Focus();
                return;
            }
            if (!double.TryParse(workBox.Text, out workTimeHours))
            {
                MessageBox.Show("Unable to parse Arbtid hours", "Error");
                workBox.Focus();
                return;
            }
            if (!double.TryParse(workBoxMin.Text, out workTimeMinutes))
            {
                MessageBox.Show("Unable to parse Arbtid minutes", "Error");
                workBoxMin.Focus();
                return;
            }

            if (workTimeHours < 0 || workTimeMinutes < 0)
            {
                MessageBox.Show("Worktime can't be negative", "Error");
                return;
            }

            workTimeHours += TimeSpan.FromMinutes(workTimeMinutes).TotalHours;
            if(editType == TimeReportEditType.AddNew && workTimeHours <= 0)
            {
                MessageBox.Show("Please specify amount of worktime", "Error");
                return;
            }

            if (travelBox.Text == "")
                travelTime = 0;
            else if (!double.TryParse(travelBox.Text, out travelTime))
            {
                MessageBox.Show("Unable to parse Restid", "Error");
                return;
            }
            if (techBox.SelectedIndex < 0)
            {
                MessageBox.Show("Select a technician", "Error");
                return;
            }

            int jobId = -1;
            if (!int.TryParse(jobIdTxtBox.Text, out jobId))
            {
                MessageBox.Show("Unable to parse JobbID.", "Error");
                return;
            }


            if (changeJobIdChkBox.IsChecked == true && jobId != relatedJob.JobID)
            {
                var newJob = WinServLib.WinServ.JobManager.GetJob(jobId, relatedReport.ArchivedJob);

                if (newJob == null)
                {
                    MessageBox.Show("Felaktigt JobbID. Det finns inget aktivt jobb med det ID't.", "Error");
                    return;
                }
                else if (MessageBox.Show(string.Format("Vill du verkligen byta JobbID till följande jobb?\r\n\r\nJobbID: {0}\r\n\r\nAnläggning: {1}", newJob.JobID, newJob.SiteName), "Säkert?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (!relatedReport.SetJobID(newJob.JobID))
                    {
                        MessageBox.Show("Okänt fel inträffade vid ändring av JobbID.");
                        return;
                    }
                    else
                        OnJobIDChanged?.Invoke(this, newJob.JobID);
                }
                else
                    return;
            }

            switch(editType)
            {
                case TimeReportEditType.AddNew:

                    string comment = commentBox.Text;

                    int reports = (int)Math.Ceiling((decimal)commentBox.Text.Length / TimeReport.CommentCharCap);

                    int earlierReports = relatedJob.GetTimeReportsCount();

                    for (int i = 0; i < reports; i++)
                    {
                        string curComment;
                        if (comment.Length > TimeReport.CommentCharCap)
                        {
                            curComment = comment.Remove(TimeReport.CommentCharCap, comment.Length - TimeReport.CommentCharCap) + "-";
                            comment = comment.Remove(0, TimeReport.CommentCharCap);
                        }
                        else
                            curComment = comment;

                        relatedJob.AddTimeReport(new TimeReport()
                        {
                            Technician = (techBox.SelectedItem as Technician).UserName,
                            JobID = relatedJob.JobID,
                            Date = datePicker.SelectedDate.Value.AddSeconds(i).AddMinutes(earlierReports),
                            DelayCode = (jobType.SelectedItem as JobTimeType).Code,
                            JobType = relatedJob.JobType,
                            WorkTime = i == 0 ? (float)workTimeHours : 0,
                            TravelTime = i == 0 ? (float)travelTime : 0,
                            Comment = curComment,
                            Traktamente = traktChkBox.IsChecked.Value
                        });
                    }

                    break;

                case TimeReportEditType.Edit:

                    relatedReport.Technician = (techBox.SelectedItem as Technician).UserName;
                    //relatedReport.JobID = relatedJob.JobID;
                    relatedReport.Date = datePicker.SelectedDate.Value;
                    relatedReport.DelayCode = (jobType.SelectedItem as JobTimeType).Code;
                    relatedReport.JobType = relatedJob.JobType;
                    relatedReport.WorkTime = (float)workTimeHours;
                    relatedReport.TravelTime = (float)travelTime;
                    relatedReport.Comment = commentBox.Text;
                    relatedReport.Traktamente = traktChkBox.IsChecked.Value;

                    relatedReport.Save();
                    break;
            }

            //DialogResult = true;
            OnDialogResult?.Invoke(this, true);
            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            OnDialogResult?.Invoke(this, false);
            Close();
        }

        private void timeBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                saveBtn_Click(this, null);
            }
        }

        private void timeBoxes_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (e.OriginalSource as TextBox);
            tb.SelectAll();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void workBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = (sender as TextBox);
            if (textBox != null)
            {

                if (!textBox.IsKeyboardFocused && textBox.Text == "0")
                {
                    e.Handled = true;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private void FinishDayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var totalTime = WinServLib.WinServ.JobManager.GetTotalTimeFromDay((techBox.SelectedItem as Technician).UserName, datePicker.SelectedDate.Value);

            if (totalTime < 8)
            {
                workBox.Text = (8 - totalTime).ToString();
            }
            else
            {
                MessageBox.Show("You're stacked!\r\n\r\nYou've reported a total of " + totalTime + "h this day.");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnDialogResult?.Invoke(this, false);
        }
    }
    enum TimeReportEditType
    {
        Edit,
        AddNew
    }
}
