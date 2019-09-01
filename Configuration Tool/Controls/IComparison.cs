using System.IO;

namespace Configuration_Tool.Controls
{
    public interface IComparison
    {
        EComparison Type { get; }

        void Load(BinaryReader reader);
        void Write(BinaryWriter writer);
    }
}
