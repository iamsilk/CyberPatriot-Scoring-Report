using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public struct POLICY_AUDIT_EVENTS_INFO
    {
        public bool AuditingMode;
        
        public IntPtr EventAuditingOptions;
        
        public uint MaximumAuditEventCount;
    }
}
