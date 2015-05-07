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
                { PreferenceType.FormatTextWrap, "cfg?EditBox/Format:TextWrapping" }
            };

        private Dictionary<string, string> propertyMappings = new Dictionary<string, string>
            {
                { "cfg?EditBox/Font:Family", "FontFamilyProperty" },
                { "cfg?EditBox/Font:Size", "FontSizeProperty" },
                { "cfg?EditBox/Colors:Foreground", "ForegroundProperty" },
                { "cfg?EditBox/Colors:Background", "BackgroundProperty" },
                { "cfg?EditBox/Format:TextWrap", "TextWrappingProperty" }
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

        private void LoadPreferences()
        {
            string[] fileContents = File.ReadAllLines(preferencesFile);
            foreach (string line in fileContents)
            {
                string[] split = line.Split(new char[] { ' ' }, 2);
                currentPreferences.Add(split[0], split[1]);
            }
        }

        private void ApplyPreferences()
        {
            foreach (KeyValuePair<string, string> pair in currentPreferences)
            {
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
            return currentPreferences[preferenceMappings[preference]];
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
        FormatTextWrap
    }
}
