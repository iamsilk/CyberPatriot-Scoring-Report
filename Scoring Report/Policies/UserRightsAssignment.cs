using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public class UserRightsAssignment
    {
        public Dictionary<string, List<SecurityIdentifier>> UserRightsSetting = new Dictionary<string, List<SecurityIdentifier>>();
    }
}
