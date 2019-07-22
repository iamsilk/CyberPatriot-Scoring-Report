using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scoring_Report.Configuration;

namespace Scoring_Report.Policies
{
    public class SeceditWrapper
    {
        public readonly string SeceditOutputFile = Path.Combine(ConfigurationManager.DefaultConfigDirectory, "secpol.inf");

        public Process Secedit { get; }

        public SeceditWrapper()
        {
            // Initializes hidden process with necessary arguments
            Secedit = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "secedit",
                    Arguments = "/export /quiet /cfg \"" + SeceditOutputFile + "\"",
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
        }

        public Dictionary<string, Dictionary<string, string>> ParsedIniFile = new Dictionary<string, Dictionary<string, string>>();

        public void LoadPolicy()
        {
            // using statement to dispose of process after usage
            using (Secedit)
            {
                // Start secedit process
                Secedit.Start();

                // Synchronously wait for exit
                Secedit.WaitForExit();
            }

            // If output file doesn't exist, return
            if (!File.Exists(SeceditOutputFile)) return;

            // Get file contents in format of string array
            string[] lines = File.ReadAllLines(SeceditOutputFile);

            // Delete file immediately after reading contents a.k.a. hiding our tracks
            File.Delete(SeceditOutputFile);

            // Parse ini file data
            parseIniFile(lines);
        }

        private void parseIniFile(string[] lines)
        {
            // Clear already parsed ini file data
            ParsedIniFile.Clear();
            
            // Specify current section
            string currentSection = "";

            // For each line in ini file
            foreach (string line in lines)
            {
                // If line starts with '[', it is a section
                if (line.StartsWith("["))
                {
                    int length = line.Length - 1;

                    // Line should end with a ']'
                    // But if it doesn't, we don't want to halt the entire program
                    if (line.EndsWith("]")) length--;

                    // Get section of line with section name
                    string sectionName = line.Substring(1, length);

                    // Set current section to newly parsed
                    currentSection = sectionName;

                    // Check/add section to parsed data
                    if (!ParsedIniFile.ContainsKey(currentSection))
                    {
                        ParsedIniFile.Add(currentSection, new Dictionary<string, string>());
                    }
                }
                else
                {
                    // If line doesn't start with [, it is a key/value pair
                    int index = line.IndexOf('=');

                    // If line contains no '=', skip as it's invalid
                    if (index < 0) continue;

                    // Get key from line and trim whitespace
                    string key = line.Substring(0, index).Trim();

                    string value = "";

                    // If '=' character is last character, the value is empty
                    // We check for the opposite to see if there is a value
                    if (line.Length - 1 != index)
                    {
                        // Get value from line and trim whitespace
                        value = line.Substring(index + 1).Trim();
                    }

                    // Add key/value pair to parsed ini file data
                    ParsedIniFile[currentSection].Add(key, value);
                }
            }
        }
    }
}
