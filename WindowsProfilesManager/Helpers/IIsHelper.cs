using System;
using System.Collections.Generic;
using Microsoft.Web.Administration;

namespace WindowsProfilesManager.Helpers
{
    public class IIsHelper
    {
        /// <summary>
        /// List all identities used on each application pool identity
        /// </summary>
        public static List<string> GetApplicationPoolsIdentities()
        {
            List<string> appPoolsIdentities = new List<string>();

            using (var serverManager = new ServerManager())
            {
                foreach (var appPool in serverManager.ApplicationPools)
                {
                    if (appPool.ProcessModel.IdentityType == ProcessModelIdentityType.ApplicationPoolIdentity)
                    {
                        appPoolsIdentities.Add(appPool.Name);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(appPool.ProcessModel.UserName))
                            appPoolsIdentities.Add(appPool.ProcessModel.UserName);
                    }
                }
            }

            return appPoolsIdentities;
        }
    }
}
