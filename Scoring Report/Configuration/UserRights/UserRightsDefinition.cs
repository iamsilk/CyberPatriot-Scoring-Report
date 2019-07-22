using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
