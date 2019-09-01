using Scoring_Report.Configuration;
using Scoring_Report.Configuration.CustomRegistry;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Sections
{
    class SectionCustomRegistry : ISection
    {
        public ESectionType Type => ESectionType.CustomRegistry;

        public string Header => TranslationManager.Translate("SectionCustomRegistry");

        public static List<RegistryKey> RegistryKeysScored { get; } = new List<RegistryKey>();

        public int MaxScore()
        {
           return RegistryKeysScored.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Loop over all score keys
            foreach (RegistryKey registryKey in RegistryKeysScored)
            {
                string strValue;

                // If can't get value, skip
                if (!registryKey.TryGetRegistryValue(out strValue)) continue;

                bool equals = false;

                // For each comparison
                foreach (IComparison comparison in registryKey.Comparisons)
                {
                    // If comparison equals, set as so
                    if (comparison.Equals(strValue))
                    {
                        equals = true;
                    }
                }

                // If value equals status matches scoring, give points
                if (registryKey.ValueEquals == equals)
                {
                    details.Points++;
                    details.Output.Add(TranslationManager.Translate("RegistryKeyCustomOutput", registryKey.CustomOutput));
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            RegistryKeysScored.Clear();

            // Get count of reg keys
            int regkeyscount = reader.ReadInt32();

            for (int i = 0; i < regkeyscount; i++)
            {
                RegistryKey registryKey = RegistryKey.Parse(reader);

                // Only add scored items
                if (registryKey.IsScored)
                    RegistryKeysScored.Add(registryKey);
            }
        }
    }
}
