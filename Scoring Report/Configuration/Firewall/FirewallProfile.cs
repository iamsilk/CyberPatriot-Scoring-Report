using NetFwTypeLib;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Configuration.Firewall
{
    public class FirewallProfile
    {
        public string Profile = "";

        public NET_FW_PROFILE_TYPE2_ ProfileType => ProfileTypes[Profile];

        public static Dictionary<string, NET_FW_PROFILE_TYPE2_> ProfileTypes { get; } = new Dictionary<string, NET_FW_PROFILE_TYPE2_>()
        {
            { "Domain Profile", NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN },
            { "Private Profile", NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE },
            { "Public Profile", NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC },
        };

        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        public static FirewallProfile Parse(BinaryReader reader)
        {
            // Create instance of profile
            FirewallProfile profile = new FirewallProfile();

            // Get profile title
            profile.Profile = reader.ReadString();

            // Get number of properties
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get property info
                string propertyName = reader.ReadString();
                bool isScored = reader.ReadBoolean();
                string propertyValue = reader.ReadString();

                // If property isn't scored, skip
                if (!isScored) continue;

                // Add property to profile instance
                profile.Properties.Add(propertyName, propertyValue);
            }

            return profile;
        }
    }
}
