using System.IO;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.SecOptions
{
    /// <summary>
    /// Interaction logic for ControlSeceditTextRegex.xaml
    /// </summary>
    public partial class ControlSeceditTextRegex : UserControl, ISecurityOption
    {
        public ESecurityOptionType Type => ESecurityOptionType.SeceditTextRegex;

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

        public string Section { get; set; }

        public string Key { get; set; }

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

        public void Write(BinaryWriter writer)
        {
            // Write all classifying information
            writer.Write(Header);
            writer.Write(IsScored);
            writer.Write(Regex);
            writer.Write(RegexMatches);
            writer.Write(Section);
            writer.Write(Key);
        }

        public void CopyTo(ISecurityOption securityOption)
        {
            // Cast interface to self's type. This function should not be called to other type
            ControlSeceditTextRegex control = securityOption as ControlSeceditTextRegex;

            if (control == null) return;

            // We don't want to copy the header in case we update a header
            //control.Header = Header;
            control.IsScored = IsScored;
            control.Regex = Regex;
            control.RegexMatches = RegexMatches;
            control.Section = Section;
            control.Key = Key;
        }

        public string Identifier()
        {
            return Section + " - " + Key;
        }

        public ControlSeceditTextRegex()
        {
            InitializeComponent();
        }
    }
}
