using Scoring_Report.Configuration;
using Scoring_Report.Configuration.SecOptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionOther : ISection
    {
        public ESectionType Type => ESectionType.Other;

        public string Header => TranslationManager.Translate("SectionOther");

        public static RegistryComboBox RemoteDesktopStatus = null;

        public static bool HostFileScored = false;

        public int MaxScore()
        {
            int max = 0;

            if (RemoteDesktopStatus.IsScored) max++;
            if (HostFileScored) max++;

            return max;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            if (RemoteDesktopStatus.IsScored)
            {
                object sectionValue = RegistryManager.GetValue(RemoteDesktopStatus.Key, RemoteDesktopStatus.ValueName);
                if (sectionValue != null)
                {
                    int value = (int)sectionValue;

                    if (value == RemoteDesktopStatus.SelectedIndex)
                    {
                        details.Points++;
                        details.Output.Add(TranslationManager.Translate("RemoteDesktop", RemoteDesktopStatus.SelectedItem));
                    }
                }
            }

            if (HostFileScored)
            {
                bool emptyOrDefault = true;

                // Gets host file path from System32 folder
                string hostFilePath = Path.Combine(Environment.SystemDirectory, @"drivers\etc\hosts");

                // If file exists, check it
                if (File.Exists(hostFilePath))
                {
                    // Read all lines of file
                    string[] lines = File.ReadAllLines(hostFilePath);

                    // Loop over each line
                    foreach (string line in lines)
                    {
                        // If line is not a comment and is not whitespace
                        if (!line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                        {
                            // If line does not contain either of the default entries
                            if (!line.Contains("127.0.0.1       localhost") &&
                                !line.Contains("::1             localhost"))
                            {
                                // File is not empty/default
                                emptyOrDefault = false;
                                break;
                            }
                        }
                    }
                }

                // If file is empty/default, give points
                if (emptyOrDefault)
                {
                    details.Points++;
                    details.Output.Add(TranslationManager.Translate("HostFile"));
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Load remote desktop info
            RemoteDesktopStatus = new RegistryComboBox();
            RemoteDesktopStatus.Parse(reader);

            // Load host file scoring status
            HostFileScored = reader.ReadBoolean();
        }
    }
}
