using Scoring_Report.Configuration;
using Scoring_Report.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionAuditPolicy : ISection
    {
        public string Header => "Audit Policy:";

        public static Configuration.AuditPolicy ConfigPolicy => ConfigurationManager.AuditPolicy;

        public static Policies.AuditPolicy SystemPolicy => SecurityPolicyManager.Settings.LocalPolicies.AuditPolicy;

        public const string Format = "'{0}' set correctly - {1}";

        public readonly Dictionary<EAuditSettings, string> AuditSettingFormat = new Dictionary<EAuditSettings, string>()
        {
            { EAuditSettings.Unchanged, "No auditing" },
            { EAuditSettings.Success, "Success" },
            { EAuditSettings.Failure, "Failure" },
            {EAuditSettings.SuccessFailure, "Success, Failure" }
        };


        public int MaxScore()
        {
            int max = 0;

            // Check all audit sections
            foreach (ScoredItem<EAuditSettings> settings in ConfigPolicy.HeaderSettingPairs.Values)
            {
                if (settings.IsScored) max++;
            }

            return max;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            SecurityPolicyManager.GetAuditPolicy();

            // Loop over every audit configuration
            foreach (KeyValuePair<string, ScoredItem<EAuditSettings>> config in ConfigPolicy.HeaderSettingPairs)
            {
                // If not scored, skip
                if (!config.Value.IsScored) continue;

                // Loop over every system audit setting
                foreach (KeyValuePair<string, POLICY_AUDIT_EVENT> systemSetting in SystemPolicy.HeaderSettingPairs)
                {
                    // Check if config and system setting are of the same policy
                    if (config.Key == systemSetting.Key)
                    {
                        // Check if integer casted values are equal
                        if ((int)config.Value.Value == (int)systemSetting.Value)
                        {
                            // Setting is properly configured
                            details.Points++;
                            details.Output.Add(string.Format(Format, config.Key, AuditSettingFormat[config.Value.Value]));
                        }
                    }
                }
            }

            return details;
        }
    }
}
