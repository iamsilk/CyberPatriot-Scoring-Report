using Configuration_Tool.Controls;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigProhibitedFiles : IConfig
    {
        public EConfigType Type => EConfigType.ProhibitedFiles;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            MainWindow.itemsProhibitedFiles.Items.Clear();

            // Get count of prohibited files
            int filecount = reader.ReadInt32();

            for (int i = 0; i < filecount; i++)
            {
                // Get File Location
                string fileLocation = reader.ReadString();

                // Create control
                ControlProhibitedFile control = new ControlProhibitedFile();
                control.Path = fileLocation;

                // Add control to items control
                MainWindow.itemsProhibitedFiles.Items.Add(control);
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Get/write number of prohibited files
            int count = MainWindow.itemsProhibitedFiles.Items.Count;
            writer.Write(count);

            // Loop over each prohibited file
            foreach (ControlProhibitedFile control in MainWindow.itemsProhibitedFiles.Items)
            {
                // Write paths
                writer.Write(control.Path);
            }
        }
    }
}
