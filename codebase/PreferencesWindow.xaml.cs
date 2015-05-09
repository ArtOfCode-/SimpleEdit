using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SimpleEdit.Tools;

namespace SimpleEdit
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        private Dictionary<string, string> _preferences;

        private Preferences _preferenceService;

        private UniversalValueConverter converter;

        public PreferencesWindow(Dictionary<string, string> preferences, Preferences preferenceService)
        {
            InitializeComponent();
            _preferences = preferences;
            _preferenceService = preferenceService;
            converter = new UniversalValueConverter();
            ForegroundPicker.SelectedColor = (Color)converter.Convert((object)_preferences["cfg?EditBox/Colors:Foreground"], typeof(Color));
            BackgroundPicker.SelectedColor = (Color)converter.Convert((object)_preferences["cfg?EditBox/Colors:Background"], typeof(Color));
            FontPicker.SelectedItem = (FontFamily)converter.Convert((object)_preferences["cfg?EditBox/Font:Family"], typeof(FontFamily));
            FontSizeInput.Text = _preferences["cfg?EditBox/Font:Size"];
            TabLengthInput.Text = _preferences["cfg?EditBox/NoProperty/Format:TabLength"];
            PreserveIndents.IsChecked = (_preferences["cfg?EditBox/NoProperty/Format:PreserveIndents"] == "True");
        }

        private void ApplyPrefs(object sender, RoutedEventArgs e)
        {
            _preferenceService.SetPreferences(
                new KeyValuePair<PreferenceType, string>(PreferenceType.ColorsBackground, BackgroundPicker.SelectedColor.ToString()),
                new KeyValuePair<PreferenceType, string>(PreferenceType.ColorsForeground, ForegroundPicker.SelectedColor.ToString()),
                new KeyValuePair<PreferenceType, string>(PreferenceType.FontFamily, FontPicker.SelectedItem.ToString()),
                new KeyValuePair<PreferenceType, string>(PreferenceType.FontSize, FontSizeInput.Text),
                new KeyValuePair<PreferenceType, string>(PreferenceType.FormatPreserveIndents, 
                    (PreserveIndents.IsChecked == true ? "True" : "False")),
                new KeyValuePair<PreferenceType, string>(PreferenceType.FormatTabLength, TabLengthInput.Text)
            );
        }

        private void FontSizeInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (!char.IsNumber(c))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
            base.OnPreviewTextInput(e);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
