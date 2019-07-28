using Scoring_Report.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionOther : ISection
    {
        public string Header => "Other";

        public static class Format
        {
            public const string RemoteDesktop = "Remote Desktop allowance set correctly - {0}";
        }

        public int MaxScore()
        {
            int max = 0;

            if (ConfigurationManager.RemoteDesktopStatus.IsScored) max++;

            return max;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            if (ConfigurationManager.RemoteDesktopStatus.IsScored)
            {
                object sectionValue = RegistryManager.GetValue(ConfigurationManager.RemoteDesktopStatus.Key, ConfigurationManager.RemoteDesktopStatus.ValueName);
                if (sectionValue != null)
                {
                    int value = (int)sectionValue;

                    if (value == ConfigurationManager.RemoteDesktopStatus.SelectedIndex)
                    {
                        details.Points++;
                        details.Output.Add(string.Format(Format.RemoteDesktop, ConfigurationManager.RemoteDesktopStatus.SelectedItem));
                    }
                }
            }

            return details;
        }
    }
}
