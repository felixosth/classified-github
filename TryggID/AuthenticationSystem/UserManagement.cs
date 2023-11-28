using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using VideoOS.ConfigurationApi.ClientService;
using VideoOS.ConfigurationAPI;
using VideoOS.Platform;

namespace AuthenticationSystem
{
    public class UserManagement
    {
        IConfigurationService configClient => ConfigurationApiClient.CreateClient();

        public MilestoneUser[] PreAuthUsers { get; private set; }

        public List<MilestoneUser> TempUsers { get; set; }

        public UserManagement()
        {
            TempUsers = new List<MilestoneUser>();
        }

        public ConfigurationItem[] GetAllBasicUsers() => configClient.GetChildItems("/" + ItemTypes.BasicUserFolder);

        public TryggLoginUser VerifyUser(TryggLoginUser[] users, string key, string password)
        {
            TryggLoginUser errorUser = null;
            foreach(var user in users)
            {
                if(user.Key == key)
                {
                    bool verificiation = user.Verify(password);
                    if(verificiation)
                    {
                        EnvironmentManager.Instance.Log(false, "TryggLogin", "Password verification for " + user.DisplayName + " succeeded.");

                        return user;
                    }
                    else
                    {
                        EnvironmentManager.Instance.Log(false, "TryggLogin", "Password verification for " + user.DisplayName + " failed");
                        errorUser = new TryggLoginUser("error", "error", "error", "error");
                    }
                }
            }

            return errorUser;
        }

        public ConfigurationItem[] GetRoles() => Query("/" + ItemTypes.RoleFolder);

        public bool CreateTmpUser(string username, string password, string rolePath)
        {
            var usrPath = CreateUser(username, password);
            var usr = configClient.GetItem(usrPath);

            bool result = AddUserToRole(usrPath, rolePath);

            if(result)
            {
                var tmpUser = new MilestoneUser(usr) { RolePath = rolePath };
                TempUsers.Add(tmpUser);
            }

            return result;
        }

        public void DeleteUser(MilestoneUser milestoneUser)
        {
            string identity = milestoneUser.Identity;

            var folderUsers = Query(string.Format("{0}/{1}", milestoneUser.RolePath, ItemTypes.UserFolder));

            foreach (var configItem in configClient.GetChildItems(milestoneUser.RolePath + "/" + ItemTypes.UserFolder))
            {
                var user = new MilestoneUser(configItem);
                if(user.Identity == identity)
                {
                    var thisRole = configClient.GetItem(milestoneUser.RolePath);
                    ConfigurationItem removeRoleMemberInvokeInfo = configClient.InvokeMethod(configClient.GetItem(milestoneUser.RolePath + "/" + ItemTypes.UserFolder), "RemoveRoleMember");

                    //removeUserInvokeInfo.Properties[1] = new Property("memberSid", identity);
                    var itemSelectionProperty = removeRoleMemberInvokeInfo.Properties.Where(property => property.Key == "ItemSelection").FirstOrDefault();
                    itemSelectionProperty.Value = user.Path;

                    ConfigurationItem invokeResultItem = configClient.InvokeMethod(removeRoleMemberInvokeInfo, "RemoveRoleMember");

                    var result = invokeResultItem.Properties.FirstOrDefault(property => property.Key == "State").Value == "Success";

                    if (result)
                        EnvironmentManager.Instance.Log(false, "AuthSystem", "Removed user " + milestoneUser.Name + " from role " + thisRole.DisplayName);
                    //break;
                }
            }

            foreach (var configItem in configClient.GetChildItems("/" + ItemTypes.BasicUserFolder))
            {
                var user = new MilestoneUser(configItem);
                if (user.Identity == identity)
                {
                    string userFolderPath = string.Format("/{0}", ItemTypes.BasicUserFolder);

                    var userFolder = configClient.GetItem(userFolderPath);
                    var removeBasicUserInvokeInfo = configClient.InvokeMethod(userFolder, "RemoveBasicUser");

                    var nameProperty = removeBasicUserInvokeInfo.Properties.Where(property => property.Key == "ItemSelection").FirstOrDefault();
                    nameProperty.Value = user.Path;

                    var invokeResultItem = configClient.InvokeMethod(removeBasicUserInvokeInfo, "RemoveBasicUser");
                    var result = invokeResultItem.Properties.FirstOrDefault(property => property.Key == "State").Value == "Success";


                    if (result)
                        EnvironmentManager.Instance.Log(false, "AuthSystem", "Deleted user " + milestoneUser.Name);


                    break;
                }
            }
        }

