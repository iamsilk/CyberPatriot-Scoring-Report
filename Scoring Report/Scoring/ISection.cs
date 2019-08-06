using System.IO;

namespace Scoring_Report.Scoring
{
    public interface ISection
    {
        ESectionType Type { get; }

        string Header { get; }
        int MaxScore();
        SectionDetails GetScore();
        
        void Load(BinaryReader reader);
    }
}
