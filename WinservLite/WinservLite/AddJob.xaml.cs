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

namespace WinservLite
{
    /// <summary>
    /// Interaction logic for AddJob.xaml
    /// </summary>
    public partial class AddJob : Window
    {
        Site selectedSite = null;
        List<Site> sites = new List<Site>();

        public int NewJobID { get; set; }

        public AddJob()
        {
            InitializeComponent();

            if(MainWindow.MyUser == "")
            {
                MessageBox.Show("No WinServ user specified. Go back and fix please :)", "Error");
                DialogResult = false;
            }

            //foreach(ListBoxItem item in techBox.Items)
            //{
            //    if (item.Content as string == MainWindow.MyUser)
            //        techBox.SelectedItem = item;
            //}
            techBox.Items.Clear();
            foreach (Technician tech in SQLFunctions.GetTechnicians(MainWindow.SQLCONNSTRING))
            {
                techBox.Items.Add(tech);
                if (tech.UserName == MainWindow.MyUser)
                    techBox.SelectedItem = tech;
            }

            try
            {
                using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
                {
                    sqlConn.Open();

                    string query = "SELECT MNR, INSTALL, KUNDNR, MTYP, MODELL, GATUADR, POSTNR, ORT FROM dbo.MASKINER";

                    SqlDataReader reader = new SqlCommand(query, sqlConn).ExecuteReader();

                    while (reader.Read())
                    {
                        sites.Add(new Site()
                        {
                            SiteID = reader.GetString(0),
                            Name = reader.GetString(1),
                            CustomerID = reader.GetString(2),
                            ModelType = reader.GetString(3),
                            Model = reader.GetString(4),
                            Address = reader.GetString(5),
                            PostNR = reader.GetString(6),
                            City = reader.GetString(7)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                if (MainWindow.IsDebug)
                    throw ex;
                MessageBox.Show(ex.ToString());
            }

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


            if (errors)
                return;

            try
            {
                var lines = new List<string>();
                for (int i = 0; i < descBox.LineCount; i++)
                {
                    lines.Add(descBox.GetLineText(i));
                }

                using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
                {
                    sqlConn.Open();

                    string user = MainWindow.MyUser;  // horrible hardcoding #1
                    if (user == "VIKLA")
                        user = "V";


                    NewJobID = 0;

                    using (var reader = new SqlCommand("SELECT JOBBNR from dbo.SYSFIL WHERE RECNUM=1", sqlConn).ExecuteReader())  // fetch next jobnumber in queue/count
                    {
                        while(reader.Read())
                        {
                            NewJobID = (int)reader.GetDecimal(0) + 1;
                        }
                    }

                    var jobbQuery = "INSERT into dbo.JOBB " +
                            "(MNR, KUNDNR, STATUS, INDAT, TEKN, REF, TEL, JOBBPRIO, [USER], JOBBNR, JOBBTYP, MTYP, MODELL, INSTALL, REG_AV, EMAIL, PNR, ORT, GATA, BETVILLK, BETVILLK_NAMN) VALUES " +
                            "(@mnr, @kundnr, 'A', '" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00.000") +"', @tekn, @ref, @tel, '2', @user, @jobbnr, @jobbtyp, @mtyp, @modell, " +
                            "@install, @regav, @email, @pnr, @ort, @gata, 'N', 'N')";

                    using (var cmd = new SqlCommand(jobbQuery, sqlConn)) 
                    {
                        var dspName = "WSLITE";
                        if (MainWindow.MyUser == "DEAH") // horrible hardcoding #2
                            dspName = "Dennis Ahlzén";
                        else if (MainWindow.MyUser == "KRHE")
                            dspName = "Kristoffer Heleander";
                        else
                        {
                            try
                            {
                                dspName = System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName;
                            }
                            catch { }
                        }

                        cmd.Parameters.AddWithValue("@mnr", selectedSite.SiteID);
                        cmd.Parameters.AddWithValue("@kundnr", selectedSite.CustomerID);
                        //cmd.Parameters.AddWithValue("@indat", DateTime.Now.ToString("yyyy-MM-dd 00:00:00.000"));
                        cmd.Parameters.AddWithValue("@tekn", (techBox.SelectedItem as Technician).UserName);
                        cmd.Parameters.AddWithValue("@ref", refName.Text);
                        cmd.Parameters.AddWithValue("@tel", refTel.Text);
                        cmd.Parameters.AddWithValue("@jobbtyp", (jobType.SelectedItem as ListBoxItem).Content.ToString()[0]);
                        cmd.Parameters.AddWithValue("@mtyp", selectedSite.ModelType);
                        cmd.Parameters.AddWithValue("@modell", selectedSite.Model);
                        cmd.Parameters.AddWithValue("@install", selectedSite.Name);
                        cmd.Parameters.AddWithValue("@regav", dspName);
                        cmd.Parameters.AddWithValue("@email", refEmail.Text);
                        cmd.Parameters.AddWithValue("@pnr", selectedSite.PostNR == "" ? "00000" : selectedSite.PostNR);
                        cmd.Parameters.AddWithValue("@ort", selectedSite.City == "" ? "Ej ifyllt" : selectedSite.City );
                        cmd.Parameters.AddWithValue("@gata", addressCheck == "" ? "Ej ifyllt" : addressCheck);
                        cmd.Parameters.AddWithValue("@user", user);
                        cmd.Parameters.AddWithValue("@jobbnr", (double)NewJobID);

                        cmd.ExecuteNonQuery();
                    }

                    var jobbFtQuery = "INSERT into dbo.JOBBFT (JOBBNR, TEXTRAD, RAD) VALUES";
                    using (var cmd = new SqlCommand(jobbFtQuery, sqlConn))  //JOBBFT
                    {
                        cmd.Parameters.AddWithValue("@jobbnr", (double)NewJobID);

                        for (int i = 0; i < lines.Count; i++)
                        {
                            cmd.CommandText += " (@jobbnr, @textrad" + i + ", @rad" + i + ")";
                            if (i + 1 != lines.Count)
                                cmd.CommandText += ",";

                            cmd.Parameters.AddWithValue("@textrad" + i, lines[i]);
                            cmd.Parameters.AddWithValue("@rad" + i, i + 1);
                        }

                        cmd.ExecuteNonQuery();
                    }

                    new SqlCommand("UPDATE dbo.SYSFIL SET JOBBNR=" + (double)(NewJobID) + "WHERE RECNUM=1", sqlConn).ExecuteNonQuery();  // update jobbnr count

                    sqlConn.Close();
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //DialogResult = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                DialogResult = false;
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

    public class Site
    {
        public string Name { get; set; } // INSTALL
        public string SiteID { get; set; }
        public string CustomerID { get; set; }

        public string ModelType { get; set; }  // MTYP
        public string Model { get; set; } // MODELL

        public string PostNR { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }
}
