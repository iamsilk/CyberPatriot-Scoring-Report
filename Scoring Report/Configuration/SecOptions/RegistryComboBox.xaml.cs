using System.IO;

namespace Scoring_Report.Configuration.SecOptions
{
    public class RegistryComboBox : ISecurityOption
    {
        public ESecurityOptionType Type => ESecurityOptionType.RegistryComboBox;

        public string Header = "";

        public bool IsScored { get; set; } = false;

        public string[] Items = null;

        public int SelectedIndex = -1;

        public string SelectedItem => Items[SelectedIndex];

        public string Key = "";

        public string ValueName = "";

        public void Parse(BinaryReader reader)
        {
            Header = reader.ReadString();

            // Get scoring status
            IsScored = reader.ReadBoolean();
            
            // Get items
            int count = reader.ReadInt32();
            Items = new string[count];

            for (int i = 0; i < count; i++)
            {
                Items[i] = reader.ReadString();
            }

            // Get selected index
            SelectedIndex = reader.ReadInt32();

            // Get key and value name
            Key = reader.ReadString();
            ValueName = reader.ReadString();
        }

        public RegistryComboBox()
        {

        }
    }
}
