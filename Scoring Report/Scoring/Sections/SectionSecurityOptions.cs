using Scoring_Report.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scoring_Report.Configuration.SecOptions;
using System.Text.RegularExpressions;
using Scoring_Report.Policies;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionSecurityOptions : ISection
    {
        public string Header => "Security Options:";

        public const string Format = "'{0}' set correctly - {1}";

        public int MaxScore()
        {
            return ConfigurationManager.SecurityOptions.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Loop over every security option configured
            foreach (ISecurityOption secOption in ConfigurationManager.SecurityOptions)
            {
                switch (secOption.Type)
                {
                    case ESecurityOptionType.RegistryComboBox:
                        {
                            RegistryComboBox option = (RegistryComboBox)secOption;

                            // Get registry value
                            object systemValue = RegistryManager.GetValue(option.Key, option.ValueName);

                            // If value exists
                            if (systemValue != null)
                            {
                                // We use .ToString() because some indexed values from secpol are strings and some are integers.
                                // Instead of individualizing each policy, we use this to generalize them
                                if (option.SelectedIndex.ToString() == systemValue.ToString())
                                {
                                    details.Points++;
                                    details.Output.Add(string.Format(Format, option.Header, option.SelectedItem));
                                }
                            }
                        }
                        break;
                    case ESecurityOptionType.RegistryTextRegex:
                        {
                            RegistryTextRegex option = (RegistryTextRegex)secOption;

                            // Get registry value
                            string systemText = RegistryManager.GetValue(option.Key, option.ValueName) as string;

                            // If value exists
                            if (systemText != null)
                            {
                                // Removes null characters. Some strings return \0 at the end (end character)
                                systemText = systemText.Replace("\0", "");

                                // Create regular expression
                                Regex regex = new Regex(option.Regex);

                                // Search for match
                                bool matches = regex.IsMatch(systemText);

                                // If match status equals expected match status
                                if (matches == option.RegexMatches)
                                {
                                    details.Points++;
                                    details.Output.Add(string.Format(Format, option.Header, "\"" + systemText + "\""));
                                }
                            }
                            break;
                        }
                    case ESecurityOptionType.RegistryRange:
                        {
                            RegistryRange option = (RegistryRange)secOption;

                            // Get registry value
                            object systemValue = RegistryManager.GetValue(option.Key, option.ValueName);

                            // If value exists
                            if (systemValue != null)
                            {
                                int value = Convert.ToInt32(systemValue);

                                // If value is within bounds of range
                                if (value >= option.Minimum && value <= option.Maximum)
                                {
                                    details.Points++;
                                    details.Output.Add(string.Format(Format, option.Header, value));
                                }
                            }

                            break;
                        }
                    case ESecurityOptionType.RegistryMultiLine:
                        {
                            RegistryMultiLine option = (RegistryMultiLine)secOption;

                            // Get registry value
                            string[] systemValue = RegistryManager.GetValue(option.Key, option.ValueName) as string[];

                            // If value exists
                            if (systemValue != null)
                            {
                                // If setting and desired config have same number of lines
                                if (systemValue.Length == option.Lines.Length)
                                {
                                    bool areEqual = true;

                                    // Check if each line in system setting equals one of the desired config
                                    foreach (string systemStr in systemValue)
                                    {
                                        List<string> desired = new List<string>(option.Lines);

                                        string match = null;
                                        bool foundMatch = false;

                                        foreach (string desiredStr in desired)
                                        {
                                            // Found match
                                            if (systemStr == desiredStr)
                                            {
                                                match = desiredStr;
                                                foundMatch = true;

                                                // Stop searching
                                                break;
                                            }
                                        }

                                        // If match was found, remove matched string for optimization
                                        if (foundMatch)
                                        {
                                            desired.Remove(match);
                                        }
                                        else
                                        {
                                            // Match not found, no points awarded
                                            areEqual = false;
                                            break;
                                        }
                                    }

                                    // If every string had a match
                                    if (areEqual)
                                    {
                                        details.Points++;
                                        details.Output.Add(string.Format(Format, option.Header, string.Join(Environment.NewLine, systemValue)));
                                    }
                                }
                            }

                            break;
                        }
                    case ESecurityOptionType.SeceditComboBox:
                        {
                            SeceditComboBox option = (SeceditComboBox)secOption;

                            // Check if value exists
                            if (SecurityPolicyManager.SeceditWrapper.ParsedIniFile.ContainsKey(option.Section) &&
                                SecurityPolicyManager.SeceditWrapper.ParsedIniFile[option.Section].ContainsKey(option.Key))
                            {
                                // Get system value from secedit
                                string systemValue = SecurityPolicyManager.SeceditWrapper.ParsedIniFile[option.Section][option.Key];

                                // If system value and desired value are equal
                                if (option.SelectedIndex.ToString() == systemValue)
                                {
                                    details.Points++;
                                    details.Output.Add(string.Format(Format, option.Header, option.SelectedItem));
                                }
                            }
                            break;
                        }
                    case ESecurityOptionType.SeceditTextRegex:
                        {
                            SeceditTextRegex option = (SeceditTextRegex)secOption;

                            // Check if value exists
                            if (SecurityPolicyManager.SeceditWrapper.ParsedIniFile.ContainsKey(option.Section) &&
                                SecurityPolicyManager.SeceditWrapper.ParsedIniFile[option.Section].ContainsKey(option.Key))
                            {
                                // Get system value from secedit
                                string systemText = SecurityPolicyManager.SeceditWrapper.ParsedIniFile[option.Section][option.Key];

                                // Create regular expression
                                Regex regex = new Regex(option.Regex);

                                // Search for match
                                bool matches = regex.IsMatch(systemText);

                                // If match status equals expected match status
                                if (matches == option.RegexMatches)
                                {
                                    details.Points++;
                                    details.Output.Add(string.Format(Format, option.Header, systemText));
                                }
                            }
                            break;
                        }
                }
            }

            return details;
        }
    }
}
