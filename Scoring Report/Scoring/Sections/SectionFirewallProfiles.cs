using NetFwTypeLib;
using Scoring_Report.Configuration;
using Scoring_Report.Configuration.Firewall;
using System;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionFirewallProfiles : ISection
    {
        public string Header => "Firewall - Profiles:";

        public static List<FirewallProfile> Profiles { get; } = new List<FirewallProfile>();

        public ESectionType Type => ESectionType.FirewallProfiles;

        public int MaxScore()
        {
            int max = 0;

            // For each firewall profile
            foreach (FirewallProfile profile in Profiles)
                // Increment max points by number of scored properties
                max += profile.Properties.Count;

            return max;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Get firewall info
            INetFwPolicy2 fwPolicy = GetFirewallPolicy();

            // For each firewall profile in config
            foreach (FirewallProfile profile in Profiles)
            {
                NET_FW_PROFILE_TYPE2_ type = profile.ProfileType;

                // For each property in firewall profile
                foreach (KeyValuePair<string, string> property in profile.Properties)
                {
                    string systemValue = null;

                    // Get system value corresponding to property
                    switch (property.Key)
                    {
                        case "Firewall state":
                            bool fwEnabled = fwPolicy.FirewallEnabled[type];

                            if (fwEnabled) systemValue = "On";
                            else systemValue = "Off";

                            break;

                        case "Inbound connections":
                            if (fwPolicy.BlockAllInboundTraffic[type])
                                systemValue = "Block all connections";
                            else if (fwPolicy.DefaultInboundAction[type] == NET_FW_ACTION_.NET_FW_ACTION_BLOCK)
                                systemValue = "Block";
                            else
                                systemValue = "Allow";

                            break;

                        case "Outbound connections":
                            if (fwPolicy.DefaultOutboundAction[type] == NET_FW_ACTION_.NET_FW_ACTION_BLOCK)
                                systemValue = "Block";
                            else
                                systemValue = "Allow";

                            break;

                        case "Display a notification":
                            if (fwPolicy.NotificationsDisabled[type])
                                systemValue = "No";
                            else
                                systemValue = "Yes";

                            break;

                        case "Allow unicast response":
                            if (fwPolicy.UnicastResponsesToMulticastBroadcastDisabled[type])
                                systemValue = "No";
                            else
                                systemValue = "Yes";

                            break;
                    }

                    // If no value was assigned, unknown property. Skip
                    if (systemValue == null) continue;

                    // Check if config and system values match
                    if (systemValue == property.Value)
                    {
                        details.Points++;
                        details.Output.Add(TranslationManager.Translate("FirewallProfileProperty", profile.Profile, property.Key, property.Value));
                    }
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Clear current configuration
            Profiles.Clear();

            // Get number of profiles
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get firewall profile
                FirewallProfile profile = FirewallProfile.Parse(reader);

                // If profile has no scored properties, skip
                if (profile.Properties.Count == 0) continue;

                // Add profile to list
                Profiles.Add(profile);
            }
        }

        public INetFwPolicy2 GetFirewallPolicy()
        {
            // Get type of firewall manager
            Type fwPolicyType = System.Type.GetTypeFromProgID("HNetCfg.FwPolicy2", false);

            // Return instance of firewall manager
            return (INetFwPolicy2)Activator.CreateInstance(fwPolicyType);
        }
    }
}
