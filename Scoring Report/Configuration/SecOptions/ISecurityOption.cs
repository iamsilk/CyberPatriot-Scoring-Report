using System.IO;

namespace Scoring_Report.Configuration.SecOptions
{
    public interface ISecurityOption
    {
        ESecurityOptionType Type { get; }

        bool IsScored { get; set; }
        void Parse(BinaryReader reader);
    }
}
