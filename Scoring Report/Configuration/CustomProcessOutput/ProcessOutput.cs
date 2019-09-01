using Scoring_Report.Configuration.Comparisons;
using System;
using System.Diagnostics;
using System.IO;

namespace Scoring_Report.Configuration.CustomProcessOutput
{
    public class ProcessOutput
    {
        public string CustomOutput = "";

        public string Path = "";

        public string Arguments = "";

        public int Timeout = 0;

        public bool Matches = true;

        public IComparison[] Comparisons = null;

        public bool IsScored = false;

        public static ProcessOutput Parse(BinaryReader reader)
        {
            // Create instance of policy storage
            ProcessOutput processOutput = new ProcessOutput();

            processOutput.Path = reader.ReadString();
            processOutput.Arguments = reader.ReadString();
            processOutput.Timeout = reader.ReadInt32();

            // Read comparison values
            processOutput.Matches = reader.ReadBoolean();

            int comparisonCount = reader.ReadInt32();

            processOutput.Comparisons = new IComparison[comparisonCount];

            for (int i = 0; i < comparisonCount; i++)
            {
                EComparison type = (EComparison)reader.ReadInt32();
                switch (type)
                {
                    case EComparison.Simple:
                        processOutput.Comparisons[i] = new ComparisonSimple();
                        break;
                    case EComparison.Regex:
                        processOutput.Comparisons[i] = new ComparisonRegex();
                        break;
                    case EComparison.Range:
                        processOutput.Comparisons[i] = new ComparisonRange();
                        break;
                }

                processOutput.Comparisons[i].Load(reader);
            }

            // Read scoring values
            processOutput.CustomOutput = reader.ReadString();
            processOutput.IsScored = reader.ReadBoolean();

            return processOutput;
        }

        private string processOutput = "";
        
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            processOutput += e.Data + Environment.NewLine;
        }

        public bool RunProcess(out int exitCode, out string output)
        {
            exitCode = 0;
            output = "";

            processOutput = "";
            
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo(Path, Arguments)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                process.OutputDataReceived += Process_OutputDataReceived;

                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    
                    if (Timeout == 0)
                    {
                        process.WaitForExit();
                    }
                    else if (!process.WaitForExit(Timeout))
                    {
                        // Process did not return in time
                        return false;
                    }

                    exitCode = process.ExitCode;
                    output = processOutput.Trim();

                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}
