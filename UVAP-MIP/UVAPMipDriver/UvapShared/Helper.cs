using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Metadata;

namespace UvapShared
{
    public static class Helper
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static int StringToSeed(string str)
        {
            MD5 md5Hasher = MD5.Create();
            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
            var ivalue = BitConverter.ToInt32(hashed, 0);
            return ivalue;
        }

        public static int RandomInt(int from, int to, int seed)
        {
            return new Random(seed).Next(from, to);
        }

        public static byte RandomByte(int seed)
        {
            byte[] buffer = new byte[1];
            new Random(seed).NextBytes(buffer);
            return buffer[0];
        }
    }
}
