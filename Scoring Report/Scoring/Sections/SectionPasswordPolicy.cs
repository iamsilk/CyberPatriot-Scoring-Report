using Scoring_Report.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scoring_Report.Policies;
using System.IO;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionPasswordPolicy : ISection
    {
        public ESectionType Type => ESectionType.PasswordPolicy;

        public string Header => "Password Policy:";

        public static Configuration.PasswordPolicy ConfigPolicy { get; set; }

        public static Policies.PasswordPolicy SystemPolicy => SecurityPolicyManager.Settings.AccountPolicies.PasswordPolicy;

        public int MaxScore()
        {
            int max = 0;

            // Check all scored items
            if (ConfigPolicy.EnforcePasswordHistory.IsScored) max++;
            if (ConfigPolicy.MaxPasswordAge.IsScored) max++;
            if (ConfigPolicy.MinPasswordAge.IsScored) max++;
            if (ConfigPolicy.MinPasswordLength.IsScored) max++;
            if (ConfigPolicy.PasswordComplexity.IsScored) max++;
            if (ConfigPolicy.ReversibleEncryption.IsScored) max++;

            return max;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            SecurityPolicyManager.GetPasswordPolicy();

            if (ConfigPolicy.EnforcePasswordHistory.IsScored &&
                ConfigPolicy.EnforcePasswordHistory.Value.WithinBounds(SystemPolicy.EnforcePasswordHistory))
            {
                details.Points++;
                details.Output.Add(ConfigurationManager.Translate("EnforcePasswordHistory", SystemPolicy.EnforcePasswordHistory));
            }
            if (ConfigPolicy.MaxPasswordAge.IsScored && 
                ConfigPolicy.MaxPasswordAge.Value.WithinBounds(SystemPolicy.MaximumPasswordAge))
            {
                details.Points++;
                details.Output.Add(ConfigurationManager.Translate("MaxPasswordAge", SystemPolicy.MaximumPasswordAge));
            }
            if (ConfigPolicy.MinPasswordAge.IsScored && 
                ConfigPolicy.MinPasswordAge.Value.WithinBounds(SystemPolicy.MinimumPasswordAge))
            {
                details.Points++;
                details.Output.Add(ConfigurationManager.Translate("MinPasswordAge", SystemPolicy.MinimumPasswordAge));
            }
            if (ConfigPolicy.MinPasswordLength.IsScored && 
                ConfigPolicy.MinPasswordLength.Value.WithinBounds(SystemPolicy.MinimumPasswordLength))
            {
                details.Points++;
                details.Output.Add(ConfigurationManager.Translate("MinPasswordLength", SystemPolicy.MinimumPasswordLength));
            }

            string[] readableNames = new string[] { "Disabled", "Enabled" };

            if (ConfigPolicy.PasswordComplexity.IsScored && ConfigPolicy.PasswordComplexity.Value == SystemPolicy.PasswordComplexity)
            {
                details.Points++;
                details.Output.Add(ConfigurationManager.Translate("PasswordComplexity", readableNames[SystemPolicy.PasswordComplexity]));
            }
            if (ConfigPolicy.ReversibleEncryption.IsScored && ConfigPolicy.ReversibleEncryption.Value == SystemPolicy.ReversibleEncryption)
            {
                details.Points++;
                details.Output.Add(ConfigurationManager.Translate("ReversibleEncryption", readableNames[SystemPolicy.ReversibleEncryption]));
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Get stored policy
            Configuration.PasswordPolicy policy = Configuration.PasswordPolicy.Parse(reader);

            // Store policy in global variable
            ConfigPolicy = policy;
        }
    }
}
