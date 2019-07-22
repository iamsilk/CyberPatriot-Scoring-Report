using System.IO;

namespace Scoring_Report.Configuration.SecOptions
{
    public class RegistryRange : ISecurityOption
    {
        public ESecurityOptionType Type => ESecurityOptionType.RegistryRange;

        public string Header = "";

        public bool IsScored { get; set; } = false;

        public int Maximum = 0;

        public int Minimum = 0;

        public string Key = "";

        public string ValueName = "";

        public void Parse(BinaryReader reader)
        {
            Header = reader.ReadString();

            // Get scoring status
            IsScored = reader.ReadBoolean();

            // Get minimum and maximum
            Minimum = reader.ReadInt32();
            Maximum = reader.ReadInt32();

            // Get key and value name
            Key = reader.ReadString();
            ValueName = reader.ReadString();
        }

        public RegistryRange()
        {

        }
    }
}
