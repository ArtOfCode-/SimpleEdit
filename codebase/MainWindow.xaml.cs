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

        private Preferences _preferences;

        // Entry point
        public MainWindow()
        {
            this.Closing += MainWindow_Closing;
            InitializeComponent();
            program = new Program(this);
            _preferences = new Preferences(program, this);
            EditBox.Focus();
            OpenFile.Initialize(this);
            FileMenuControls.Initialize(this);
            EditMenuControls.Initialize(this);
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            FileMenuControls.Exit(e);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeChangeRatio heightRatio = new SizeChangeRatio(e.PreviousSize.Height, e.NewSize.Height);
            SizeChangeRatio widthRatio = new SizeChangeRatio(e.PreviousSize.Width, e.NewSize.Width);
            // Console.WriteLine("Size changed: height * {0}, width * {1}", heightRatio.GetRatio(), widthRatio.GetRatio());
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
                string tab = new string(' ', 4);
                int caretPosition = EditBox.CaretIndex;
                EditBox.Text = EditBox.Text.Insert(caretPosition, tab);
                EditBox.CaretIndex = caretPosition + 4;
                OpenFile.SetSaved(false);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
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
            SetStatusBar("Ready", Colors.DodgerBlue);
        }

        private void FileMenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SetStatusBar("Saving", Colors.DarkOrange);
            FileMenuControls.SaveAs();
            SetStatusBar("Ready", Colors.DodgerBlue);
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
            string title = "Confirm Exit";
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

        private void EditBox_Up(object sender, EventArgs e)
        {
            int caretPosition = EditBox.CaretIndex;
            int lineIndex = EditBox.GetLineIndexFromCharacterIndex(caretPosition);
            int column = caretPosition - EditBox.GetCharacterIndexFromLineIndex(lineIndex);
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
            }
            else
            {
                EditBox.TextWrapping = TextWrapping.NoWrap;
            }
        }

        private void FormatMenuPrefs_Click(object sender, RoutedEventArgs e)
        {
            _preferences.ManagePreferencesWindow();
        }
    }
}
