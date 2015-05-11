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
