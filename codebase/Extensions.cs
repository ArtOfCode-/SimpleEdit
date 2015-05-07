using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEdit.Tools
{
    static class Extensions
    {
        public static IEnumerable<int> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("The string to find should not be empty.", "value");
            }
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }

        public static Int32 CountInstancesOfSubstring(this string str, string searchText)
        {
            if (searchText == null)
            {
                throw new ArgumentNullException("searchText");
            }
            if (String.IsNullOrEmpty(searchText))
            {
                throw new ArgumentException("Search text should not be an empty string.");
            }
            return ((str.Length - str.Replace(searchText, "").Length) / searchText.Length);
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            if (search == null || replace == null)
            {
                throw new ArgumentNullException((search == null ? "search" : "replace"));
            }
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
