using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Scoring_Report.Configuration.Groups;

namespace Scoring_Report.Configuration
{
    public static class ConfigurationManager
    {
        public const string DefaultConfigDirectory = @"C:\CyberPatriot Scoring Report";

        public const string DefaultConfigFile = "Configuration.dat";

        public static readonly string DefaultConfigPath = Path.Combine(DefaultConfigDirectory, DefaultConfigFile);

        public static string CurrentConfigDirectory { get; private set; } = "";

        public static string CurrentConfigPath { get; private set; } = "";

        public static bool LoadedConfigFromFile { get; private set; } = false;

        public static FileStream ConfigFileStream { get; private set; } = null;

        public static List<UserSettings> Users { get; } = new List<UserSettings>();

        public static List<GroupSettings> Groups { get; } = new List<GroupSettings>();

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
                // Binary reader for parsing of data
                using (BinaryReader reader = new BinaryReader(ConfigFileStream))
                {
                    loadUserSettings(reader);

                    loadGroupSettings(reader);
                }
            }
        }

        public static void Save(string configPath = "")
        {
            // If path is specified
            if (!string.IsNullOrWhiteSpace(configPath))
            {
                // Set current config path
                CurrentConfigPath = configPath;

                // Set current config directory
                CurrentConfigDirectory = Path.GetDirectoryName(configPath);
            }

            saveConfig();
        }

        private static void saveConfig()
        {
            // If config directory doesn't exist
            if (!Directory.Exists(CurrentConfigDirectory))
            {
                Directory.CreateDirectory(CurrentConfigDirectory);
            }

            // Get stream
            using (ConfigFileStream = new FileStream(CurrentConfigPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(ConfigFileStream))
                {
                    saveUserSettings(writer);

                    saveGroupSettings(writer);
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

        private static void loadUserSettings(BinaryReader reader)
        {
            // Clear current list of user settings
            Users.Clear();

            // Number of user settings instances
            int count = reader.ReadInt32();

            // For each user settings instance
            for (int i = 0; i < count; i++)
            {
                // Parse user settings instance from binary reader
                UserSettings settings = UserSettings.Parse(reader);

                // Add user settings to main list
                Users.Add(settings);
            }
        }

        private static void saveUserSettings(BinaryWriter writer)
        {
            // Get number of user settings instances and write
            int count = Users.Count;
            writer.Write(count);

            // For each user settings instance
            foreach (UserSettings settings in Users)
            {
                // Write user settings to stream
                settings.Write(writer);
            }
        }

        private static void loadGroupSettings(BinaryReader reader)
        {
            // Clear current list of group settings
            Groups.Clear();

            // Get number of group settings instances
            int count = reader.ReadInt32();

            // Enumerate every instance of group settings
            for (int i = 0; i < count; i++)
            {
                // Get instance of group settings
                GroupSettings settings = GroupSettings.Parse(reader);

                // Add group settings to list
                Groups.Add(settings);
            }
        }

        private static void saveGroupSettings(BinaryWriter writer)
        {
            // Get number of group settings instances and write
            int count = Groups.Count;
            writer.Write(count);

            // For each group settings control
            foreach (GroupSettings settings in Groups)
            {
                // Write group settings to stream
                settings.Write(writer);
            }
        }
    }
}
