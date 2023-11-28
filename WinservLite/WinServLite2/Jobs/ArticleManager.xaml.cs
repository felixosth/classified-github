using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinServLib.Objects;
using static WinServLib.Objects.Job;

namespace WinServLite2.Jobs
{
    /// <summary>
    /// Interaction logic for ArticleManager.xaml
    /// </summary>
    public partial class ArticleManager : UserControl
    {
        Job jobToDisplay;
        List<Article> articles;

        internal List<Article> LocalArticles => articles;

        public event EventHandler OnUserChange;

        public void Init(Job job)
        {
            jobToDisplay = job;
            RefreshArticles();
        }

        public ArticleManager()
        {
            InitializeComponent();

            articlesListView.Items.Clear();
        }

        private void RefreshArticles()
        {
            articles = new List<Article>(jobToDisplay.Articles);
            articlesListView.ItemsSource = articles;

            CollectionViewSource.GetDefaultView(articlesListView.ItemsSource).Refresh();
        }


        private void deleteReportBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this article?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
                return;

            var article = (GetDependencyObjectFromVisualTree(e.OriginalSource as DependencyObject, typeof(ListViewItem)) as ListViewItem).Content as Article;
            articles.Remove(article);
            OnUserChange?.Invoke(this, e);
            CollectionViewSource.GetDefaultView(articlesListView.ItemsSource).Refresh();
        }

        public void Disable()
        {
            articlesListView.IsEnabled = false;
            addArticleBtn.IsEnabled = false;
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
            if (!int.TryParse(articleQntTxtBox.Text, out quantity))
            {
                MessageBox.Show("Invalid input: Quantity");
                return;
            }
            else if (quantity < 1)
            {
                MessageBox.Show("Invalid input: Quantity");
                return;
            }

            if (!decimal.TryParse(articlePriceTxtBox.Text, out price))
            {
                MessageBox.Show("Invalid input: Price");
                return;
            }
            else if (price < 0)
            {
                MessageBox.Show("Invalid input: Price");
                return;
            }

            if (string.IsNullOrEmpty(articleTextTxtBox.Text) || string.IsNullOrWhiteSpace(articleTextTxtBox.Text))
            {
                MessageBox.Show("Invalid input: Article text");
                return;
            }

            articles.Add(new Article()
            {
                Quantity = quantity,
                ArticlePrice = price,
                ArticleText = articleTextTxtBox.Text
            });

            articleTextTxtBox.Text = "";
            articleQntTxtBox.Text = "1";
            articlePriceTxtBox.Text = "99999";
            OnUserChange?.Invoke(this, e);
            //RefreshArticles();
            CollectionViewSource.GetDefaultView(articlesListView.ItemsSource).Refresh();
        }

        private void articlePriceTxtBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            articlePriceTxtBox.Select(0, articlePriceTxtBox.Text.Length);
        }

        private void articleQntTxtBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            articleQntTxtBox.Select(0, articleQntTxtBox.Text.Length);
        }
    }
}
