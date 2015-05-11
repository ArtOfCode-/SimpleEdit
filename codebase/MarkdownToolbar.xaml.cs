using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleEdit
{
    /// <summary>
    /// Interaction logic for MarkdownToolbar.xaml
    /// </summary>
    public partial class MarkdownToolbar : Window
    {
        private MainWindow _window;

        private static bool _hasInstance = false;
        public static bool HasInstance
        {
            get
            {
                return _hasInstance;
            }
            private set
            {
                _hasInstance = value;
            }
        }

        private static MarkdownToolbar _instance;
        public static MarkdownToolbar Instance
        {
            get
            {
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        private Dictionary<string, string> _additionStrings = new Dictionary<string, string>
        {
            { "ItemBold", "**text**" },
            { "ItemItalic", "*text*" },
            { "ItemBulletList", "- text" },
            { "ItemLink", "[{text}]({url})" },
            { "ItemImage", "![{alt}]({url})" },
            { "ItemCode", "    " }
        };

        public MarkdownToolbar(MainWindow window)
        {
            _window = window;
            InitializeComponent();
            Instance = this;
            HasInstance = true;
            this.Closing += MarkdownToolbar_Closing;
        }

        private void MarkdownToolbar_Closing(object sender, CancelEventArgs e)
        {
            HasInstance = false;
            Instance = null;
        }

        private void AdditionItem_Click(object sender, RoutedEventArgs e)
        {
            string addition = _additionStrings[((FrameworkElement)sender).Name];
            if (_window.EditBox.SelectedText.Length > 1)
            {
                int selectionStart = _window.EditBox.SelectionStart;
                int selectionLength = _window.EditBox.SelectionLength;
                addition = addition.Replace("text", _window.EditBox.SelectedText);
                string text = _window.EditBox.Text.Remove(selectionStart, selectionLength);
                _window.EditBox.Text = text.Insert(selectionStart, addition);
            }
            else
            {
                _window.EditBox.Text = _window.EditBox.Text.Insert(_window.EditBox.CaretIndex, addition);
            }
            _window.Focus();
        }

        private void ItemImage_Click(object sender, RoutedEventArgs e)
        {
            MDInsertDialog dialog = new MDInsertDialog("Alt text:", "Image URL:");
            if (_window.EditBox.SelectedText.Length > 1)
            {
                int selectionStart = _window.EditBox.SelectionStart;
                int selectionLength = _window.EditBox.SelectionLength;
                dialog.FirstInput.Text = _window.EditBox.SelectedText;
                _window.EditBox.Text = _window.EditBox.Text.Remove(selectionStart, selectionLength);
            }
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string addition = _additionStrings[((FrameworkElement)sender).Name];
                addition = addition.Replace("{alt}", dialog.FirstInputText);
                addition = addition.Replace("{url}", dialog.SecondInputText);
                _window.EditBox.Text = _window.EditBox.Text.Insert(_window.EditBox.CaretIndex, addition);
            }
        }

        private void ItemLink_Click(object sender, RoutedEventArgs e)
        {
            MDInsertDialog dialog = new MDInsertDialog("Link text:", "URL:");
            if (_window.EditBox.SelectedText.Length > 1)
            {
                int selectionStart = _window.EditBox.SelectionStart;
                int selectionLength = _window.EditBox.SelectionLength;
                dialog.FirstInput.Text = _window.EditBox.SelectedText;
                _window.EditBox.Text = _window.EditBox.Text.Remove(selectionStart, selectionLength);
            }
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string addition = _additionStrings[((FrameworkElement)sender).Name];
                addition = addition.Replace("{text}", dialog.FirstInputText);
                addition = addition.Replace("{url}", dialog.SecondInputText);
                _window.EditBox.Text = _window.EditBox.Text.Insert(_window.EditBox.CaretIndex, addition);
            }
        }
    }
}
