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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SimpleEdit.Tools;
using SimpleEdit.Menus;

namespace SimpleEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO: Complete menus
        //     : Add configuration menu item

        private Program program;

        public Preferences Preferences;

        private int EditingColumn = 0;

        private Dictionary<string, string> _additionStrings = new Dictionary<string, string>
        {
            { "ItemBold", "**text**" },
            { "ItemItalic", "*text*" },
            { "ItemBulletList", "- text" },
            { "ItemLink", "[{text}]({url})" },
            { "ItemImage", "![{alt}]({url})" },
            { "ItemCode", "    " }
        };

        // Entry point
        public MainWindow()
        {
            this.Closing += MainWindow_Closing;
            InitializeComponent();
            program = new Program(this);
            EditBox.Focus();
            OpenFile.Initialize(this);
            FileMenuControls.Initialize(this);
            EditMenuControls.Initialize(this);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            FileMenuControls.Exit(e);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeChangeRatio heightRatio = new SizeChangeRatio(e.PreviousSize.Height, e.NewSize.Height);
            SizeChangeRatio widthRatio = new SizeChangeRatio(e.PreviousSize.Width, e.NewSize.Width);
            foreach (UIElement element in Container.Children)
            {
                FrameworkElement fElement = (FrameworkElement)element;
                fElement.Height *= (heightRatio.GetRatio() != Double.PositiveInfinity ? heightRatio.GetRatio() : 1);
                fElement.Width *= (widthRatio.GetRatio() != Double.PositiveInfinity ? widthRatio.GetRatio() : 1);
            }
        }

        private void EditBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                int tabLength = Convert.ToInt32(Preferences.GetPreference(PreferenceType.FormatTabLength));
                int caretPosition = EditBox.CaretIndex;
                int column = EditingColumn;
                int spacesToNextTabPosition = (column % tabLength == 0 ? tabLength : tabLength - (column % tabLength));
                EditBox.Text = EditBox.Text.Insert(caretPosition, new string(' ', spacesToNextTabPosition));
                EditBox.CaretIndex = caretPosition + spacesToNextTabPosition;
                OpenFile.SetSaved(false);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                if (Preferences.GetPreference(PreferenceType.FormatPreserveIndents) == "True")
                {
                    int caretPosition = EditBox.CaretIndex;
                    int lineIndex = EditBox.GetLineIndexFromCharacterIndex(caretPosition);
                    string lineContent = EditBox.GetLineText(lineIndex);
                    int whiteSpace = GetLeadingWhiteSpaceLength(lineContent);
                    EditBox.Text = EditBox.Text.Insert(caretPosition, "\r\n" + new string(' ', whiteSpace));
                    EditBox.CaretIndex = caretPosition + whiteSpace + 2;
                    OpenFile.SetSaved(false);
                    e.Handled = true;
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                bool handled = OpenFile.HandleControlChar(e.Key);
                e.Handled = handled;
            }
            else
            {
                e.Handled = false;
                OpenFile.SetSaved(false);
            }

            base.OnPreviewKeyDown(e);
        }

        public static int GetLeadingWhiteSpaceLength(string ofString)
        {
            if (ofString == null)
            {
                throw new ArgumentNullException("ofString");
            }
            char[] chars = ofString.ToCharArray();
            int whiteSpaceChars = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if (Char.IsWhiteSpace(chars[i]))
                {
                    whiteSpaceChars++;
                }
                else
                {
                    break;
                }
            }
            return whiteSpaceChars;
        }

        public void SetText(string newText)
        {
            EditBox.Text = newText;
        }

        private void FileMenuNew_Click(object sender, RoutedEventArgs e)
        {
            FileMenuControls.New();
        }

        private void FileMenuOpen_Click(object sender, RoutedEventArgs e)
        {
            FileMenuControls.Open();
        }

        private void FileMenuSave_Click(object sender, RoutedEventArgs e)
        {
            SetStatusBar("Saving", Colors.DarkOrange);
            FileMenuControls.Save();
        }

        private void FileMenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SetStatusBar("Saving", Colors.DarkOrange);
            FileMenuControls.SaveAs();
        }

        private void FileMenuExit_Click(object sender, RoutedEventArgs e)
        {
            FileMenuControls.Exit();
        }

        public void SetStatusBar(string status, Color color)
        {
            Status.Background = new SolidColorBrush(color);
            ReadyState.Content = status;
        }

        public bool ConfirmWithoutSaving(string action)
        {
            string title = "Confirm Action";
            MessageBoxResult result = MessageBox.Show("You have not saved your work! Are you sure you wish to " + action + " without saving?", title, MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
            {
                return false;
            }
            else if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            else if (result == MessageBoxResult.No)
            {
                OpenFile.Save();
                MessageBox.Show("Saved.");
                return true;
            }
            else
            {
                MessageBox.Show("Erm... OK then. I don't know what you pressed, so I'm assuming it was Cancel.");
                return false;
            }
        }

        private void EditBox_Up(object sender, KeyEventArgs e)
        {
            int caretPosition = EditBox.CaretIndex;
            int lineIndex = EditBox.GetLineIndexFromCharacterIndex(caretPosition);
            int column = caretPosition - EditBox.GetCharacterIndexFromLineIndex(lineIndex);
            EditingColumn = column;
            CursorLine.Content = "Ln " + (lineIndex + 1).ToString(CultureInfo.CurrentCulture);
            CursorCol.Content = "Col " + column;
        }

        private void EditBox_MouseUp(object sender, MouseEventArgs e)
        {
            int caretPosition = EditBox.CaretIndex;
            int lineIndex = EditBox.GetLineIndexFromCharacterIndex(caretPosition);
            int column = caretPosition - EditBox.GetCharacterIndexFromLineIndex(lineIndex);
            EditingColumn = column;
            CursorLine.Content = "Ln " + (lineIndex + 1).ToString(CultureInfo.CurrentCulture);
            CursorCol.Content = "Col " + column;
        }

        public void TemporaryStatusMessage(string message, Color color, int duration)
        {
            SetStatusBar(message, color);
            new Thread(new ParameterizedThreadStart(ResetStatusBar)).Start((object)duration);
        }

        private void ResetStatusBar(object duration)
        {
            Thread.Sleep((int)duration);
            Dispatcher.Invoke(new Action(delegate() {
                SetStatusBar("Ready", Colors.DodgerBlue);
            }));
        }

        private void EditMenuFind_Click(object sender, RoutedEventArgs e)
        {
            EditMenuControls.Find();
        }

        private void FormatMenuWrap_Click(object sender, RoutedEventArgs e)
        {
            if (FormatMenuWrap.IsChecked)
            {
                EditBox.TextWrapping = TextWrapping.Wrap;
                Preferences.SetPreference(PreferenceType.FormatTextWrap, "Wrap");
            }
            else
            {
                EditBox.TextWrapping = TextWrapping.NoWrap;
                Preferences.SetPreference(PreferenceType.FormatTextWrap, "NoWrap");
            }
        }

        private void FormatMenuPrefs_Click(object sender, RoutedEventArgs e)
        {
            Preferences.ManagePreferencesWindow();
        }

        private void MarkdownMode_Click(object sender, RoutedEventArgs e)
        {
            if (MDToolMenu.Height == 0)
            {
                MDToolMenu.Height = 26.00;
                EditBox.Height = EditBox.Height - 26.00;
            }
            else
            {
                MDToolMenu.Height = 0.00;
                EditBox.Height = EditBox.Height + 26.00;
            }
        }

        private void AdditionItem_Click(object sender, RoutedEventArgs e)
        {
            string addition = _additionStrings[((FrameworkElement)sender).Name];
            int caretIndex = EditBox.CaretIndex;
            if (EditBox.SelectedText.Length > 1)
            {
                int selectionStart = EditBox.SelectionStart;
                int selectionLength = EditBox.SelectionLength;
                addition = addition.Replace("text", EditBox.SelectedText);
                string text = EditBox.Text.Remove(selectionStart, selectionLength);
                EditBox.Text = text.Insert(selectionStart, addition);
                EditBox.CaretIndex = caretIndex + addition.Length;
            }
            else
            {
                EditBox.Text = EditBox.Text.Insert(EditBox.CaretIndex, addition);
                EditBox.CaretIndex = caretIndex + addition.Length;
            }
        }

        private void ItemImage_Click(object sender, RoutedEventArgs e)
        {
            MDInsertDialog dialog = new MDInsertDialog("Alt text:", "Image URL:");
            int caretIndex = EditBox.CaretIndex;
            if (EditBox.SelectedText.Length > 1)
            {
                int selectionStart = EditBox.SelectionStart;
                int selectionLength = EditBox.SelectionLength;
                dialog.FirstInput.Text = EditBox.SelectedText;
                EditBox.Text = EditBox.Text.Remove(selectionStart, selectionLength);
            }
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string addition = _additionStrings[((FrameworkElement)sender).Name];
                addition = addition.Replace("{alt}", dialog.FirstInputText);
                addition = addition.Replace("{url}", dialog.SecondInputText);
                EditBox.Text = EditBox.Text.Insert(EditBox.CaretIndex, addition);
                EditBox.CaretIndex = caretIndex + addition.Length;
            }
        }

        private void ItemLink_Click(object sender, RoutedEventArgs e)
        {
            MDInsertDialog dialog = new MDInsertDialog("Link text:", "URL:");
            int caretIndex = EditBox.CaretIndex;
            if (EditBox.SelectedText.Length > 1)
            {
                int selectionStart = EditBox.SelectionStart;
                int selectionLength = EditBox.SelectionLength;
                dialog.FirstInput.Text = EditBox.SelectedText;
                EditBox.Text = EditBox.Text.Remove(selectionStart, selectionLength);
            }
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string addition = _additionStrings[((FrameworkElement)sender).Name];
                addition = addition.Replace("{text}", dialog.FirstInputText);
                addition = addition.Replace("{url}", dialog.SecondInputText);
                EditBox.Text = EditBox.Text.Insert(EditBox.CaretIndex, addition);
                EditBox.CaretIndex = caretIndex + addition.Length;
            }
        }
    }
}
