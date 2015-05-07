using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleEdit
{
    static class EditMenuControls
    {
        private static MainWindow _window;

        public static FindReplaceDialog _findDialog;

        public static void Initialize(MainWindow window)
        {
            _window = window;
        }

        public static void Find()
        {
            if (_findDialog == null)
            {
                FindReplaceDialog dialog = new FindReplaceDialog(_window);
                dialog.Show();
                dialog.Closed += delegate(object sender, EventArgs e)
                {
                    EditMenuControls._findDialog = null;
                };
                _findDialog = dialog;
            }
            else
            {
                _findDialog.Focus();
            }
        }
    }
}
