using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
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
using WinServLib.Objects;
using WinServLite2.CloseableTab;
using WinServLite2.Jobs;
using static WinServLib.Objects.Job;
using Outlook = Microsoft.Office.Interop.Outlook;


namespace WinServLite2
{
    /// <summary>
    /// Interaction logic for JobViewer.xaml
    /// </summary>
    public partial class JobViewer : DynamicUserControl
    {
        Job jobToDisplay;
        List<TimeReport> timeReports;

        TimeReportWindow currentReportWindow;

        public event EventHandler<Job> OnJobChanged;

        public event EventHandler<int> OnJobIDChanged;

        public int OpenJobID => jobToDisplay.JobID;

        Graph.PlanTaskDetails taskDetails;

        public JobViewer()
        {
            InitializeComponent();

            jobStatusComboBox.ItemsSource = WinServLib.WinServ.JobStatus;
        }

        public void Init(Job job)
        {
            jobToDisplay = job;
            this.Tag = jobToDisplay;
            refName.Text = jobToDisplay.RefName;
            refTel.Text = jobToDisplay.RefPhoneNumber;
            refEmail.Text = jobToDisplay.RefEmailAddress;
            addressTxtBox.Text = string.Format("{0}, {1} {2}", jobToDisplay.Address, jobToDisplay.PostNumber, jobToDisplay.City);

            if (jobToDisplay.PlannerTaskId != null)
            {
                plannerBtn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 32, 158, 99));
                plannerBtn.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);
            }

            if (jobToDisplay.JobStatus == Job.JobStatusType.Archived)
            {
                jobStatusComboBox.ItemsSource = new[] { "Inaktiv" };
                jobStatusComboBox.SelectedIndex = 0;
                jobStatusComboBox.IsEnabled = false;

                addTimeBtn.IsEnabled = false;
                descriptionText.IsReadOnly = true;
                refName.IsReadOnly = true;
                refTel.IsReadOnly = true;
                refEmail.IsReadOnly = true;
                articleManager.Disable();
            }
            else
            {
                jobStatusComboBox.SelectedItem = WinServLib.WinServ.JobStatus.First(s => s.Nr == jobToDisplay.Status);
                jobStatusComboBox.SelectionChanged += (s, e) => OnUserInput(s, null);

                articleManager.Init(job);
                articleManager.OnUserChange += (s, e) =>
                {
                    OnUserInput(s, null);
                };
            }


            siteNameLabel.Text = "[" + job.JobID + "] " + job.SiteName;

            descriptionText.Text = job.FormatJobDescriptions();

            RefreshList();

            UpdateTimeLabel();

            refName.TextChanged += OnUserInput;
            refTel.TextChanged += OnUserInput;
            refEmail.TextChanged += OnUserInput;
            descriptionText.TextChanged += OnUserInput;


            CheckPlannerTasks();
        }

        async void CheckPlannerTasks()
        {
            if (jobToDisplay.PlannerTaskId != null) // Vi har en reggad task
            {
                var auth = await Graph.GraphAPI.CheckLogin(System.Windows.Application.Current.MainWindow);

                taskDetails = await Graph.GraphAPI.GetTaskDetails(auth, jobToDisplay.PlannerTaskId);

                if (taskDetails != null)
                {
                    plannerChecklistListView.ItemsSource = taskDetails.checklist.Values.ToList();
                }
            }
        }

        public void RefreshList()
        {
            reportListView.ItemsSource = null;

            timeReports = jobToDisplay.GetTimeReports();
            reportListView.Items.Clear();
            reportListView.ItemsSource = timeReports;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(reportListView.ItemsSource);

            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Technician");
            view.GroupDescriptions.Add(groupDescription);
            view.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));

            UpdateTimeLabel();
        }

        private void OnUserInput(object sender, TextChangedEventArgs e)
        {
            var tabParent = this.Parent as CloseableTab.CloseableTab;
            if (tabParent != null)
                tabParent.IndicateChanges(UserChangedAnything(out _));
        }

        public override void OnClosing(out bool cancel)
        {
            bool updateDescription;
            cancel = UserChangedAnything(out updateDescription);

            if (cancel)
            {
                switch (MessageBox.Show("Det finns osparade ändringar. Vill du spara dom innan du stänger fönstret?", "Bekräfta",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Information))
                {
                    case MessageBoxResult.Yes:
                        cancel = false;
                        SaveJob(updateDescription);
                        break;
                    case MessageBoxResult.No:
                        cancel = false;
                        break;
                    case MessageBoxResult.Cancel:
                        cancel = true;
                        break;
                }
            }
        }

        private bool UserChangedAnything(out bool descriptionChanged)
        {
            descriptionChanged = false;
            if (jobToDisplay.JobStatus == JobStatusType.Archived)
                return false;

            string prevJobDesc = jobToDisplay.FormatJobDescriptions();
            if (descriptionText.Text != prevJobDesc)
            {
                descriptionChanged = true;
                return true;
            }

            if (refName.Text != jobToDisplay.RefName ||
                refEmail.Text != jobToDisplay.RefEmailAddress ||
                refTel.Text != jobToDisplay.RefPhoneNumber ||
                CompareArticles(articleManager.LocalArticles, jobToDisplay.Articles) == false ||
                ((Status)jobStatusComboBox.SelectedItem).Nr != jobToDisplay.Status)
                return true;

            return false;
        }

        bool CompareArticles(List<Article> a, List<Article> b)
        {
            if (jobToDisplay.JobStatus == JobStatusType.Archived)
                return true;

            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
            {
                if (a[i].ArticlePrice != b[i].ArticlePrice || a[i].ArticleText != b[i].ArticleText || a[i].Quantity != b[i].Quantity)
                    return false;
            }

            return true;
        }

        private void SaveJob(bool updateDescription)
        {
            jobToDisplay.RefName = refName.Text;
            jobToDisplay.RefPhoneNumber = refTel.Text;
            jobToDisplay.RefEmailAddress = refEmail.Text;
            jobToDisplay.Status = ((Status)jobStatusComboBox.SelectedItem).Nr;
            jobToDisplay.Articles = new List<Article>(articleManager.LocalArticles);
            jobToDisplay.CompleteJobDescription = descriptionText.Text;

            jobToDisplay.Save(newJobDescriptions: (updateDescription ? GetLinesFromTextBox(descriptionText) : null), saveArticles: true);

            OnJobChanged?.Invoke(this, jobToDisplay);
        }

        string[] GetLinesFromTextBox(System.Windows.Controls.TextBox textBox)
        {
            string[] lines = new string[textBox.LineCount];

            for (int line = 0; line < textBox.LineCount; line++)
                lines[line] = textBox.GetLineText(line);

            return lines;
        }



        private void UpdateTimeLabel()
        {
            float totalWorkH = 0, totalTravelH = 0;

            foreach (var tr in timeReports)
            {
                totalWorkH += tr.WorkTime;
                totalTravelH += tr.TravelTime;
            }

            timeLabel.Content = string.Format("Total tid: {0}h ({1}h arbtid, {2} restid)", totalWorkH + totalTravelH, totalWorkH, totalTravelH);
        }

        private void AddTimeBtn_Click(object sender, RoutedEventArgs e)
        {
            currentReportWindow = new TimeReportWindow(jobToDisplay);

            currentReportWindow.Title = "[" + jobToDisplay.JobID + "] " + currentReportWindow.Title;

            currentReportWindow.OnDialogResult += (s, dialogresult) =>
            {
                addTimeBtn.IsEnabled = true;
                currentReportWindow = null;
                RefreshList();
            };

            addTimeBtn.IsEnabled = false;
            currentReportWindow.Show();
        }


        private void DeleteReportBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (jobToDisplay.JobStatus == JobStatusType.Archived)
                return;

            if (MessageBox.Show("Är du säker på att du vill ta bort denna rapport?", "Varning", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            var report = (GetDependencyObjectFromVisualTree(e.OriginalSource as DependencyObject, typeof(ListViewItem)) as ListViewItem).Content as TimeReport;
            report.Delete();

            RefreshList();
        }

        private void EditReportBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (jobToDisplay.JobStatus == JobStatusType.Archived || currentReportWindow != null)
                return;

            var report = (GetDependencyObjectFromVisualTree(e.OriginalSource as DependencyObject, typeof(ListViewItem)) as ListViewItem).Content as TimeReport;

            currentReportWindow = new TimeReportWindow(report, jobToDisplay)
            {
                Owner = System.Windows.Window.GetWindow(this)
            };

            currentReportWindow.OnDialogResult += (s, dialogresult) =>
            {
                addTimeBtn.IsEnabled = true;
                currentReportWindow = null;
                RefreshList();
            };

            currentReportWindow.OnJobIDChanged += (s, ji) =>
            {
                OnJobIDChanged?.Invoke(s, ji);
            };

            addTimeBtn.IsEnabled = false;

            currentReportWindow.Show();
        }

        private void BookmarkJobImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (isBookmarked)
            //{
            //    bookmarkJobImg.Source = ConvertBitmap(Properties.Resources.unstar);
            //    isBookmarked = false;

            //    foreach (Job job in MainWindow.JobBookmarks)
            //    {
            //        if (job.JobID == jobToDisplay.JobID)
            //        {
            //            MainWindow.JobBookmarks.Remove(job);
            //            MainWindow.SaveJobBookmarks();
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    bookmarkJobImg.Source = ConvertBitmap(Properties.Resources.star);
            //    isBookmarked = true;

            //    MainWindow.JobBookmarks.Add(jobToDisplay);
            //    MainWindow.SaveJobBookmarks();
            //}

            //CollectionViewSource.GetDefaultView(mainWindow.BookmarksListView.ItemsSource).Refresh();
        }

        private void ShareJobBtn_Click(object s, RoutedEventArgs e)
        {
            try
            {
                var mapsLink = string.Format("https://www.google.se/maps/search/{0}+{1}+{2}", jobToDisplay.SiteName, jobToDisplay.Address, jobToDisplay.City);

                string subject = "Job: " + jobToDisplay.SiteName + " [" + jobToDisplay.JobID + "]";
                string body = string.Format(
                        "Here's a job for you!" +
                        "<br><br>" +
                        "{9}" +
                        "<br><br>" +
                        (jobToDisplay.PlannerTaskId != null ? "<a href='https://tasks.office.com/insupport.se/Home/Task/" + jobToDisplay.PlannerTaskId + "'>Öppna i Planner</a><br><br>" : "") +
                        "{1}<br>" +
                        "{2}<br>" +
                        "{3}, {4}<br>" +
                        "<a href='{8}'><img src=\"https://portal.tryggconnect.se/api/googleMapsImage/create.php?location={8}&size=300x250&zoom=11\"></a>" +
                        "<br><br>" +
                        "<a href='mailto:{5}'>{6}</a> - <a href='tel:{7}'>{7}</a>",
                        jobToDisplay.JobID, jobToDisplay.SiteName, jobToDisplay.Address, jobToDisplay.PostNumber,
                        jobToDisplay.City, jobToDisplay.RefEmailAddress, jobToDisplay.RefName, jobToDisplay.RefPhoneNumber, mapsLink, jobToDisplay.CompleteJobDescription.Replace("\r\n", "<br>"));

                if (MainWindow.Settings.ContainsKey("CompabilityJobShare") && (bool)MainWindow.Settings["CompabilityJobShare"] == true)
                {
                    body = "<html><head><meta charset=\"UTF-8\"></head><body>" + body + "</body></html>";

                    ClipboardHelper.CopyToClipboard(body, "nothing here :(");

                    MessageBox.Show("Jobbet är kopierat till urklipp!", "Succé");
                }
                else
                {
                    Outlook.Application oApp = new Outlook.Application();

                    Outlook.MailItem oMailItem = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);

                    oMailItem.HTMLBody = body;
                    oMailItem.Subject = subject;
                    oMailItem.Display(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void CopyAddressBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(addressTxtBox.Text);
            AnimateThumbsUp(copyAddressBtn);
        }

        ImageSource GetImageFromAssembly(string psResourceName)
        {
            Uri oUri = new Uri("pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/" + psResourceName, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(oUri);
        }

        private void AnimateThumbsUp(System.Windows.Controls.Image image)
        {
            Duration fadeTime = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            var thumb = GetImageFromAssembly("Resources/like.png");
            var originalSource = image.Source;


            var fadeOutAnimation = new DoubleAnimation(0d, fadeTime);
            var fadeInAnimation = new DoubleAnimation(1d, fadeTime);

            fadeInAnimation.Completed += (o, e) =>
            {
                Thread.Sleep(1000);

                var newFadeOut = new DoubleAnimation(0d, fadeTime);
                var newFadeIn = new DoubleAnimation(1d, fadeTime.Add(new Duration(new TimeSpan(0, 0, 0, 0, 100))));
                newFadeOut.Completed += (o2, e2) =>
                {
                    image.Source = originalSource;
                    image.BeginAnimation(System.Windows.Controls.Image.OpacityProperty, newFadeIn);
                };

                image.BeginAnimation(System.Windows.Controls.Image.OpacityProperty, newFadeOut);
            };

            fadeOutAnimation.Completed += (o, e) =>
            {
                image.Source = thumb;
                image.BeginAnimation(System.Windows.Controls.Image.OpacityProperty, fadeInAnimation);
            };

            image.BeginAnimation(System.Windows.Controls.Image.OpacityProperty, fadeOutAnimation);

        }

        private DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            DependencyObject parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                else

                    parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            bool updateDesc = false;
            if (e.Key == Key.S && Keyboard.IsKeyDown(Key.LeftCtrl) && UserChangedAnything(out updateDesc))
            {
                SaveJob(updateDesc);
                OnUserInput(this, null);
                e.Handled = true;
            }
        }

        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private void SortByColumns(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);

            if (listViewSortAdorner != null && listViewSortAdorner.Direction == ListSortDirection.Descending && listViewSortCol == column)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                listViewSortAdorner = null;
                listViewSortCol = null;

                RefreshList();
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
        private void SortBy(string sort, ListSortDirection direction)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(reportListView.ItemsSource);

            view.GroupDescriptions.Clear();
            view.SortDescriptions.Clear();

            view.SortDescriptions.Add(new SortDescription(sort, direction));
            view.Refresh();
        }

        private void excelExportBtn_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = $"{jobToDisplay.SiteName} #{jobToDisplay.JobID}.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                Workbook wb = app.Workbooks.Add(1);
                Worksheet ws = wb.Sheets.Add(Missing.Value, Missing.Value, 1, Missing.Value);

                ws.Cells[1, 1] = jobToDisplay.FormatJobDescriptions();
                ws.Range[ws.Cells[1, 1], ws.Cells[1, 7]].Merge();
                //ws.Rows[1, 1].AutoFit();

                ws.Cells[2, 1] = "Technician";
                ws.Cells[2, 2] = "Date";
                ws.Cells[2, 3] = "Worktime";
                ws.Cells[2, 4] = "Traveltime";
                ws.Cells[2, 5] = "Comment";
                ws.Cells[2, 6] = "ReportID";
                ws.Cells[2, 7] = "JobID";


                bool useAllowance = timeReports.Any(tr => tr.Traktamente == true);

                if (useAllowance)
                    ws.Cells[1, 8] = "Allowance";

                int row = 3;
                foreach (var tr in timeReports.OrderByDescending(tr => tr.Date))
                {
                    ws.Cells[row, 1] = tr.Technician;
                    ws.Cells[row, 2] = tr.Date.ToShortDateString();
                    ws.Cells[row, 3] = tr.WorkTime;
                    ws.Cells[row, 4] = tr.TravelTime;
                    ws.Cells[row, 5] = tr.Comment;
                    ws.Cells[row, 6] = tr.UniqueID;
                    ws.Cells[row, 7] = jobToDisplay.JobID;

                    if (useAllowance)
                        ws.Cells[row, 8] = tr.Traktamente;
                    row++;
                }
                ws.Columns.AutoFit();
                //ws.Rows.AutoFit();
                wb.SaveAs(saveFileDialog.FileName, Missing.Value, Missing.Value, Missing.Value, false, Missing.Value, XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                wb.Close();
                app.Quit();
            }
        }

        private async void plannerBtn_Click(object sender, RoutedEventArgs e)
        {
            var auth = await Graph.GraphAPI.CheckLogin(System.Windows.Application.Current.MainWindow);
            if (jobToDisplay.PlannerTaskId != null) // Vi har en reggad task
            {
                var plan = await Graph.GraphAPI.GetTask(auth, jobToDisplay.PlannerTaskId);
                if (plan.id == null) // Reggad task är borttagen
                {
                    if (MessageBox.Show("Det finns inget ärende i Planner, det som fanns har tagits bort. Vill du skapa ett nytt?", "Planner", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var task = await CreateTask(auth);
                        if (jobToDisplay.SetPlannerTaskId(task.id))
                        {
                            Process.Start(task.GetUrl());
                        }
                    }
                    else
                    {
                        jobToDisplay.SetPlannerTaskId(null);
                        jobToDisplay.PlannerTaskId = null;
                    }
                }
                else // Öppna task
                {
                    var details = await Graph.GraphAPI.GetTaskDetails(auth, jobToDisplay.PlannerTaskId);
                    Process.Start($"https://tasks.office.com/insupport.se/Home/Task/{plan.id}");
                }
            }
            else // Finns ingen task i planner
            {
                if (MessageBox.Show("Det finns inget ärende i Planner. Vill du skapa ett?", "Planner", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var task = await CreateTask(auth);
                    if (jobToDisplay.SetPlannerTaskId(task.id))
                    {
                        CheckPlannerTasks();

                        //Process.Start(task.GetUrl());
                    }
                }
            }
        }

        private async Task<Graph.PlanTask> CreateTask(Microsoft.Identity.Client.AuthenticationResult auth)
        {
            var task = await Graph.GraphAPI.CreatePlan(auth, $"[{jobToDisplay.JobID}] {jobToDisplay.SiteName}", descriptionText.Text);
            jobToDisplay.PlannerTaskId = task.id;
            plannerBtn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 32, 158, 99));
            plannerBtn.Foreground = new SolidColorBrush(System.Windows.Media.Colors.White);

            return task;
        }

        private void initEmailHyperLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;
            try
            {
                Process.Start("mailto:" + refEmail.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void initCallHyperLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;
            try
            {
                Process.Start("tel:" + refTel.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void PlannerTaskCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var auth = await Graph.GraphAPI.CheckLogin(System.Windows.Application.Current.MainWindow);

            Dictionary<string, Graph.PlannerChecklistItemForUpload> uploadChecklist = new Dictionary<string, Graph.PlannerChecklistItemForUpload>();
            foreach (var chk in taskDetails.checklist)
                uploadChecklist[chk.Key] = new Graph.PlannerChecklistItemForUpload()
                {
                    isChecked = chk.Value.isChecked,
                    title = chk.Value.title
                };

            await Graph.GraphAPI.PatchAsync(new { checklist = uploadChecklist }, $"{taskDetails.id}/details", auth, taskDetails.etag);
        }
    }
}
