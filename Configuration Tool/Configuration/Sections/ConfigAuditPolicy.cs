using Configuration_Tool.Configuration.Audit;
using Configuration_Tool.Controls;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigAuditPolicy : IConfig
    {
        public EConfigType Type => EConfigType.AuditPolicy;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Get stored policy
            AuditPolicy policy = AuditPolicy.Parse(reader);

            // Write policy settings to window
            policy.WriteToWindow(MainWindow);
        }

        public void Save(BinaryWriter writer)
        {
            // Get policy from main window
            AuditPolicy policy = AuditPolicy.GetFromWindow(MainWindow);

            // Write policy settings to stream
            policy.Write(writer);
        }
    }
}
