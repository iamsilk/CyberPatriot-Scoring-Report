using System.IO;
using System.Text.RegularExpressions;

namespace Scoring_Report.Configuration.Comparisons
{
    public class ComparisonRegex : IComparison
    {
        public EComparison Type => EComparison.Regex;

        public Regex Regex = null; 

        public bool Equals(string value)
        {
            return Regex.IsMatch(value);
        }

        public void Load(BinaryReader reader)
        {
            string pattern = reader.ReadString();
            RegexOptions options = (RegexOptions)reader.ReadInt32();

            Regex = new Regex(pattern, options);
        }
    }
}
