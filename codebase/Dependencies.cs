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
using System.Windows.Media;
using System.Reflection;

namespace SimpleEdit.Tools
{
    public static class Dependencies
    {
        public static DependencyProperty GetDependencyPropertyByName(DependencyObject dependencyObject, string dpName)
        {
            if (dependencyObject == null || dpName == null)
            {
                throw new ArgumentNullException(dpName == null ? "dpName" : "dependencyObject");
            }
            return GetDependencyPropertyByName(dependencyObject.GetType(), dpName);
        }

        private static DependencyProperty GetDependencyPropertyByName(Type dependencyObjectType, string dpName)
        {
            DependencyProperty dp = null;

            var fieldInfo = dependencyObjectType.GetField(dpName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (fieldInfo != null)
            {
                dp = fieldInfo.GetValue(null) as DependencyProperty;
            }
            else
            {
                throw new NullReferenceException("GetField returned null where a FieldInfo object was required.");
            }

            return dp;
        }

        public static DependencyObject SetDependencyProperty(DependencyObject dependencyObject, string propertyName, object value)
        {
            if (dependencyObject == null || propertyName == null || value == null)
            {
                throw new ArgumentNullException((dependencyObject == null ? "dependencyObject" : (propertyName == null ? "propertyName" : "value")));
            }
            try
            {
                DependencyProperty property = GetDependencyPropertyByName(dependencyObject, propertyName);
                dependencyObject.SetValue(property, value);
            }
            catch (NullReferenceException e)
            {
                throw new MissingFieldException(string.Format("The field {0} could not be found as a child of {1}", propertyName, dependencyObject), e);
            }
            return dependencyObject;
        }
    }
}
