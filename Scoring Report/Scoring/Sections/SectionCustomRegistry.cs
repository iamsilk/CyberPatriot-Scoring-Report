using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scoring_Report.Configuration.CustomRegistry;

namespace Scoring_Report.Scoring.Sections
{
    class SectionCustomRegistry : ISection
    {
        public ESectionType Type => ESectionType.Registry;

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
                registryKey.Value = registryKey.GetRegistryValue();

                if(registryKey.KeyEquals.IsScored)
                {
                    if (registryKey.Equalsbool) {
                        if (registryKey.Value == registryKey.KeyEqualsStr)
                        {
                            details.Points++;
                            details.Output.Add(TranslationManager.Translate("RegistryKeyCustomOutput", registryKey.customoutput));
                        }
                    }
                    else
                    {
                        if (registryKey.Value != registryKey.KeyEqualsStr)
                        {
                            details.Points++;
                            details.Output.Add(TranslationManager.Translate("RegistryKeyCustomOutput", registryKey.customoutput));
                        }
                    }
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

                RegistryKeysScored.Add(registryKey);
            }
        }
    }
}
