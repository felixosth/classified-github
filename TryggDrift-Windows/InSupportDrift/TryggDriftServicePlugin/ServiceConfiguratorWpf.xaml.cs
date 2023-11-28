using InSupport.Drift.Plugins;
using InSupport.Drift.Plugins.Wpf;
using InSupportDrift;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace TryggDriftServicePlugin
{
    /// <summary>
    /// Interaction logic for ServiceConfiguratorWpf.xaml
    /// </summary>
    public partial class ServiceConfiguratorWpf : BasePluginConfigWpf
    {
        private readonly List<ListBoxService> allServices = new List<ListBoxService>();
        private readonly ObservableCollection<ListBoxService> selectedServices;
        private ServiceMonitorConfig cfg = new ServiceMonitorConfig();

        string[] existingServices = new string[0];

        public ServiceConfiguratorWpf()
        {
            InitializeComponent();

            selectedServices = new ObservableCollection<ListBoxService>();
        }

        public override void LoadSettings(Dictionary<string, string> config)
        {
            if (config.ContainsKey(ServiceMonitorManager._CFG_KEY))
            {
                try
                {
                    cfg = JsonConvert.DeserializeObject<ServiceMonitorConfig>(config[ServiceMonitorManager._CFG_KEY]);
                    existingServices = cfg.ServicesToMonitor.ToArray();
                    autostartServicesChkBox.IsChecked = cfg.AutoStartStoppedServices;
                }
                catch
                {
                    MessageBox.Show("Failed to load existing settings");
                }
            }
        }

        public override bool VerifySettings()
        {
            return true;
        }

        public override Dictionary<string, string> GetSettings()
        {
            return new Dictionary<string, string>()
            {
                {
                    ServiceMonitorManager._CFG_KEY, JsonConvert.SerializeObject(new ServiceMonitorConfig()
                    {
                        ServicesToMonitor = selectedServices.Select(service => service.ToString()),
                        AutoStartStoppedServices = autostartServicesChkBox.IsChecked == true,
                        AutoStartServiceAfterMinutes = cfg.AutoStartServiceAfterMinutes
                    })
                }
            };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var s in ServiceController.GetServices().OrderBy(x => x.DisplayName))
            {
                allServices.Add(new ListBoxService(s));
                if (existingServices != null && existingServices.Contains(s.ServiceName))
                    selectedServices.Add(new ListBoxService(s));
            }

            allServicesListBox.ItemsSource = allServices;
            selectedServicesListBox.ItemsSource = selectedServices;

            CollectionView view = CollectionViewSource.GetDefaultView(allServicesListBox.ItemsSource) as CollectionView;
            view.Filter = ServicesSearchFilter;
        }

        private bool ServicesSearchFilter(object obj)
        {
            if (string.IsNullOrEmpty(allServicesFilterTxtBox.Text))
                return !selectedServices.Any(lbs => lbs.Service.ServiceName == obj.ToString());
            else
                return
                    !selectedServices.Any(lbs => lbs.Service.ServiceName == obj.ToString()) &&
                    (obj.ToString().IndexOf(allServicesFilterTxtBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void AllServicesTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var service = (sender as TextBlock).DataContext as ListBoxService;
                if (!selectedServices.Contains(service))
                {
                    selectedServices.Add(service);
                    CollectionViewSource.GetDefaultView(allServicesListBox.ItemsSource).Refresh();
                }
            }
        }

        private void SelectedServicesTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                selectedServices.Remove((sender as TextBlock).DataContext as ListBoxService);
                CollectionViewSource.GetDefaultView(allServicesListBox.ItemsSource).Refresh();
            }
        }

        private void allServicesFilterTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(allServicesListBox.ItemsSource).Refresh();
        }
    }
}
