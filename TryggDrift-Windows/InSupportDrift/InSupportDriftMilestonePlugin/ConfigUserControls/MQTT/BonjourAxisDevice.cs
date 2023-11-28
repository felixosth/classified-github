using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeroconf;

namespace InSupportDriftMilestonePlugin.ConfigUserControls.MQTT
{
    public class BonjourAxisDevice : INotifyPropertyChanged
    {
        private IZeroconfHost _host;
        public IZeroconfHost Host
        {
            get => _host; set
            {
                _host = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IPAddress)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayName => Host.DisplayName;
        public string IPAddress => Host.IPAddresses.OrderByDescending(s => s).FirstOrDefault();

        private string _mac = null;
        public string MacAddress
        {
            get
            {
                if (_mac == null)
                {
                    try
                    {
                        _mac = Host.Services[BonjourSearcher.AxisBonjourService].Properties[0]["macaddress"].ToUpper();
                        return _mac;
                    }
                    catch { }
                }
                return _mac;
            }
        }

        public BonjourAxisDevice(IZeroconfHost host)
        {
            this.Host = host;
        }
        
        public override string ToString() => IPAddress + " - " + DisplayName;
    }
}
