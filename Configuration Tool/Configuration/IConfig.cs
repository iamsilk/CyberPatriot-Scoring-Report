using Configuration_Tool.Controls;
using System.IO;

namespace Configuration_Tool.Configuration
{
    public interface IConfig
    {
        EConfigType Type { get; }

        MainWindow MainWindow { get; set; }

        void Load(BinaryReader reader);
        void Save(BinaryWriter writer);
    }
}
