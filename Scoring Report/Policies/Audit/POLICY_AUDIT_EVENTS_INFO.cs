using System;

namespace Scoring_Report.Policies.Audit
{
    public struct POLICY_AUDIT_EVENTS_INFO
    {
        public bool AuditingMode;
        
        public IntPtr EventAuditingOptions;
        
        public uint MaximumAuditEventCount;
    }
}
