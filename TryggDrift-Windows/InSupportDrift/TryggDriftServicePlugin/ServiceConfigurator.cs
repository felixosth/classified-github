using InSupport.Drift.Plugins;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;

namespace InSupportDrift
{
    public partial class ServiceConfigurator : BasePluginConfig
    {
        BindingSource bs = new BindingSource();
        List<ListBoxService> allServices = new List<ListBoxService>(), visibleServices = new List<ListBoxService>();

        public ServiceConfigurator()
        {
            InitializeComponent();

            var services = ServiceController.GetServices().OrderBy(x => x.DisplayName).ToList();
            foreach (var s in services)
            {
                allServices.Add(new ListBoxService(s));
                visibleServices.Add(new ListBoxService(s));
            }

            bs.DataSource = visibleServices;
            servicesListBox.DataSource = bs;
            servicesListBox.DisplayMember = "Display";

            selectedServicesListBox.DisplayMember = "Display";

            servicesListBox.DoubleClick += (s, e) =>
            {
                if (servicesListBox.SelectedItem != null)
                {
                    foreach (ListBoxService service in selectedServicesListBox.Items)
                    {
                        if (service == servicesListBox.SelectedItem as ListBoxService)
                        {
                            return;
                        }
                    }
                    selectedServicesListBox.Items.Add(servicesListBox.SelectedItem);
                }
            };

            selectedServicesListBox.DoubleClick += (s, e) =>
            {
                if (selectedServicesListBox.SelectedItem != null)
                {
                    selectedServicesListBox.Items.Remove(selectedServicesListBox.SelectedItem);
                }
            };
        }

        public override bool ValidateForm()
        {
            if (selectedServicesListBox.Items.Count < 1)
            {
                if (MessageBox.Show("Are you sure you want to continue without any services?", "Configurator", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return false;
                }
            }

            return true;
        }

        public override string[] GetSettings() => new string[] { "Service=" + string.Join(",", selectedServicesListBox.Items.Cast<ListBoxService>()) };

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            visibleServices.Clear();
            for (int i = 0; i < allServices.Count; i++)
            {
                if (allServices[i].Service.DisplayName.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    visibleServices.Add(allServices[i]);
                }
                else
                {
                    visibleServices.Remove(allServices[i]);
                }
            }
            bs.ResetBindings(false);
        }
    }

    internal class ListBoxService
    {
        public ServiceController Service { get; set; }
        public string Display => Service.DisplayName;
        public override string ToString() => Service.ServiceName;

        public ListBoxService(ServiceController service)
        {
            this.Service = service;
        }
    }
}
