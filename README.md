# Windows User Profiles Manager

This project has the focus on manage user profiles on Windows Operation System.
There are no commands available to manage user profile easilly, and you can use it to achieve that goal.

Basically, the following function is used to delete profiles:

```cs
[DllImport("userenv.dll", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
private static extern bool DeleteProfile(string sidString, string profilePath, string computerName);
```

Additional functions related to user profiles were implemented aiming in to make easier all those tasks.

## Using this script
To view instructions, call WindowsProfileManager.exe withou any parameter. You will see an output like this:

```
==============================================================================
Usage: WindowsProfilesManager.exe action [printType]
  action [required]: one of the following
     GetUser userNameOrSID [printType]
     GetProfile userNameOrSID [printType]
     ListProfiles [printType]
     ListInvalidProfiles [printType]
     ListTemporaryProfiles [printType]
     DeleteProfile userNameOrSID
     DeleteInvalidProfiles
     DeleteTemporaryProfiles
  printType [optional]: indicates how to print the result (List | Formatted)
==============================================================================
```

Below are described what exactly each action performs:
- *GetUser:* search for user on local machine and active directory domain
-- Usefull to check if a user exists and is enabled
-- WindowsProfileManager.exe GetUser user.name

- *GetProfile:* search for local profile to a specific user and if it was found, search for its status (like getUser action)
-- Usefull to check if a profile exists for a specific user
-- WindowsProfileManager.exe GetProfile user.name

- *ListProfiles:* shows all local profiles with additional information, like user status and if profile is temporary or not
-- Usefull to list all local profiles
-- WindowsProfileManager.exe ListProfiles

- *ListInvalidProfiles:* list all local profiles where the user doesn't exist or is disabled
-- Usefull to check if a clean up of unused profiles should be executed to free up disk space
-- WindowsProfileManager.exe ListInvalidProfiles

- *ListTemporaryProfiles:* sometimes when a user logs on a windows machine, by undefined reasons, it receive a temporary profile and its marked at registry with an .bak extension. This action search for that extension.
-- Usefull to check if there are profile in the situation described above
-- WindowsProfileManager.exe ListTemporaryProfiles

- *DeleteProfile:* deletes a specific user profile
-- Usefull to delete a specific profile
-- WindowsProfileManager.exe DeleteProfile userNameOrSID

- *DeleteInvalidProfiles:* should be used with action ListInvalidProfiles. This will delete those profiles from disk.
-- Usefull to free up disk space, deleting unused profiles
-- WindowsProfileManager.exe DeleteInvalidProfiles

- *DeleteTemporaryProfiles:* should be used with action ListTemporaryProfiles. This will delete those profiles from disk.
-- Usefull to delete temporary profiles, independent of user. All temporary profiles will be deleted


## License
You this program by your own responsibility !!!
You can make changes and use it for what you want and with no restrictions !!!

Enjoy yourself ...

