using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration.Groups
{
    public class MemberUsername : IMember
    {
        public string IDType => "Username";
        
        public string Identifier { get; set; } = "";
    }
}
