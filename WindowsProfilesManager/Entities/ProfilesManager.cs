using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Win32;
using WindowsProfilesManager.Helpers;

namespace WindowsProfilesManager.Entities
{
    public class ProfilesManager
    {
        // Internal fields
        private string domainName;
        private List<UserProfileInfo> localProfilesList;

        // Win32 APIs interfaces
        [DllImport("userenv.dll", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
        private static extern bool DeleteProfile(string sidString, string profilePath, string computerName);

        /// <summary>
        /// Get local profiles
        /// </summary>
        public List<UserProfileInfo> LocalProfilesList
        {
            get
            {
                if (localProfilesList == null)
                    localProfilesList = GetProfilesList();

                return localProfilesList;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProfilesManager()
        {
            this.domainName = GetCurrentDomainName();
        }

        /// <summary>
        /// Get details from a specific user
        /// </summary>
        /// <param name="userNameOrSID"></param>
        /// <returns></returns>
        public UserInfo GetUser(string userNameOrSID)
        {
            IdentityType identityType = GetAccountIdentityType(userNameOrSID);

            // Try get local account
            var userInfo = FindUser(userNameOrSID, identityType);

            // Try get domain account
            if (userInfo == null)
                userInfo = FindUser(userNameOrSID, identityType, this.domainName);

            // Return
            return userInfo;
        }

        /// <summary>
        /// Get profile details from a specific user
        /// </summary>
        /// <param name="userNameOrSID"></param>
        /// <param name="identityType"></param>
        /// <returns></returns>
        public UserProfileInfo GetProfile(string userNameOrSID)
        {
            try
            {
                IdentityType identityType = GetAccountIdentityType(userNameOrSID);
                UserProfileInfo userProfileInfo = null;

                // Get user
                UserInfo user = GetUser(userNameOrSID);

                // User not found, create an empty one
                if (user == null)
                {
                    // Find profile locally
                    userProfileInfo = (identityType == IdentityType.Sid ?
                                                       FindLocalProfileBySID(userNameOrSID) :
                                                       FindLocalProfileByUserName(userNameOrSID));

                    if (userProfileInfo == null)
                        return null;

                    // Set user settings based on profile settings
                    user = UserInfo.CreateToUserNotFound(userNameOrSID, identityType);
                    user.Name = UserProfileHelper.GetUserNameFromProfilePath(userProfileInfo.ProfileImagePath);
                }
                else
                {
                    // Parsing user profile path
                    string userProfileRegistryPath = System.IO.Path.Combine(UserProfileHelper.USERS_PROFILES_REGISTRY_BASE_PATH, user.SID);

                    // Create user profile settings
                    RegistryKey userProfileKey = Registry.LocalMachine.OpenSubKey(userProfileRegistryPath, false);

                    // User profile not found
                    if (userProfileKey == null)
                        return null;

                    // Create object
                    userProfileInfo = UserProfileInfo.CreateUserProfileInfo(userProfileKey);
                }

                // Set user into profile
                userProfileInfo.User = user;

                // Return
                return userProfileInfo;
            }
            catch (NullReferenceException ex)
            {
                // User profile not found
                return null;
            }
        }

        /// <summary>
        /// Search local profiles by user SID
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public UserProfileInfo FindLocalProfileBySID(string sid)
        {
            return this.LocalProfilesList.Where(p => p.User.SID.Equals(sid, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Search local profiles by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserProfileInfo FindLocalProfileByUserName(string userName)
        {
            return this.LocalProfilesList.Where(p => p.User.Name.Equals(userName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Get all profiles from windows registry
        /// </summary>
        public List<UserProfileInfo> GetProfilesList()
        {
            List<UserProfileInfo> finalProfileList = new List<UserProfileInfo>();

            // Get all profiles available from registry
            RegistryKey profilesRootKey = Registry.LocalMachine.OpenSubKey(UserProfileHelper.USERS_PROFILES_REGISTRY_BASE_PATH, false);

            if (profilesRootKey != null)
            {
                foreach (string userSID in profilesRootKey.GetSubKeyNames())
                {
                    try
                    {
                        // Opening current user profile
                        string profileKeyPath = System.IO.Path.Combine(UserProfileHelper.USERS_PROFILES_REGISTRY_BASE_PATH, userSID);
                        RegistryKey profileKey = Registry.LocalMachine.OpenSubKey(profileKeyPath, false);

                        // Create new object and add to final list
                        var newProfileInfo = UserProfileInfo.CreateUserProfileInfo(profileKey);

                        // Get user
                        var user = GetUser(userSID);

                        if (user != null)
                            newProfileInfo.User = user;

                        // Add to final list
                        finalProfileList.Add(newProfileInfo);
                    }
                    catch
                    {
                    }
                }
            }

            // Return list
            return finalProfileList;
        }

        /// <summary>
        /// Get profiles from windows registry which user doesn't exists
        /// </summary>
        public List<UserProfileInfo> GetInvalidProfilesList()
        {
            return GetProfilesList().Where(p => p.User == null || p.User.Enabled.HasValue == false || p.User.Enabled == false).ToList();
        }

        /// <summary>
        /// Get all profiles from windows registry which is marked with .bak extension
        /// </summary>
        public List<UserProfileInfo> GetTemporaryProfilesList()
        {
            List<UserProfileInfo> temporaryProfileList = new List<UserProfileInfo>();

            // Get all profiles available from registry
            RegistryKey profilesRootKey = Registry.LocalMachine.OpenSubKey(UserProfileHelper.USERS_PROFILES_REGISTRY_BASE_PATH, false);

            if (profilesRootKey != null)
            {
                foreach (string userSID in profilesRootKey.GetSubKeyNames())
                {
                    if (!userSID.EndsWith(UserProfileHelper.USER_PROFILE_TEMPORARY_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    try
                    {
                        // Opening current user profile
                        string profileKeyPath = System.IO.Path.Combine(UserProfileHelper.USERS_PROFILES_REGISTRY_BASE_PATH, userSID);
                        RegistryKey profileKey = Registry.LocalMachine.OpenSubKey(profileKeyPath, false);

                        // Create new object and add to final list
                        var newProfileInfo = UserProfileInfo.CreateUserProfileInfo(profileKey);
                        newProfileInfo.User = GetUser(userSID.Replace(UserProfileHelper.USER_PROFILE_TEMPORARY_EXTENSION, ""));

                        // Add to final list
                        temporaryProfileList.Add(newProfileInfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("erro: {0}", ex.Message);
                    }
                }
            }

            // Return list
            return temporaryProfileList;
        }

        /// <summary>
        /// Deletes an user profile
        /// </summary>
        public void DeleteUserProfile(string userSID)
        {
            try
            {
                if (string.IsNullOrEmpty(userSID))
                    throw new ArgumentNullException(userSID);

                // Delete user profile
                DeleteProfile(userSID, null, null);

                // Delete .bak entry from registry
                RegistryKey profilesRootKey = Registry.LocalMachine.OpenSubKey(UserProfileHelper.USERS_PROFILES_REGISTRY_BASE_PATH, true);
                string userTemporaryKeyName = UserProfileHelper.GetTemporaryProfileRegistryKeyName(userSID);

                if (profilesRootKey != null)
                    profilesRootKey.DeleteSubKeyTree(userTemporaryKeyName);
            }
            catch (Exception ex)
            {
                // Nothing to do
            }
        }

        /// <summary>
        /// Deletes all profiles from unknown user accounts
        /// </summary>
        public void DeleteInvalidUsersProfiles()
        {
            var invalidProfiles = GetInvalidProfilesList();

            foreach (var profile in invalidProfiles)
            {
                DeleteUserProfile(profile.User.SID);
            }
        }

        /// <summary>
        /// Validate if a local user exists looking for its name or SID
        /// </summary>
        /// <param name="userNameOrSID"></param>
        /// <param name="identityType"></param>
        /// <returns></returns>
        private UserInfo FindUser(string userNameOrSID, IdentityType identityType, string domainName = null)
        {
            ContextType contextType = (string.IsNullOrEmpty(domainName) ? ContextType.Machine : ContextType.Domain);
            PrincipalContext principalContext = (contextType == ContextType.Machine ? new PrincipalContext(contextType) : new PrincipalContext(contextType, domainName));

            try
            {
                var userPrincipal = UserPrincipal.FindByIdentity(principalContext, identityType, userNameOrSID);

                // Looking for special accounts
                if (userPrincipal == null)
                {
                    var groupPrincipal = GroupPrincipal.FindByIdentity(principalContext, identityType, userNameOrSID);

                    if (groupPrincipal == null)
                    {
                        // Looking for IIs accounts
                        var iisUser = FindIIsAccount(userNameOrSID, identityType);

                        if (iisUser == null)
                            return null;

                        // Return iis account
                        return iisUser;
                    }

                    // Return based on group principal
                    return UserInfo.CreateFromGroupPrincipal(groupPrincipal, contextType);
                }

                // Return based on user principal
                return UserInfo.CreateFromUserPrincipal(userPrincipal, contextType);
            }
            catch (InvalidCastException ex)
            {
                var groupPrincipal = GroupPrincipal.FindByIdentity(principalContext, identityType, userNameOrSID);
                return UserInfo.CreateFromGroupPrincipal(groupPrincipal, contextType);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                // Release context
                if (principalContext != null)
                    principalContext.Dispose();
            }
        }

        /// <summary>
        /// Find IIS account (it's a special kind of local account!)
        /// Since IIS7, IIS uses virtual local accounts, which is in the format "IIS APPPOOL\applicationPoolName"
        /// </summary>
        /// <param name="userNameOrSID"></param>
        /// <param name="identityType"></param>
        /// <returns></returns>
        private UserInfo FindIIsAccount(string userNameOrSID, IdentityType identityType)
        {
            try
            {
                ContextType contextType = ContextType.Machine;

                var user = new NTAccount(string.Format("IIS APPPOOL\\{0}", userNameOrSID));
                string userSid = userNameOrSID;

                if (identityType != IdentityType.Sid)
                    userSid = user.Translate(typeof(SecurityIdentifier)).Value;

                using (var principalContext = new PrincipalContext(contextType))
                {
                    using (var groupPrincipal = GroupPrincipal.FindByIdentity(principalContext, IdentityType.Sid, userSid))
                    {
                        return UserInfo.CreateFromGroupPrincipal(groupPrincipal, contextType);
                    }
                }
            }
            catch (IdentityNotMappedException)
            {
                // User not found 
                return null;
            }
        }

        /// <summary>
        /// Validate if an account type is Sid or Name
        /// </summary>
        private IdentityType GetAccountIdentityType(string userNameOrSID, IdentityType defaultIdentityType = IdentityType.SamAccountName)
        {
            // Reference
            // https://support.microsoft.com/pt-br/help/243330/well-known-security-identifiers-in-windows-operating-systems
            if (userNameOrSID.StartsWith("S-1-0") ||
               userNameOrSID.StartsWith("S-1-1") ||
               userNameOrSID.StartsWith("S-1-2") ||
               userNameOrSID.StartsWith("S-1-3") ||
               userNameOrSID.StartsWith("S-1-4") ||
               userNameOrSID.StartsWith("S-1-5") ||
               userNameOrSID.StartsWith("S-1-16"))
            {
                return IdentityType.Sid;
            }

            // Return default identityType
            return defaultIdentityType;
        }

        /// <summary>
        /// Get current active directory domain
        /// </summary>
        private string GetCurrentDomainName()
        {
            return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
        }
    }
}