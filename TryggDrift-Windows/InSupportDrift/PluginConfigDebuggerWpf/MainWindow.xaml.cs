using InSupport.Drift.Monitor;
using InSupport.Drift.Plugins.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace PluginConfigDebuggerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BasePluginConfigWpf basePluginConfig;
        BaseMonitor basePlugin;

        public MainWindow()
        {
            InitializeComponent();

            SetupMonitor();

            basePluginConfig = new InSupportDriftMilestonePlugin.MilestoneConfigUserControlWpf();

            //loadBtn_Click(this, new RoutedEventArgs());
            //grid.Children.Add(basePluginConfig);

            grid.Children.Add(basePluginConfig);
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (basePlugin != null)
                basePlugin.Close();
        }

        private void BasePlugin_OnLog(object sender, string e)
        {
            MessageBox.Show(e);
        }

        private void SetupMonitor()
        {
            if (basePlugin != null)
            {
                basePlugin.OnLog -= BasePlugin_OnLog;
                basePlugin.Close();
            }

            //basePlugin = new InSupport.Drift.Plugins.PerformanceMonitor();
            basePlugin = new InSupport.Drift.Plugins.MilestoneMonitor();

            basePlugin.OnLog += BasePlugin_OnLog;
        }

        private void verifyBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Verify result: " + basePluginConfig.VerifySettings());
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(basePluginConfig.GetSettings(), Formatting.Indented));
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            SetupMonitor();
            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("config.json"));

            try
            {
                basePluginConfig.LoadSettings(settings);
                basePlugin.LoadSettings(settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            var varia = 55;

            var text = varia > 60 ? "mer än 60" : " mindre än 60";
        }

        private void collectDataBtn_Click(object sender, RoutedEventArgs e)
        {
            var data = JsonConvert.SerializeObject(basePlugin, Formatting.Indented);
            var showDataWindow = new ShowDataWindow() { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            showDataWindow.textBlock.Text = data;
            showDataWindow.ShowDialog();

        }
    }
}
