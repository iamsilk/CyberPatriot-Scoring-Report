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
        }

        public void Save(BinaryWriter writer)
        {
            // Save remote desktop info
            MainWindow.rdpRegistry.Write(writer);
        }
    }
}
