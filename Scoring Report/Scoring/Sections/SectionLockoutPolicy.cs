using Scoring_Report.Configuration;
using Scoring_Report.Policies;
using System.Collections.Generic;
using System.IO;
using config = Scoring_Report.Configuration.Lockout;
using policy = Scoring_Report.Policies.Lockout;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionLockoutPolicy : ISection
    {
        public ESectionType Type => ESectionType.LockoutPolicy;

        public string Header => "Account Lockout Policy:";

        public static config.LockoutPolicy ConfigPolicy { get; set; }

        public static policy.LockoutPolicy SystemPolicy => SecurityPolicyManager.Settings.AccountPolicies.LockoutPolicy;

        public int MaxScore()
        {
            int max = 0;

            // Check all scored items
            if (ConfigPolicy.AccountLockoutDuration.IsScored) max++;
            if (ConfigPolicy.AccountLockoutThreshold.IsScored) max++;
            if (ConfigPolicy.ResetLockoutCounterAfter.IsScored) max++;

            return max;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            SecurityPolicyManager.GetLockoutPolicy();

            if (ConfigPolicy.AccountLockoutDuration.IsScored &&
                ConfigPolicy.AccountLockoutDuration.Value.WithinBounds(SystemPolicy.AccountLockoutDuration))
            {
                details.Points++;
                details.Output.Add(ConfigurationManager.Translate("AccountLockoutDuration", SystemPolicy.AccountLockoutDuration));
            }
            if (ConfigPolicy.AccountLockoutThreshold.IsScored &&
                ConfigPolicy.AccountLockoutThreshold.Value.WithinBounds(SystemPolicy.AccountLockoutThreshold))
            {
                details.Points++;
                details.Output.Add(ConfigurationManager.Translate("AccountLockoutThreshold", SystemPolicy.AccountLockoutThreshold));
            }
            if (ConfigPolicy.ResetLockoutCounterAfter.IsScored &&
                ConfigPolicy.ResetLockoutCounterAfter.Value.WithinBounds(SystemPolicy.ResetLockoutCounterAfter))
            {
                details.Points++;
                details.Output.Add(ConfigurationManager.Translate("ResetLockoutCounterAfter", SystemPolicy.ResetLockoutCounterAfter));
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Get stored policy
            Configuration.Lockout.LockoutPolicy policy = config.LockoutPolicy.Parse(reader);

            // Store policy in global variable
            ConfigPolicy = policy;
        }
    }
}
