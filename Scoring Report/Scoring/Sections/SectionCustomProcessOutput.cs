using Scoring_Report.Configuration;
using Scoring_Report.Configuration.Comparisons;
using Scoring_Report.Configuration.CustomProcessOutput;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionCustomProcessOutput : ISection
    {
        public string Header => TranslationManager.Translate("SectionCustomProcessOutput");

        public ESectionType Type => ESectionType.CustomProcessOutput;

        public List<ProcessOutput> ProcessOutputs { get; } = new List<ProcessOutput>();

        public int MaxScore()
        {
            return ProcessOutputs.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Loop over each process output
            foreach (ProcessOutput processOutput in ProcessOutputs)
            {
                int exitCode;
                string output;
                
                // If process ran properly
                if (processOutput.RunProcess(out exitCode, out output))
                {
                    bool equals = false;

                    // For each comparison
                    foreach (IComparison comparison in processOutput.Comparisons)
                    {
                        string value = output;

                        // Comparison range needs exit code
                        if (comparison is ComparisonRange) value = exitCode.ToString();

                        // If comparison matches, set as so
                        if (comparison.Equals(value))
                        {
                            equals = true;
                        }
                    }

                    if (processOutput.Matches == equals)
                    {
                        details.Points++;
                        details.Output.Add(TranslationManager.Translate("ProcessCustomOutput", processOutput.CustomOutput));
                    }
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Clear custom process outputs
            ProcessOutputs.Clear();

            // Get number of custom process outputs
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get process output details
                ProcessOutput processOutput = ProcessOutput.Parse(reader);

                // Add details to list if scored
                if (processOutput.IsScored)
                    ProcessOutputs.Add(processOutput);
            }
        }
    }
}
