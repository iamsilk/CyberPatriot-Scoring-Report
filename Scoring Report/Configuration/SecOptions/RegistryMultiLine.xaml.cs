using System;
using System.IO;

namespace Scoring_Report.Configuration.SecOptions
{
    public class RegistryMultiLine : ISecurityOption
    {
        public ESecurityOptionType Type => ESecurityOptionType.RegistryMultiLine;

        public string Header = "";

        public bool IsScored { get; set; } = false;

        public string[] Lines = null;

        public string Key = "";

        public string ValueName = "";

        public void Parse(BinaryReader reader)
        {
            Header = reader.ReadString();

            // Get scoring status
            IsScored = reader.ReadBoolean();

            // Get all lines
            int count = reader.ReadInt32();
            Lines = new string[count];
            for (int i = 0; i < count; i++)
            {
                Lines[i] = reader.ReadString();
            }

            // Get key and value name
            Key = reader.ReadString();
            ValueName = reader.ReadString();
        }

        public RegistryMultiLine()
        {

        }
    }
}
