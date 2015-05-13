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

namespace SimpleEdit
{
    /// <summary>
    /// Interaction logic for MDInsertDialog.xaml
    /// </summary>
    public partial class MDInsertDialog : Window
    {
        public string FirstInputText;
        public string SecondInputText;

        public MDInsertDialog(string labelText, string secondLabelText)
        {
            InitializeComponent();
            FirstInput_Label.Content = labelText;
            SecondInput_Label.Content = secondLabelText;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            FirstInputText = FirstInput.Text;
            SecondInputText = SecondInput.Text;
            DialogResult = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
