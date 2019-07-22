using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public class AuditPolicy
    {
        public Dictionary<string, POLICY_AUDIT_EVENT> HeaderSettingPairs = new Dictionary<string, POLICY_AUDIT_EVENT>()
        {
            { "Audit account logon events", POLICY_AUDIT_EVENT.UNCHANGED },
            { "Audit account management", POLICY_AUDIT_EVENT.UNCHANGED },
            { "Audit directory service access", POLICY_AUDIT_EVENT.UNCHANGED },
            { "Audit logon events", POLICY_AUDIT_EVENT.UNCHANGED },
            { "Audit object access", POLICY_AUDIT_EVENT.UNCHANGED },
            { "Audit policy change", POLICY_AUDIT_EVENT.UNCHANGED },
            { "Audit privilege use", POLICY_AUDIT_EVENT.UNCHANGED },
            { "Audit process tracking", POLICY_AUDIT_EVENT.UNCHANGED },
            { "Audit system events", POLICY_AUDIT_EVENT.UNCHANGED }
        };
    }
}
