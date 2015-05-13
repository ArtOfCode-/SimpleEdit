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

namespace SimpleEdit
{
    public static class ApplicationFiles
    {
        public static string UserConfig = "{WorkingPath}\\user.cfg";

        public static string GetFilePath(string workingPath, string file)
        {
            if (workingPath == null || file == null)
            {
                throw new ArgumentNullException(file == null ? "file" : "workingPath");
            }
            if (!file.Contains("{WorkingPath}"))
            {
                throw new ArgumentException("The argument must have a {WorkingPath} replacement locator indicator.", "file");
            }
            return file.Replace("{WorkingPath}", workingPath);
        }
    }
}
