using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
