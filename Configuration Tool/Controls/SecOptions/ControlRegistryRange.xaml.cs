using System.IO;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.SecOptions
{
    /// <summary>
    /// Interaction logic for ControlRegistryRange.xaml
    /// </summary>
    public partial class ControlRegistryRange : UserControl, ISecurityOption
    {
        public ESecurityOptionType Type => ESecurityOptionType.RegistryRange;

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

        public int Maximum
        {
            get
            {
                return numericMax.Number;
            }
            set
            {
                numericMin.Maximum = value;
                numericMax.Number = value;
            }
        }

        public int Minimum
        {
            get
            {
                return numericMin.Number;
            }
            set
            {
                numericMax.Minimum = value;
                numericMin.Number = value;
            }
        }

        public int TotalMaximum
        {
            get
            {
                return numericMax.Maximum;
            }
            set
            {
                numericMax.Maximum = value;
            }
        }

        public int TotalMinimum
        {
            get
            {
                return numericMin.Minimum;
            }
            set
            {
                numericMin.Minimum = value;
            }
        }

        public ControlRegistryRange()
        {
            InitializeComponent();

            numericMax.Minimum = Minimum;
            numericMin.Maximum = Maximum;

            numericMin.PropertyChanged += NumericMin_PropertyChanged;
            numericMax.PropertyChanged += NumericMax_PropertyChanged;
        }

        public string Key { get; set; }

        public string ValueName { get; set; }

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

        public void Write(BinaryWriter writer)
        {
            // Write all classifying information
            writer.Write(Header);
            writer.Write(IsScored);
            writer.Write(Minimum);
            writer.Write(Maximum);
            writer.Write(Key);
            writer.Write(ValueName);
        }

        public void CopyTo(ISecurityOption securityOption)
        {
            // Cast interface to self's type. This function should not be called to other type
            ControlRegistryRange control = securityOption as ControlRegistryRange;

            if (control == null) return;

            // We don't want to copy the header in case we update a header
            //control.Header = Header;
            control.IsScored = IsScored;
            control.Minimum = Minimum;
            control.Maximum = Maximum;
            // We don't want to copy total min/max for same reason as header
            //control.TotalMinimum = TotalMinimum;
            //control.TotalMaximum = TotalMaximum;
            control.Key = Key;
            control.ValueName = ValueName;
        }

        public string Identifier()
        {
            return Key + " - " + ValueName;
        }

        private void NumericMin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Number")
            {
                numericMax.Minimum = Minimum;
            }
        }
        private void NumericMax_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Number")
            {
                numericMin.Maximum = Maximum;
            }
        }
    }
}
