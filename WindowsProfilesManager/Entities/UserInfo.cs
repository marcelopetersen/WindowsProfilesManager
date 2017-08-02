using System;
using System.DirectoryServices.AccountManagement;

namespace WindowsProfilesManager.Entities
{
    public class UserInfo
    {
        public string Name { get; set; }
        public string SID { get; set; }
        public bool? Enabled { get; set; }
        public ContextType? Context { get; set; }
        public bool Exists { get; set; }
        public bool IsUserPrincipal { get; set; }

        /// <summary>
        /// Factory method which creates a new object to a not found user
        /// </summary>
        public static UserInfo CreateToUserNotFound(string userNameOrSID, IdentityType identityType)
        {
            return new UserInfo()
            {
                Name = (identityType == IdentityType.SamAccountName ? userNameOrSID : null),
                SID = (identityType == IdentityType.Sid ? userNameOrSID : null),
                Context = null,
                Enabled = null,
                Exists = false,
                IsUserPrincipal = false
            };
        }

        /// <summary>
        /// Factory method which creates a new object based on UserPrincipal object
        /// </summary>
        public static UserInfo CreateFromUserPrincipal(UserPrincipal userPrincipal, ContextType contextType)
        {
            return new UserInfo()
            {
                Name = userPrincipal.Name,
                SID = userPrincipal.Sid.ToString(),
                Enabled = userPrincipal.Enabled,
                Context = contextType,
                IsUserPrincipal = true,
                Exists = true
            };
        }

        /// <summary>
        /// Factory method which creates a new object based on GroupPrincipal object
        /// </summary>
        public static UserInfo CreateFromGroupPrincipal(GroupPrincipal groupPrincipal, ContextType contextType)
        {
            return new UserInfo()
            {
                Name = groupPrincipal.SamAccountName,
                SID = groupPrincipal.Sid.ToString(),
                Context = contextType,
                IsUserPrincipal = false,
                Exists = true,
                Enabled = true
            };
        }

        /// <summary>
        /// Override of ToString() method
        /// </summary>
        public override string ToString()
        {
            return string.Format("UserName: {0} | UserSID: {1} | Context: {2} | Enabled: {3}",
                                            this.Name,
                                            this.SID,
                                            this.Context.ToString(),
                                            (this.Enabled.HasValue ? this.Enabled.ToString() : "Not defined"));
        }
    }
}