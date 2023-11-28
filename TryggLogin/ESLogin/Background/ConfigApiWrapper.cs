using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VideoOS.ConfigurationAPI;
using VideoOS.ConfigurationAPI.ConfigurationFaultException;

namespace ESLogin.Background
{
    internal static class ConfigApiWrapper
    {
        static IConfigurationService GetConfigClient() => ConfigurationApiClient.CreateClient();


        public static ConfigurationItem GetUser(string sid)
        {
            var client = GetConfigClient();
            var basicUsers = client.GetChildItems("/" + ItemTypes.BasicUserFolder);

            foreach(var user in basicUsers)
            {
                var sidProperty = user.Properties.Where(p => p.Key == "Sid").FirstOrDefault();
                if (sidProperty != null && sidProperty.Value == sid)
                    return user;
            }
            return null;
        }

        public static ConfigurationItem[] GetRoles()
        {
            return GetConfigClient().GetChildItems("/" + ItemTypes.RoleFolder);
        }

        public static bool AddUserToRole(string sid, string rolePath)
        {
            var configClient = GetConfigClient();

            // Adding role members is possible with the AddRoleMember method on the UserFolder of a specific Role
            string userFolderPath = string.Format("{0}/{1}", rolePath, ItemTypes.UserFolder);
            var userFolder = configClient.GetItem(userFolderPath);

            var users = configClient.GetChildItems(userFolder.ParentPath + "/" + ItemTypes.UserFolder);

            ConfigurationItem authorizeUserInfokeInfo = configClient.InvokeMethod(userFolder, "AddRoleMember");

            // Changing the properties of the invokeInfo object to the SID of the user
            var nameProperty = authorizeUserInfokeInfo.Properties.Where(property => property.Key == "Sid").FirstOrDefault();
            nameProperty.Value = sid;


            ConfigurationItem invokeResultItem = null;
            try
            {
                invokeResultItem = configClient.InvokeMethod(authorizeUserInfokeInfo, "AddRoleMember");
            }
            catch (FaultException<ServerExceptionFault> ex)
            {
                if (ex.Detail.Type == "VideoOS.Management.Server.AddMemberIdentityAlreadyMemberException")
                {
                    return true;
                }
                else
                    throw;
            }

            return invokeResultItem.Properties.FirstOrDefault(property => property.Key == "State").Value == "Success";
        }

        public static bool RemoveUserFromRole(string sid, string rolePath)
        {
            var client = GetConfigClient();

            var roleUser = client.GetItem("User[" + sid + "]");

            ConfigurationItem removeRoleMemberInvokeInfo = client.InvokeMethod(client.GetItem(rolePath + "/" + ItemTypes.UserFolder), "RemoveRoleMember");

            var itemSelectionProperty = removeRoleMemberInvokeInfo.Properties.Where(property => property.Key == "ItemSelection").FirstOrDefault();
            itemSelectionProperty.Value = roleUser.Path;

            ConfigurationItem invokeResultItem = null;
            try
            {
                invokeResultItem = client.InvokeMethod(removeRoleMemberInvokeInfo, "RemoveRoleMember");
            }
            catch (FaultException<ServerExceptionFault> ex)
            {
                if (ex.Detail.Type == "VideoOS.Management.Server.RemoveRoleMemberNotMemberException")
                {
                    return true;
                }
                else
                    throw;
            }

            return invokeResultItem.Properties.FirstOrDefault(property => property.Key == "State").Value == "Success";
        }

    }
}
