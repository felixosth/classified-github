using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace DigitalRevision
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DataSourceManager dataSourceManager = new DataSourceManager();

        public MainWindow()
        {
            InitializeComponent();

            dataSourcesListView.ItemsSource = dataSourceManager.DataSources;
        }


        private async void collectDataBtn_Click(object sender, RoutedEventArgs e)
        {
            collectDataBtn.IsEnabled = false;

            string zipFilePath = DataSourceManager.DefaultZipPath();

#if DEBUG
            zipFilePath = null;
#else
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "Zip|*.zip",
                InitialDirectory = Path.GetDirectoryName(zipFilePath),
                FileName = Path.GetFileName(zipFilePath)
            };

            // Let the user choose filepath and let them cancel
            if (saveFileDialog.ShowDialog() == true)
            {
                zipFilePath = saveFileDialog.FileName;
            }   
            else
            {
                return;
            }   
#endif

            var dataSourcesContentsPath = await dataSourceManager.CollectAllData(zipFilePath);

#if DEBUG
            Process.Start("explorer.exe", $"\"{dataSourcesContentsPath}\"");

#else
            // Prompt the user to view the zip
            if (MessageBox.Show("Digital revision slutförd. Vill du öppna mappen med .zip-filen?", "Digital revision", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Process.Start("explorer.exe", $"/select,\"{dataSourcesContentsPath}\"");
            }
#endif
            collectDataBtn.IsEnabled = true;
        }



        private void DataSourceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Disable our collect data btn if all data sources are disabled
            collectDataBtn.IsEnabled = dataSourceManager.DataSources.Any(ds => ds.IsEnabled);
        }
    }
}
