using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public class LocalPolicies
    {
        public AuditPolicy AuditPolicy = new AuditPolicy();

        public UserRightsAssignment UserRightsAssignment = new UserRightsAssignment();
    }
}
