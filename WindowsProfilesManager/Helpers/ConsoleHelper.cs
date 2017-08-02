using System;
using System.Collections.Generic;
using WindowsProfilesManager.Entities;

namespace WindowsProfilesManager.Helpers
{
    public class ConsoleHelper
    {
        private static ConsoleColor defaultConsoleForegroundColor = Console.ForegroundColor;
        private static ConsoleColor successConsoleForegroundColor = ConsoleColor.Yellow;
        private static ConsoleColor warningConsoleForegroundColor = ConsoleColor.Red;

        /// <summary>
        /// Show help about how to use this script
        /// </summary>
        public static void ShowProgramHelp()
        {
            Console.ForegroundColor = successConsoleForegroundColor;
            Console.WriteLine("");
            Console.WriteLine("==================================================================================");
            Console.WriteLine("== Usage: WindowsProfilesManager.exe action [printType]");
            Console.WriteLine("==================================================================================");
            Console.WriteLine("  action [required]: one of the following");
            Console.WriteLine("     GetUser userNameOrSID [printType]");
            Console.WriteLine("     GetProfile userNameOrSID [printType]");
            Console.WriteLine("     ListProfiles [printType]");
            Console.WriteLine("     ListInvalidProfiles [printType]");
            Console.WriteLine("     ListTemporaryProfiles [printType]");
            Console.WriteLine("     DeleteProfile userNameOrSID");
            Console.WriteLine("     DeleteInvalidProfiles");
            Console.WriteLine("     DeleteTemporaryProfiles");
            Console.WriteLine("  printType [optional]: indicates how to print the result (List | Formatted)");
            Console.WriteLine("==================================================================================");
            Console.WriteLine("");
            Console.ForegroundColor = defaultConsoleForegroundColor;
        }

        /// <summary>
        /// Get command line parameters
        /// </summary>
        public static Dictionary<string, object> ParseCommandLineParameters(string[] args)
        {
            try
            {
                /*//////////////////////////////////////////////////////////
                  Available actions: 
                     GetUser userName [printType]
                     GetProfile userName [printType]
                     DeleteProfile userName
                     ListProfiles [printType]
                     ListTemporaryProfiles [printType]
                     ListInvalidProfiles [printType]
                     DeleteTemporaryProfiles
                     DeleteInvalidProfiles
                //////////////////////////////////////////////////////////*/

                if (args == null || args.Length == 0)
                    return null;

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                // Common parameters
                string action = args[0].ToLower();
                parameters.Add("action", action);
                parameters.Add("printType", PrintType.Formatted);

                // Validate specific parameters to each action
                switch (action)
                {
                    case "getuser":
                    case "getprofile":
                    case "deleteprofile":
                        {
                            if (args.Length < 2)
                                throw new Exception();

                            parameters.Add("userName", args[1]);

                            if (args.Length == 3)
                                parameters["printType"] = args[2].CapitalizeText().ToEnum<PrintType>();

                            break;
                        }
                    case "listprofiles":
                    case "listtemporaryprofiles":
                    case "listinvalidprofiles":
                    case "listinvalidiisprofiles":
                    case "deleteinvalidprofiles":
                    case "deletetemporaryprofiles":
                        {
                            if (args.Length == 2)
                                parameters["printType"] = args[1].CapitalizeText().ToEnum<PrintType>();

                            break;
                        }
                    default:
                        {
                            throw new Exception("Specified action is invalid !!!");
                        }
                }

                // return
                return parameters;
            }
            catch
            {
                // Error, return null
                return null;
            }
        }

        /// <summary>
        /// Print success message
        /// </summary>
        public static void PrintSuccessMessage(string message, params object[] parameters)
        {
            Console.ForegroundColor = successConsoleForegroundColor;
            Console.WriteLine(string.Format(message, parameters));
            Console.ForegroundColor = defaultConsoleForegroundColor;
        }

