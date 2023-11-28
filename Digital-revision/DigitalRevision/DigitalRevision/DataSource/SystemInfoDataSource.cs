using DigitalRevision.Global;
using Microsoft.Win32;
using System.Collections;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DigitalRevision.DataSource
{
    class SystemInfoDataSource : DataSourceBase
    {
        public override string Name => "Systeminformation";

        public override double Version => 1.0;

        private const string _B = "++++++++++";

        public override async Task CollectData(string folderDestination)
        {
            ProgressIsIndeterminate = true;

            string dataFileName = Path.Combine(folderDestination, "computer.txt");

            string path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(path);

            var os = WMIQuery("Win32_OperatingSystem");
            string osMguid = Helper.GetMGUID();
            string osCurrentBuild = rk.GetValue("CurrentBuild") as string;
            string osCurrentVersion = rk.GetValue("DisplayVersion") as string;
            var installDate = ManagementDateTimeConverter.ToDateTime((string)os["InstallDate"]);
            var osName = os["Caption"];
            string osProductKey = DecodeProductKey(rk.GetValue("DigitalProductId") as byte[]);

            var totalRamInGb = long.Parse(WMIQuery("Win32_ComputerSystem")["TotalPhysicalMemory"].ToString()) / 1024.0 / 1024.0 / 1024.0;

            var processor = WMIQuery("Win32_Processor");
            var processorName = processor["Name"];
            var processorCores = int.Parse(processor["NumberOfCores"].ToString());
            //var processorSocket = processor["SocketDesignation"];

            using (var fileStream = new FileStream(dataFileName, FileMode.Create))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                await writer.WriteLineAsync($"{_B} Computer {_B}");
                await writer.WriteLineAsync($"OS: {osName} {osCurrentVersion} ({osCurrentBuild})");
                await writer.WriteLineAsync($"OS MachineGUID: {osMguid}");
                await writer.WriteLineAsync($"OS product key: {osProductKey}");
                await writer.WriteLineAsync($"OS install date: {installDate}");
                await writer.WriteLineAsync();
                await writer.WriteLineAsync($"Total RAM: {totalRamInGb:0.0} GB");
                await writer.WriteLineAsync($"CPU name: {processorName}");
                await writer.WriteLineAsync($"CPU cores: {processorCores}");
                //await writer.WriteLineAsync($"CPU socket: {processorSocket}");
                await writer.WriteLineAsync();

                await writer.WriteLineAsync($"{_B} Network {_B}");

                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        await writer.WriteLineAsync(ni.Name + ":");
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                await writer.WriteLineAsync(ip.Address.ToString());
                            }
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }

            ProgressIsIndeterminate = false;
            ProgressPercentage = 100;
        }

        static ManagementObject WMIQuery(string q, string path = "root\\CIMV2")
        {
            return new ManagementObjectSearcher(path, "SELECT * FROM " + q).Get().OfType<ManagementObject>().First();
        }

        static string DecodeProductKey(byte[] digitalProductId)
        {
            // Offset of first byte of encoded product key in 
            //  'DigitalProductIdxxx" REG_BINARY value. Offset = 34H.
            const int keyStartIndex = 52;
            // Offset of last byte of encoded product key in 
            //  'DigitalProductIdxxx" REG_BINARY value. Offset = 43H.
            const int keyEndIndex = keyStartIndex + 15;
            // Possible alpha-numeric characters in product key.
            char[] digits = new char[]
            {
                'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'M', 'P', 'Q', 'R',
                'T', 'V', 'W', 'X', 'Y', '2', '3', '4', '6', '7', '8', '9',
            };
            // Length of decoded product key
            const int decodeLength = 29;
            // Length of decoded product key in byte-form.
            // Each byte represents 2 chars.
            const int decodeStringLength = 15;
            // Array of containing the decoded product key.
            char[] decodedChars = new char[decodeLength];
            // Extract byte 52 to 67 inclusive.
            ArrayList hexPid = new ArrayList();
            for (int i = keyStartIndex; i <= keyEndIndex; i++)
            {
                hexPid.Add(digitalProductId[i]);
            }
            for (int i = decodeLength - 1; i >= 0; i--)
            {
                // Every sixth char is a separator.
                if ((i + 1) % 6 == 0)
                {
                    decodedChars[i] = '-';
                }
                else
                {
                    // Do the actual decoding.
                    int digitMapIndex = 0;
                    for (int j = decodeStringLength - 1; j >= 0; j--)
                    {
                        int byteValue = (digitMapIndex << 8) | (byte)hexPid[j];
                        hexPid[j] = (byte)(byteValue / 24);
                        digitMapIndex = byteValue % 24;
                        decodedChars[i] = digits[digitMapIndex];
                    }
                }
            }
            return new string(decodedChars);
        }
    }
}
