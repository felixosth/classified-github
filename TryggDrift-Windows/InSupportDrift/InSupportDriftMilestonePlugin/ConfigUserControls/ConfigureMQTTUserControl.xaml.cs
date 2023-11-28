using InSupport.Drift.Plugins;
using InSupportDriftMilestonePlugin.ConfigUserControls.MQTT;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace InSupportDriftMilestonePlugin.ConfigUserControls
{
    /// <summary>
    /// Interaction logic for ConfigureMQTTUserControl.xaml
    /// </summary>
    public partial class ConfigureMQTTUserControl : UserControl
    {
        private readonly ObservableCollection<BonjourAxisDeviceListViewItem> devices = new ObservableCollection<BonjourAxisDeviceListViewItem>();
        private readonly BonjourSearcher bonjourSearcher = new BonjourSearcher();
        private readonly IEnumerable<IPAddress> hostIps;

        private readonly MqttBroker mqttBroker = new MqttBroker();

        private readonly Version embeddedSIMQTTVersion = new Version("1.2.0");


        public ConfigureMQTTUserControl()
        {
            InitializeComponent();

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });


            StartBonjour();

            devicesListView.ItemsSource = devices;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(devicesListView.ItemsSource);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("IPAddress", System.ComponentModel.ListSortDirection.Ascending));


            // Get interfaces
            IPHostEntry iphostentry = Dns.GetHostEntry(Dns.GetHostName());
            hostIps = iphostentry.AddressList.Where(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            interfacesComboBox.ItemsSource = hostIps;
            if (hostIps.Count() > 0)
                interfacesComboBox.SelectedIndex = 0;
        }

        private void BonjourSearcher_OnDevicesFound(object sender, System.Collections.Generic.IEnumerable<BonjourAxisDevice> e)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var newDevice in e)
                {
                    devices.Add(new BonjourAxisDeviceListViewItem(newDevice));
                }

                allDevicesCheckBox_CheckedChanged(this, new RoutedEventArgs());
            });
        }

        private void BonjourSearcher_OnDeviceChanged(object sender, BonjourAxisDevice e)
        {
            var device = devices.FirstOrDefault(d => d.MacAddress == e.MacAddress);

            if (device != null)
                device.BonjourAxisDevice.Host = e.Host;
        }

        private async void configureCamerasBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to configure these cameras to use MQTT?", "Sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                configureCamerasBtn.IsEnabled = false;
                checkBtn.IsEnabled = false;
                StopBonjour();

                try
                {
                    // Try to start a temporary MQTT broker so the cameras are able to connect after configuration.
                    // (as of 2021-09-15, the cameras will never try to reconnect if the initial connect failed. Fred Juhlin (SIMQTT Author) might change this in the future.)
                    await mqttBroker.StartServerAsync();
                }
                catch { /* A MQTT server is already running */ }

                string username = usrBox.Text;
                string password = pwdBox.Password;
                string ipaddress = ((IPAddress)interfacesComboBox.SelectedItem).ToString();

                var tasks = devices.Where(d => d.IsChecked)
                    .Select(d => ConfigureDevice(d, username, password, ipaddress));

                await Task.WhenAll(tasks);
                configureCamerasBtn.IsEnabled = true;
                checkBtn.IsEnabled = true;
                StartBonjour();
            }
        }

        private async Task ConfigureDevice(BonjourAxisDeviceListViewItem device, string username, string password, string ipToServer)
        {
            device.IsConfiguring = true;
            device.CheckboxEnabled = false;
            device.StatusText = "Fetching information";
            device.IsConfiguring = true;

            Propertylist devicePropertyList = default;

            try
            {
                // Get the device parameters
                devicePropertyList = await device.BonjourAxisDevice.GetBasicDataInfoAsync(username, password);
            }
            catch (UnauthorizedAccessException) // Will throw if unauthorized
            {
                device.StatusText = "Wrong password!";
                device.IsConfiguring = false;
                return;
            }
            catch // something else went wrong?
            {
                device.StatusText = "Failed!";
                device.IsConfiguring = false;

                return;
            }

            // List all installed acaps
            var acaps = await device.BonjourAxisDevice.GetApplicationListAsync(username, password, devicePropertyList.UseHttps);

            // Check if SIMQTT is installed alredy
            var installedSIMQTTAcap = acaps.application.FirstOrDefault(acap => acap.Name == "simqtt");

            Version installedSimqttVersion = installedSIMQTTAcap == null ? null : new Version(installedSIMQTTAcap.Version.Replace('-', '.'));

            if (installedSIMQTTAcap == null || embeddedSIMQTTVersion.CompareTo(installedSimqttVersion) > 0) // Install MQTT if it's not installed or the version is older
            {
                string architecture = devicePropertyList.Architecture;

                Stream acapStream = default;
                Assembly _assembly = Assembly.GetExecutingAssembly();
                string _fileName = default;

                // Get the ACAP from assembly resources
                switch (architecture)
                {
                    // Don't forget to change embeddedSIMQTTVersion if you change the version here!
                    case "armv7hf":
                        acapStream = _assembly.GetManifestResourceStream("InSupportDriftMilestonePlugin.Resources.SIMQTT_1_2_0_armv7hf.eap");
                        _fileName = "SIMQTT_1_2_0_armv7hf.eap";
                        break;
                    case "mips":
                        acapStream = _assembly.GetManifestResourceStream("InSupportDriftMilestonePlugin.Resources.SIMQTT_1_2_0_mipsisa32r2el.eap");
                        _fileName = "SIMQTT_1_2_0_mipsisa32r2el.eap";
                        break;
                    case "aarch64":
                        acapStream = _assembly.GetManifestResourceStream("InSupportDriftMilestonePlugin.Resources.SIMQTT_1_2_0_aarch64.eap");
                        _fileName = "SIMQTT_1_2_0_aarch64.eap";
                        break;
                    default:
                        // not supported
                        break;
                }

                if (acapStream != null)
                {
                    // upload and install the ACAP
                    device.StatusText = "Installing ACAP";
                    var response = await device.BonjourAxisDevice.UploadACAPAsync(username, password, acapStream, _fileName, devicePropertyList.UseHttps);
                    device.StatusText = "ACAP uploaded: " + response.ToString();
                }
                else
                {
                    device.StatusText = "Not supported";
                }
            }

            // Check if the existing acap is running or start the newly installed acap
            var startAcapResult = installedSIMQTTAcap?.Status == "Running" ?
                ApplicationControlResults.OK :
                await device.BonjourAxisDevice.ApplicationControlAsync(username, password, "simqtt", ApplicationControlActions.Start, devicePropertyList.UseHttps);

            if (startAcapResult == ApplicationControlResults.OK ||
                startAcapResult == ApplicationControlResults.AppAlreadyRunning)
            {
                // Configure SIMQTT if the ACAP is running
                var simqttConfig = new SIMQTTConfig()
                {
                    address = ipToServer,
                    clientID = "simqtt-" + devicePropertyList.SerialNumber
                };

                device.StatusText = "Configuring SIMQTT";

                if (await SetSIMQTTParams(device.BonjourAxisDevice, username, password, JsonConvert.SerializeObject(simqttConfig), devicePropertyList.UseHttps))
                {
                    device.StatusText = "Done!";
                    device.ProgressValue = 100;
                }
                else
                {
                    device.StatusText = "Failed to configure SIMQTT";
                }
            }
            else
            {
                device.StatusText = "Failed: " + startAcapResult;
            }

            device.CheckboxEnabled = true;
            device.IsConfiguring = false;
        }

        /// <summary>
        /// http://172.16.100.xxx/local/simqtt/mqtt?set=<JSON>&_=1631277676670
        /// set: {"connect":false,"address":"localhost","port":"1883","user":"","password":"","clientID":"simqtt-B8A44F0122BE","preTopic":"simqtt","tls":false,"verify":false,"lwt":null,"announce":null}
        /// </summary>
        /// <param name="device"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="payload">JSON, will be URL encoded</param>
        /// <param name="useHttps"></param>
        /// <returns></returns>
        private async Task<bool> SetSIMQTTParams(BonjourAxisDevice device, string username, string password, string payload, bool useHttps)
        {
            var client = new RestClient($"{(useHttps ? "https" : "http")}://{device.IPAddress}");
            var request = new RestRequest($"/local/simqtt/mqtt?set={HttpUtility.UrlEncode(payload)}&_={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}", Method.GET)
            {
                Credentials = new NetworkCredential(username, password)
            };

            var response = await client.ExecuteAsync(request);

            return response.Content == "MQTT Updated";
        }

        private async Task<SIMQTTConfig> GetSIMQTTParams(BonjourAxisDevice device, string username, string password, bool useHttps)
        {
            var client = new RestClient($"{(useHttps ? "https" : "http")}://{device.IPAddress}");
            var request = new RestRequest($"/local/simqtt/mqtt?&_={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}", Method.GET)
            {
                Credentials = new NetworkCredential(username, password)
            };

            var response = await client.ExecuteAsync(request);

            return JsonConvert.DeserializeObject<SIMQTTConfig>(response.Content);
        }


        private async void checkBtn_Click(object sender, RoutedEventArgs e)
        {
            StopBonjour();

            checkBtn.IsEnabled = false;
            configureCamerasBtn.IsEnabled = false;

            string username = usrBox.Text;
            string password = pwdBox.Password;
            string ipaddress = ((IPAddress)interfacesComboBox.SelectedItem).ToString();

            var tasks = devices.Where(d => d.IsChecked)
                .Select(d => CheckDevice(d, username, password));

            await Task.WhenAll(tasks);
            configureCamerasBtn.IsEnabled = true;
            checkBtn.IsEnabled = true;

            StartBonjour();
        }

        private async Task CheckDevice(BonjourAxisDeviceListViewItem device, string username, string password)
        {
            device.ProgressValue = 0;
            device.IsConfiguring = true;
            device.StatusText = "Checking";

            Propertylist propertylist = default;

            try
            {
                // Get basic device information
                // Will be null if not supported
                propertylist = await device.BonjourAxisDevice.GetBasicDataInfoAsync(username, password);
            }
            catch (UnauthorizedAccessException)
            {
                device.StatusText = "Wrong password!";
            }

            if (propertylist != null &&
                (propertylist.Architecture == "armv7hf" || propertylist.Architecture == "mips" || propertylist.Architecture == "aarch64"))
            {
                AxisApplicationListResponse acapsReply = await device.BonjourAxisDevice.GetApplicationListAsync(username, password, propertylist.UseHttps);

                if (acapsReply != null)
                {
                    device.CheckboxEnabled = true;

                    var simqttAcap = acapsReply.application.FirstOrDefault(acap => acap.Name == "simqtt");
                    if (simqttAcap != null)
                    {
                        var olderVersion = embeddedSIMQTTVersion.CompareTo(new Version(simqttAcap.Version.Replace('-', '.'))) > 0;

                        if (simqttAcap.Status == "Running")
                        {
                            var simqttConfig = await GetSIMQTTParams(device.BonjourAxisDevice, username, password, propertylist.UseHttps);

                            // Check if the SIMQTT configuration matches our machine
                            if (hostIps.Any(ip => ip.ToString() == simqttConfig.address))
                            {
                                device.StatusText = (olderVersion ? "[Old version] " : "") + "ACAP running, valid config";
                                device.IsChecked = olderVersion;
                            }
                            else // The device is configured for another machine
                                device.StatusText = (olderVersion ? "[Old version] " : "") + "ACAP running, wrong IP configured";
                        }
                        else
                            device.StatusText = (olderVersion ? "[Old version] " : "") + "ACAP not running";
                    }
                    else
                        device.StatusText = "Unconfigured";
                }
            }

            if (device.StatusText == "Checking")
            {
                device.StatusText = "Not supported";
                device.CheckboxEnabled = false;
                device.IsChecked = false;
            }

            device.IsConfiguring = false;
        }


        private void StartBonjour()
        {
            bonjourSearcher.OnDeviceChanged += BonjourSearcher_OnDeviceChanged;
            bonjourSearcher.OnDevicesFound += BonjourSearcher_OnDevicesFound;
            bonjourSearcher.StartScan();
        }

        private void StopBonjour()
        {
            bonjourSearcher.StopScan();
            bonjourSearcher.OnDevicesFound -= BonjourSearcher_OnDevicesFound;
            bonjourSearcher.OnDeviceChanged -= BonjourSearcher_OnDeviceChanged;
        }

        private void DeviceCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!ignoreAllDevicesCheck)
            {
                ignoreAllDevicesCheck = true;

                if (devices.All(d => d.IsChecked))
                {
                    allDevicesCheckBox.IsChecked = true;
                }
                if (devices.Any(d => d.IsChecked))
                {
                    allDevicesCheckBox.IsChecked = null;
                }
                else
                {
                    allDevicesCheckBox.IsChecked = false;
                }
                ignoreAllDevicesCheck = false;
            }
        }

        private bool ignoreAllDevicesCheck;
        private void allDevicesCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!ignoreAllDevicesCheck)
            {
                ignoreAllDevicesCheck = true;

                foreach (var d in devices.Where(d => d.CheckboxEnabled))
                {
                    d.IsChecked = allDevicesCheckBox.IsChecked == true;
                }

                ignoreAllDevicesCheck = false;
            }
        }

        private async void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            await mqttBroker.StopServerAsync();
        }
    }


    /// <summary>
    /// Wrapper for BonjourAxisDevice to display in the ListView
    /// </summary>
    public class BonjourAxisDeviceListViewItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
                }
            }
        }

        private bool _isConfiguring;
        public bool IsConfiguring
        {
            get => _isConfiguring;
            set
            {
                if (_isConfiguring != value)
                {
                    _isConfiguring = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConfiguring)));
                }
            }
        }

        private bool _checkboxEnabled = true;
        public bool CheckboxEnabled
        {
            get => _checkboxEnabled;
            set
            {
                if (_checkboxEnabled != value)
                {
                    _checkboxEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckboxEnabled)));
                }
            }
        }

        private int _progressValue = 0;
        public int ProgressValue
        {
            get => _progressValue; set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressValue)));
                }
            }
        }

        public string Name => BonjourAxisDevice.Host.DisplayName;
        public string DisplayName => BonjourAxisDevice.DisplayName;
        public string IPAddress => BonjourAxisDevice.IPAddress;
        public string MacAddress => BonjourAxisDevice.MacAddress;

        public BonjourAxisDevice BonjourAxisDevice { get; private set; }
        public BonjourAxisDeviceListViewItem(BonjourAxisDevice bonjourAxisDevice)
        {
            this.BonjourAxisDevice = bonjourAxisDevice;
            bonjourAxisDevice.PropertyChanged += BonjourAxisDevice_PropertyChanged;
        }

        private void BonjourAxisDevice_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AnnounceChange(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public override string ToString() => BonjourAxisDevice.ToString();
    }

    /// <summary>
    /// Json structure for SIMQTT config
    /// </summary>
    public class SIMQTTConfig
    {
        public bool connect { get; set; } = false;
        public string address { get; set; }
        public string port { get; set; } = 1883.ToString();
        public string user { get; set; } = "";
        public string password { get; set; } = "";
        public string clientID { get; set; }
        public string preTopic { get; set; } = "simqtt";
        public bool tls { get; set; } = false;
        public bool verify { get; set; } = false;
        public object lwt { get; set; } = null;
        public object announce { get; set; } = null;
    }

}
