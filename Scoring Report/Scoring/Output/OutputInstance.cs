using System;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Output
{
    public class OutputInstance
    {
        public OutputInstance Defaults => OutputManager.Defaults;

        public string OutputFile { get; }

        public string FormatDetailsDirectory => OutputFile + "_format";

        public string MainFormatFile => FormatDetailsDirectory + "\\format.txt";

        public string SectionHeadPrefix => FormatDetailsDirectory + "\\sectionhead_prefix.txt";

        public string SectionHeadSuffix => FormatDetailsDirectory + "\\sectionhead_suffix.txt";

        public string SectionTextPrefix => FormatDetailsDirectory + "\\sectiontext_prefix.txt";

        public string SectionTextSuffix => FormatDetailsDirectory + "\\sectiontext_suffix.txt";

        public string SectionTextNewLine => FormatDetailsDirectory + "\\sectiontext_newline.txt";

        public string SectionNothing => FormatDetailsDirectory + "\\section_nothing.txt";

        public OutputInstance(string outputFile)
        {
            OutputFile = outputFile;
        }

        public string GetMainFormatFile()
        {
            return CheckFile(MainFormatFile, Defaults.MainFormatFile);
        }

        public string GetSectionHeadPrefix()
        {
            return CheckFile(SectionHeadPrefix, Defaults.SectionHeadPrefix);
        }

        public string GetSectionHeadSuffix()
        {
            return CheckFile(SectionHeadSuffix, Defaults.SectionHeadSuffix);
        }

        public string GetSectionTextPrefix()
        {
            return CheckFile(SectionTextPrefix, Defaults.SectionTextPrefix);
        }

        public string GetSectionTextSuffix()
        {
            return CheckFile(SectionTextSuffix, Defaults.SectionTextSuffix);
        }

        public string GetSectionTextNewLine()
        {
            return CheckFile(SectionTextNewLine, Defaults.SectionTextNewLine);
        }

        public string GetSectionNothing()
        {
            return CheckFile(SectionNothing, Defaults.SectionNothing);
        }

        /* Function setup to read backup from file because backup will
         * often be the default file. Path instead of text is given
         * as we do not want to waste time reading the file if the
         * specified path already exists. */
        public string CheckFile(string path, string fallbackFile)
        {
            // If the file exists
            if (File.Exists(path))
            {
                // return the file's text
                return File.ReadAllText(path);
            }
            else
            {
                // If not, read the fallback file's text
                string fallback = File.ReadAllText(fallbackFile);

                // Write the text to the path we are checking
                File.WriteAllText(path, fallback);

                // And return fallback
                return fallback;
            }
        }

        public void Output(List<SectionDetails> details)
        {
            try
            {
                // Attempt getting write access to file
                using (StreamWriter writer = new StreamWriter(OutputFile))
                {
                    // Get all dependent section formatting details
                    string mainFormat = GetMainFormatFile();
                    string headPrefix = GetSectionHeadPrefix();
                    string headSuffix = GetSectionHeadSuffix();
                    string textPrefix = GetSectionTextPrefix();
                    string textSuffix = GetSectionTextSuffix();
                    string textNewLine = GetSectionTextNewLine();
                    string nothing = GetSectionNothing();

                    // Within formatting details, specified as {0}
                    // Total score accumulated
                    int totalScore = 0;

                    // Within formatting details, specified as {1}
                    int maxScore = ScoringManager.MaxScore;
                    
                    // Within formatting details, specified as {2}
                    // Score percentage, calculated later
                    float scorePercent;

                    // Within formatting details, specified as {3}
                    // Current time
                    string lastUpdated = DateTime.Now.ToString("MMMM dd yyyy - HH:mm:ss \"GMT\"zzz");

                    // Within formatting details, specified as {4}
                    // Main output placed in sections area
                    string mainOutput = "";

                    // For each section details
                    foreach (SectionDetails secDetails in details)
                    {
                        // Add score for section to total
                        totalScore += secDetails.Points;

                        // If there isn't output for this section, skip
                        if (secDetails.Output.Count == 0) continue;

                        // Add prefix, header, then suffix
                        mainOutput += headPrefix;
                        mainOutput += secDetails.Section.Header;
                        mainOutput += headSuffix;

                        // Add section text prefix
                        mainOutput += textPrefix;

                        // For each line index
                        for (int i = 0; i < secDetails.Output.Count; i++)
                        {
                            // Add output line to main output
                            mainOutput += secDetails.Output[i];

                            // If this line isn't the last
                            if (i != secDetails.Output.Count - 1)
                            {
                                // Prepare for next line
                                mainOutput += textNewLine;
                            }
                        }

                        // Add section text suffix
                        mainOutput += textSuffix;
                    }

                    // Calculated score percent
                    scorePercent = (float)totalScore / (float)maxScore * 100;

                    writer.Write(string.Format(mainFormat, totalScore, maxScore, scorePercent, lastUpdated, mainOutput));
                }
            }
            catch (IOException)
            {
                // Cannot gain access to the file
                return;
            }
        }
    }
}
