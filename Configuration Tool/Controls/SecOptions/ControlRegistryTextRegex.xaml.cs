using Configuration_Tool.Configuration;
using System.IO;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.SecOptions
{
    /// <summary>
    /// Interaction logic for ControlRegistryTextRegex.xaml
    /// </summary>
    public partial class ControlRegistryTextRegex : UserControl, ISecurityOption
    {
        public ESecurityOptionType Type => ESecurityOptionType.RegistryTextRegex;

        public string Header
        {
            get
            {
                return txtHeader.Text;
            }
            set
            {
                txtHeader.Text = value;
            }
        }

        public bool IsScored
        {
            get
            {
                if (!checkBoxScored.IsChecked.HasValue) return false;

                return checkBoxScored.IsChecked.Value;
            }
            set
            {
                checkBoxScored.IsChecked = value;
            }
        }

        public string Regex
        {
            get
            {
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
            }
        }

        public bool RegexMatches
        {
            get
            {
                if (!checkBoxRegexMatches.IsChecked.HasValue) return false;

                return checkBoxRegexMatches.IsChecked.Value;
            }
            set
            {
                checkBoxRegexMatches.IsChecked = value;
            }
        }

        public string Key { get; set; }

        public string ValueName { get; set; }

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

        public void Write(BinaryWriter writer)
        {
            // Write all classifying information
            writer.Write(Header);
            writer.Write(IsScored);
            writer.Write(Regex);
            writer.Write(RegexMatches);
            writer.Write(Key);
            writer.Write(ValueName);
        }

        public void CopyTo(ISecurityOption securityOption)
        {
            // Cast interface to self's type. This function should not be called to other type
            ControlRegistryTextRegex control = securityOption as ControlRegistryTextRegex;

            if (control == null) return;

            // We don't want to copy the header in case we update a header
            //control.Header = Header;
            control.IsScored = IsScored;
            control.Regex = Regex;
            control.RegexMatches = RegexMatches;
            control.Key = Key;
            control.ValueName = ValueName;
        }

        public string Identifier()
        {
            return Key + " - " + ValueName;
        }

        public ControlRegistryTextRegex()
        {
            InitializeComponent();
        }
    }
}
