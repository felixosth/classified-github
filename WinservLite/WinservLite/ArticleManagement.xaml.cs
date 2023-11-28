using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ArticleManagement.xaml
    /// </summary>
    public partial class ArticleManagement : Window
    {
        Job jobToDisplay;
        List<WSArticle> articles = new List<WSArticle>();
        public ArticleManagement(Job jobToDisplay)
        {
            InitializeComponent();
            this.jobToDisplay = jobToDisplay;

            articlesListView.Items.Clear();
            RefreshArticles();

            //articleQntTxtBox.Select(0, 0);
        }

        private void RefreshArticles()
        {
            articles.Clear();
            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();

                string query = "SELECT RECNUM, TEXT, UTPRIS, ANTAL FROM dbo.DELAR WHERE JOBBNR=@jobbnr";
                using (var sqlCmd = new SqlCommand(query, sqlConn))
                {
                    sqlCmd.Parameters.AddWithValue("@jobbnr", jobToDisplay.JobID);
                    var reader = sqlCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        articles.Add(new WSArticle()
                        {
                            RecNum = (int)reader.GetInt64(0),
                            ArticleText = reader.GetString(1),
                            ArticlePrice = reader.GetDecimal(2),
                            Quantity = reader.GetDecimal(3)
                        });
                    }
                    reader.Close();
                }
                sqlConn.Close();
            }

            articlesListView.ItemsSource = articles;

            ICollectionView view = CollectionViewSource.GetDefaultView(articlesListView.ItemsSource);
            view.Refresh();
        }

        private void editReportBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void deleteReportBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this article?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
                return;

            var article = (GetDependencyObjectFromVisualTree(e.OriginalSource as DependencyObject, typeof(ListViewItem)) as ListViewItem).Content as WSArticle;

            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();

                string query = "DELETE FROM dbo.DELAR WHERE RECNUM=@recnum";
                using (var cmd = new SqlCommand(query, sqlConn))
                {
                    cmd.Parameters.Add("@recnum", SqlDbType.BigInt).Value = article.RecNum;
                    cmd.ExecuteNonQuery();
                }
                sqlConn.Close();
            }

            RefreshArticles();
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

        private void AddArticleBtn_Click(object sender, RoutedEventArgs e)
        {
            //check validation
            int quantity = 0;
            decimal price = 0;
            if(!int.TryParse(articleQntTxtBox.Text, out quantity))
            {
                MessageBox.Show("Invalid input: Quantity");
                return;
            }
            else if(quantity < 1)
            {
                MessageBox.Show("Invalid input: Quantity");
                return;
            }

            if (!decimal.TryParse(articlePriceTxtBox.Text, out price))
            {
                MessageBox.Show("Invalid input: Price");
                return;
            }
            else if(price < 0)
            {
                MessageBox.Show("Invalid input: Price");
                return;
            }

            if (string.IsNullOrEmpty(articleTextTxtBox.Text) || string.IsNullOrWhiteSpace(articleTextTxtBox.Text))
            {
                MessageBox.Show("Invalid input: Article text");
                return;
            }

            using (var sqlConn = new SqlConnection(MainWindow.SQLCONNSTRING))
            {
                sqlConn.Open();
                string query = "INSERT INTO dbo.DELAR (JOBBNR, RADNR, ARTNR, ANTAL, INPRIS, UTPRIS, RAD_TOTAL_IN, RAD_TOTAL_UT, TEXT, TEKN_LAGER, DATUM, KUNDNR, MNR, MTYP, FAKTURA, ARTGRUPP, KATEGORI, BEST_ANTAL, PRISENHET, LEV_DATUM) VALUES " +
                                                     "(@jobbnr, @radnr, @artnr, @antal, @inpris, @utpris, @inpris, @utpris, @text, @tekn_lager, @datum, @kundnr, @mnr, @mtyp, @faktura, @artgrupp, @kategori, @best_antal, @prisenhet, @datum)";
                using (var sqlCommand = new SqlCommand(query, sqlConn))
                {
                    sqlCommand.Parameters.AddWithValue("@jobbnr", jobToDisplay.JobID);
                    sqlCommand.Parameters.AddWithValue("@radnr", articles.Count+1);
                    sqlCommand.Parameters.AddWithValue("@artnr", WSArticle._DefArticleNumber);
                    sqlCommand.Parameters.AddWithValue("@antal", (decimal)quantity);
                    sqlCommand.Parameters.AddWithValue("@inpris", 0);
                    sqlCommand.Parameters.AddWithValue("@utpris", price);
                    sqlCommand.Parameters.AddWithValue("@text", articleTextTxtBox.Text); //max30
                    sqlCommand.Parameters.AddWithValue("@tekn_lager", "L");
                    sqlCommand.Parameters.AddWithValue("@datum", DateTime.Now);
                    sqlCommand.Parameters.AddWithValue("@kundnr", jobToDisplay.CustomerID);  // max 30chars
                    sqlCommand.Parameters.AddWithValue("@mnr", jobToDisplay.SiteID);
                    sqlCommand.Parameters.AddWithValue("@mtyp", 1);
                    sqlCommand.Parameters.AddWithValue("@faktura", 1);
                    sqlCommand.Parameters.AddWithValue("@artgrupp", "Tjä");
                    sqlCommand.Parameters.AddWithValue("@best_antal", quantity);
                    sqlCommand.Parameters.AddWithValue("@prisenhet", "St");

                    sqlCommand.Parameters.AddWithValue("@kategori", "02");
                    //sqlCommand.Parameters.AddWithValue("@prisenhet", "St");


                    sqlCommand.ExecuteNonQuery();
                }

                query = "UPDATE dbo.JOBB SET JOBB_RAD=@artiklar WHERE JOBBNR=@jobbnr";
                using (var sqlCommand = new SqlCommand(query, sqlConn))
                {
                    sqlCommand.Parameters.AddWithValue("@jobbnr", jobToDisplay.JobID);
                    sqlCommand.Parameters.AddWithValue("@artiklar", articles.Count + 1);
                    sqlCommand.ExecuteNonQuery();
                }
                sqlConn.Close();
            }

            articleTextTxtBox.Text = "";
            articleQntTxtBox.Text = "";
            articlePriceTxtBox.Text = "";
            RefreshArticles();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void articlePriceTxtBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            articlePriceTxtBox.Select(articlePriceTxtBox.Text.Length, 0);
        }

        private void articleQntTxtBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            articleQntTxtBox.Select(articleQntTxtBox.Text.Length, 0);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ThemeManager.RemoveWindow(this);
        }
    }

    public class WSArticle
    {
        public int RecNum { get; set; }
        public string ArticleText { get; set; }
        public decimal ArticlePrice { get; set; }
        public decimal Quantity { get; set; }

        public const string _DefArticleNumber = "80000";
    }
}
