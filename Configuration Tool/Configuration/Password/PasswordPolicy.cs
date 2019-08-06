using Configuration_Tool.Controls;
using System.IO;

namespace Configuration_Tool.Configuration.Password
{
    public class PasswordPolicy
    {
        public ScoredItem<Range> EnforcePasswordHistory = new ScoredItem<Range>(new Range(0, 24), false);
        
        public ScoredItem<Range> MaxPasswordAge = new ScoredItem<Range>(new Range(0, 999), false);
        
        public ScoredItem<Range> MinPasswordAge = new ScoredItem<Range>(new Range(0, 998), false);
        
        public ScoredItem<Range> MinPasswordLength = new ScoredItem<Range>(new Range(0,20), false);

        // Disabled (0)/Enabled (1)
        public ScoredItem<int> PasswordComplexity = new ScoredItem<int>(1, false);

        // Disabled (0)/Enabled (1)
        public ScoredItem<int> ReversibleEncryption = new ScoredItem<int>(0, false);

        public static PasswordPolicy GetFromWindow(MainWindow window)
        {
            // Create instance of policy storage
            PasswordPolicy policy = new PasswordPolicy();

            // Retrieve values defining policy
            policy.EnforcePasswordHistory = 
                window.settingEnforcePasswordHistory.GetScoredItem();
            policy.MaxPasswordAge = 
                window.settingMaxPasswordAge.GetScoredItem();
            policy.MinPasswordAge =
                window.settingMinPasswordAge.GetScoredItem();
            policy.MinPasswordLength =
                window.settingMinPasswordLen.GetScoredItem();
            policy.PasswordComplexity =
                window.settingPasswordComplexity.GetScoredItem();
            policy.ReversibleEncryption =
                window.settingReversibleEncryption.GetScoredItem();

            return policy;
        }

        public void WriteToWindow(MainWindow window)
        {
            // Set window settings from stored policy
            window.settingEnforcePasswordHistory.SetFromScoredItem(EnforcePasswordHistory);
            window.settingMaxPasswordAge.SetFromScoredItem(MaxPasswordAge);
            window.settingMinPasswordAge.SetFromScoredItem(MinPasswordAge);
            window.settingMinPasswordLen.SetFromScoredItem(MinPasswordLength);
            window.settingPasswordComplexity.SetFromScoredItem(PasswordComplexity);
            window.settingReversibleEncryption.SetFromScoredItem(ReversibleEncryption);
        }

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

        public void Write(BinaryWriter writer)
        {
            // Write all values defining policy
            EnforcePasswordHistory.Write(writer);
            MaxPasswordAge.Write(writer);
            MinPasswordAge.Write(writer);
            MinPasswordLength.Write(writer);
            PasswordComplexity.Write(writer);
            ReversibleEncryption.Write(writer);
        }
    }
}
