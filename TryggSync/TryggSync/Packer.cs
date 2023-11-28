using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TryggSync
{
    public static class Packer
    {

        public static string SerializeObject(object objectToSerialize)
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

        public static object DeserializeObject(string serializedObject)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(serializedObject)))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return bf.Deserialize(ms);
                }
            }
            catch
            { return null; }
        }
    }
}
