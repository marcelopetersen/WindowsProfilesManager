using System;
using System.Collections.Generic;

namespace WindowsProfilesManager.Helpers
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Get a value from dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValue<T>(this Dictionary<string, object> dictionary, string key)
        {
            return (T)Convert.ChangeType(dictionary[key], typeof(T));
        }
    }
}
