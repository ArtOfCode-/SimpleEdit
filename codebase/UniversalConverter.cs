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
using System.Windows.Data;
using System.ComponentModel;
using System.Globalization;

namespace SimpleEdit.Tools
{
    public class UniversalValueConverter : IValueConverter
    {
        /// <summary>
        /// Attempts to convert the given object to a new object of specified type.
        /// </summary>
        /// <param name="value">The System.Object to convert</param>
        /// <param name="targetType">The type to convert to</param>
        /// <returns>The converted object</returns>
        public object Convert(object value, Type targetType)
        {
            // obtain the conveter for the target type
            TypeConverter converter = TypeDescriptor.GetConverter(targetType);

            try
            {
                // determine if the supplied value is of a suitable type
                if (converter.CanConvertFrom(value.GetType()))
                {
                    // return the converted value
                    return converter.ConvertFrom(value);
                }
                else
                {
                    // try to convert from the string representation
                    return converter.ConvertFrom(value.ToString());
                }
            }
            catch (Exception)
            {
                return value;
            }
        }

        /// <summary>
        /// Attempts to convert the given object to a new object of specified type.
        /// </summary>
        /// <param name="value">The System.Object to convert</param>
        /// <param name="targetType">The type to convert to</param>
        /// <param name="parameter"></param>
        /// <param name="culture">Culture information to use in conversion</param>
        /// <returns>The converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
