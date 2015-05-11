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
using System.ComponentModel;
using SimpleEdit.Tools;

namespace SimpleEdit.Menus
{
    static class FileMenuControls
    {
        private static MainWindow _window;

        public static void Initialize(MainWindow window)
        {
            _window = window;
        }

        public static void Save()
        {
            OpenFile.Save();
        }

        public static void SaveAs()
        {
            OpenFile.SaveAs();
        }

        public static void Open()
        {
            if (!OpenFile.IsSaved())
            {
                bool open = _window.ConfirmWithoutSaving("open a new file");
                if (open)
                {
                    OpenFile.Open();
                }
            }
            else
            {
                OpenFile.Open();
            }
        }

        public static void Exit()
        {
            if (!OpenFile.IsSaved())
            {
                bool exit = _window.ConfirmWithoutSaving("exit");
                if (exit)
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                Environment.Exit(0);
            }
        }
        public static void Exit(CancelEventArgs e)
        {
            if (!OpenFile.IsSaved())
            {
                bool exit = _window.ConfirmWithoutSaving("exit");
                if (exit)
                {
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public static void New()
        {
            if (!OpenFile.IsSaved())
            {
                bool createNew = _window.ConfirmWithoutSaving("create a new file");
                if (createNew)
                {
                    OpenFile.NewFile();
                }
            }
            else
            {
                OpenFile.NewFile();
            }
        }
    }
}
