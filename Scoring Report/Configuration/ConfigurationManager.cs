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

        public static List<UserSettings> Users { get; } = new List<UserSettings>();

        public static List<GroupSettings> Groups { get; } = new List<GroupSettings>();

        public static PasswordPolicy PasswordPolicy { get; set; } = new PasswordPolicy();

        public static LockoutPolicy LockoutPolicy { get; set; } = new LockoutPolicy();

        public static AuditPolicy AuditPolicy { get; set; } = new AuditPolicy();

        public static List<UserRightsDefinition> UserRightsDefinitions { get; } = new List<UserRightsDefinition>();

        public static List<ISecurityOption> SecurityOptions { get; } = new List<ISecurityOption>();

        public static Dictionary<string, bool> InstalledPrograms { get; } = new Dictionary<string, bool>();

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

                // Get new max score as new config might have introduced more/less points available
                ScoringManager.GetMaxScore();
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

                        loadUserSettings(reader);

                        loadGroupSettings(reader);

                        loadPasswordPolicy(reader);

                        loadLockoutPolicy(reader);

                        loadAuditPolicy(reader);

                        loadUserRights(reader);

                        loadSecurityOptions(reader);

                        loadInstalledPrograms(reader);
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

        private static void loadPasswordPolicy(BinaryReader reader)
        {
            // Get stored policy
            PasswordPolicy policy = PasswordPolicy.Parse(reader);

            // Store policy in global variable
            PasswordPolicy = policy;
        }

        private static void loadLockoutPolicy(BinaryReader reader)
        {
            // Get stored policy
            LockoutPolicy policy = LockoutPolicy.Parse(reader);

            // Store policy in global variable
            LockoutPolicy = policy;
        }

        private static void loadAuditPolicy(BinaryReader reader)
        {
            // Get stored policy
            AuditPolicy policy = AuditPolicy.Parse(reader);

            // Store policy in global variable
            AuditPolicy = policy;
        }

        private static void loadUserRights(BinaryReader reader)
        {
            // Clear current user rights definitions
            UserRightsDefinitions.Clear();

            // Get count of user rights definitions
            int count = reader.ReadInt32();

            // For number of user rights definitions
            for (int i = 0; i < count; i++)
            {
                // Get constant name
                string constantName = reader.ReadString();

                // Get setting
                string setting = reader.ReadString();

                // Get and set scoring status
                bool isScored = reader.ReadBoolean();

                // Create instance of definition object
                UserRightsDefinition userRights = new UserRightsDefinition(constantName, setting);

                // Get number of identifiers
                int identifiersCount = reader.ReadInt32();

                // For number of identifiers
                for (int j = 0; j < identifiersCount; j++)
                {
                    // Get identifier type and identifier
                    EUserRightsIdentifierType type = (EUserRightsIdentifierType)reader.ReadInt32();
                    string strIdentifier = reader.ReadString();

                    // Initialize storage for identifier
                    UserRightsIdentifier identifier = new UserRightsIdentifier();
                    identifier.Type = type;
                    identifier.Identifier = strIdentifier;

                    // Add identifier to list
                    userRights.Identifiers.Add(identifier);
                }

                // Optimization, only add scored items
                if (isScored)
                {
                    UserRightsDefinitions.Add(userRights);
                }
            }
        }

        private static void loadSecurityOptions(BinaryReader reader)
        {
            // Clear current storage
            SecurityOptions.Clear();

            // Get number of security option settings
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get type of sec option
                ESecurityOptionType type = (ESecurityOptionType)reader.ReadInt32();

                ISecurityOption secOption = null;

                // Initialize secOption based on type
                switch (type)
                {
                    case ESecurityOptionType.RegistryComboBox:
                        secOption = new RegistryComboBox();
                        break;
                    case ESecurityOptionType.RegistryTextRegex:
                        secOption = new RegistryTextRegex();
                        break;
                    case ESecurityOptionType.RegistryRange:
                        secOption = new RegistryRange();
                        break;
                    case ESecurityOptionType.RegistryMultiLine:
                        secOption = new RegistryMultiLine();
                        break;
                    case ESecurityOptionType.SeceditComboBox:
                        secOption = new SeceditComboBox();
                        break;
                    case ESecurityOptionType.SeceditTextRegex:
                        secOption = new SeceditTextRegex();
                        break;
                }

                if (secOption == null)
                {
                    // Uh oh.. corrupted file?
                    throw new Exception("Unknown security option type. Possible configuration file corruption.");
                }

                // Parse information based on type
                secOption.Parse(reader);

                // Only store scored options
                if (secOption.IsScored)
                {
                    SecurityOptions.Add(secOption);
                }
            }
        }
        private static void loadInstalledPrograms(BinaryReader reader)
        {
            InstalledPrograms.Clear();

            // Get count of program configs
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get info of program config
                string header = reader.ReadString();
                bool installed = reader.ReadBoolean();

                InstalledPrograms.Add(header, installed);
            }
        }
    }
}
