using Scoring_Report.Configuration;
using Scoring_Report.Configuration.CustomFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionCustomFiles : ISection
    {
        public string Header => TranslationManager.Translate("SectionCustomFiles");

        public ESectionType Type => ESectionType.CustomFiles;

        public List<CustomFile> CustomFiles { get; } = new List<CustomFile>();

        public int MaxScore()
        {
            return CustomFiles.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Loop over each custom file
            foreach (CustomFile customFile in CustomFiles)
            {
                try
                {
                    // If file doesn't exist, skip
                    if (!File.Exists(customFile.Path)) continue;

                    // Get file contents
                    string contents = File.ReadAllText(customFile.Path);

                    bool equals = false;

                    // For each comparison
                    foreach (IComparison comparison in customFile.Comparisons)
                    {
                        // If comparison equals, set as so
                        if (comparison.Equals(contents))
                        {
                            equals = true;
                        }
                    }

                    // If value equals status matches scoring, give points
                    if (customFile.Matches == equals)
                    {
                        details.Points++;
                        details.Output.Add(TranslationManager.Translate("CustomFilesCustomOutput", customFile.CustomOutput));
                    }
                }
                catch { }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Clear custom files
            CustomFiles.Clear();

            // Get number of custom files
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get custom file
                CustomFile customFile = CustomFile.Parse(reader);

                // If custom file is scored, add custom file to list
                if (customFile.IsScored)
                    CustomFiles.Add(customFile);
            }
        }
    }
}
