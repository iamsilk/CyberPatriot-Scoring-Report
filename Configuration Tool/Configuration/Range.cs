using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration
{
    public class Range
    {
        public int Max { get; set; }

        public int Min { get; set; }

        public Range(int min, int max)
        {
            Max = max;
            Min = min;
        }

        public bool WithinBounds(int value)
        {
            return WithinBounds(value, true);
        }

        public bool WithinBounds(int value, bool inclusive)
        {
            if (inclusive)
            {
                return value >= Min && value <= Max;
            }

            return value > Min && value < Max;
        }
    }
}
