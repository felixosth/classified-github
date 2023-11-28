using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;

namespace LoginShared.Administration
{
    [Serializable]
    public class User
    {
        public string DisplayName { get; set; }

        public string SID { get; set; }

        public string AuthData { get; set; }

        public string[] Roles { get; set; }

        public Constants.Authenticators Authenticator { get; set; }

        public bool IsGroup { get; set; }

        [JsonIgnore]
        public FQID EndPoint { get; set; }

        [JsonIgnore]
        public string AttachedData { get; set; }

        public User()
        {

        }

        public User(string sid)
        {
            this.SID = sid;
            var userPrincipal = GetUserPrincipal();
            DisplayName = userPrincipal.DisplayName;
        }

        public GroupPrincipal GetGroupPrincipal()
        {
            if (!IsGroup)
                throw new Exception("User is not a group");

            using (var context = Helper.GetPrincipalContext())
            {
                return GroupPrincipal.FindByIdentity(context, IdentityType.Sid, SID);
            }
        }

        public UserPrincipal GetUserPrincipal()
        {
            if (IsGroup)
                throw new Exception("User is a group");

            using (var context = Helper.GetPrincipalContext())
            {
                return UserPrincipal.FindByIdentity(context, IdentityType.Sid, SID);
            }
        }

        public List<GroupPrincipal> GetGroups()
        {
            if (IsGroup)
                throw new Exception("User is a group");

            List<GroupPrincipal> result = new List<GroupPrincipal>();

            using (var context = Helper.GetPrincipalContext())
            {
                UserPrincipal me = UserPrincipal.FindByIdentity(context, IdentityType.Sid, SID);

                if(me != null)
                {
                    var groups = me.GetAuthorizationGroups();

                    foreach(Principal p in groups)
                    {
                        if(p is GroupPrincipal)
                        {
                            result.Add((GroupPrincipal)p);
                        }
                    }
                }
            }
            return result;
        }
    }
}
