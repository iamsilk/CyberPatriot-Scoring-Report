using System;
using System.IO;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.SecOptions
{
    /// <summary>
    /// Interaction logic for ControlRegistryMultiLine.xaml
    /// </summary>
    public partial class ControlRegistryMultiLine : UserControl, ISecurityOption
    {
        public ESecurityOptionType Type => ESecurityOptionType.RegistryMultiLine;

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

        public string Text
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

        public string[] Lines
        {
            get
            {
                return Text.Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string Key { get; set; }

        public string ValueName { get; set; }

        public void Parse(BinaryReader reader)
        {
            Header = reader.ReadString();

            // Get scoring status
            IsScored = reader.ReadBoolean();

            // Get all lines
            int count = reader.ReadInt32();
            string[] lines = new string[count];
            for (int i = 0; i < count; i++)
            {
                lines[i] = reader.ReadString();
            }

            // Set text to joined lines
            Text = string.Join(Environment.NewLine, lines);

            // Get key and value name
            Key = reader.ReadString();
            ValueName = reader.ReadString();
        }

        public void Write(BinaryWriter writer)
        {
            // Write all classifying information
            writer.Write(Header);
            writer.Write(IsScored);

            string[] lines = Lines;
            writer.Write(lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                writer.Write(lines[i]);
            }

            writer.Write(Key);
            writer.Write(ValueName);
        }

        public void CopyTo(ISecurityOption securityOption)
        {
            // Cast interface to self's type. This function should not be called to other type
            ControlRegistryMultiLine control = securityOption as ControlRegistryMultiLine;

            if (control == null) return;

            // We don't want to copy the header in case we update a header
            //control.Header = Header;
            control.IsScored = IsScored;
            control.Text = Text;
            control.Key = Key;
            control.ValueName = ValueName;
        }

        public string Identifier()
        {
            return Key + " - " + ValueName;
        }

        public ControlRegistryMultiLine()
        {
            InitializeComponent();
        }
    }
}
