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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

[assembly:CLSCompliant(true)]
namespace SimpleEdit.Tools
{
    public class Program
    {
        private MainWindow _window;

        private bool isFirstRun = true;
        public bool IsFirstRun
        {
            get
            {
                return isFirstRun;
            }
            private set
            {
                isFirstRun = value;
            }
        }

        public string WorkingDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public Program(MainWindow window)
        {
            _window = window;
            IsFirstRun = CheckIfFirstRun();
            if (IsFirstRun)
            {
                SetupFirstRun();
            }
            else
            {
                _window.Preferences = new Preferences(this, _window);
            }
            ParseCommandLineArgs();
        }

        private bool CheckIfFirstRun()
        {
            return !File.Exists(ApplicationFiles.GetFilePath(WorkingDirectory, ApplicationFiles.UserConfig));
        }

        private void SetupFirstRun()
        {
            string[] lines = new string[] {
                    "cfg?EditBox/Font:Family Consolas",
                    "cfg?EditBox/Font:Size 12",
                    "cfg?EditBox/Colors:Foreground Black",
                    "cfg?EditBox/Colors:Background White",
                    "cfg?EditBox/Format:TextWrapping Wrap",
                    "cfg?EditBox/NoProperty/Format:TabLength 4",
                    "cfg?EditBox/NoProperty/Format:PreserveIndents True"
                };
            File.WriteAllLines(ApplicationFiles.GetFilePath(WorkingDirectory, ApplicationFiles.UserConfig), lines);
            _window.Preferences = new Preferences(this, _window, lines);
        }

        private void ParseCommandLineArgs()
        {
            List<string> arguments = Environment.GetCommandLineArgs().ToList<string>();
            arguments.RemoveAt(0);
            string[] args = arguments.ToArray<string>();
            Thread parseThread = new Thread(new ParameterizedThreadStart(ParseArgsThread));
            parseThread.Start((object)args);
        }

        private void ParseArgsThread(object arguments)
        {
            string[] args = (string[])arguments;
            bool isLoaded = false;
            bool isInitialized = false;
            _window.Dispatcher.BeginInvoke(new Action(delegate()
            {
                isLoaded = _window.IsLoaded;
                isInitialized = _window.IsInitialized;
            }));
            while (!isLoaded || !isInitialized)
            {
                Thread.Sleep(100);
                _window.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    isLoaded = _window.IsLoaded;
                    isInitialized = _window.IsInitialized;
                }));
            }
            if (args.Length > 0) {
                if (File.Exists(args[0]))
                {
                    OpenFile.InvokeOpen(args[0]);
                }
                if (args.Contains("--reset"))
                {
                    File.Delete(ApplicationFiles.GetFilePath(WorkingDirectory, ApplicationFiles.UserConfig));
                    Environment.Exit(0);
                }
                if (args.Contains("--recreate"))
                {
                    File.Delete(ApplicationFiles.GetFilePath(WorkingDirectory, ApplicationFiles.UserConfig));
                    _window.Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        SetupFirstRun();
                    }));
                }
                else if (args.Contains("--recreate-exit"))
                {
                    File.Delete(ApplicationFiles.GetFilePath(WorkingDirectory, ApplicationFiles.UserConfig));
                    _window.Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        SetupFirstRun();
                    }));
                    Environment.Exit(0);
                }
            }
        }
    }
}