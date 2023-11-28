using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InSupport.Client.LoginBookmarks
{
    public static class LoginSettingsManager
    {
        static string path = @"InSupport Plugin\loginbookmarks.xml";
        static string combinedPath;
        static XmlNode rootNode;
        static XmlDocument xmlDoc;

        public static void Initialize()
        {
            combinedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), path);
            xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(combinedPath);
            }
            catch
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"InSupport Plugin"));
                xmlDoc = new XmlDocument();
                xmlDoc.AppendChild(xmlDoc.CreateElement("Bookmarks"));
                xmlDoc.Save(combinedPath);
            }
            rootNode = xmlDoc.DocumentElement;
        }

        public static List<Login> GetLogins()
        {
            var logins = new List<Login>();
            
            foreach(XmlNode node in xmlDoc.GetElementsByTagName("Login"))
            {
                logins.Add(new Login(node.InnerText, node.Attributes["Username"].Value, node.Attributes["Password"].Value));
            }

            return logins;
        }

        public static void RemoveIndex(int i)
        {
            xmlDoc.DocumentElement.RemoveChild(xmlDoc.GetElementsByTagName("Login")[i]);
            xmlDoc.Save(combinedPath);
        }

        public static void AddNewlogin(Login login)
        {
            XmlNode loginNode = xmlDoc.CreateElement("Login");
            XmlAttribute attribute = xmlDoc.CreateAttribute("Username");
            attribute.Value = login.Username;
            loginNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("Password");
            attribute.Value = login.EncryptedPassword;
            loginNode.Attributes.Append(attribute);

            loginNode.InnerText = login.SettingsName;

            rootNode.AppendChild(loginNode);

            xmlDoc.Save(combinedPath);
        }

    }
}
