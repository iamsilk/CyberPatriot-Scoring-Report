using System.IO;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.Firewall
{
    /// <summary>
    /// Interaction logic for ControlProfile.xaml
    /// </summary>
    public partial class ControlProfile : UserControl
    {
        public string Header { get; set; }

        public ControlProfile()
        {
            InitializeComponent();

            DataContext = this;
        }

        public void Load(BinaryReader reader)
        {
            // Read number of property title/property pairs
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Read property title and value
                string title = reader.ReadString();
                bool isScored = reader.ReadBoolean();
                string property = reader.ReadString();

                ControlSettingComboBox propertyControl = null;

                // Find matching combo box
                foreach (ControlSettingComboBox control in itemsProperties.Items)
                {
                    // If header matches loaded config header
                    if (control.Header == title)
                    {
                        // Found match, break loop
                        propertyControl = control;
                        break;
                    }
                }

                // If match was found
                if (propertyControl != null)
                {
                    // Set scoring status and selected property
                    propertyControl.IsScored = isScored;
                    propertyControl.SelectedItem = property;
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            // Write number of properties
            writer.Write(itemsProperties.Items.Count);

            // For each property
            foreach (ControlSettingComboBox control in itemsProperties.Items)
            {
                // Write property title, scoring status, and value
                writer.Write(control.Header);
                writer.Write(control.IsScored);
                writer.Write(control.SelectedItem);
            }
        }
    }
}
