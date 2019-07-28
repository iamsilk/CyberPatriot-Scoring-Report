using Configuration_Tool.Controls;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigPasswordPolicy : IConfig
    {
        public EConfigType Type => EConfigType.PasswordPolicy;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Get stored policy
            PasswordPolicy policy = PasswordPolicy.Parse(reader);

            // Write policy settings to window
            policy.WriteToWindow(MainWindow);
        }

        public void Save(BinaryWriter writer)
        {
            // Get policy from main window
            PasswordPolicy policy = PasswordPolicy.GetFromWindow(MainWindow);

            // Write policy settings to stream
            policy.Write(writer);
        }
    }
}
