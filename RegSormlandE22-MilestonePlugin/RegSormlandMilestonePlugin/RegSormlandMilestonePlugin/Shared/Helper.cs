using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Xml;
using VideoOS.Platform;

namespace RegSormlandMilestonePlugin.Shared
{
    public static class Helper
    {
        public static T FromBase64<T>(string base64str)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64str)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
            }
        }

        public static string ToBase64(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }


        public static string SerializeXml(this FQID fqid)
        {
            return fqid.ToXmlNode().OuterXml;
        }

        public static IEnumerable<Item> GetAllItems(List<Item> items)
        {
            List<Item> result = new List<Item>();
            foreach (var item in items)
            {
                if (item.FQID.FolderType == FolderType.No)
                    result.Add(item);
                else
                    result.AddRange(GetAllItems(item.GetChildren()));
            }
            return result;
        }


        private static int ColorValuesIndex = 0;
        private static string[] ColourValues = new string[] {
            "FF0000", "00FF00", "0000FF", "FFFF00", "FF00FF", "00FFFF", "000000",
            "800000", "008000", "000080", "808000", "800080", "008080", "808080",
            "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
            "400000", "004000", "000040", "404000", "400040", "004040", "404040",
            "200000", "002000", "000020", "202000", "200020", "002020", "202020",
            "600000", "006000", "000060", "606000", "600060", "006060", "606060",
            "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
            "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0",
        };

        public static void ResetDistinctColorIndex()
        {
            ColorValuesIndex = 0;
        }

        public static Brush GetDistinctBrush()
        {
            if (ColorValuesIndex >= ColourValues.Length)
                ColorValuesIndex = 0;

            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + ColourValues[ColorValuesIndex++]));
        }

        public static Color GetDistinctColor()
        {
            if (ColorValuesIndex >= ColourValues.Length)
                ColorValuesIndex = 0;

            return (Color)ColorConverter.ConvertFromString("#" + ColourValues[ColorValuesIndex++]);
        }

        public static Color Invert(this Color color)
        {
            return Color.FromRgb((byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B));
            //return ToColor(0xFFFFFFFF ^ color.ToUint());
        }

        //private static uint ToUint(this Color c)
        //{
        //    return (uint)(((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B) & 0xffffffffL);
        //}

        //private static Color ToColor(this uint value)
        //{
        //    return Color.FromArgb((byte)((value >> 24) & 0xFF),
        //               (byte)((value >> 16) & 0xFF),
        //               (byte)((value >> 8) & 0xFF),
        //               (byte)(value & 0xFF));
        //}


        public static System.Windows.Forms.Screen GetScreen(this Window window)
        {
            return System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(window).Handle);
        }
    }
}
