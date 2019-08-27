using System.IO;

namespace Scoring_Report.Configuration
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

        public static Range Parse(BinaryReader reader)
        {
            int min = reader.ReadInt32();
            int max = reader.ReadInt32();

            return new Range(min, max);
        }
    }
}
