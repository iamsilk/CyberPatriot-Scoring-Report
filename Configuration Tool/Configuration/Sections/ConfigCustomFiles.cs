using Configuration_Tool.Controls;
using Configuration_Tool.Controls.CustomFiles;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigCustomFiles : IConfig
    {
        public EConfigType Type => EConfigType.CustomFiles;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Clear custom files
            MainWindow.itemsCustomFiles.Items.Clear();

            // Get number of custom files
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get control with properties
                ControlCustomFile control = ControlCustomFile.Parse(reader);

                // Add control to ItemsControl
                MainWindow.itemsCustomFiles.Items.Add(control);
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Save number of custom files
            writer.Write(MainWindow.itemsCustomFiles.Items.Count);

            // Save each custom file
            foreach (ControlCustomFile control in MainWindow.itemsCustomFiles.Items)
            {
                control.Write(writer);
            }
        }
    }
}
