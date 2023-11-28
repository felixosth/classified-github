using System.DirectoryServices.AccountManagement;

namespace LoginShared.AD
{
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("User")]
    public class UserPrincipalEx : UserPrincipal
    {
        // Implement the constructor using the base class constructor. 
        public UserPrincipalEx(PrincipalContext context) : base(context)
        { }

        // Implement the constructor with initialization parameters.    
        public UserPrincipalEx(PrincipalContext context,
                                string samAccountName,
                                string password,
                                bool enabled) : base(context, samAccountName, password, enabled)
        { }

        // Create the required property.    
        [DirectoryProperty("employeeNumber")]
        public string EmployeeNumber
        {
            get
            {
                if (ExtensionGet("employeeNumber").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("employeeNumber")[0];
            }
            set { ExtensionSet("employeeNumber", value); }
        }

        // Implement the overloaded search method FindByIdentity.
        public static new UserPrincipalEx FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityValue);
        }

        // Implement the overloaded search method FindByIdentity. 
        public static new UserPrincipalEx FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityType, identityValue);
        }
    }
}
