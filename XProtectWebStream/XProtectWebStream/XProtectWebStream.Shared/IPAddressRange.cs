using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Shared.Net
{
    public class IPAddressRange
    {
        readonly AddressFamily addressFamily;
        readonly byte[] lowerBytes;
        readonly byte[] upperBytes;

        public IPAddressRange(IPAddress lowerInclusive, IPAddress upperInclusive)
        {
            // Assert that lower.AddressFamily == upper.AddressFamily

            this.addressFamily = lowerInclusive.AddressFamily;
            this.lowerBytes = lowerInclusive.GetAddressBytes();
            this.upperBytes = upperInclusive.GetAddressBytes();
        }

        public bool IsInRange(IPAddress address)
        {
            if (address.AddressFamily != addressFamily)
            {
                return false;
            }

            byte[] addressBytes = address.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (int i = 0; i < this.lowerBytes.Length &&
                (lowerBoundary || upperBoundary); i++)
            {
                if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
                    (upperBoundary && addressBytes[i] > upperBytes[i]))
                {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == upperBytes[i]);
            }

            return true;
        }
    }

    public class IPaddressPair
    {
        public string FromIP { get; set; }
        public string ToIP { get; set; }

        public IPAddress FromIPAddress() => IPAddress.Parse(FromIP);

        public IPAddress ToIPAddress() => IPAddress.Parse(ToIP);

        public bool IsInRange(IPAddress ip)
        {
            IPAddressRange range = new IPAddressRange(FromIPAddress(), ToIPAddress());
            return range.IsInRange(ip);
        }

        public static IPaddressPair[] GetIPAddresses()
        {
            List<IPaddressPair> pairs = new List<IPaddressPair>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    var split = ip.ToString().Split('.').ToList();
                    split.RemoveAt(3);

                    var from = string.Join(".", split) + ".0";
                    var to = string.Join(".", split) + ".255";
                    pairs.Add(new IPaddressPair() { FromIP = from, ToIP = to });
                }
            }
            return pairs.ToArray();
        }
    }
}
