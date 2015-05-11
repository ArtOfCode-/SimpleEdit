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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;
using SimpleEdit.Tools;

namespace SimpleEdit
{
    /// <summary>
    /// Interaction logic for FindReplaceDialog.xaml
    /// </summary>
    public partial class FindReplaceDialog : Window
    {
        private MainWindow _window;

        private string _findText;
        private int _findAlreadyFound = -1;

        public FindReplaceDialog(MainWindow window)
        {
            InitializeComponent();
            _window = window;
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string text = _window.EditBox.Text;
            string toFind = FindTextToFind.Text;
            List<int> indexes = text.AllIndexesOf(toFind).ToList();
            int length = toFind.Length;
            if (indexes.Count() == 0)
            {
                MessageBox.Show("Could not find the text '" + toFind + "'.");
                return;
            }
            if (text == _findText)
            {
                if ((_findAlreadyFound + 1) < indexes.Count())
                {
                    _window.EditBox.SelectionStart = indexes[_findAlreadyFound + 1];
                    _window.EditBox.SelectionLength = length;
                    _findAlreadyFound += 1;
                    _window.Focus();
                }
                else
                {
                    MessageBox.Show("The end of the document has been reached.");
                    _findAlreadyFound = -1;
                }
            }
            else
            {
                _findText = text;
                _findAlreadyFound = 0;
                _window.EditBox.SelectionStart = indexes[0];
                _window.EditBox.SelectionLength = length;
                _window.Focus();
            }
        }

        private void ReplaceSubmit_Click(object sender, RoutedEventArgs e)
        {
            string text = _window.EditBox.Text;
            string toReplace = ReplaceTextToFind.Text;
            string replaceWith = ReplaceReplaceWith.Text;
            int index = text.IndexOf(toReplace, StringComparison.Ordinal);
            string newText = text.ReplaceFirst(toReplace, replaceWith);
            _window.EditBox.Text = newText;
            _window.EditBox.SelectionStart = index;
            _window.EditBox.SelectionLength = replaceWith.Length;
            _window.Focus();
        }

        private void ReplaceAllSubmit_Click(object sender, RoutedEventArgs e)
        {
            string text = _window.EditBox.Text;
            string toReplace = ReplaceTextToFind.Text;
            string replaceWith = ReplaceReplaceWith.Text;
            int count = text.CountInstancesOfSubstring(toReplace);
            string newText = text.Replace(toReplace, replaceWith);
            _window.EditBox.Text = newText;
            _window.Focus();
            MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "Replaced {0} instances of '{1}'.", count, toReplace));
        }
    }
}
