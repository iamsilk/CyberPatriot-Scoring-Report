using Configuration_Tool.Controls;
using Configuration_Tool.Controls.Firewall;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigFirewallProfiles : IConfig
    {
        public EConfigType Type => EConfigType.FirewallProfiles;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Read number of firewall profiles
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Read profile header
                string header = reader.ReadString();

                bool foundMatch = false;

                // For each profile in items control
                foreach (ControlProfile profile in MainWindow.itemsFirewallProfiles.Items)
                {
                    // If profile header matches config
                    if (profile.Header == header)
                    {
                        // Load config into control
                        profile.Load(reader);

                        foundMatch = true;
                    }
                }

                // If no match was found
                if (!foundMatch)
                {
                    // Create instance of new profile
                    ControlProfile profile = new ControlProfile();

                    // Load profile to skip outdated info in stream
                    profile.Load(reader);

                    // We do not add this to the list as it is likely outdated information
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Write number of firewall profiles
            writer.Write(MainWindow.itemsFirewallProfiles.Items.Count);

            // For each firewall profile
            foreach (ControlProfile profile in MainWindow.itemsFirewallProfiles.Items)
            {
                // Write profile header
                writer.Write(profile.Header);

                // Write profile
                profile.Write(writer);
            }
        }
    }
}
