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
using WinServLib.Objects;
using WinServLite2;
using WinServLite2.CloseableTab;

namespace WinServLite2.Jobs
{
    /// <summary>
    /// Interaction logic for AddJob.xaml
    /// </summary>
    public partial class AddJob : DynamicUserControl
    {
        Site selectedSite = null;
        List<Site> sites;

        public int NewJobID { get; set; }
        JobBrowser jb;

        public AddJob(JobBrowser jb)
        {
            InitializeComponent();
            this.jb = jb;

            techBox.Items.Clear();
            foreach (Technician tech in WinServLib.WinServ.GetTechnicians())
            {
                techBox.Items.Add(tech);
                if (tech.UserName == (string)MainWindow.Settings["user"])
                    techBox.SelectedItem = tech;
            }

            sites = WinServLib.WinServ.GetSites().ToList();

            var jobTypes = WinServLib.WinServ.GetJobTypes();
            jobType.ItemsSource = jobTypes;

            textBox.Focus();
        }

        

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && resultStack.Children.Count > 0)
            {
                for (int i = 0; i < resultStack.Children.Count; i++)
                {
                    if ((resultStack.Children[i] as CustomTextBlock).IsSelected && (i + 1) != resultStack.Children.Count)
                    {
                        (resultStack.Children[i] as CustomTextBlock).UnSelect();
                        (resultStack.Children[i + 1] as CustomTextBlock).Select();

                        if (i > 12)
                        {
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 22);
                        }

                        break;
                    }
                }
            }
            else if (e.Key == Key.Up && resultStack.Children.Count > 0)
            {
                for (int i = 0; i < resultStack.Children.Count; i++)
                {
                    if ((resultStack.Children[i] as CustomTextBlock).IsSelected && (i - 1) >= 0)
                    {
                        (resultStack.Children[i] as CustomTextBlock).UnSelect();
                        (resultStack.Children[i - 1] as CustomTextBlock).Select();


                        if (i < resultStack.Children.Count - 13)
                        {
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 22);
                            //MessageBox.Show(hej.ToString());
                        }
                        break;
                    }
                }
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            //var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            
            if (e.Key == Key.Enter)
            {
                foreach (CustomTextBlock txtBlock in resultStack.Children)
                {
                    if (txtBlock.IsSelected)
                    {
                        textBox.Text = txtBlock.Text;
                        selectedSite = txtBlock.Site;
                        siteNoBox.Text = selectedSite.SiteID;
                        txtBlock.UnSelect();
                        suggestionBorder.Visibility = Visibility.Collapsed;
                        descBox.Focus();
                    }
                }
            }
            else if (e.Key != Key.Up && e.Key != Key.Down)
            {
                bool found = false;
                var data = sites;

                string query = (sender as TextBox).Text;
                siteNoBox.Text = "";
                selectedSite = null;

                if (query.Length == 0)
                {
                    // Clear   
                    resultStack.Children.Clear();
                    suggestionBorder.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    suggestionBorder.Visibility = System.Windows.Visibility.Visible;
                }

                // Clear the list   
                resultStack.Children.Clear();

                // Add the result   
                foreach (var obj in data)
                {
                    if (obj.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 || obj.SiteID.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // The word starts with this... Autocomplete must work   
                        addItem(obj);
                        found = true;
                    }
                }

                if (!found)
                {
                    resultStack.Children.Add(new TextBlock() { Text = "No results found." });
                }
                else
                    (resultStack.Children[0] as CustomTextBlock).Select();
            }
        }

        private void addItem(Site site)
        {
            CustomTextBlock block = new CustomTextBlock();

            block.Site = site;

            // Add the text   
            block.Text = /*"(" + site.SiteID + ") " +  */site.Name;

            // A little style...   
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;

            // Mouse events   
            block.MouseLeftButtonUp += (sender, e) =>
            {
                textBox.Text = (sender as CustomTextBlock).Text;
                selectedSite = (sender as CustomTextBlock).Site;

                var border = (resultStack.Parent as ScrollViewer).Parent as Border;
                border.Visibility = Visibility.Collapsed;
                siteNoBox.Text = selectedSite.SiteID;
            };

            block.MouseEnter += (sender, e) =>
            {
                //b.Background = Brushes.PeachPuff;
                foreach (CustomTextBlock txtBlock in resultStack.Children)
                {
                    txtBlock.UnSelect();
                }

                (sender as CustomTextBlock).Select();
            };

            block.MouseLeave += (sender, e) =>
            {
                (sender as CustomTextBlock).UnSelect();
            };

            // Add to the panel   
            resultStack.Children.Add(block);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            errors = false;
            if(selectedSite == null)
            {
                Invalidate(textBox);
                MessageBox.Show("Please select a site", "No site selected");
            }
            if(descBox.Text == "")
            {
                Invalidate(descBox);
                MessageBox.Show("Please input a description", "No description");
            }
            if(refName.Text == "")
            {
                Invalidate(refName);
                MessageBox.Show("Please input a reference name", "No reference name");
            }
            if(refTel.Text == "")
            {
                Invalidate(refTel);
                MessageBox.Show("Please input a reference tel", "No reference tel");
            }

            if(techBox.SelectedItem == null)
            {
                errors = true;
                MessageBox.Show("Please specify technician","No technician");
            }


            if (!errors)
            {
                var addressCheck = selectedSite.Address;
                if (addressCheck.Length > 40)
                {
                    if (MessageBox.Show("The site address contains too many characters. The address row can only contain 40 characters. (Current char count: " + addressCheck.Length + "/40)\r\n\r\nDo you want to continue with a incomplete address?\r\n(Select No to cancel)",
                        "Char count error", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        addressCheck = addressCheck.Remove(40, addressCheck.Length - 40);
                    }
                    else
                        errors = true;
                }
            }


            if (errors)
                return;

            try
            {
                var lines = new List<string>();
                for (int i = 0; i < descBox.LineCount; i++)
                {
                    lines.Add(descBox.GetLineText(i));
                }


                var me = WinServLib.WinServ.GetTechnicians().FirstOrDefault(t => t.UserName == MainWindow.Settings["user"] as string);

                var dspName = me != null ? me.Name : "WSLITE";

                var createdJob = WinServLib.WinServ.AddJob(MainWindow.Settings["user"] as string, dspName, new Job()
                {
                    Technician = (techBox.SelectedItem as Technician).UserName,
                    RefName = refName.Text,
                    RefEmailAddress = refEmail.Text,
                    RefPhoneNumber = refTel.Text,
                    JobType = (jobType.SelectedItem as JobType).NR,
                }, lines, selectedSite);


                jb.RefreshList();
                jb.OpenJob(createdJob);
                RequestClose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Escape)
            //    DialogResult = false;
        }

        bool errors = false;
        private void Invalidate(TextBox txtBox)
        {
            errors = true;
            txtBox.BorderBrush = new SolidColorBrush(Colors.Red);
            txtBox.TextChanged += new TextChangedEventHandler((s, e) =>
            {
                if (txtBox.Text != "")
                    Validate(txtBox);
            });
        }

        private void Validate(TextBox txtBox)
        {
            txtBox.BorderBrush = new SolidColorBrush(Colors.Green);
            //txtBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 171, 173, 179));
        }

        private void DynamicUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            textBox.Focus();
        }
    }

    class CustomTextBlock : TextBlock
    {
        public Site Site { get; set; }
        private bool isSlct;
        public bool IsSelected { get { return isSlct; } }
        public void Select()
        {
            Background = Brushes.PeachPuff;
            isSlct = true;
        }
        public void UnSelect()
        {
            Background = Brushes.Transparent;
            isSlct = false;
        }
    }
}
