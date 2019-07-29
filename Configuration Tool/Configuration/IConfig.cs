using Configuration_Tool.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
