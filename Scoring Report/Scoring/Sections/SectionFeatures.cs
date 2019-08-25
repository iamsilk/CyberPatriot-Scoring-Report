using Scoring_Report.Configuration;
using Scoring_Report.Configuration.Features;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionFeatures : ISection
    {
        public string Header => TranslationManager.Translate("SectionFeatures");

        public ESectionType Type => ESectionType.Features;

        public static List<WindowsFeature> ScoredFeatures { get; } = new List<WindowsFeature>();

        public int MaxScore()
        {
            return ScoredFeatures.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Get windows features on system
            List<WindowsFeature> systemFeatures = WindowsFeature.GetWindowsFeatures();

            // For each configured windows feature
            foreach (WindowsFeature scoredFeature in ScoredFeatures)
            {
                // For each system windows feature
                foreach (WindowsFeature systemFeature in systemFeatures)
                {
                    // If features have the same name (same features)
                    if (scoredFeature.Name == systemFeature.Name)
                    {
                        // If installation statuses are equal, give points
                        if (scoredFeature.Installed == systemFeature.Installed)
                        {
                            details.Points++;

                            if (scoredFeature.Installed)
                            {
                                details.Output.Add(TranslationManager.Translate("WindowsFeatureInstalled", scoredFeature.Name));
                            }
                            else
                            {
                                details.Output.Add(TranslationManager.Translate("WindowsFeatureNotInstalled", scoredFeature.Name));
                            }
                        }

                        // Found match, break loop
                        break;
                    }
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Get number of features
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get feature info
                WindowsFeature feature = WindowsFeature.Parse(reader);

                // If feature isn't scored, skip
                if (!feature.IsScored) continue;

                // Add feature to scored list
                ScoredFeatures.Add(feature);
            }
        }
    }
}
