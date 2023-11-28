using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WinServLib.Objects
{
    public class Status
    {
        public string Nr { get; set; }

        public string Name { get; set; }

        public string HexColor { get; set; }


        public Status(string nr, string name, string colorStr)
        {
            Nr = nr;
            Name = name;


            try
            {
                if (colorStr == "16777215")
                {
                    HexColor = "#00FFFFFF";
                }
                else
                {
                    var colorBytes = BitConverter.GetBytes(int.Parse(colorStr));
                    var color = Color.FromRgb(colorBytes[0], colorBytes[1], colorBytes[2]);
                    HexColor = color.ToString();
                }
            }
            catch
            {

            }
        }

        public override string ToString() => Name;


    }
}
