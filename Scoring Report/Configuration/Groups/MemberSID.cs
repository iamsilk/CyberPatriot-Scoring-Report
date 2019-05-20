using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Configuration.Groups
{
    public class MemberSID : IMember
    {
        public string IDType => "SID";

        public string Identifier { get; set; } = "";
    }
}
