using Scoring_Report.Configuration;
using Scoring_Report.Configuration.Firewall;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionFirewallInboundRules : ISection
    {
        public string Header => TranslationManager.Translate("SectionFirewallInboundRules");

        public List<Rule> InboundRules { get; } = new List<Rule>();

        public ESectionType Type => ESectionType.FirewallInboundRules;

        public int MaxScore()
        {
            return InboundRules.Count;
        }

        public void Load(BinaryReader reader)
        {
            // Clear inbound rules list
            InboundRules.Clear();

            // Read number of inbound rules
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Parse rule from binary reader
                Rule rule = Rule.Parse(reader);

                // If for some reason rule isn't scored, skip
                if (!rule.IsScored) continue;

                // Add inbound rule to list
                InboundRules.Add(rule);
            }
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Get system's inbound rules
            List<Rule> systemRules = Rule.GetInboundRules();

            // For each configured inbound rule
            foreach (Rule rule in InboundRules)
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
                    details.Output.Add(TranslationManager.Translate("FirewallInboundRule", 
                        rule.Name, 
                        rule.Group, 
                        rule.Description, 
                        rule.ProfileString, 
                        rule.Enabled ? TranslationManager.Translate("Enabled") : TranslationManager.Translate("Disabled"),
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
