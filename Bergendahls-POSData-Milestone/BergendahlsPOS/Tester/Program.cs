using BergendahlsPOS;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, byte[]> downloadedFiles = new Dictionary<string, byte[]>();
            using (SftpClient client = new SftpClient(new ConnectionInfo("file.bergendahls.se", "insupport", new PasswordAuthenticationMethod("insupport", "NX523MPk"))))
            {
                client.Connect();

                var files = client.ListDirectory("/");

                foreach(var file in files)
                {
                    using (var ms = new MemoryStream())
                    {
                        client.DownloadFile(file.FullName, ms);
                        downloadedFiles[file.FullName] = ms.ToArray();
                    }
                }


                client.Disconnect();
            }

            var posdata = new POSData(downloadedFiles.First().Value);
            //var posdata = new POSData(@"xml\måndag.xml");

            var stt = posdata.GetStronglyTypedTransactions();

            var doc = posdata.GetTransactions();

            int i = 1;
            foreach (var transaction in doc)
            {
                Console.WriteLine("Transaction {0}", i++);
                foreach (var kvp in transaction)
                {
                    Console.WriteLine("---- {0}: {1}", kvp.Key, kvp.Value);
                }
            }

            Console.ReadKey();
        }
    }
}
