using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace SimpleEdit.Tools
{
    static class OpenFile
    {
        private static string _filename = "New File";

        private static Dictionary<Key, Action> _controlChars = new Dictionary<Key, Action>
        {
            {Key.S, Save},
            {Key.O, Open},
            {Key.F, EditMenuControls.Find}
        };

        private static MainWindow _window;

        public static void Initialize(MainWindow window)
        {
            _window = window;
        }

        private static bool _saved = true;

        public static bool IsSaved()
        {
            return _saved;
        }
        public static void SetSaved(bool isSaved)
        {
            _saved = isSaved;
            if (isSaved)
            {
                _window.Title = _filename + " - SimpleEdit";
            }
            else
            {
                _window.Title = _filename + "* - SimpleEdit";
            }
        }

        public static void Save()
        {
            string successMessage = "Saved";
            _window.SetStatusBar("Saving", Colors.OrangeRed);
            if (_filename != "New File")
            {
                string text = _window.EditBox.Text;
                File.WriteAllText(_filename, text);
                SetSaved(true);
                _window.TemporaryStatusMessage(successMessage, Colors.Green, 1000);
            }
            else
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveDialog.Filter = "All Documents|*.*";
                Nullable<bool> result = saveDialog.ShowDialog();
                if (result == true)
                {
                    string filename = saveDialog.FileName;
                    _filename = filename;
                    string text = _window.EditBox.Text;
                    File.WriteAllText(_filename, text);
                    SetSaved(true);
                    _window.TemporaryStatusMessage(successMessage, Colors.Green, 1000);
                }
            }
        }

        public static void SaveAs()
        {
            string successMessage = "Saved";
            _window.SetStatusBar("Saving", Colors.OrangeRed);
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDialog.Filter = "All Documents|*.*";
            Nullable<bool> result = saveDialog.ShowDialog();
            if (result == true)
            {
                string filename = saveDialog.FileName;
                _filename = filename;
                string text = _window.EditBox.Text;
                File.WriteAllText(_filename, text);
                SetSaved(true);
                _window.TemporaryStatusMessage(successMessage, Colors.Green, 1000);
            }
        }

        public static void NewFile()
        {
            _window.SetText("");
            _filename = "New File";
            SetSaved(true);
        }

        public static bool HandleControlChar(Key character)
        {
            if (_controlChars.ContainsKey(character))
            {
                _controlChars[character].Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Open()
        {
            _window.SetStatusBar("Opening document", Colors.OrangeRed);
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Documents|*.*";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string filepath = dialog.FileName;
                _filename = filepath;
                _window.SetText(File.ReadAllText(filepath));
                SetSaved(true);
            }
            _window.SetStatusBar("Ready", Colors.DodgerBlue);
        }
        public static void Open(string filename)
        {
            _window.SetStatusBar("Opening document", Colors.OrangeRed);
            if (!IsSaved())
            {
                bool open = _window.ConfirmWithoutSaving("open a new file");
                if (open)
                {
                    _filename = filename;
                    _window.SetText(File.ReadAllText(filename));
                    SetSaved(true);
                }
            }
            _window.SetStatusBar("Ready", Colors.DodgerBlue);
        }

        public static void InvokeOpen(string filename)
        {
            // Console.WriteLine("invokeopen call");
            _window.Dispatcher.BeginInvoke(new Action(delegate()
            {
                _window.SetStatusBar("Opening document", Colors.OrangeRed);
                if (!IsSaved())
                {
                    bool open = _window.ConfirmWithoutSaving("open a new file");
                    if (open)
                    {
                        _filename = filename;
                        _window.SetText(File.ReadAllText(filename));
                        SetSaved(true);
                    }
                }
                else
                {
                    _filename = filename;
                    _window.SetText(File.ReadAllText(filename));
                    SetSaved(true);
                }
                _window.SetStatusBar("Ready", Colors.DodgerBlue);
            }));
        }
    }
}
