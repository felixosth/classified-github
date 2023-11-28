using Newtonsoft.Json;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WinServLite2
{
    /// <summary>
    /// Interaction logic for FirstTimeSetupWindow.xaml
    /// </summary>
    public partial class FirstTimeSetupWindow : Window
    {
        public FirstTimeSetupWindow()
        {
            InitializeComponent();
            usrGrid.Visibility = Visibility.Hidden;
            this.Title = $"WinServ Lite v{MainWindow.Version}";
            headerLabel.Content = "Välkommen till WinServ Lite v" + MainWindow.Version;
        }

        bool Validate()
        {
            try
            {
                WinServLib.WinServ.Initialize($"Data Source={serverTxtBox.Text},{portTxtBox.Text};Initial Catalog={databaseTxtBox.Text};User ID={usernameTxtBox.Text};Password={passwordTxtBox.Password};");
                WinServLib.WinServ.GetTechnicians();
            }
            catch
            {
                MessageBox.Show("Invalid server settings.");
                return false;
            }

            return true;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Validate() && saveBtn.Content as string == "Anslut")
            {
                saveBtn.Content = "Till havs!";
                usrComboBox.ItemsSource = WinServLib.WinServ.GetTechnicians();
                usrComboBox.SelectedIndex = 0;

                serverTxtBox.IsEnabled = false;
                portTxtBox.IsEnabled = false;
                databaseTxtBox.IsEnabled = false;
                usernameTxtBox.IsEnabled = false;
                passwordTxtBox.IsEnabled = false;
                useAutoSearchChkBox.IsEnabled = false;
                copyFromClipboardBtn.IsEnabled = false;
                FadeIn(usrGrid);
            }
            else
            {
                Dictionary<string, object> properties = new Dictionary<string, object>()
                {
                    { "server", serverTxtBox.Text },
                    { "port", portTxtBox.Text },
                    { "database", databaseTxtBox.Text },
                    { "username", usernameTxtBox.Text },
                    { "password", passwordTxtBox.Password },
                    { "autosearch", useAutoSearchChkBox.IsChecked },
                    { "user", usrComboBox.Text }
                };

                new Config(MainWindow.CompanyName).SaveConfig(properties);
                new MainWindow().Show();
                Close();
            }

            void FadeIn(UIElement element)
            {
                element.Opacity = 0;
                element.Visibility = Visibility.Visible;
                var a = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    FillBehavior = FillBehavior.HoldEnd,
                    Duration = new Duration(TimeSpan.FromSeconds(0.5))
                };
                var storyboard = new Storyboard();

                storyboard.Children.Add(a);
                Storyboard.SetTarget(a, element);
                Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
                storyboard.Begin();
            }
        }

        private void CopyFromClipboardBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Clipboard.ContainsText())
            {
                try
                {
                    string clipboardText = Clipboard.GetText(TextDataFormat.Text);

                    foreach(var obj in JsonConvert.DeserializeObject<Dictionary<string, string>>(clipboardText))
                    {
                        switch(obj.Key)
                        {
                            case "server":
                                serverTxtBox.Text = obj.Value;
                                break;
                            case "port":
                                portTxtBox.Text = obj.Value;
                                break;
                            case "database":
                                databaseTxtBox.Text = obj.Value;
                                break;
                            case "username":
                                usernameTxtBox.Text = obj.Value;
                                break;
                            case "password":
                                passwordTxtBox.Password = obj.Value;
                                break;
                            case "autosearch":
                                useAutoSearchChkBox.IsChecked = bool.Parse(obj.Value);
                                break;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Hoppsan, det fungerade minsann inte!", ":(");
                }
            }
        }
    }
}
