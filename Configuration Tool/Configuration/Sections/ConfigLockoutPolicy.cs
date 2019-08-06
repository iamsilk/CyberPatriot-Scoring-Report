using Configuration_Tool.Configuration.Lockout;
using Configuration_Tool.Controls;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigLockoutPolicy : IConfig
    {
        public EConfigType Type => EConfigType.LockoutPolicy;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Get stored policy
            LockoutPolicy policy = LockoutPolicy.Parse(reader);

            // Write policy settings to window
            policy.WriteToWindow(MainWindow);
        }

        public void Save(BinaryWriter writer)
        {
            // Get policy from main window
            LockoutPolicy policy = LockoutPolicy.GetFromWindow(MainWindow);

            // Write policy settings to stream
            policy.Write(writer);
        }
    }
}
