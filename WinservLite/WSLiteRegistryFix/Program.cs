using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSLiteRegistryFix
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length > 0)
            {
                var appPath = args[0];

                var root = Registry.ClassesRoot.CreateSubKey("wslite");
                root.SetValue("", "\"URL:WinservLite Protocol\"");
                root.SetValue("URL Protocol", "\"\"");

                root.CreateSubKey("DefaultIcon").SetValue("", "\"WinServLite.exe,1\"");

                var cmd = root.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
                cmd.SetValue("", "\"" + appPath + "\" \"%1\"");
            }
        }
    }
}
