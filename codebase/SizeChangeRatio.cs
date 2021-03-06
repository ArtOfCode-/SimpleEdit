﻿/* SimpleEdit - Windows Notepad replacement text editor
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

namespace SimpleEdit.Tools
{
    public class SizeChangeRatio
    {
        private double _ratio;

        public SizeChangeRatio(double originalSize, double newSize)
        {
            _ratio = newSize / originalSize;
        }

        public void SetRatio(double newRatio)
        {
            _ratio = newRatio;
        }

        public void SetRatioByCalculation(double originalSize, double newSize)
        {
            _ratio = newSize / originalSize;
        }

        public double GetRatio()
        {
            return _ratio;
        }
    }
}
