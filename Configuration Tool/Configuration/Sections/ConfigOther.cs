using Configuration_Tool.Controls;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigOther : IConfig
    {
        public EConfigType Type => EConfigType.Other;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Load remote desktop info
            MainWindow.rdpRegistry.Parse(reader);

            // Load host file scoring status
            MainWindow.checkBoxHostFileScored.IsChecked = reader.ReadBoolean();
        }

        public void Save(BinaryWriter writer)
        {
            // Save remote desktop info
            MainWindow.rdpRegistry.Write(writer);

            // Save host file scoring status
            bool hostFileScored = false;

            if (MainWindow.checkBoxHostFileScored.IsChecked.HasValue)
                hostFileScored = MainWindow.checkBoxHostFileScored.IsChecked.Value;

            writer.Write(hostFileScored);
        }
    }
}
