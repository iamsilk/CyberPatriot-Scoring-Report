using System.IO;

namespace Scoring_Report.Configuration.Comparisons
{
    public class ComparisonSimple : IComparison
    {
        public EComparison Type => EComparison.Simple;

        public string Value = "";

        public bool Equals(string value)
        {
            return Value == value;
        }

        public void Load(BinaryReader reader)
        {
            Value = reader.ReadString();
        }
    }
}
