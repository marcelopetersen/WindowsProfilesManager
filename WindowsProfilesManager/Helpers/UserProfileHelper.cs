using System;
using Microsoft.Win32;

namespace WindowsProfilesManager.Helpers
{
    public class UserProfileHelper
    {
        // Constants
        public const string USERS_PROFILES_REGISTRY_BASE_PATH = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList";
        public const string USER_PROFILE_TEMPORARY_EXTENSION = ".bak";

        /// <summary>
        /// Get user name from user profile path
        /// Path: C:\Users\administrator => administrator
        /// </summary>
        public static string GetUserNameFromProfilePath(string profilePath)
        {
            if (string.IsNullOrEmpty(profilePath) ||            // null or empty
                string.IsNullOrWhiteSpace(profilePath) ||       // null or empty space
                profilePath.IndexOf("\\") == -1)                // backslash doesn't exist
            {
                return null;
            }

            return profilePath.Substring(profilePath.LastIndexOf("\\") + 1);
        }

        /// <summary>
        /// Get user SID from registry key full name
        /// Ex: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\S-1-5-21-290744199-52574505-398547282-568635  =>> S-1-5-21-290744199-52574505-398547282-568635
        /// </summary>
        public static string GetUserSIDFromProfilePath(string registryKeyFullPath)
        {
            if (string.IsNullOrEmpty(registryKeyFullPath) ||            // null or empty
                string.IsNullOrWhiteSpace(registryKeyFullPath) ||       // null or empty space
                registryKeyFullPath.IndexOf("\\") == -1)                // backslash doesn't exist
            {
                return null;
            }

            return registryKeyFullPath.Substring(registryKeyFullPath.LastIndexOf("\\") + 1);
        }

        /// <summary>
        /// Get registry key name to a temporary profile
        /// Ex: S-1-5-21-290744199-52574505-398547282-568635.bak
        /// </summary>
        /// <param name="userSID"></param>
        /// <returns></returns>
        public static string GetTemporaryProfileRegistryKeyName(string userSID)
        {
            if (USER_PROFILE_TEMPORARY_EXTENSION.StartsWith("."))
                return string.Format("{0}{1}", userSID, USER_PROFILE_TEMPORARY_EXTENSION);
            else
                return string.Format("{0}.{1}", userSID, USER_PROFILE_TEMPORARY_EXTENSION);
        }

        /// <summary>
        /// Validate if an profile registry ends with .bak extension
        /// </summary>
        public static bool IsTemporaryProfile(RegistryKey userProfileKey)
        {
            return IsTemporaryProfile(userProfileKey.Name);
        }

        /// <summary>
        /// Validate if an profile registry ends with .bak extension
        /// </summary>
        public static bool IsTemporaryProfile(string userProfileKeyName)
        {
            if (string.IsNullOrEmpty(userProfileKeyName))
                return false;

            return (userProfileKeyName.EndsWith(USER_PROFILE_TEMPORARY_EXTENSION, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
