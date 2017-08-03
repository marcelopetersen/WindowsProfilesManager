using System;
using System.Collections.Generic;
using System.Linq;
using WindowsProfilesManager.Helpers;
using WindowsProfilesManager.Entities;

namespace WindowsProfilesManager
{
    class Program
    {
        // Internal fields
        private static Dictionary<string, object> parameters;
        private static ProfilesManager profilesMgr = new ProfilesManager();
        
        /// <summary>
        /// Program main entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Parsing command line parameters
            parameters = ConsoleHelper.ParseCommandLineParameters(args);

            // Validate parameters
            if (parameters == null || parameters.Count == 0)
            {
                ConsoleHelper.ShowProgramHelp();
                return;
            }

            // Execute defined action
            switch (parameters.GetValue<string>("action"))
            {
                case "getuser":
                    {
                        GetUserAction();
                        break;
                    }
                case "getprofile":
                    {
                        GetProfileAction();
                        break;
                    }
                case "deleteprofile":
                    {
                        DeleteUserProfileAction();
                        break;
                    }
                case "listprofiles":
                    {
                        ListProfilesAction();
                        break;
                    }
                case "listtemporaryprofiles":
                    {
                        ListTemporaryProfilesAction();
                        break;
                    }
                case "listinvalidprofiles":
                    {
                        ListInvalidProfilesAction();
                        break;
                    }
                case "deletetemporaryprofiles":
                    {
                        DeleteTemporaryProfilesAction();
                        break;
                    }
                case "deleteinvalidprofiles":
                    {
                        DeleteInvalidProfilesAction();
                        break;
                    }
            }
        }

        /// <summary>
        /// Process get user action
        /// </summary>
        private static void GetUserAction()
        {
            UserInfo user = profilesMgr.GetUser(parameters.GetValue<string>("userName"));
            ConsoleHelper.PrintUserInfo(user);
        }

        /// <summary>
        /// Process get profile action
        /// </summary>
        private static void GetProfileAction()
        {
            UserProfileInfo profile = profilesMgr.GetProfile(parameters.GetValue<string>("userName"));
            ConsoleHelper.PrintUserProfileInfo(profile, parameters.GetValue<PrintType>("printType"));
        }

        /// <summary>
        /// Delete a user profile
        /// </summary>
        private static void DeleteUserProfileAction()
        {
            // Find user to determine SID
            string userName = parameters.GetValue<string>("userName");

            // Finding user profile looking for profile settings
            var profile = profilesMgr.GetProfile(userName);

            if (profile == null)
            {
                ConsoleHelper.PrintErrorMessageWithSurrondedSpaces("Profile not found of user [{0}].", userName);
                return;
            }
            else
            {
                profilesMgr.DeleteUserProfile(profile.User.SID);
                ConsoleHelper.PrintSuccessMessageWithSurrondedSpaces("Profile deleted successfully !!!");
            }
        }

        /// <summary>
        /// List all profiles
        /// </summary>
        private static void ListProfilesAction()
        {
            PrintType printType = parameters.GetValue<PrintType>("printType");
            List<UserProfileInfo> profilesList = profilesMgr.GetProfilesList();

            foreach (UserProfileInfo profile in profilesList.OrderBy(p => p.User.Enabled).ThenBy(p => p.User.Name))
            {
                ConsoleHelper.PrintUserProfileInfo(profile, printType);
            }
        }

        /// <summary>
        /// List all profiles which have .bak extension on windows registry
        /// </summary>
        private static void ListTemporaryProfilesAction()
        {
            PrintType printType = parameters.GetValue<PrintType>("printType");
            List<UserProfileInfo> profilesList = profilesMgr.GetTemporaryProfilesList();

            if (profilesList == null || profilesList.Count == 0)
            {
                ConsoleHelper.PrintErrorMessageWithSurrondedSpaces("No temporary profiles found !!!");
                return;
            }

            foreach (UserProfileInfo profile in profilesList.OrderBy(p => p.User.Enabled).ThenBy(p => p.User.Name))
            {
                ConsoleHelper.PrintUserProfileInfo(profile, printType);
            }
        }

        /// <summary>
        /// Delete all temporary profiles
        /// </summary>
        private static void DeleteTemporaryProfilesAction()
        {
            List<UserProfileInfo> temporaryProfiles = profilesMgr.GetTemporaryProfilesList();

            if (temporaryProfiles == null || temporaryProfiles.Count == 0)
            {
                ConsoleHelper.PrintErrorMessageWithSurrondedSpaces("No temporary profiles found !!!");
                return;
            }

            ConsoleHelper.PrintSuccessMessageWithSurrondedSpaces("Temporary profiles found: {0}", temporaryProfiles.Count());

            foreach (var profile in temporaryProfiles)
            {
                profilesMgr.DeleteUserProfile(profile.User.SID);
                ConsoleHelper.PrintSuccessMessage("Profile [{0}] deleted successfully !!!", profile.User.SID);
            }
        }

        /// <summary>
        /// List all invalid profiles (when user isn't found or is disabled)
        /// </summary>
        private static void ListInvalidProfilesAction()
        {
            PrintType printType = parameters.GetValue<PrintType>("printType");
            List<UserProfileInfo> invalidProfilesList = profilesMgr.GetInvalidProfilesList();

            if (invalidProfilesList == null || invalidProfilesList.Count() == 0)
            {
                ConsoleHelper.PrintErrorMessageWithSurrondedSpaces("No invalid profiles found!");
                return;
            }

            // Print profiles
            ConsoleHelper.PrintErrorMessageWithSurrondedSpaces("Profiles found: {0}", invalidProfilesList.Count());

            foreach (UserProfileInfo profile in invalidProfilesList)
            {
                ConsoleHelper.PrintUserProfileInfo(profile, printType);
            }
        }

        /// <summary>
        /// Delete all temporary profiles
        /// </summary>
        private static void DeleteInvalidProfilesAction()
        {
            List<UserProfileInfo> invalidProfilesList = profilesMgr.GetInvalidProfilesList();

            if (invalidProfilesList == null || invalidProfilesList.Count() == 0)
            {
                ConsoleHelper.PrintErrorMessageWithSurrondedSpaces("No invalid profiles found!");
                return;
            }

            ConsoleHelper.PrintSuccessMessageWithSurrondedSpaces("Invalid profiles found: {0}", invalidProfilesList.Count());

            foreach (var profile in invalidProfilesList)
            {
                profilesMgr.DeleteUserProfile(profile.User.SID);
                ConsoleHelper.PrintSuccessMessage("Profile [{0}] deleted successfully !!!", profile.User.SID);
            }
        }

        /// <summary>
        /// Compare all application identities with profiles list.
        /// </summary>
        private static void ListInvalidIIsProfilesAction()
        {
            throw new NotImplementedException();

            List<UserProfileInfo> profilesToRemove = new List<UserProfileInfo>();
            List<UserProfileInfo> profilesList = profilesMgr.GetProfilesList();
            List<string> applicationPoolIdentities = IIsHelper.GetApplicationPoolsIdentities();
            PrintType printType = parameters.GetValue<PrintType>("printType");

            foreach (var profile in profilesList)
            {
                if (!applicationPoolIdentities.Any(u => u.Equals(profile.User.Name, StringComparison.InvariantCultureIgnoreCase)))
                    profilesToRemove.Add(profile);
            }

            foreach (var item in profilesToRemove)
            {
                ConsoleHelper.PrintUserProfileInfo(item, printType);
            }
        }
    }
}