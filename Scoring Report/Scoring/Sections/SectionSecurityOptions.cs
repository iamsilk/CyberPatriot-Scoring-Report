using Scoring_Report.Configuration;
using Scoring_Report.Configuration.SecOptions;
using Scoring_Report.Policies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionSecurityOptions : ISection
    {
        public ESectionType Type => ESectionType.SecurityOptions;

        public string Header => "Security Options:";

        public static List<ISecurityOption> SecurityOptions { get; } = new List<ISecurityOption>();

        public int MaxScore()
        {
            return SecurityOptions.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Loop over every security option configured
            foreach (ISecurityOption secOption in SecurityOptions)
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
                                    details.Output.Add(TranslationManager.Translate("SecurityOptions", option.Header, option.SelectedItem));
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
                                    details.Output.Add(TranslationManager.Translate("SecurityOptions", option.Header, "\"" + systemText + "\""));
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
                                    details.Output.Add(TranslationManager.Translate("SecurityOptions", option.Header, value));
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
                                        details.Output.Add(TranslationManager.Translate("SecurityOptions", option.Header, string.Join(Environment.NewLine, systemValue)));
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
                                    details.Output.Add(TranslationManager.Translate("SecurityOptions", option.Header, option.SelectedItem));
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
                                    details.Output.Add(TranslationManager.Translate("SecurityOptions", option.Header, systemText));
                                }
                            }
                            break;
                        }
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Clear current storage
            SecurityOptions.Clear();

            // Get number of security option settings
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get type of sec option
                ESecurityOptionType type = (ESecurityOptionType)reader.ReadInt32();

                ISecurityOption secOption = null;

                // Initialize secOption based on type
                switch (type)
                {
                    case ESecurityOptionType.RegistryComboBox:
                        secOption = new RegistryComboBox();
                        break;
                    case ESecurityOptionType.RegistryTextRegex:
                        secOption = new RegistryTextRegex();
                        break;
                    case ESecurityOptionType.RegistryRange:
                        secOption = new RegistryRange();
                        break;
                    case ESecurityOptionType.RegistryMultiLine:
                        secOption = new RegistryMultiLine();
                        break;
                    case ESecurityOptionType.SeceditComboBox:
                        secOption = new SeceditComboBox();
                        break;
                    case ESecurityOptionType.SeceditTextRegex:
                        secOption = new SeceditTextRegex();
                        break;
                }

                if (secOption == null)
                {
                    // Uh oh.. corrupted file?
                    throw new Exception("Unknown security option type. Possible configuration file corruption.");
                }

                // Parse information based on type
                secOption.Parse(reader);

                // Only store scored options
                if (secOption.IsScored)
                {
                    SecurityOptions.Add(secOption);
                }
            }
        }
    }
}
