using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XProtectWebStream.Shared
{
    public static class Utils
    {
        static private readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

        public static class Packer
        {
            public static T Deserialize<T>(string serializedObject)
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(serializedObject)))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (T)bf.Deserialize(ms);
                }
            }

            public static string Serialize(object objectToSerialize)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, objectToSerialize);
                    ms.Position = 0;
                    byte[] buffer = new byte[(int)ms.Length];
                    ms.Read(buffer, 0, buffer.Length);
                    return Convert.ToBase64String(buffer);
                }
            }
        }

        public static string MakePathSafe(this string path)
        {
            return new string(path.Where(ch => !invalidFileNameChars.Contains(ch)).ToArray());
        }

        public static bool IsValidEmail(this string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

        public static string GetTimeToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }

        public static string CreateMD5(byte[] inputBytes)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                //byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static bool IsLocalMachine(this IPEndPoint iPEndPoint)
        {
            return iPEndPoint.Address.ToString() == "127.0.0.1" || iPEndPoint.Address.ToString() == "::1";
        }

        public static string GetMGUID()
        {
            string x64Result = string.Empty;
            string x86Result = string.Empty;
            RegistryKey keyBaseX64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey keyBaseX86 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey keyX64 = keyBaseX64.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
            RegistryKey keyX86 = keyBaseX86.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
            object resultObjX64 = keyX64.GetValue("MachineGuid", (object)"default");
            object resultObjX86 = keyX86.GetValue("MachineGuid", (object)"default");
            keyX64.Close();
            keyX86.Close();
            keyBaseX64.Close();
            keyBaseX86.Close();
            keyX64.Dispose();
            keyX86.Dispose();
            keyBaseX64.Dispose();
            keyBaseX86.Dispose();
            keyX64 = null;
            keyX86 = null;
            keyBaseX64 = null;
            keyBaseX86 = null;
            if (resultObjX64 != null && resultObjX64.ToString() != "default")
            {
                return resultObjX64.ToString();
            }
            if (resultObjX86 != null && resultObjX86.ToString() != "default")
            {
                return resultObjX86.ToString();
            }

            throw new Exception("MGUID not found");
        }

        public static bool DynPropertyExist(dynamic dyn, string name)
        {
            if (dyn is ExpandoObject)
                return ((IDictionary<string, object>)dyn).ContainsKey(name);

            return dyn.GetType().GetProperty(name) != null;
        }

    }
}
