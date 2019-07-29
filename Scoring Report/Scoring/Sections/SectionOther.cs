using Scoring_Report.Configuration;
using Scoring_Report.Configuration.SecOptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionOther : ISection
    {
        public ESectionType Type => ESectionType.Other;

        public string Header => "Other";

        public static RegistryComboBox RemoteDesktopStatus = null;

        public static class Format
        {
            public const string RemoteDesktop = "Remote Desktop allowance set correctly - {0}";
        }

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
                        details.Output.Add(string.Format(Format.RemoteDesktop, RemoteDesktopStatus.SelectedItem));
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
