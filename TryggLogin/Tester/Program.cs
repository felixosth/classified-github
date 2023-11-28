using LoginShared.AD;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            //DirectoryEntry entry = new DirectoryEntry("ldap://" + Domain.GetCurrentDomain().l);

            //DirectorySearcher mySearcher = new DirectorySearcher(entry);


            using (var context = new PrincipalContext(ContextType.Domain))
            {
                //UserPrincipal u1 = UserPrincipal.FindByIdentity(context, "felix test");
                //string x = u1.DisplayName;
                
                UserPrincipalEx u2 = UserPrincipalEx.FindByIdentity(context, "felix test");
                string y = u2.EmployeeNumber;
            }
        }

        static string Traverse(DirectoryEntry entry, string propName)
        {
            if (entry.Properties.Contains(propName))
                return (string)entry.Properties[propName].Value;
            else
            {
                return null;
                //var entry = (DirectoryEntry)entry
            }
        }

        static DirectoryContext GetDirectoryContext()
        {
            return new DirectoryContext(DirectoryContextType.Forest);
        }

        static PrincipalContext GetPrincipalContext()
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
