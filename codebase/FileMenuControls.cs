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
