using System.Collections.Generic;
using System.Security.Principal;

namespace Scoring_Report.Policies.UserRights
{
    public class UserRightsAssignment
    {
        public Dictionary<string, List<SecurityIdentifier>> UserRightsSetting = new Dictionary<string, List<SecurityIdentifier>>();
    }
}
