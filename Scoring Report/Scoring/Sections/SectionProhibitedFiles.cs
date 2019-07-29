using Microsoft.Win32;
using Scoring_Report.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring.Sections
{
    class SectionProhibitedFiles : ISection
    {
        public ESectionType Type => ESectionType.ProhibitedFiles;

        public string Header => "Prohibited Files:";

        public static List<string> ProhibitedFiles { get; } = new List<string>();

        //
        // {0} - File Location
        //
        public const string Format = "File '{0}' has been deleted";

        public int MaxScore()
        {
            return ProhibitedFiles.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Loop over all prohibited files
            foreach (string filelocation in ProhibitedFiles)
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

        public void Load(BinaryReader reader)
        {
            ProhibitedFiles.Clear();

            // Get count of prohibited files
            int filecount = reader.ReadInt32();

            for (int i = 0; i < filecount; i++)
            {
                // Get File Location
                string fileLocation = reader.ReadString();

                ProhibitedFiles.Add(fileLocation);
            }
        }
    }
}
