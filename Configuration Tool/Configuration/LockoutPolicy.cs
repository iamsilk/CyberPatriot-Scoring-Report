using Configuration_Tool.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration
{
    public class LockoutPolicy
    {
        public ScoredItem<Range> AccountLockoutDuration = new ScoredItem<Range>(new Range(0, 99999), false);

        public ScoredItem<Range> AccountLockoutThreshold = new ScoredItem<Range>(new Range(0, 999), false);

        public ScoredItem<Range> ResetLockoutCounterAfter = new ScoredItem<Range>(new Range(0, 99999), false);

        public static LockoutPolicy GetFromWindow(MainWindow window)
        {
            // Create instance of policy storage
            LockoutPolicy policy = new LockoutPolicy();

            // Retrieve values defining policy
            policy.AccountLockoutDuration = 
                window.settingAccountLockoutDuration.GetScoredItem();
            policy.AccountLockoutThreshold = 
                window.settingAccountLockoutThreshold.GetScoredItem();
            policy.ResetLockoutCounterAfter =
                window.settingResetLockoutCounterAfter.GetScoredItem();

            return policy;
        }

        public void WriteToWindow(MainWindow window)
        {
            // Set window settings from stored policy
            window.settingAccountLockoutDuration.SetFromScoredItem(AccountLockoutDuration);
            window.settingAccountLockoutThreshold.SetFromScoredItem(AccountLockoutThreshold);
            window.settingResetLockoutCounterAfter.SetFromScoredItem(ResetLockoutCounterAfter);
        }

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

        public void Write(BinaryWriter writer)
        {
            // Write all values defining policy
            AccountLockoutDuration.Write(writer);
            AccountLockoutThreshold.Write(writer);
            ResetLockoutCounterAfter.Write(writer);
        }
    }
}
