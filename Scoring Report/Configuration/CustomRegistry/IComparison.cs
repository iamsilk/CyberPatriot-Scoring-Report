using System.IO;

namespace Scoring_Report.Configuration.CustomRegistry
{
    public interface IComparison
    {
        EComparison Type { get; }

        bool Equals(string value);

        void Load(BinaryReader reader);
    }
}
