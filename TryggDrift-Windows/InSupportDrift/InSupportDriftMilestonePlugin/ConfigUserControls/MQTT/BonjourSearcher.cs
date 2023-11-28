using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace InSupportDriftMilestonePlugin.ConfigUserControls.MQTT
{
    internal class BonjourSearcher
    {
        internal const string AxisBonjourService = "_axis-video._tcp.local.";

        public event EventHandler<IEnumerable<BonjourAxisDevice>> OnDevicesFound;
        public event EventHandler<BonjourAxisDevice> OnDeviceChanged;
        private TimeSpan ScanTime { get; set; }

        private bool _searching;

        CancellationTokenSource cts = new CancellationTokenSource();

        public BonjourSearcher()
        {
            ScanTime = TimeSpan.FromSeconds(2);
        }

        public void StartScan()
        {
            if (_searching)
                return;

            new Thread(SearchThread) { IsBackground = true }.Start();
        }

        public void StopScan()
        {
            _searching = false;
            cts.Cancel();
        }

        private async void SearchThread()
        {
            Dictionary<string, BonjourAxisDevice> foundHosts = new Dictionary<string, BonjourAxisDevice>();
            List<BonjourAxisDevice> newHosts = new List<BonjourAxisDevice>();
            _searching = true;

            while (_searching)
            {
                newHosts.Clear();
                IReadOnlyList<IZeroconfHost> hosts = null;

                try
                {
                    hosts = await ZeroconfResolver.ResolveAsync(AxisBonjourService, scanTime: ScanTime, cancellationToken: cts.Token);
                }
                catch (TaskCanceledException) { break; }
                catch (Exception) { break; }

                foreach (var host in hosts)
                {
                    var device = new BonjourAxisDevice(host);

                    if (device.MacAddress != null && device.IPAddress != null)
                        if (foundHosts.ContainsKey(device.MacAddress))
                        {
                            var existingDevice = foundHosts[device.MacAddress];

                            if (existingDevice.IPAddress != device.IPAddress || existingDevice.DisplayName != device.DisplayName)
                            {
                                foundHosts[device.MacAddress] = device;
                                OnDeviceChanged?.Invoke(this, device);
                            }
                        }
                        else
                        {
                            newHosts.Add(device);
                            foundHosts.Add(device.MacAddress, device);
                        }
                }

                if (newHosts.Count > 0)
                    OnDevicesFound?.Invoke(this, newHosts.ToArray());
            }
            _searching = false;
        }
    }
}