        private ConfigurationItem[] Query(string folder) => configClient.GetChildItems(folder);


        private string CreateUser(string userName, string password)
        {
            // The create user functionality is a method on BasicUserFolder
            string userFolderPath = string.Format("/{0}", ItemTypes.BasicUserFolder);

            var userFolder = configClient.GetItem(userFolderPath);
            var addUserInvokeInfo = configClient.InvokeMethod(userFolder, "AddBasicUser");

            // Changing the properties of the invokeInfo object to the provided parameters
            var nameProperty = addUserInvokeInfo.Properties.Where(property => property.Key == "Name").FirstOrDefault();
            nameProperty.Value = userName;

            var passwordProperty = addUserInvokeInfo.Properties.Where(property => property.Key == "Password").FirstOrDefault();
            passwordProperty.Value = password;

            var descriptionProperty = addUserInvokeInfo.Properties.Where(property => property.Key == "Description").FirstOrDefault();
            descriptionProperty.Value = "Temporary login";

            var canChangePasswordProperty = addUserInvokeInfo.Properties.Where(property => property.Key == "CanChangePassword").FirstOrDefault();
            canChangePasswordProperty.Value = false.ToString();

            // Calling the AddBasicUser method on BasicUserFolder again with the prepared invokeInfo object
            var addUserInvokeResult = configClient.InvokeMethod(addUserInvokeInfo, "AddBasicUser");

            return addUserInvokeResult.Path;
        }

        private bool AddUserToRole(string userPath, string rolePath)
        {
            var user = configClient.GetItem(userPath);
            var sidProperty = user.Properties.Where(property => property.Key == "Sid").FirstOrDefault();

            // Adding role members is possible with the AddRoleMember method on the UserFolder of a specific Role
            string userFolderPath = string.Format("{0}/{1}", rolePath, ItemTypes.UserFolder);
            var userFolder = configClient.GetItem(userFolderPath);

            ConfigurationItem authorizeUserInfokeInfo = configClient.InvokeMethod(userFolder, "AddRoleMember");

            // Changing the properties of the invokeInfo object to the SID of the user
            var nameProperty = authorizeUserInfokeInfo.Properties.Where(property => property.Key == "Sid").FirstOrDefault();
            nameProperty.Value = sidProperty.Value;

            ConfigurationItem invokeResultItem = configClient.InvokeMethod(authorizeUserInfokeInfo, "AddRoleMember");

            return invokeResultItem.Properties.FirstOrDefault(property => property.Key == "State").Value == "Success";
        }
    }

    public class MilestoneUser
    {
        private ConfigurationItem configItem;
        public string Name => configItem.DisplayName;
        public string Path => configItem.Path;

        public FQID LastEndPoint { get; set; }

        public bool IsRequestingLogin { get; set; }
        
        public string Identity => configItem.Properties.Where(p => p.Key == "Sid").FirstOrDefault().Value;
        public string RoleName => configItem.Properties.Where(p => p.Key == "Description").FirstOrDefault().Value.Trim().Split('|')[1];
        public string PersonalNumber => configItem.Properties.Where(p => p.Key == "Description").FirstOrDefault().Value.Trim().Split('|')[0];
        public string RolePath { get; set; }

        public MilestoneUser(ConfigurationItem configItem)
        {
            this.configItem = configItem;
        }
    }

    [Serializable]
    public class TryggLoginUser
    {
        public TryggLoginUser(string name, string key, string role, string password = "", string hashedPw = "")
        {
            this.DisplayName = name;
            this.Key = key;
            this.BelongingRolePath = role;
            //this.RoleName = role;
            if (!string.IsNullOrEmpty(password))
                this.HashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            else
                HashedPassword = hashedPw;
        }

        public bool Verify(string password)
        {
            if (HashedPassword == "")
                return true;
            return BCrypt.Net.BCrypt.Verify(password, HashedPassword);
        }

        public string DisplayName { get; set; }
        public string Key { get; set; }
        public string BelongingRolePath { get; set; }
        public string RoleName { get; set; }
        public string HashedPassword { get; set; }
        public AuthKeyTypes KeyType { get; set; }
        public override string ToString() => DisplayName;

    }
    public enum AuthKeyTypes
    {
        BankID,
        Yubikey
    }
}
