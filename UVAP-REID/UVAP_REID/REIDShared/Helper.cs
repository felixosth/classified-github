using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace REIDShared
{
    public class Helper
    {
        public static int StringToSeed(string str)
        {
            MD5 md5Hasher = MD5.Create();
            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
            var ivalue = BitConverter.ToInt32(hashed, 0);
            return ivalue;
        }

        // Return a Mac address based on seed
        public static string GetMacAddressWithSeed(int seed)
        {
            var random = new Random(seed);
            var buffer = new byte[6];
            random.NextBytes(buffer);
            var result = String.Concat(buffer.Select(x => string.Format("{0}:", x.ToString("X2"))).ToArray());
            return result.TrimEnd(':');
        }

        public static Guid StringToGuid(string src)
        {
            byte[] stringbytes = Encoding.UTF8.GetBytes(src);
            byte[] hashedBytes = new System.Security.Cryptography
                .SHA1CryptoServiceProvider()
                .ComputeHash(stringbytes);
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes);
        }
    }
}
