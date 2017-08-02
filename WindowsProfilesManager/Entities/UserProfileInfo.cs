using System;
using Microsoft.Win32;
using WindowsProfilesManager.Helpers;

namespace WindowsProfilesManager.Entities
{
    public class UserProfileInfo
    {
        public UserInfo User { get; set; }
        public string ProfileImagePath { get; set; }
        public string Flags { get; set; }
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UserProfileInfo()
        {
            this.User = new UserInfo();
        }

        /// <summary>
        /// Factory method that creates a new UserProfileInfo object
        /// </summary>
        /// <param name="userSID"></param>
        /// <param name="userProfileKey"></param>
        /// <returns></returns>
        public static UserProfileInfo CreateUserProfileInfo(RegistryKey userProfileKey)
        {
            var newProfileInfo = new UserProfileInfo();
            newProfileInfo.ProfileImagePath = userProfileKey.GetValue("ProfileImagePath", string.Empty).ToString();
            newProfileInfo.Flags = userProfileKey.GetValue("Flags", string.Empty).ToString();
            newProfileInfo.IsTemporary = UserProfileHelper.IsTemporaryProfile(userProfileKey);
            newProfileInfo.User.Name = UserProfileHelper.GetUserNameFromProfilePath(newProfileInfo.ProfileImagePath);
            newProfileInfo.User.SID = UserProfileHelper.GetUserSIDFromProfilePath(userProfileKey.Name);
            return newProfileInfo;
        }

        /// <summary>
        /// Override of ToString() method
        /// </summary>
        public override string ToString()
        {
            return string.Format("UserName: {0} | SID: {1} | Enabled: {2} | Context: {3} | ProfileImagePath: {4} | IsTemporary: {5} | Flags: {6}",
                                            this.User.Name,
                                            this.User.SID,
                                            this.User.Enabled,
                                            (this.User.Context.HasValue ? this.User.Context.ToString() : "Not defined"),
                                            this.ProfileImagePath,
                                            this.IsTemporary,
                                            this.Flags);
        }
    }
}