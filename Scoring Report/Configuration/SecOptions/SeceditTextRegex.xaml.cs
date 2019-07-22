using System.IO;

namespace Scoring_Report.Configuration.SecOptions
{
    public class SeceditTextRegex : ISecurityOption
    {
        public ESecurityOptionType Type => ESecurityOptionType.SeceditTextRegex;

        public string Header = "";

        public bool IsScored { get; set; } = false;

        public string Section = "";

        public string Key = "";

        public string Regex = "";

        public bool RegexMatches = false;

        public void Parse(BinaryReader reader)
        {
            Header = reader.ReadString();

            // Get scoring status
            IsScored = reader.ReadBoolean();

            // Get regex pattern and matching status
            Regex = reader.ReadString();
            RegexMatches = reader.ReadBoolean();

            // Get section and key
            Section = reader.ReadString();
            Key = reader.ReadString();
        }

        public SeceditTextRegex()
        {

        }
    }
}
