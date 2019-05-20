using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration.Groups
{
    public interface IMember
    {
        string IDType { get; }
        string Identifier { get; set; }
    }
}
