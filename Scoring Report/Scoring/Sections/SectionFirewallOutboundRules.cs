using Scoring_Report.Configuration;
using Scoring_Report.Configuration.Firewall;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionFirewallOutboundRules : ISection
    {
        public string Header => "Firewall - Outbound Rules:";

        public List<Rule> OutboundRules { get; } = new List<Rule>();

        public ESectionType Type => ESectionType.FirewallOutboundRules;

        public int MaxScore()
        {
            return OutboundRules.Count;
        }

        public void Load(BinaryReader reader)
        {
            // Clear outbound rules list
            OutboundRules.Clear();

            // Read number of outbound rules
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Parse rule from binary reader
                Rule rule = Rule.Parse(reader);

                // If for some reason rule isn't scored, skip
                if (!rule.IsScored) continue;

                // Add outbound rule to list
                OutboundRules.Add(rule);
            }
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Get system's outbound rules
            List<Rule> systemRules = Rule.GetOutboundRules();

            // For each configured outbound rule
            foreach (Rule rule in OutboundRules)
            {
                bool foundMatch = false;

                // For each system rule
                foreach (Rule systemRule in systemRules)
                {
                    // If rules match
                    if (rule.Equals(systemRule))
                    {
                        foundMatch = true;

                        // Match was found, break loop
                        break;
                    }
                }

                // If match wasn't found, give points
                if (!foundMatch)
                {
                    details.Points++;
                    details.Output.Add(TranslationManager.Translate("FirewallOutboundRule",
                        rule.Name,
                        rule.Group,
                        rule.Description,
                        rule.ProfileString,
                        rule.Enabled ? "Enabled" : "Disabled",
                        rule.ActionString,
                        rule.ApplicationName,
                        rule.ServiceName,
                        rule.LocalAddresses,
                        rule.RemoteAddresses,
                        rule.Protocol,
                        rule.LocalPorts,
                        rule.RemotePorts));
                }
            }

            return details;
        }
    }
}
