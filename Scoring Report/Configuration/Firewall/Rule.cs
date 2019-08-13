using NetFwTypeLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scoring_Report.Configuration.Firewall
{
    public class Rule
    {
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public string ApplicationName { get; set; } = "";

        public string ServiceName { get; set; } = "";

        public int intProtocol = 0;

        public static Dictionary<string, int> Protocols { get; } = new Dictionary<string, int>()
        {
            { "Any", 256 },
            { "HOPOPT", 0 },
            { "ICMPv4", 1 },
            { "IGMP", 2 },
            { "TCP", 6 },
            { "UDP", 17 },
            { "IPv6", 41 },
            { "IPv6-Route", 43 },
            { "IPv6-Frag", 44 },
            { "GRE", 47 },
            { "ICMPv6", 58 },
            { "IPv6-NoNxt", 59 },
            { "IPv6-Opts", 60 },
            { "VRRP", 112 },
            { "PGM", 113 },
            { "L2TP", 115 },
        };

        public static Dictionary<string, int>.KeyCollection ProtocolNames => Protocols.Keys;

        public string Protocol
        {
            get
            {
                foreach (KeyValuePair<string, int> pair in Protocols)
                {
                    if (pair.Value == intProtocol) return pair.Key;
                }
                
                return "Custom";
            }
            set
            {
                intProtocol = Protocols[value];
            }
        }

        public string LocalPorts { get; set; } = "";

        public string RemotePorts { get; set; } = "";

        public string LocalAddresses { get; set; } = "";

        public string RemoteAddresses { get; set; } = "";

        public bool Enabled { get; set; } = false;

        public string Group { get; set; } = "";

        public NET_FW_PROFILE_TYPE2_ Profile = 0;

        public string ProfileString
        {
            get
            {
                NET_FW_PROFILE_TYPE2_ all = NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN | NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE | NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC;

                // If all profiles are selected
                if ((Profile & all) == all)
                {
                    return "All";
                }

                List<string> profiles = new List<string>();

                if (IsDomainProfile)
                {
                    profiles.Add("Domain");
                }
                if (IsPrivateProfile)
                {
                    profiles.Add("Private");
                }
                if (IsPublicProfile)
                {
                    profiles.Add("Public");
                }

                return string.Join(", ", profiles);
            }
        }

        public bool IsDomainProfile
        {
            get { return (Profile & NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN) != 0; }
            set
            {
                // Check if value isn't changing
                if (value == IsDomainProfile) return;

                if (value)
                    Profile |= NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN;
                else
                    Profile &= ~NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN;
            }
        }

        public bool IsPrivateProfile
        {
            get { return (Profile & NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE) != 0; }
            set
            {
                // Check if value isn't changing
                if (value == IsPrivateProfile) return;

                if (value)
                    Profile |= NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE;
                else
                    Profile &= ~NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE;
            }
        }

        public bool IsPublicProfile
        {
            get { return (Profile & NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC) != 0; }
            set
            {
                // Check if value isn't changing
                if (value == IsPublicProfile) return;

                if (value)
                    Profile |= NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC;
                else
                    Profile &= ~NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC;
            }
        }

        public NET_FW_ACTION_ Action { get; set; } = 0;

        public string ActionString
        {
            get
            {
                if (IsAllowed) return "Allow the connection";
                return "Block the connection";
            }
        }

        public bool IsAllowed
        {
            get { return Action == NET_FW_ACTION_.NET_FW_ACTION_ALLOW; }
            set
            {
                Action = value ? NET_FW_ACTION_.NET_FW_ACTION_ALLOW : NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            }
        }

        public bool IsBlocked
        {
            get { return Action == NET_FW_ACTION_.NET_FW_ACTION_BLOCK; }
            set
            {
                Action = value ? NET_FW_ACTION_.NET_FW_ACTION_BLOCK : NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            }
        }

        public bool IsScored { get; set; } = false;

        public static INetFwPolicy2 GetFirewallPolicy()
        {
            // Get type of firewall manager
            Type fwPolicyType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2", false);

            // Return instance of firewall manager
            return (INetFwPolicy2)Activator.CreateInstance(fwPolicyType);
        }

        // Used to set any null strings to be empty instead of null
        private static string checkString(string value)
        {
            return value == null ? "" : value;
        }

        public static List<Rule> GetInboundRules()
        {
            return GetFirewallRules(NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN);
        }

        public static List<Rule> GetOutboundRules()
        {
            return GetFirewallRules(NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT);
        }

        public static List<Rule> GetFirewallRules(NET_FW_RULE_DIRECTION_ direction)
        {
            List<Rule> rules = new List<Rule>();

            // Get firewall info
            INetFwPolicy2 fwPolicy = GetFirewallPolicy();

            // Get firewall rules
            INetFwRules fwRules = fwPolicy.Rules;

            // Get rule enumerator
            IEnumerator enumerator = fwRules.GetEnumerator();

            // For each firewall rule
            while (enumerator.MoveNext())
            {
                INetFwRule fwRule = (INetFwRule)enumerator.Current;

                // If rule is not the direction we're looking for, skip
                if (fwRule.Direction != direction) continue;

                Rule rule = new Rule(fwRule.Name)
                {
                    Description = checkString(fwRule.Description),
                    ApplicationName = checkString(fwRule.ApplicationName),
                    ServiceName = checkString(fwRule.serviceName),
                    intProtocol = fwRule.Protocol,
                    LocalPorts = checkString(fwRule.LocalPorts),
                    RemotePorts = checkString(fwRule.RemotePorts),
                    LocalAddresses = checkString(fwRule.LocalAddresses),
                    RemoteAddresses = checkString(fwRule.RemoteAddresses),
                    Enabled = fwRule.Enabled,
                    Group = checkString(fwRule.Grouping),
                    Profile = (NET_FW_PROFILE_TYPE2_)fwRule.Profiles,
                    Action = fwRule.Action,
                };

                rules.Add(rule);
            }

            return rules;
        }

        public Rule(string name)
        {
            Name = name;
        }

        public static bool PortsEqual(string strPorts1, string strPorts2)
        {
            // If string representations are equal, return true
            if (strPorts1 == strPorts2) return true;

            // If either are null, return false (as we already checked if both are equal)
            if (strPorts1 == null || strPorts2 == null) return false;

            // Trim strings
            strPorts1 = strPorts1.Trim();
            strPorts2 = strPorts2.Trim();

            // If either of the given ports are 'Any' or '', return false
            if (strPorts1 == "Any" || strPorts2 == "Any") return false;
            if (strPorts1 == "*" || strPorts2 == "*") return false;
            if (strPorts1 == "" || strPorts2 == "") return false;

            // Convert ports from string interpretation to arrays. Convert one array into list so we can remove
            string[] ports1 = strPorts1.Split(',');
            List<string> ports2 = new List<string>(strPorts2.Split(','));

            // If they are of different lengths, they aren't the same
            if (ports1.Length != ports2.Count) return false;

            // Trim every string in ports2 list
            ports2 = ports2.Select(x => x.Trim()).ToList();

            // For each port in first list
            foreach (string port in ports1)
            {
                // Trim port being checked
                string check = port.Trim();

                // If ports2 doesn't contain the port, return
                if (!ports2.Contains(check)) return false;

                // Remove port from list
                ports2.Remove(check);
            }

            return true;
        }

        public static bool AddressesEqual(string strAddresses1, string strAddresses2)
        {
            // If string representations are equal, return true
            if (strAddresses1 == strAddresses2) return true;

            // If either are null, return false (as we already checked if both are equal)
            if (strAddresses1 == null || strAddresses2 == null) return false;

            // Trim strings
            strAddresses1 = strAddresses1.Trim();
            strAddresses2 = strAddresses2.Trim();

            // If either of the given addresses are 'Any' or '', return false
            if (strAddresses1 == "Any" || strAddresses2 == "Any") return false;
            if (strAddresses1 == "*" || strAddresses2 == "*") return false;
            if (strAddresses1 == "" || strAddresses2 == "") return false;

            // Convert addresses from string interpretation to string arrays. Convert one array into list so we can remove
            string[] addresses1 = strAddresses1.Split(',');
            List<string> addresses2 = new List<string>(strAddresses2.Split(','));

            // If they are of different lengths, they aren't the same
            if (addresses1.Length != addresses2.Count) return false;

            // Trim every string in addresses2 list
            addresses2 = addresses2.Select(x => x.Trim()).ToList();

            // For each address in first list
            foreach (string address in addresses1)
            {
                // Trim address being checked
                string check = address.Trim();

                // If addresses2 doesn't contain the address, return
                if (!addresses2.Contains(check)) return false;

                // Remove address from list
                addresses2.Remove(check);
            }

            return true;
        }
        
        public bool Equals(Rule rule)
        {
            return Name == rule.Name &&
                Description == rule.Description &&
                ApplicationName == rule.ApplicationName &&
                ServiceName == rule.ServiceName &&
                intProtocol == rule.intProtocol &&
                PortsEqual(LocalPorts, rule.LocalPorts) &&
                PortsEqual(RemotePorts, rule.RemotePorts) &&
                AddressesEqual(LocalAddresses, rule.LocalAddresses) &&
                AddressesEqual(RemoteAddresses, rule.RemoteAddresses) &&
                Enabled == rule.Enabled &&
                Group == rule.Group &&
                Profile == rule.Profile &&
                Action == rule.Action;
        }

        public static Rule Parse(BinaryReader reader)
        {
            string name = reader.ReadString();

            Rule rule = new Rule(name)
            {
                Description = reader.ReadString(),
                ApplicationName = reader.ReadString(),
                ServiceName = reader.ReadString(),
                intProtocol = reader.ReadInt32(),
                LocalPorts = reader.ReadString(),
                RemotePorts = reader.ReadString(),
                LocalAddresses = reader.ReadString(),
                RemoteAddresses = reader.ReadString(),
                Enabled = reader.ReadBoolean(),
                Group = reader.ReadString(),
                Profile = (NET_FW_PROFILE_TYPE2_)reader.ReadInt32(),
                Action = (NET_FW_ACTION_)reader.ReadInt32(),
                IsScored = reader.ReadBoolean(),
            };

            return rule;
        }
    }
}
