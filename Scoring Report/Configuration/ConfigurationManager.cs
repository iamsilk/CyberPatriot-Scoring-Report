using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Scoring_Report.Configuration.Groups;
using Scoring_Report.Configuration.SecOptions;
using Scoring_Report.Configuration.UserRights;
using Scoring_Report.Scoring;

namespace Scoring_Report.Configuration
{
    public static class ConfigurationManager
    {
        public static string DefaultConfigDirectory { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;

        public const string DefaultConfigFile = "Configuration.dat";

        public static readonly string DefaultConfigPath = Path.Combine(DefaultConfigDirectory, DefaultConfigFile);

        public static string CurrentConfigDirectory { get; private set; } = "";

        public static string CurrentConfigPath { get; private set; } = "";

        public static bool LoadedConfigFromFile { get; private set; } = false;

        public static FileStream ConfigFileStream { get; private set; } = null;

        public static DateTime LastUpdated { get; private set; } = DateTime.MinValue;

        /* Reason for multiple output files support is in case
         * any users want to setup their own scripts/programs
         * for uploading/checking scoring without needing to
         * negotiate handles between possible competitors.
         * Most often, this is not necessary and another script
         * could just get read access alongside the viewer;
         * however, this is a worst case scenario implementation */

        public const string DefaultOutputFile = "Scoring Report.html";

        public static List<string> OutputFiles { get; } = new List<string>()
        {
            Path.Combine(DefaultConfigDirectory, DefaultOutputFile)
        };

        public static void Startup(string startupParameter)
        {
            CurrentConfigPath = startupParameter;

            // If no custom configuration path is specified
            if (string.IsNullOrWhiteSpace(CurrentConfigPath))
            {
                // Set current configuration directory/path as default
                CurrentConfigDirectory = DefaultConfigDirectory;
                CurrentConfigPath = DefaultConfigPath;
            }
            else
            {
                // Set current configuration directory based on given path
                CurrentConfigDirectory = Path.GetDirectoryName(CurrentConfigPath);
            }

            loadConfig();
        }

        public static void Loop()
        {
            // Get latest creation time of config file
            DateTime latestWriteTime = File.GetLastWriteTime(CurrentConfigPath);

            // If latest was after the last time we updated, load new config
            if (latestWriteTime > LastUpdated)
            {
                loadConfig();
            }
        }

        public static void LoadConfig(string configPath)
        {
            // If no config path is specified
            if (string.IsNullOrWhiteSpace(configPath))
            {
                // Stop loading non-existent configuration
                return;
            }

            CurrentConfigPath = configPath;
            CurrentConfigDirectory = Path.GetDirectoryName(configPath);

            loadConfig();
        }

        private static void loadConfig()
        {
            // If file/directory doesn't exist
            if (!checkFiles())
            {
                // Stop loading configuration
                return;
            }

            /* Open file with only read permissions.
             * We don't need write permissions as we want
             * as little file permission errors to occur
             * as possible. We do not need write permissions
             * until it is time to save */
            using (ConfigFileStream = File.Open(CurrentConfigPath, FileMode.Open, FileAccess.Read))
            {
                LastUpdated = File.GetLastWriteTime(CurrentConfigPath);

                try
                {
                    // Binary reader for parsing of data
                    using (BinaryReader reader = new BinaryReader(ConfigFileStream))
                    {
                        loadOutputFiles(reader);

                        // For each scoring section, load its config
                        foreach (ISection section in ScoringManager.ScoringSections)
                        {
                            section.Load(reader);
                        }
                    }
                }
                catch
                {
                    // Likely outdated configuration file with updated program. No action is likely needed.
                    // TODO, add version checking to allow outdated configuration files
                }
            }
        }

        private static bool checkFiles()
        {
            // If config file directory doesn't exist
            if (!Directory.Exists(CurrentConfigDirectory))
            {
                // Stop process of loading configuration
                return false;
            }

            // If file doesn't exist
            if (!File.Exists(CurrentConfigPath))
            {
                // Stop process of loading configuration
                return false;
            }

            return true;
        }

        private static void loadOutputFiles(BinaryReader reader)
        {
            // Clear current list of output files
            OutputFiles.Clear();

            // Get number of output files
            int count = reader.ReadInt32();

            // For each output file
            for (int i = 0; i < count; i++)
            {
                // Get output file
                string file = reader.ReadString();

                // Add output file to list
                OutputFiles.Add(file);
            }
        }
    }
}
