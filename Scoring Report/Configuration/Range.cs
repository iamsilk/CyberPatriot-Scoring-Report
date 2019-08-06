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
    }
}
