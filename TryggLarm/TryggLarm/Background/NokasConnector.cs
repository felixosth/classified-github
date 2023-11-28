using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TryggLarm.Background
{
    class NokasConnector
    {
        string primaryIP, secondaryIP;
        int port;

        string sendCode, pinCode;

        public NokasConnector(string sendCode, string pinCode)
        {
            this.sendCode = sendCode;
            this.pinCode = pinCode;

            primaryIP = "62.119.138.212";
            secondaryIP = "62.119.183.53";
            port = 19000;
        }

        public NokasResponse SendAlarm(string type, string info)
        {
            try
            {
                var xml = BuildXml(type, info);

                TcpClient tcpClient = new TcpClient();

                tcpClient.Connect(IPAddress.Parse(primaryIP), port);

                if (!tcpClient.Connected)
                {
                    tcpClient.Connect(IPAddress.Parse(secondaryIP), port);
                }

                if (!tcpClient.Connected)
                    throw new Exception("You're screwed.");


                var stream = tcpClient.GetStream();


                //byte[] msgBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(xml); /// åäö = å__
                byte[] msgBytes = Encoding.ASCII.GetBytes(xml);


                //var decoded = Encoding.GetEncoding("ISO-8859-1").GetString(msgInBytes);

                //ASCIIEncoding asen = new ASCIIEncoding();

                stream.Write(msgBytes, 0, msgBytes.Length);

                var data = new Byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                var responseXml = new XmlDocument();
                responseXml.LoadXml(responseData);


                var responseCode = int.Parse(responseXml.GetElementsByTagName("R")[0].InnerText);
                var iTags = responseXml.GetElementsByTagName("I");

                string responseMsg = "";
                if (iTags != null && iTags.Count > 0)
                    responseMsg = iTags[0].InnerText;

                return new NokasResponse((NokasResponse.ResponseCode)responseCode, responseMsg);

                //return (ResponseCode)responseCode;
            }
            catch
            {
                throw;
            }

        }


        private string BuildXml(string type, string info)
        {
            var xml = new XmlDocument();
            var root = xml.AppendChild(xml.CreateElement("B"));

            CreateElement(xml, root, "H", sendCode);
            CreateElement(xml, root, "C", pinCode);
            CreateElement(xml, root, "T", type);
            CreateElement(xml, root, "I", info);

            string xmlInText = "";

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xml.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                xmlInText = stringWriter.GetStringBuilder().ToString();
            }

            return xmlInText;
        }

        private void CreateElement(XmlDocument doc, XmlNode parent, string name, string text)
        {
            XmlElement element = (XmlElement)parent.AppendChild(doc.CreateElement(name));
            element.InnerText = text;
        }



    }

    class NokasResponse
    {
        public ResponseCode Response { get; set; }
        public string Message { get; set; }

        public NokasResponse(ResponseCode code, string msg)
        {
            Message = msg;
            Response = code;
        }

        public override string ToString() => Response.ToString() + (Message != "" ? ": " + Message : "");

        public enum ResponseCode
        {
            OK = 0,
            InvalidPin = 1,
            XmlSyntaxError = 2,
            InvalidContent = 3
        }
    }
}
