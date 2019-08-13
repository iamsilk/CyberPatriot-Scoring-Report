using Configuration_Tool.Configuration.Startup;
using Configuration_Tool.Controls;
using System.Collections.Generic;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigStartup : IConfig
    {
        public MainWindow MainWindow { get; set; }

        public EConfigType Type => EConfigType.Startup;

        public void Load(BinaryReader reader)
        {
            // Clear startup infos
            ConfigurationManager.StartupInfos.Clear();

            // Populate startup infos
            MainWindow.PopulateStartupInfos();

            // Get number of startup infos to load
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get startup info
                StartupInfo loadedInfo = StartupInfo.Parse(reader);

                // If is not scored, we don't load to remove clutter
                // If startup is still apart of the system, it will be loaded
                // If we are loading defaults, do not skip and instead load
                if (!ConfigurationManager.LoadingDefaults && !loadedInfo.IsScored) continue;

                // Get string of startup info for comparisons
                string infoString = loadedInfo.ToString();

                bool matchFound = false;

                // Search for match
                foreach (StartupInfo info in ConfigurationManager.StartupInfos)
                {
                    // If match is found
                    if (infoString == info.ToString())
                    {
                        matchFound = true;

                        // Set scoring status to loaded status
                        info.IsScored = loadedInfo.IsScored;
                    }
                }

                // If no match was found, add it to the list
                if (!matchFound) ConfigurationManager.StartupInfos.Add(loadedInfo);
            }
        }

        public void Save(BinaryWriter writer)
        {
            List<StartupInfo> list = new List<StartupInfo>();

            // Loop over each startup info
            foreach (StartupInfo info in ConfigurationManager.StartupInfos)
            {
                // If is scored, add to list
                if (info.IsScored) list.Add(info);
            }

            // Write number of startup infos
            writer.Write(list.Count);

            // Write each startup info
            foreach (StartupInfo info in list) info.Write(writer);
        }
    }
}
