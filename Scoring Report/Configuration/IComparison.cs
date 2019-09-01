using System.IO;

namespace Scoring_Report.Configuration
{
    public interface IComparison
    {
        EComparison Type { get; }

        bool Equals(string value);

        void Load(BinaryReader reader);
    }
}
