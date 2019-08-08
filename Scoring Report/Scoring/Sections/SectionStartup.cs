using Scoring_Report.Configuration;
using Scoring_Report.Configuration.Startup;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionStartup : ISection
    {
        public string Header => "Startup:";

        public static List<StartupInfo> StartupInfos { get; } = new List<StartupInfo>();

        public ESectionType Type => ESectionType.Startup;

        public int MaxScore()
        {
            return StartupInfos.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            List<StartupInfo> systemStartups = new List<StartupInfo>();

            // Get current startup infos
            StartupInfo.GetStartupInfos(systemStartups);

            // Create list of strings to contain string representations of startups
            List<string> startups = new List<string>(systemStartups.Count);

            // For each system startup info
            foreach (StartupInfo info in systemStartups)
            {
                // Add string representation to list
                startups.Add(info.ToString());
            }

            // For each startup info config
            foreach (StartupInfo info in StartupInfos)
            {
                // Get string representation of startup info
                // Used to compare against others
                string infoString = info.ToString();

                // If string doesn't exist within list
                if (!startups.Contains(infoString))
                {
                    // Increase score and add to output
                    details.Points++;
                    details.Output.Add(ConfigurationManager.Translate("Startup", info.Owner, info.Name, info.Command, info.StartupTypeString, info.Location));
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Clear startup infos
            StartupInfos.Clear();

            // Get number of startup infos to load
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get startup info
                StartupInfo loadedInfo = StartupInfo.Parse(reader);

                // If it's scored, add to global list
                if (loadedInfo.IsScored)
                    StartupInfos.Add(loadedInfo);
            }
        }
    }
}
