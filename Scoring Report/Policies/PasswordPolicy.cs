using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public class PasswordPolicy
    {
        // Range: 0-24, Default: 0
        public int EnforcePasswordHistory = 0;

        // Range: 0-999, Default: 42
        public int MaximumPasswordAge = 42;

        // Range: 0 - 999, Default: 0
        public int MinimumPasswordAge = 0;

        // Range: 0 - 20, Default: 0
        public int MinimumPasswordLength = 0;

        // Disabled (0), Enabled (1). Default: Disabled
        public int PasswordComplexity = 0;

        // Disabled (0), Enabled (1). Default: Disabled
        public int ReversibleEncryption = 0;
    }
}
