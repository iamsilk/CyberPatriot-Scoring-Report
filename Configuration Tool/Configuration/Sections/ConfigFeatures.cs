using Configuration_Tool.Configuration.Features;
using Configuration_Tool.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigFeatures : IConfig
    {
        public EConfigType Type => EConfigType.Features;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Load system's features
            WindowsFeature.GetWindowsFeatures(ConfigurationManager.Features);

            // Get number of features
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get feature info
                WindowsFeature feature = WindowsFeature.Parse(reader);

                // Search for feature in list
                WindowsFeature match = ConfigurationManager.Features.FirstOrDefault(x => x.Name == feature.Name);

                // If match was found
                if (match != null)
                {
                    // Copy info to shown WindowsFeature
                    match.Installed = feature.Installed;
                    match.IsScored = feature.IsScored;
                }
                else
                {
                    // Add feature to list
                    ConfigurationManager.Features.Add(feature);
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Get scored features
            IEnumerable<WindowsFeature> scoredFeatures = ConfigurationManager.Features.Where(x => x.IsScored);

            // Write count of scored features
            writer.Write(scoredFeatures.Count());

            // Write each scored feature
            foreach (WindowsFeature feature in scoredFeatures)
                feature.Write(writer);
        }
    }
}
