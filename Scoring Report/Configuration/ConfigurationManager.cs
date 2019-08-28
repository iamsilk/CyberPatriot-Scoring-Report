using Scoring_Report.Configuration.Startup;
using Scoring_Report.Scoring;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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
                loadConfig(true);
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

        private static string Key = "cH4N63th!S!!1!}~";
        private static string IV = "7wwkEANRXQJr2Uxs";

        private static void loadConfig(bool loadedPrev = false)
        {
            // If file/directory doesn't exist
            if (!checkFiles())
            {
                // Stop loading configuration
                return;
            }

            // If config has been loaded already
            if (loadedPrev)
            {
                // Resetup the scoring manager to clear current configs,
                // We need to clear current configs incase the newly loaded config file has had a section removed
                // If this is the case, unless we clear current configs, the section that was removed will persist
                // through the new loading
                ScoringManager.Setup();
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
                    // Create aes managed class for creating decryptor
                    AesManaged aesManaged = new AesManaged();
                    aesManaged.KeySize = 256;
                    aesManaged.Mode = CipherMode.CBC;
                    aesManaged.Padding = PaddingMode.PKCS7;
                    aesManaged.Key = Encoding.ASCII.GetBytes(Key);
                    aesManaged.IV = Encoding.ASCII.GetBytes(IV);
                    ICryptoTransform decryptor = aesManaged.CreateDecryptor();

                    // Crypto stream to decrypt config file
                    using (CryptoStream cryptoStream = new CryptoStream(ConfigFileStream, decryptor, CryptoStreamMode.Read))
                    // Binary reader for parsing of data
                    using (BinaryReader reader = new BinaryReader(cryptoStream))
                    {
                        loadOutputFiles(reader);

                        // Get number of sections
                        int count = reader.ReadInt32();

                        // For each config section, load it
                        for (int i = 0; i < count; i++)
                        {
                            // Get config section type
                            ESectionType type = (ESectionType)reader.ReadInt32();

                            // Get length of data for specific section
                            int bufferLength = reader.ReadInt32();

                            // Read buffer for section to isolate from others
                            byte[] buffer = reader.ReadBytes(bufferLength);

                            // Find section for loading
                            ISection config = ScoringManager.ScoringSections.FirstOrDefault(x => x.Type == type);

                            // Possibly removed section, skip it
                            if (config == null) continue;

                            // Create isolated binary reader for section loading
                            using (MemoryStream bufferStream = new MemoryStream(buffer))
                            using (BinaryReader sectionReader = new BinaryReader(bufferStream))
                            {
                                try
                                {
                                    // Load config
                                    config.Load(sectionReader);
                                }
                                catch
                                {
                                    // Issue with specific section, likely something was added to section
                                    // But it's an outdated config file
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Likely outdated configuration file with updated program. No action is likely needed.
                    // TODO, add version checking to allow outdated configuration files
                }

                LoadedConfigFromFile = true;
            }
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
