using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public enum POLICY_AUDIT_EVENT_TYPE
    {
        AuditCategorySystem,
        AuditCategoryLogon,
        AuditCategoryObjectAccess,
        AuditCategoryPrivilegeUse,
        AuditCategoryDetailedTracking,
        AuditCategoryPolicyChange,
        AuditCategoryAccountManagement,
        AuditCategoryDirectoryServiceAccess,
        AuditCategoryAccountLogon
    }
}
