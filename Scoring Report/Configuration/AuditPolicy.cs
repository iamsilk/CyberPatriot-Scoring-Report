using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Configuration
{
    public class AuditPolicy
    {
        public Dictionary<string, ScoredItem<EAuditSettings>> HeaderSettingPairs = new Dictionary<string, ScoredItem<EAuditSettings>>()
        {
            { "Audit account logon events", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit account management", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit directory service access", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit logon events", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit object access", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit policy change", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit privilege use", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit process tracking", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit system events", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) }
        };

        public static AuditPolicy Parse(BinaryReader reader)
        {
            // Create instance of policy storage
            AuditPolicy policy = new AuditPolicy();

            List<string> keys = policy.HeaderSettingPairs.Keys.ToList();

            // Enumerate audits in dictionary storage
            foreach (string key in keys)
            {
                policy.HeaderSettingPairs[key] = ScoredItem<EAuditSettings>.ParseAuditSettings(reader);
            }

            return policy;
        }
    }
}
