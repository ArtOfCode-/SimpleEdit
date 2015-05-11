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
using System.Windows;

namespace SimpleEdit
{
    static class EditMenuControls
    {
        private static MainWindow _window;

        public static FindReplaceDialog _findDialog;

        public static void Initialize(MainWindow window)
        {
            _window = window;
        }

        public static void Find()
        {
            if (_findDialog == null)
            {
                FindReplaceDialog dialog = new FindReplaceDialog(_window);
                dialog.Show();
                dialog.Closed += delegate(object sender, EventArgs e)
                {
                    EditMenuControls._findDialog = null;
                };
                _findDialog = dialog;
            }
            else
            {
                _findDialog.Focus();
            }
        }
    }
}
