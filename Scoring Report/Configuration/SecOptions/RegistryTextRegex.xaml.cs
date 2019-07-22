using System.IO;

namespace Scoring_Report.Configuration.SecOptions
{
    public class RegistryTextRegex : ISecurityOption
    {
        public ESecurityOptionType Type => ESecurityOptionType.RegistryTextRegex;

        public string Header = "";

        public bool IsScored { get; set; } = false;

        public string Regex = "";

        public bool RegexMatches = false;

        public string Key = "";

        public string ValueName = "";

        public void Parse(BinaryReader reader)
        {
            Header = reader.ReadString();

            // Get scoring status
            IsScored = reader.ReadBoolean();

            // Get regex pattern and matching status
            Regex = reader.ReadString();
            RegexMatches = reader.ReadBoolean();

            // Get key and value name
            Key = reader.ReadString();
            ValueName = reader.ReadString();
        }

        public RegistryTextRegex()
        {

        }
    }
}
