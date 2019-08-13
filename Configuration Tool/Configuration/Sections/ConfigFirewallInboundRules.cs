using Configuration_Tool.Configuration.Firewall;
using Configuration_Tool.Controls;
using System.Collections.Generic;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigFirewallInboundRules : IConfig
    {
        public EConfigType Type => EConfigType.FirewallInboundRules;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Clear inbound rules list
            ConfigurationManager.InboundRules.Clear();

            // Populate inbound rules list
            MainWindow.PopulateFirewallInboundRules();

            // Read number of inbound rules
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Parse rule from binary reader
                Rule rule = Rule.Parse(reader);

                Rule match = null;

                // For each inbound rule
                foreach (Rule possibleMatch in ConfigurationManager.InboundRules)
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
                    ConfigurationManager.InboundRules.Add(rule);
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

            // For each inbound rule
            foreach (Rule rule in ConfigurationManager.InboundRules)
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
