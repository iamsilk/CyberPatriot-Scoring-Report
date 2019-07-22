using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public class SecuritySettings
    {
        public static SecuritySettings Default { get; } = new SecuritySettings();

        public AccountPolicies AccountPolicies = new AccountPolicies();

        public LocalPolicies LocalPolicies = new LocalPolicies();
    }
}
