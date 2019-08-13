using Scoring_Report.Configuration;
using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Scoring.Output
{
    public static class OutputManager
    {
        public static OutputInstance Defaults { get; private set; }

        public static List<OutputInstance> Outputs { get; } = new List<OutputInstance>();

        public static void Setup()
        {
            // Setup default output
            Defaults = new OutputInstance(
                Path.Combine(ConfigurationManager.DefaultConfigDirectory, "Scoring Report.html"));

            // Clears current list of outputs. Done incase setup is called more than once
            Outputs.Clear();

            // For every output file loaded in configuration
            foreach (string outputFile in ConfigurationManager.OutputFiles)
            {
                // Get instance of output
                OutputInstance output = new OutputInstance(outputFile);

                // Add instance to global list
                Outputs.Add(output);
            }
        }

        public static void OutputToAll(List<SectionDetails> details)
        {
            // For each output instance
            foreach (OutputInstance instance in Outputs)
            {
                // Output details
                instance.Output(details);
            }
        }
    }
}