        /// <summary>
        /// Print success message
        /// </summary>
        public static void PrintSuccessMessageWithSurrondedSpaces(string message, params object[] parameters)
        {
            Console.ForegroundColor = successConsoleForegroundColor;
            Console.WriteLine("");
            Console.WriteLine(string.Format(message, parameters));
            Console.WriteLine("");
            Console.ForegroundColor = defaultConsoleForegroundColor;
        }

        /// <summary>
        /// Print error message
        /// </summary>
        public static void PrintErrorMessage(string message, params object[] parameters)
        {
            Console.ForegroundColor = warningConsoleForegroundColor;
            Console.WriteLine(string.Format(message, parameters));
            Console.ForegroundColor = defaultConsoleForegroundColor;
        }

        /// <summary>
        /// Print error message
        /// </summary>
        public static void PrintErrorMessageWithSurrondedSpaces(string message, params object[] parameters)
        {
            Console.ForegroundColor = warningConsoleForegroundColor;
            Console.WriteLine("");
            Console.WriteLine(string.Format(message, parameters));
            Console.WriteLine("");
            Console.ForegroundColor = defaultConsoleForegroundColor;
        }

        /// <summary>
        /// Print user details
        /// </summary>
        public static void PrintUserInfo(UserInfo user, PrintType printType = PrintType.Formatted)
        {
            if (user == null)
            {
                PrintErrorMessageWithSurrondedSpaces("User not found !!!");
            }
            else
            {
                if (printType == PrintType.Formatted)
                {
                    Console.ForegroundColor = successConsoleForegroundColor;
                    Console.WriteLine("");
                    Console.WriteLine("{0,-10} {1,5:N1}", "Name", user.Name);
                    Console.WriteLine("{0,-10} {1,5:N1}", "Exists", user.Exists);
                    Console.WriteLine("{0,-10} {1,5:N1}", "Enabled", user.Enabled);
                    Console.WriteLine("{0,-10} {1,5:N1}", "Context", user.Context.ToString());
                    Console.WriteLine("{0,-10} {1,5:N1}", "SID", user.SID);
                    Console.WriteLine("");
                    Console.ForegroundColor = defaultConsoleForegroundColor;
                }
                else
                {
                    Console.WriteLine(user.ToString());
                }
            }
        }

        /// <summary>
        /// Print user profile
        /// </summary>
        public static void PrintUserProfileInfo(UserProfileInfo profile, PrintType printType = PrintType.Formatted)
        {
            if (profile == null)
            {
                PrintErrorMessageWithSurrondedSpaces("User profile not found !!!");
            }
            else
            {
                if (printType == PrintType.Formatted)
                {
                    Console.ForegroundColor = successConsoleForegroundColor;
                    Console.WriteLine("");

                    // User details
                    Console.WriteLine("{0,-20} {1,5:N1}", "User Name", profile.User.Name);
                    Console.WriteLine("{0,-20} {1,5:N1}", "User Exists", profile.User.Exists);
                    Console.WriteLine("{0,-20} {1,5:N1}", "User is Enabled", (profile.User.Enabled.HasValue ? profile.User.Enabled.ToString() : "Not defined"));
                    Console.WriteLine("{0,-20} {1,5:N1}", "User Context", profile.User.Context.ToString());
                    Console.WriteLine("{0,-20} {1,5:N1}", "User SID", profile.User.SID);

                    // Profile details
                    Console.WriteLine("{0,-20} {1,5:N1}", "ProfileImagePath", profile.ProfileImagePath);
                    Console.WriteLine("{0,-20} {1,5:N1}", "IsTemporary", profile.IsTemporary);
                    Console.WriteLine("{0,-20} {1,5:N1}", "Flags", profile.Flags);
                    Console.WriteLine("");
                    Console.ForegroundColor = defaultConsoleForegroundColor;
                }
                else
                {
                    Console.WriteLine(profile.ToString());
                }
            }
        }
    }
}