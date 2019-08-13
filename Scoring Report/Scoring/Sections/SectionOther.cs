using Scoring_Report.Configuration;
using Scoring_Report.Configuration.SecOptions;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionOther : ISection
    {
        public ESectionType Type => ESectionType.Other;

        public string Header => "Other";

        public static RegistryComboBox RemoteDesktopStatus = null;

        public int MaxScore()
        {
            int max = 0;

            if (RemoteDesktopStatus.IsScored) max++;

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
                        details.Output.Add(ConfigurationManager.Translate("RemoteDesktop", RemoteDesktopStatus.SelectedItem));
                    }
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Load remote desktop info
            RemoteDesktopStatus = new RegistryComboBox();
            RemoteDesktopStatus.Parse(reader);
        }
    }
}
