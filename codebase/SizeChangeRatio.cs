using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEdit.Tools
{
    class SizeChangeRatio
    {
        private double _ratio;

        public SizeChangeRatio(int originalSize, int newSize)
        {
            _ratio = newSize / originalSize;
        }

        public SizeChangeRatio(double originalSize, double newSize)
        {
            _ratio = newSize / originalSize;
        }

        public double GetRatio()
        {
            return _ratio;
        }
    }
}
