using Scoring_Report.Policies.Lockout;
using Scoring_Report.Policies.Password;

namespace Scoring_Report.Policies
{
    public class AccountPolicies
    {
        public PasswordPolicy PasswordPolicy = new PasswordPolicy();

        public LockoutPolicy LockoutPolicy = new LockoutPolicy();
    }
}
