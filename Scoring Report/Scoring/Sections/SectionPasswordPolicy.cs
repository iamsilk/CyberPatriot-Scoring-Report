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

        public static class Format
        {
            public const string EnforcePasswordHistory = "'Enforce password history' set correctly - {0} passwords remembered";
            public const string MaxPasswordAge = "'Maximum password age' set correctly - {0} days";
            public const string MinPasswordAge = "'Minimum password age' set correctly - {0} days";
            public const string MinPasswordLength = "'Minimum password length' set correctly - {0} characters";
            public const string PasswordComplexity = "'Password must meet complexity requirements' set correctly - {0}";
            public const string ReversibleEncryption = "'Store passwords using reversible encryption' set correctly - {0}";
        }

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
                details.Output.Add(string.Format(Format.EnforcePasswordHistory, SystemPolicy.EnforcePasswordHistory));
            }
            if (ConfigPolicy.MaxPasswordAge.IsScored && 
                ConfigPolicy.MaxPasswordAge.Value.WithinBounds(SystemPolicy.MaximumPasswordAge))
            {
                details.Points++;
                details.Output.Add(string.Format(Format.MaxPasswordAge, SystemPolicy.MaximumPasswordAge));
            }
            if (ConfigPolicy.MinPasswordAge.IsScored && 
                ConfigPolicy.MinPasswordAge.Value.WithinBounds(SystemPolicy.MinimumPasswordAge))
            {
                details.Points++;
                details.Output.Add(string.Format(Format.MinPasswordAge, SystemPolicy.MinimumPasswordAge));
            }
            if (ConfigPolicy.MinPasswordLength.IsScored && 
                ConfigPolicy.MinPasswordLength.Value.WithinBounds(SystemPolicy.MinimumPasswordLength))
            {
                details.Points++;
                details.Output.Add(string.Format(Format.MinPasswordLength, SystemPolicy.MinimumPasswordLength));
            }

            string[] readableNames = new string[] { "Disabled", "Enabled" };

            if (ConfigPolicy.PasswordComplexity.IsScored && ConfigPolicy.PasswordComplexity.Value == SystemPolicy.PasswordComplexity)
            {
                details.Points++;
                details.Output.Add(string.Format(Format.PasswordComplexity, readableNames[SystemPolicy.PasswordComplexity]));
            }
            if (ConfigPolicy.ReversibleEncryption.IsScored && ConfigPolicy.ReversibleEncryption.Value == SystemPolicy.ReversibleEncryption)
            {
                details.Points++;
                details.Output.Add(string.Format(Format.ReversibleEncryption, readableNames[SystemPolicy.ReversibleEncryption]));
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
