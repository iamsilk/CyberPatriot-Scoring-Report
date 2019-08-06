using System.Collections.Generic;

namespace Scoring_Report.Configuration.UserRights
{
    public class UserRightsDefinition
    {
        public string ConstantName = "";

        public string Setting = "";

        public List<UserRightsIdentifier> Identifiers = new List<UserRightsIdentifier>();

        public UserRightsDefinition(string constantName, string setting)
        {
            ConstantName = constantName;
            Setting = setting;
        }
    }
}
