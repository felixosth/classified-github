using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BergendahlsPOS
{
    public class POSData
    {
        private byte[] XMLBytes { get; set; }

        public POSData(byte[] bytes)
        {
            XMLBytes = bytes;
        }

        public List<Dictionary<string, string>> GetTransactions()
        {
            XmlDocument doc = null;

            if(XMLBytes.Length > 0)
            {
                doc = new XmlDocument();
                using (var ms = new MemoryStream(XMLBytes))
                    doc.Load(ms);
            }

            List<Dictionary<string, string>> posDocument = new List<Dictionary<string, string>>();

            foreach(XmlNode transaction in doc.DocumentElement.ChildNodes)
            {
                Dictionary<string, string> transactionDictionary = new Dictionary<string, string>();

                foreach(XmlNode childNode in transaction.ChildNodes)
                {
                    transactionDictionary.Add(childNode.Name, childNode.InnerText);
                }

                posDocument.Add(transactionDictionary);
            }
            return posDocument;
        }

        public Transactions GetStronglyTypedTransactions()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Transactions));

            // Declare an object variable of the type to be deserialized.
            Transactions transactions = null;

            using (Stream readStream = new MemoryStream(XMLBytes))
            {
                // Call the Deserialize method to restore the object's state.
                transactions = (Transactions)serializer.Deserialize(readStream);
            }
            return transactions;
        }
    }
}
