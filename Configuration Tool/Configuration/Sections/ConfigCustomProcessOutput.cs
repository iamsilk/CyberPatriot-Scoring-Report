using Configuration_Tool.Controls;
using Configuration_Tool.Controls.CustomProcesses;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigCustomProcessOutput : IConfig
    {
        public EConfigType Type => EConfigType.CustomProcessOutput;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Clear custom process outputs
            MainWindow.itemsCustomProcessOutputs.Items.Clear();

            // Get number of custom process outputs
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get control with properties
                ControlCustomProcessOutput control = ControlCustomProcessOutput.Parse(reader);

                // Add control to ItemsControl
                MainWindow.itemsCustomProcessOutputs.Items.Add(control);
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Save number of custom process outputs
            writer.Write(MainWindow.itemsCustomProcessOutputs.Items.Count);

            // Save each custom process output
            foreach (ControlCustomProcessOutput control in MainWindow.itemsCustomProcessOutputs.Items)
            {
                control.Write(writer);
            }
        }
    }
}
