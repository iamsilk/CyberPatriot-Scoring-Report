using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration
{
    public interface IUserRightsIdentifier
    {
        EUserRightsIdentifierType Type { get; }
        string Identifier { get; set; }
    }
}
