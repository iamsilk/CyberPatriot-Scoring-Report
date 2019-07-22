using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Configuration
{
    public class PasswordPolicy
    {
        // 0 - 24
        public ScoredItem<Range> EnforcePasswordHistory = new ScoredItem<Range>(new Range(0, 24), false);

        // 0 - 999
        public ScoredItem<Range> MaxPasswordAge = new ScoredItem<Range>(new Range(0, 999), false);

        // 0 - 998 (must be less than max password age)
        public ScoredItem<Range> MinPasswordAge = new ScoredItem<Range>(new Range(0, 998), false);

        // 0 - 20
        public ScoredItem<Range> MinPasswordLength = new ScoredItem<Range>(new Range(0, 20), false);

        // Enabled (0)/Disabled (1)
        public ScoredItem<int> PasswordComplexity = new ScoredItem<int>(0, false);

        // Enabled (0)/Disabled (1)
        public ScoredItem<int> ReversibleEncryption = new ScoredItem<int>(1, false);
        
        public static PasswordPolicy Parse(BinaryReader reader)
        {
            // Create instance of policy storage
            PasswordPolicy policy = new PasswordPolicy();

            // Retrieve values defining policy
            policy.EnforcePasswordHistory = ScoredItem<Range>.ParseRange(reader);
            policy.MaxPasswordAge = ScoredItem<Range>.ParseRange(reader);
            policy.MinPasswordAge = ScoredItem<Range>.ParseRange(reader);
            policy.MinPasswordLength = ScoredItem<Range>.ParseRange(reader);
            policy.PasswordComplexity = ScoredItem<int>.ParseInt32(reader);
            policy.ReversibleEncryption = ScoredItem<int>.ParseInt32(reader);

            return policy;
        }
    }
}
