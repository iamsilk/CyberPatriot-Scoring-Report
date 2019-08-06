using Scoring_Report.Policies.Audit;
using Scoring_Report.Policies.UserRights;

namespace Scoring_Report.Policies
{
    public class LocalPolicies
    {
        public AuditPolicy AuditPolicy = new AuditPolicy();

        public UserRightsAssignment UserRightsAssignment = new UserRightsAssignment();
    }
}
