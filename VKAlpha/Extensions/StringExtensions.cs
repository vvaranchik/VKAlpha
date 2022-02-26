using System;

namespace VKAlpha.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp) => source.IndexOf(toCheck, comp) >= 0;

        /// <summary>
        /// Returns a new string in which all occurrences of a specified Unicode characters
        /// in this instance are replaced with empty Unicode character.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="charsToReplace">The Unicode characters to be replaced.</param>
        /// <returns> 
        /// A string that is equivalent to this instance except that all instances of charsToReplace
        /// are replaced with empty char. If charsToReplace is not found in the current instance, the
        /// method returns the current instance unchanged.
        /// </returns>
        public static string Replace(this string str, char[] charsToReplace)
        {
            var result = str;
            foreach(var c in charsToReplace)
                result = result.Replace(c, '\0');
            return result;
        }

        /// <summary>
        /// Returns a new string in which all occurrences of a specified Unicode characters
        /// in this instance are replaced with empty Unicode character.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="stringsToReplace">The Unicode strings to be replaced.</param>
        /// <returns> 
        /// A string that is equivalent to this instance except that all instances of stringsToReplace
        /// are replaced with empty char. If charsToReplace is not found in the current instance, the
        /// method returns the current instance unchanged.
        /// </returns>
        public static string Replace(this string str, string[] stringsToReplace)
        {
            var result = str;
            foreach (var s in stringsToReplace)
                result = result.Replace(s, "");
            return result;
        }
    }
}
