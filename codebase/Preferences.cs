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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SimpleEdit.Tools
{
    public class Preferences
    {
        private string preferencesFile;
        private Program _program;
        private MainWindow _window;

        private Dictionary<string, string> currentPreferences;

        private Dictionary<PreferenceType, string> preferenceMappings = new Dictionary<PreferenceType, string>
            {
                { PreferenceType.FontFamily, "cfg?EditBox/Font:Family" },
                { PreferenceType.FontSize, "cfg?EditBox/Font:Size" },
                { PreferenceType.ColorsForeground, "cfg?EditBox/Colors:Foreground" },
                { PreferenceType.ColorsBackground, "cfg?EditBox/Colors:Background" },
                { PreferenceType.FormatTextWrap, "cfg?EditBox/Format:TextWrapping" },
                { PreferenceType.FormatTabLength, "cfg?EditBox/NoProperty/Format:TabLength" },
                { PreferenceType.FormatPreserveIndents, "cfg?EditBox/NoProperty/Format:PreserveIndents" }
            };

        private Dictionary<string, string> propertyMappings = new Dictionary<string, string>
            {
                { "cfg?EditBox/Font:Family", "FontFamilyProperty" },
                { "cfg?EditBox/Font:Size", "FontSizeProperty" },
                { "cfg?EditBox/Colors:Foreground", "ForegroundProperty" },
                { "cfg?EditBox/Colors:Background", "BackgroundProperty" },
                { "cfg?EditBox/Format:TextWrapping", "TextWrappingProperty" }
            };

        private Dictionary<string, Type> propertyTypes = new Dictionary<string, Type>
            {
                { "FontFamilyProperty", typeof(FontFamily) },
                { "FontSizeProperty", typeof(double) },
                { "ForegroundProperty", typeof(SolidColorBrush) },
                { "BackgroundProperty", typeof(SolidColorBrush) },
                { "TextWrappingProperty", typeof(TextWrapping) }
            };

        private DependencyObject element;

        public Preferences(Program program, MainWindow window)
        {
            if (program == null)
            {
                throw new ArgumentNullException("program");
            }
            _program = program;
            _window = window;
            preferencesFile = ApplicationFiles.GetFilePath(_program.WorkingDirectory, ApplicationFiles.UserConfig);
            currentPreferences = new Dictionary<string, string>();
            element = _window.EditBox;
            LoadPreferences();
            ApplyPreferences();
        }
        public Preferences(Program program, MainWindow window, string[] initialPreferences)
        {
            if (program == null)
            {
                throw new ArgumentNullException("program");
            }
            _program = program;
            _window = window;
            preferencesFile = ApplicationFiles.GetFilePath(_program.WorkingDirectory, ApplicationFiles.UserConfig);
            currentPreferences = new Dictionary<string, string>();
            element = _window.EditBox;
            LoadPreferences(initialPreferences);
            ApplyPreferences();
        }

        private void LoadPreferences()
        {
            string[] fileContents = File.ReadAllLines(preferencesFile);
            foreach (string line in fileContents)
            {
                Console.WriteLine("Read preference: {0}", line);
                string[] split = line.Split(new char[] { ' ' }, 2);
                currentPreferences.Add(split[0], split[1]);
            }
        }
        private void LoadPreferences(string[] fileContents)
        {
            foreach (string line in fileContents)
            {
                Console.WriteLine("Found preference: {0}", line);
                string[] split = line.Split(new char[] { ' ' }, 2);
                currentPreferences.Add(split[0], split[1]);
            }
        }

        private void ApplyPreferences()
        {
            foreach (KeyValuePair<string, string> pair in currentPreferences)
            {
                if (pair.Key.Contains("NoProperty"))
                {
                    continue;
                }
                UniversalValueConverter converter = new UniversalValueConverter();
                object value;
                try
                {                    
                    value = converter.Convert(pair.Value, propertyTypes[propertyMappings[pair.Key]]);
                    Dependencies.SetDependencyProperty(element, propertyMappings[pair.Key], value);
                }
                catch (Exception e)
                {
                    _window.TemporaryStatusMessage("User settings not applied: " + e.Message, Colors.Crimson, 5000);
                }
            }
        }

        public void SetPreference(PreferenceType preference, string value)
        {
            currentPreferences[preferenceMappings[preference]] = value;
            SavePreferences();
            ApplyPreferences();
        }
        public void SetPreferences(params KeyValuePair<PreferenceType, string>[] values)
        {
            foreach (KeyValuePair<PreferenceType, string> pair in values)
            {
                currentPreferences[preferenceMappings[pair.Key]] = pair.Value;
            }
            SavePreferences();
            ApplyPreferences();
        }

        public string GetPreference(PreferenceType preference)
        {
            string value;
            try
            {
                value = currentPreferences[preferenceMappings[preference]];
            }
            catch (KeyNotFoundException)
            {
                value = null;
            }
            return value;
        }

        private void SavePreferences()
        {
            List<string> preferences = new List<string>();
            foreach (KeyValuePair<string, string> pair in currentPreferences)
            {
                string pref = pair.Key + " " + pair.Value;
                preferences.Add(pref);
            }
            string[] lines = preferences.ToArray<string>();
            File.WriteAllLines(preferencesFile, lines);
        }

        public void ManagePreferencesWindow()
        {
            PreferencesWindow window = new PreferencesWindow(currentPreferences, this);
            window.Show();
        }
    }

    public enum PreferenceType
    {
        FontFamily,
        FontSize,
        ColorsForeground,
        ColorsBackground,
        FormatTextWrap,
        FormatTabLength,
        FormatPreserveIndents
    }
}
