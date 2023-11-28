using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LoginShared
{
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

    public static class Helper
    {
        public static PrincipalContext GetPrincipalContext()
        {
            try
            {
                return new PrincipalContext(ContextType.Domain);
            }
            catch
            {
                return new PrincipalContext(ContextType.Machine);
            }
        }
    }
}
