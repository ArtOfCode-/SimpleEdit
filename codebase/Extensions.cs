/* SimpleEdit - Windows Notepad replacement text editor
Copyright (C) 2015 Owen Jenkins

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/copyleft/gpl.html>. */


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
