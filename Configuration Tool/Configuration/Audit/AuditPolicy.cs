using Configuration_Tool.Controls;
using Configuration_Tool.Controls.Audit;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Configuration_Tool.Configuration.Audit
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

        public static AuditPolicy GetFromWindow(MainWindow window)
        {
            // Create instance of policy storage
            AuditPolicy policy = new AuditPolicy();

            // Enumerate every control in items control containing policy
            foreach (ControlSettingAudit control in window.itemsAuditPolicy.Items.Cast<ControlSettingAudit>())
            {
                policy.HeaderSettingPairs[control.Header] = control.GetScoredItem();
            }

            return policy;
        }

        public void WriteToWindow(MainWindow window)
        {
            // Enumerate every control in items control
            foreach (ControlSettingAudit control in window.itemsAuditPolicy.Items.Cast<ControlSettingAudit>())
            {
                // Set control from stored scored item
                control.SetFromScoredItem(HeaderSettingPairs[control.Header]);
            }
        }

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

        public void Write(BinaryWriter writer)
        {
            // Enumerate audits in dictionary storage
            foreach (KeyValuePair<string, ScoredItem<EAuditSettings>> pair in HeaderSettingPairs)
            {
                pair.Value.Write(writer);
            }
        }
    }
}
