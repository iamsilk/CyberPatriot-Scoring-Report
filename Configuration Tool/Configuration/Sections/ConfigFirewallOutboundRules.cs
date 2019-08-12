using Configuration_Tool.Configuration.Firewall;
using Configuration_Tool.Controls;
using System.Collections.Generic;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigFirewallOutboundRules : IConfig
    {
        public EConfigType Type => EConfigType.FirewallOutboundRules;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Clear outbound rules list
            ConfigurationManager.OutboundRules.Clear();

            // Populate outbound rules list
            MainWindow.PopulateFirewallOutboundRules();

            // Read number of outbound rules
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Parse rule from binary reader
                Rule rule = Rule.Parse(reader);

                Rule match = null;

                // For each outbound rule
                foreach (Rule possibleMatch in ConfigurationManager.OutboundRules)
                {
                    // If rules match
                    if (rule.Equals(possibleMatch))
                    {
                        match = possibleMatch;
                    }
                }

                // If no match was found
                if (match == null)
                {
                    // Add rule to list
                    ConfigurationManager.OutboundRules.Add(rule);
                }
                else
                {
                    // Set scoring status
                    match.IsScored = rule.IsScored;
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            List<Rule> scoredRules = new List<Rule>();

            // For each outbound rule
            foreach (Rule rule in ConfigurationManager.OutboundRules)
            {
                // If rule is scored, add to list
                if (rule.IsScored) scoredRules.Add(rule);
            }

            // Write number of scored rules
            writer.Write(scoredRules.Count);

            // For each scored rule, write it
            foreach (Rule rule in scoredRules)
                rule.Write(writer);
        }
    }
}
