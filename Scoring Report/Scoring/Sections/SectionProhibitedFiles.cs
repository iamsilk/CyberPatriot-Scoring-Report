using Microsoft.Win32;
using Scoring_Report.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring.Sections
{
    class SectionProhibitedFiles : ISection
    {
        public string Header => "Prohibited Files:";

        //
        // {0} - File Location
        //
        public const string Format = "File '{0}' has been deleted";

        public int MaxScore()
        {
            return ConfigurationManager.ProhibitedFiles.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Loop over all prohibited files
            foreach (string filelocation in ConfigurationManager.ProhibitedFiles)
            {
                // Get attributes of files, if -1 is returned, no file/directory/ADS was found
                if(WinAPI.GetFileAttributes(filelocation) == -1)
                {
                    details.Points++;
                    details.Output.Add(string.Format(Format, filelocation));
                }
            }

            return details;
        }
    }
}
