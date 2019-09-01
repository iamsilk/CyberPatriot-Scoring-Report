using System.IO;

namespace Scoring_Report.Configuration.Comparisons
{
    public class ComparisonRange : IComparison
    {
        public EComparison Type => EComparison.Range;

        public Range Range = null;

        public bool Equals(string value)
        {
            int intValue;

            // If value is not integer, return false
            if (!int.TryParse(value, out intValue)) return false;

            return Range.WithinBounds(intValue);
        }

        public void Load(BinaryReader reader)
        {
            Range = Range.Parse(reader);
        }
    }
}
