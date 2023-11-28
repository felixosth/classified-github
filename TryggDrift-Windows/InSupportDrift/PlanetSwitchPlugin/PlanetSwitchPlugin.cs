using EncryptStringSample;
using InSupport.Drift.Monitor;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace InSupport.Drift.Plugins
{
    public class PlanetSwitchPlugin : BaseMonitor
    {
        private List<PlanetSwitch> _switches;
        public PlanetSwitchData[] PlanetSwitches
        {
            get
            {
                PlanetSwitchData[] planetSwitchData = new PlanetSwitchData[_switches.Count];

                for (int i = 0; i < _switches.Count; i++)
                {
                    _switches[i].UpdateData();
                    planetSwitchData[i] = new PlanetSwitchData(_switches[i]);
                }

                return planetSwitchData;
            }
        }

        public PlanetSwitchPlugin() : base()
        {
            _switches = new List<PlanetSwitch>();
        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey("PlanetSwitches"))
            {
                var pSwitches = JsonConvert.DeserializeObject<List<PlanetSwitch>>(settings["PlanetSwitches"]);

                foreach (var ps in pSwitches)
                {
                    _switches.Add(PlanetSwitch.FromSettings(ps.EncryptedUsername, ps.EncryptedPassword, ps.IP));
                }
            }
        }

        public override string MonitorName => "PlanetSwitch";

        public override float MonitorVersion => 0.1f;
    }

    public class PlanetSwitch
    {
        const string _key = "doakdposa KPOSAK podsakpod kaposdm pasmD";
        public string IP { get; set; }

        internal PlanetSwitchLib.PlanetSwitch pSwitch;

        public bool Connected { get; set; }

        [JsonIgnore]
        private string username, password;

        public string EncryptedUsername { get; set; }
        public string EncryptedPassword { get; set; }


        public List<PlanetSwitchLib.PlanetSwitchPhysicalPort> PhysicalPorts { get; set; }

        public Dictionary<string, string> SystemInformation { get; set; }

        public PlanetSwitch(string username, string password, PlanetSwitchLib.PlanetSwitch pSwitch)
        {
            this.pSwitch = pSwitch;
            this.username = username;
            this.password = password;

            if (username != null && password != null)
            {
                EncryptedUsername = StringCipher.Encrypt(username, _key);
                EncryptedPassword = StringCipher.Encrypt(password, _key);
            }

            if (pSwitch != null)
            {
                IP = pSwitch.IP;
            }
        }

        public PlanetSwitch()
        {

        }

        public void UpdateData()
        {
            PhysicalPorts = null;
            SystemInformation = null;
            Connected = pSwitch.Login(username, password);

            if (Connected)
            {
                PhysicalPorts = pSwitch.Actions.GetPhysicalPortStatus();
                SystemInformation = pSwitch.Actions.GetSystemInformation();
            }
        }

        public static PlanetSwitch FromSettings(string encryptedUsername, string encryptedPassword, string ip)
        {
            return new PlanetSwitch(StringCipher.Decrypt(encryptedUsername, _key), StringCipher.Decrypt(encryptedPassword, _key), new PlanetSwitchLib.PlanetSwitch(ip));
        }
    }

    public class PlanetSwitchData
    {
        public bool Connected => pSwitch.Connected;
        public Dictionary<string, string> SystemInformation => pSwitch.SystemInformation;
        public List<PlanetSwitchLib.PlanetSwitchPhysicalPort> PhysicalPorts => pSwitch.PhysicalPorts;

        PlanetSwitch pSwitch;
        public PlanetSwitchData(PlanetSwitch originalSwitch)
        {
            pSwitch = originalSwitch;
        }
    }
}
