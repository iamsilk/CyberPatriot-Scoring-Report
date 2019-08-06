using System.IO;

namespace Configuration_Tool.Controls.SecOptions
{
    public interface ISecurityOption
    {
        ESecurityOptionType Type { get; }

        void Parse(BinaryReader reader);

        void Write(BinaryWriter writer);

        void CopyTo(ISecurityOption securityOption);

        string Identifier();
    }
}
