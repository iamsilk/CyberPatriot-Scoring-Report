using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public class AccountPolicies
    {
        public PasswordPolicy PasswordPolicy = new PasswordPolicy();

        public LockoutPolicy LockoutPolicy = new LockoutPolicy();
    }
}
