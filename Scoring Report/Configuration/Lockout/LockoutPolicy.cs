using System.IO;

namespace Scoring_Report.Configuration.Lockout
{
    public class LockoutPolicy
    {
        public ScoredItem<Range> AccountLockoutDuration = new ScoredItem<Range>(new Range(0, 99999), false);

        public ScoredItem<Range> AccountLockoutThreshold = new ScoredItem<Range>(new Range(0, 999), false);

        public ScoredItem<Range> ResetLockoutCounterAfter = new ScoredItem<Range>(new Range(0, 99999), false);

        public static LockoutPolicy Parse(BinaryReader reader)
        {
            // Create instance of policy storage
            LockoutPolicy policy = new LockoutPolicy();

            // Retrieve values defining policy
            policy.AccountLockoutDuration = ScoredItem<Range>.ParseRange(reader);
            policy.AccountLockoutThreshold = ScoredItem<Range>.ParseRange(reader);
            policy.ResetLockoutCounterAfter = ScoredItem<Range>.ParseRange(reader);

            return policy;
        }
    }
}
