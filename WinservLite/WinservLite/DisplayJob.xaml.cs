using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;

namespace WinservLite
{
    /// <summary>
    /// Interaction logic for DisplayJob.xaml
    /// </summary>
    public partial class DisplayJob : Window
    {
        public bool ModifiedJob = false;

        MainWindow mainWindow;

        Job jobToDisplay;
        List<TimeReport> timeReports = new List<TimeReport>();
        int previousLineCount;


        //drag n drop
        Point startPoint;

        public DisplayJob(Job job, MainWindow mw)
        {
            mainWindow = mw;
            try
            {
                InitializeComponent();

                reportListView.Items.Clear();

                jobToDisplay = job;
                refName.Text = jobToDisplay.RefName;
                refTel.Text = jobToDisplay.RefPhoneNumber;
                refEmail.Text = jobToDisplay.RefEmailAddress;
                addressTxtBox.Text = string.Format("{0}, {1} {2}", jobToDisplay.Address, jobToDisplay.PostNumber, jobToDisplay.City);

                if (jobToDisplay.Status == "U")
                    jobCompletedChkBox.IsChecked = true;

                jobCompletedChkBox.Checked += jobCompletedChkBox_Checked;
                jobCompletedChkBox.Unchecked += jobCompletedChkBox_Unchecked;

                if (jobToDisplay.JobStatus == JobStatusType.Archived)
                    addTimeBtn.IsEnabled = false;

                siteNameLabel.Text = "[" + job.JobID + "] " + job.SiteName;

                descriptionText.Clear();
                foreach(JobDescription jobDesc in job.JobDescriptions)
                {
                    if(jobDesc.JobText == "")
                        descriptionText.AppendText("\r\n");
                    else
                        descriptionText.AppendText(jobDesc.JobText);
                }
                UpdateLayout();

                //descriptionText.Text = job.CompleteJobDescription;

                FetchReports(job.JobID);
                UpdateTimeLabel();

                foreach(Job bookmark in MainWindow.JobBookmarks)
                {
                    if (bookmark.JobID == jobToDisplay.JobID)
                    {
                        isBookmarked = true;
                        bookmarkJobImg.Source = ConvertBitmap(Properties.Resources.star);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.Title = string.Format("[{0}] {1}", job.JobID, job.SiteName);


            reportListView.AllowDrop = true;

            reportListView.PreviewMouseLeftButtonDown += ReportListView_PreviewMouseLeftButtonDown;

            reportListView.MouseMove += ReportListView_MouseMove;
            reportListView.DragEnter += (s, e) =>
            {
                if (!e.Data.GetDataPresent("timeReportData") || s == e.Source)
                {
                    e.Effects = DragDropEffects.None;
                }
            };

            reportListView.Drop += (s, e) =>
            {
                try
                {
                    if (e.Data.GetDataPresent("timeReportData"))
                    {
                        if (e.Data.GetData("sourceList") == s || jobToDisplay.JobStatus == JobStatusType.Archived)
                            return;

                        TimeReport report = e.Data.GetData("timeReportData") as TimeReport;
                        if (AddFromClass(report) > 0)
                        {
                            DeleteReport(report.UniqueID);
                            (e.Data.GetData("sourceJobDisplay") as DisplayJob).RefreshList();
                        }
                        RefreshList();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("An error occured during drag & drop\r\n" + ex.ToString());
                }
            };
        }

        private int AddFromClass(TimeReport report)
        {
            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();

                string query = "INSERT into dbo.TEKNJOBB (TEKN, JOBBNR, JOBBTYP, PLANDAT, DELAYCODE, STARTDAT, SLUTDAT, ARBTID, RESTID, KOMMENTAR, TRAKTAMENTE) VALUES " +
                    "(@tekn, @jobbnr, @jobbtyp, @plandat, @delaycode, @startdat, @slutdat, @arbtid, @restid, @kommentar, @trakt)";
                using (var cmd = new SqlCommand(query))
                {
                    cmd.Connection = sqlConn;
                    cmd.Parameters.Add("@tekn", SqlDbType.NVarChar).Value = report.Technician;
                    cmd.Parameters.Add("@jobbnr", SqlDbType.Decimal).Value = jobToDisplay.JobID;
                    cmd.Parameters.Add("@plandat", SqlDbType.DateTime).Value = report.StartDate.ToString();
                    cmd.Parameters.Add("@delaycode", SqlDbType.NVarChar).Value = ((int)Enum.Parse(typeof(TimeReport.JobTypes), report.JobType)).ToString();
                    cmd.Parameters.Add("@startdat", SqlDbType.DateTime).Value = report.StartDate.ToString();
                    cmd.Parameters.Add("@slutdat", SqlDbType.DateTime).Value = report.StartDate.ToString();
                    cmd.Parameters.Add("@jobbtyp", SqlDbType.NVarChar).Value = report.JobType;
                    cmd.Parameters.Add("@arbtid", SqlDbType.Decimal).Value = (decimal)report.WorkTime;
                    cmd.Parameters.Add("@restid", SqlDbType.Decimal).Value = (decimal)report.TravelTime;
                    cmd.Parameters.Add("@kommentar", SqlDbType.NVarChar).Value = report.Comment;
                    cmd.Parameters.Add("@trakt", SqlDbType.Bit).Value = report.Traktamente;

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        private void ReportListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void ReportListView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Point mousePos = e.GetPosition(null);
                Vector diff = startPoint - mousePos;

                if (e.LeftButton == MouseButtonState.Pressed &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    ListView listView = sender as ListView;
                    ListViewItem listViewItem =
                        FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                    if (listViewItem == null)
                        return;

                    TimeReport report = (TimeReport)listView.ItemContainerGenerator.
                        ItemFromContainer(listViewItem);

                    DataObject dragData = new DataObject("timeReportData", report);
                    dragData.SetData("sourceList", reportListView);
                    dragData.SetData("sourceJobDisplay", this);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("An error occured during drag & drop\r\n" + ex.ToString());
            }
        }

        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void FetchReports(int jobId, bool addGroupDesc = true)
        {
            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();

                string table = "";

                if (jobToDisplay.JobStatus == JobStatusType.Archived)
                    table = "dbo.JOBBTEKN";  // arkiverat
                else
                    table = "dbo.TEKNJOBB";  // aktivt

                string query = "SELECT JOBBNR, TEKN,KOMMENTAR, STARTDAT, ARBTID, RESTID, RECNUM, JOBBTYP, TRAKTAMENTE FROM " + table + " WHERE JOBBNR = " + jobId + ".00";

                try
                {
                    SqlDataReader reader = new SqlCommand(query, sqlConn).ExecuteReader();

                    while (reader.Read())
                    {
                        timeReports.Add(new TimeReport()
                        {
                            Technician = reader.GetString(1),
                            Comment = reader.GetString(2),
                            StartDate = reader.GetDateTime(3),
                            WorkTime = (float)reader.GetDecimal(4),
                            TravelTime = (float)reader.GetDecimal(5),
                            UniqueID = (int)reader.GetInt64(6),
                            JobType = reader.GetString(7),
                            Traktamente = reader.IsDBNull(8) ? false : reader.GetBoolean(8)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                sqlConn.Close();
            }
            reportListView.ItemsSource = timeReports;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(reportListView.ItemsSource);

            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Technician");
            view.GroupDescriptions.Add(groupDescription);
            view.SortDescriptions.Add(new SortDescription("StartDate", ListSortDirection.Ascending));
            //view.SortDescriptions.Add(new SortDescription("Technician", ListSortDirection.Ascending));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddTimeReport addTimeReport = new AddTimeReport(jobToDisplay);
            addTimeReport.Owner = this;
            ThemeManager.AddWindow(addTimeReport);
            if(addTimeReport.ShowDialog() == true)
            {
                RefreshList();
            }
        }

        void RefreshList()
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(reportListView.ItemsSource);
            view.GroupDescriptions.Clear();
            view.SortDescriptions.Clear();
            timeReports.Clear();
            FetchReports(jobToDisplay.JobID, false);
            UpdateTimeLabel();
        }

        private void UpdateTimeLabel()
        {
            float totalWorkH = 0, totalTravelH = 0;

            foreach (var tr in timeReports)
            {
                totalWorkH += tr.WorkTime;
                totalTravelH += tr.TravelTime;
            }

            timeLabel.Content = string.Format("Time summary: {0}h ({1}h travel)", totalWorkH, totalTravelH);
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

        private void editReportBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (jobToDisplay.JobStatus == JobStatusType.Archived)
                return;

            var report = (GetDependencyObjectFromVisualTree(e.OriginalSource as DependencyObject, typeof(ListViewItem)) as ListViewItem).Content as TimeReport;
            var timeReportDialog = new AddTimeReport(report);
            timeReportDialog.Owner = this;
            ThemeManager.AddWindow(timeReportDialog);
            if(timeReportDialog.ShowDialog() == true)
            {
                RefreshList();
            }
        }

        private void deleteReportBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (jobToDisplay.JobStatus == JobStatusType.Archived)
                return;

            if (MessageBox.Show("Are you sure you want to delete this report?", "Warning", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            var report = (GetDependencyObjectFromVisualTree(e.OriginalSource as DependencyObject, typeof(ListViewItem)) as ListViewItem).Content as TimeReport;

            DeleteReport(report.UniqueID);

            RefreshList();
        }

        private void DeleteReport(int recnum)
        {
            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();

                string query = "DELETE FROM dbo.TEKNJOBB WHERE RECNUM=@recnum";
                using (var cmd = new SqlCommand(query, sqlConn))
                {
                    cmd.Parameters.Add("@recnum", SqlDbType.BigInt).Value = recnum;
                    cmd.ExecuteNonQuery();
                }
                sqlConn.Close();
            }
        }

        BitmapImage ConvertBitmap(System.Drawing.Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                //BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void editDescriptionBtn_MouseDown_edit(object sender, MouseButtonEventArgs e)
        {
            if(descriptionText.IsReadOnly && jobToDisplay.JobStatus != JobStatusType.Archived)
            {
                previousLineCount = descriptionText.LineCount;

                editDescriptionBtn.Source = ConvertBitmap(Properties.Resources.save);

                descriptionText.IsReadOnly = false;
                //descriptionText.KeyDown += DescriptionText_KeyDown;

                e.Handled = true;
                descriptionText.Focus();
                descriptionText.SelectAll();

                editDescriptionBtn.MouseDown -= editDescriptionBtn_MouseDown_edit;
                editDescriptionBtn.MouseDown += editDescriptionBtn_MouseDown_save;
            }
        }

        private void editDescriptionBtn_MouseDown_save(object sender, MouseButtonEventArgs e)
        {
            editDescriptionBtn.Source = ConvertBitmap(Properties.Resources.edit);
            editDescriptionBtn.MouseDown += editDescriptionBtn_MouseDown_edit;
            editDescriptionBtn.MouseDown -= editDescriptionBtn_MouseDown_save;
            //descriptionText.KeyDown -= DescriptionText_KeyDown;
            ModifiedJob = true;
            descriptionText.Select(0, 0);
            descriptionText.IsReadOnly = true;

            // update stuff

            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();
                string query = "DELETE FROM dbo.JOBBFT WHERE JOBBNR=@jobbnr";
                using (var sqlCommand = new SqlCommand(query, sqlConn))
                {
                    sqlCommand.Parameters.AddWithValue("@jobbnr", jobToDisplay.JobID);
                    //sqlCommand.Parameters.AddWithValue("@rad", i + 1);

                    sqlCommand.ExecuteNonQuery();
                }

                for (int i = 0; i < descriptionText.LineCount; i++)
                {
                    query = "INSERT into dbo.JOBBFT (JOBBNR, TEXTRAD, RAD) VALUES (@jobbnr, @text, @rad)";
                    using (var sqlCommand = new SqlCommand(query, sqlConn))
                    {
                        sqlCommand.Parameters.AddWithValue("@jobbnr", jobToDisplay.JobID);
                        sqlCommand.Parameters.AddWithValue("@rad", i + 1);
                        sqlCommand.Parameters.AddWithValue("@text", descriptionText.GetLineText(i));

                        sqlCommand.ExecuteNonQuery();
                    }

                }
                sqlConn.Close();
            }
        }

        private void DescriptionText_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                editDescriptionBtn_MouseDown_save(sender, null);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!descriptionText.IsReadOnly || !refName.IsReadOnly || !refTel.IsReadOnly || !refEmail.IsReadOnly)
                if (MessageBox.Show("Changes not yet saved. All changes will be lost. Continue?", "Warning", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }


            if(!e.Cancel && ModifiedJob)
            {
                //ThemeManager.RemoveWindow(this);
                mainWindow.RefreshJobList();
            }
        }

        void StartRefEdit(Image me, TextBox myBox, string columnField)
        {
            me.Source = ConvertBitmap(Properties.Resources.save);
            myBox.IsReadOnly = false;
            //e.Handled = true;
            myBox.Focus();
            myBox.SelectAll();

            myBox.KeyDown += (s,e) =>
            {
                if(e.Key == Key.Enter && !myBox.IsReadOnly)
                    StopRefEdit(me, myBox, columnField);
            };
        }

        void StopRefEdit(Image me, TextBox myBox, string columnField)
        {
            me.Source = ConvertBitmap(Properties.Resources.edit);
            myBox.IsReadOnly = true;
            myBox.Select(0, 0);

            RemoveRoutedEventHandlers(myBox, UIElement.KeyDownEvent);

            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();
                new SqlCommand("UPDATE dbo.JOBB SET " + columnField + "='" + myBox.Text + "' WHERE JOBBNR=" + (double)jobToDisplay.JobID, sqlConn).ExecuteNonQuery();
                sqlConn.Close();
            }
            ModifiedJob = true;
        }

        bool modifyingRefName, modifyingRefTel, modifyingRefEmail;
        private void editRefNameBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (jobToDisplay.JobStatus == JobStatusType.Archived)
                return;
            var me = editRefNameBtn;
            var myBox = refName;
            if(modifyingRefName)
            {
                modifyingRefName = false;
                StopRefEdit(me, myBox, "REF");
            }
            else
            {
                modifyingRefName = true;
                StartRefEdit(me, myBox, "REF");
            }
        }

        private void editRefTelBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (jobToDisplay.JobStatus == JobStatusType.Archived)
                return;
            var me = editRefTelBtn;
            var myBox = refTel;
            if(modifyingRefTel)
            {
                modifyingRefTel = false;
                StopRefEdit(me, myBox, "TEL");
            }
            else
            {
                modifyingRefTel = true;
                StartRefEdit(me, myBox, "TEL");
            }
        }

        bool isBookmarked = false;
        private void bookmarkJobImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(isBookmarked)
            {
                bookmarkJobImg.Source = ConvertBitmap(Properties.Resources.unstar);
                isBookmarked = false;

                foreach(Job job in MainWindow.JobBookmarks)
                {
                    if(job.JobID == jobToDisplay.JobID)
                    {
                        MainWindow.JobBookmarks.Remove(job);
                        MainWindow.SaveJobBookmarks();
                        break;
                    }
                }
            }
            else
            {
                bookmarkJobImg.Source = ConvertBitmap(Properties.Resources.star);
                isBookmarked = true;

                MainWindow.JobBookmarks.Add(jobToDisplay);
                MainWindow.SaveJobBookmarks();
            }

            CollectionViewSource.GetDefaultView(mainWindow.BookmarksListView.ItemsSource).Refresh();

            //foreach (var column in (mainWindow.BookmarksListView.View as GridView).Columns)
            //{
            //    //column
            //    // If this is an "auto width" column...
            //    if (double.IsNaN(column.Width))
            //    {
            //        // Set its Width back to NaN to auto-size again
            //        column.Width = column.ActualWidth;
            //        column.Width = double.NaN;
            //    }
            //}

            //CollectionViewSource.GetDefaultView(mainWindow.BookmarksListView.ItemsSource).Refresh();
        }

        private void addArtBtn_Click(object sender, RoutedEventArgs e)
        {
            var am = new ArticleManagement(jobToDisplay)
            {
                Owner = this
            };
            //ThemeManager.AddWindow(am);
            am.ShowDialog();
            //using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            //{
            //    sqlConn.Open();
            //    string query = "INSERT INTO dbo.DELAR (JOBBNR, RADNR, ARTNR, ANTAL, INPRIS, UTPRIS, RAD_TOTAL_IN, RAD_TOTAL_UT, TEXT, TEKN_LAGER, DATUM, KUNDNR, MNR, FAKTURA, ARTGRUPP, BEST_ANTAL, PRISENHET, LEV_DATUM) VALUES " +
            //                                         "(@jobbnr, @radnr, @artnr, @antal, @inpris, @utpris, @inpris, @utpris, @text, @tekn_lager, @datum, @kundnr, @mnr, @faktura, @artgrupp, @best_antal, @prisenhet, @datum)";
            //    using (var sqlCommand = new SqlCommand(query, sqlConn))
            //    {
            //        sqlCommand.Parameters.AddWithValue("@jobbnr", jobToDisplay.JobID);
            //        sqlCommand.Parameters.AddWithValue("@radnr", 1);
            //        sqlCommand.Parameters.AddWithValue("@artnr", "80000");
            //        sqlCommand.Parameters.AddWithValue("@antal", 1);
            //        sqlCommand.Parameters.AddWithValue("@inpris", 0);
            //        sqlCommand.Parameters.AddWithValue("@utpris", 5000);
            //        sqlCommand.Parameters.AddWithValue("@text", "Geggamojja"); //max30
            //        sqlCommand.Parameters.AddWithValue("@tekn_lager", "L");
            //        sqlCommand.Parameters.AddWithValue("@datum", DateTime.Now);
            //        sqlCommand.Parameters.AddWithValue("@kundnr", jobToDisplay.CustomerID);  // max 30chars
            //        sqlCommand.Parameters.AddWithValue("@mnr", jobToDisplay.SiteID);
            //        sqlCommand.Parameters.AddWithValue("@faktura", 1);
            //        sqlCommand.Parameters.AddWithValue("@artgrupp", "Tjä");
            //        sqlCommand.Parameters.AddWithValue("@best_antal", 1.0);
            //        sqlCommand.Parameters.AddWithValue("@prisenhet", "St");


            //        sqlCommand.ExecuteNonQuery();
            //    }
            //}
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
                //DialogResult = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var desc = jobToDisplay.CompleteJobDescription;
            try
            {
                Outlook.Application oApp = new Outlook.Application();

                Outlook.MailItem oMailItem = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
                var mapsLink = string.Format("https://www.google.se/maps/search/{0}+{1}+{2}", jobToDisplay.SiteName, jobToDisplay.Address, jobToDisplay.City);

                oMailItem.HTMLBody = string.Format(
                    "Here's a job for you!" +
                    "<br><br>" +
                    "{9}" +
                    "<br><br>" +
                    "{1}<br>" +
                    "{2}<br>" +
                    "{3}, {4}<br>" +
                    "<a href='{8}'><img src=\"https://portal.tryggconnect.se/api/googleMapsImage/create.php?location={8}&size=300x250&zoom=11\"></a>" +
                    "<br><br>" +
                    "<a href='mailto:{5}'>{6}</a> - <a href='tel:{7}'>{7}</a>",
                    jobToDisplay.JobID, jobToDisplay.SiteName, jobToDisplay.Address, jobToDisplay.PostNumber,
                    jobToDisplay.City, jobToDisplay.RefEmailAddress, jobToDisplay.RefName, jobToDisplay.RefPhoneNumber, mapsLink, jobToDisplay.CompleteJobDescription.Replace("\r\n", "<br>"));
                oMailItem.Subject = "Job: " + jobToDisplay.SiteName + " [" + jobToDisplay.JobID + "]";
                oMailItem.Display(false);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void jobCompletedChkBox_Checked(object sender, RoutedEventArgs e)
        {
            SetJobStatus("U");
        }

        private void jobCompletedChkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetJobStatus("A");
        }

        private void SetJobStatus(string status)
        {
            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();
                string query = "UPDATE dbo.JOBB SET STATUS=@status WHERE JOBBNR=@jobbnr";
                using (var sqlCommand = new SqlCommand(query, sqlConn))
                {
                    sqlCommand.Parameters.AddWithValue("@jobbnr", jobToDisplay.JobID);
                    sqlCommand.Parameters.AddWithValue("@status", status);

                    sqlCommand.ExecuteNonQuery();
                }

                sqlConn.Close();
            }
            ModifiedJob = true;
        }

        private void copyAddressBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(addressTxtBox.Text);
            AnimateThumbsUp(copyAddressBtn);
        }

        ImageSource GetImageFromAssembly(string psResourceName)
        {
            Uri oUri = new Uri("pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/" + psResourceName, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(oUri);
        }

        private void AnimateThumbsUp(Image image)
        {
            Duration fadeTime = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            var thumb = GetImageFromAssembly("Resources/like.png");
            var originalSource = image.Source;


            var fadeOutAnimation = new DoubleAnimation(0d, fadeTime);
            var fadeInAnimation = new DoubleAnimation(1d, fadeTime);

            fadeInAnimation.Completed += (o , e) => 
            {
                Thread.Sleep(1000);

                var newFadeOut = new DoubleAnimation(0d, fadeTime);
                var newFadeIn = new DoubleAnimation(1d, fadeTime.Add(new Duration(new TimeSpan(0,0,0,0,100))));
                newFadeOut.Completed += (o2, e2) =>
                {
                    image.Source = originalSource;
                    image.BeginAnimation(Image.OpacityProperty, newFadeIn);
                };

                image.BeginAnimation(Image.OpacityProperty, newFadeOut);
            };

            fadeOutAnimation.Completed += (o, e) =>
            {
                image.Source = thumb;
                image.BeginAnimation(Image.OpacityProperty, fadeInAnimation);
            };

            image.BeginAnimation(Image.OpacityProperty, fadeOutAnimation);

        }

        private void editRefEmailBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (jobToDisplay.JobStatus == JobStatusType.Archived)
                return;
            var me = editRefEmailBtn;
            var myBox = refEmail;
            if(modifyingRefEmail)
            {
                modifyingRefEmail = false;
                StopRefEdit(me, myBox, "EMAIL");
            }
            else
            {
                modifyingRefEmail = true;
                StartRefEdit(me, myBox, "EMAIL");
            }
        }

        public static void RemoveRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
        {
            // Get the EventHandlersStore instance which holds event handlers for the specified element.
            // The EventHandlersStore class is declared as internal.
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty(
                "EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);

            // If no event handlers are subscribed, eventHandlersStore will be null.
            // Credit: https://stackoverflow.com/a/16392387/1149773
            if (eventHandlersStore == null)
                return;

            // Invoke the GetRoutedEventHandlers method on the EventHandlersStore instance 
            // for getting an array of the subscribed event handlers.
            var getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod(
                "GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(
                eventHandlersStore, new object[] { routedEvent });

            // Iteratively remove all routed event handlers from the element.
            foreach (var routedEventHandler in routedEventHandlers)
                element.RemoveHandler(routedEvent, routedEventHandler.Handler);
        }
    }

    [Serializable]
    public class TimeReport
    {
        public string Technician { get; set; }
        public string Comment { get; set; }
        public int JobID { get; set; }
        public bool ArchivedJob { get; set; }
        public bool Traktamente { get; set; }

        public DateTime StartDate { get; set; }
        public float WorkTime { get; set; }
        public float TravelTime { get; set; }

        public string SiteName { get; set; }

        public int UniqueID { get; set; }

        public string JobType { get; set; }

        public enum JobTypes
        {
            I = 1,
            S = 2,
            F = 3,
            D = 4,
            P = 5,
            U = 6
        }
    }

}
