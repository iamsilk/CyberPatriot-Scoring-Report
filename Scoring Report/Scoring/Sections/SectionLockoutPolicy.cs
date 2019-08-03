using Scoring_Report.Configuration;
using Scoring_Report.Policies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionLockoutPolicy : ISection
    {
        public ESectionType Type => ESectionType.LockoutPolicy;

        public string Header => "Account Lockout Policy:";

        public static Configuration.LockoutPolicy ConfigPolicy { get; set; }

        public static Policies.LockoutPolicy SystemPolicy => SecurityPolicyManager.Settings.AccountPolicies.LockoutPolicy;

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
            Configuration.LockoutPolicy policy = Configuration.LockoutPolicy.Parse(reader);

            // Store policy in global variable
            ConfigPolicy = policy;
        }
    }
}
