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

            foreach (string filelocation in ConfigurationManager.ProhibitedFiles)
            {
                bool fileExists = false;

                if(WinAPI.GetFileAttributes(filelocation) != -1)
                {
                    fileExists = true;
                }
                if(filelocation.ToUpper().Contains("C:\\Windows\\System32".ToUpper()))
                {
                    string sysnativeFile = filelocation.ToUpper().Replace("C:\\WINDOWS\\SYSTEM32", "C:\\WINDOWS\\SYSNATIVE");
                    if (WinAPI.GetFileAttributes(sysnativeFile) != -1)
                    {
                        fileExists = true;
                    }
                }
                if(!fileExists)
                {
                    details.Points++;
                    details.Output.Add(string.Format(Format, filelocation));
                }
            }
            return details;
        }
    }
}
