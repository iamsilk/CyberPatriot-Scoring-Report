using Configuration_Tool.Controls;
using Configuration_Tool.Controls.CustomRegistry;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigCustomRegistry : IConfig
    {
        public EConfigType Type => EConfigType.CustomRegistry;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Clear custom registry values
            MainWindow.listCustomRegistryValues.Items.Clear();

            // Get number of custom registry values
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get control with properties
                ControlCustomRegistryValue control = ControlCustomRegistryValue.Parse(reader);

                // Add control to ItemsControl
                MainWindow.listCustomRegistryValues.Items.Add(control);
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Save number of custom registry values
            writer.Write(MainWindow.listCustomRegistryValues.Items.Count);

            // Save each custom registry value
            foreach (ControlCustomRegistryValue control in MainWindow.listCustomRegistryValues.Items)
            {
                control.Write(writer);
            }
        }
    }
}
