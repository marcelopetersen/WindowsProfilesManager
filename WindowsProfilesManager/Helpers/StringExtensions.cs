using System;
using System.Threading;
using System.Globalization;

namespace WindowsProfilesManager.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert an string to enumeration
        /// </summary>
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Capitalizes a text
        /// Ex: list => List
        /// </summary>
        public static string CapitalizeText(this string value)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(value);
        }
    }
}