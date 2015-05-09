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
            if (_window.IsLoaded && _window.IsInitialized)
            {
                if (args[0] != null && File.Exists(args[0]))
                {
                    OpenFile.Open(args[0]);
                }
            }
            else
            {
                Thread parseThread = new Thread(new ParameterizedThreadStart(ParseArgsThread));
                parseThread.Start((object)args);
            }
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
            if (args.Length > 0 && File.Exists(args[0]))
            {
                OpenFile.InvokeOpen(args[0]);
            }
        }
    }
}
