using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Configuration_Tool.Controls;
using System.Windows;
using Configuration_Tool.Configuration.Groups;
using System.Windows.Input;
using System.ComponentModel;
using Configuration_Tool.Controls.SecOptions;

namespace Configuration_Tool.Configuration
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

        /* Reason for multiple output files support is in case
         * any users want to setup their own scripts/programs
         * for uploading/checking scoring without needing to
         * negotiate handles between possible competitors.
         * Most often, this is not necessary and another script
         * could just get read access alongside the viewer;
         * however, this is a worst case scenario implementation */

        public const string DefaultOutputFile = "Scoring Report.html";

        public static BindingList<string> OutputFiles { get; } = new BindingList<string>()
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
                MainWindow mainWindow = null;

                // For each window in current application
                foreach (Window window in Application.Current.Windows)
                {
                    // If window is main window (should only be one)
                    if (window is MainWindow)
                    {
                        // Set instance of main window to 
                        // current iteration and exit loop
                        mainWindow = window as MainWindow;
                        break;
                    }
                }

                // If main window wasn't found
                if (mainWindow == null)
                {
                    // Don't know what happened here :/
                    throw new Exception("Couldn't find main window while loading configuration.");
                }
                
                try
                {
                    // Binary reader for parsing of data
                    using (BinaryReader reader = new BinaryReader(ConfigFileStream))
                    {
                        loadOutputFiles(reader);

                        loadUserSettings(reader, mainWindow);

                        loadGroupSettings(reader, mainWindow);

                        loadPasswordPolicy(reader, mainWindow);

                        loadLockoutPolicy(reader, mainWindow);

                        loadAuditPolicy(reader, mainWindow);

                        loadUserRights(reader, mainWindow);

                        loadSecurityOptions(reader, mainWindow);

                        loadInstalledPrograms(reader, mainWindow);

                        loadProhibitedFiles(reader, mainWindow);

                        loadOther(reader, mainWindow);
                    }
                }
                catch
                {
                    MessageBox.Show("There was an error loading the configuration. Information that could be recovered has been loaded." +
                        Environment.NewLine + "You may be using an outdated configuration file." +
                        Environment.NewLine + "Please resave the configuration with the recovered information.");
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

            MainWindow mainWindow = null;

            // For each window in current application
            foreach (Window window in Application.Current.Windows)
            {
                // If window is main window (should only be one)
                if (window is MainWindow)
                {
                    // Set instance of main window to 
                    // current iteration and exit loop
                    mainWindow = window as MainWindow;
                    break;
                }
            }

            // If main window wasn't found
            if (mainWindow == null)
            {
                // Don't know what happened here :/
                throw new Exception("Couldn't find main window while loading configuration.");
            }

            // Update all bindings
            mainWindow.UpdateBindingSources();

            // Get stream
            using (ConfigFileStream = new FileStream(CurrentConfigPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(ConfigFileStream))
                {
                    saveOutputFiles(writer);

                    saveUserSettings(writer, mainWindow);

                    saveGroupSettings(writer, mainWindow);

                    savePasswordPolicy(writer, mainWindow);

                    saveLockoutPolicy(writer, mainWindow);

                    saveAuditPolicy(writer, mainWindow);

                    saveUserRights(writer, mainWindow);

                    saveSecurityOptions(writer, mainWindow);

                    saveInstalledPrograms(writer, mainWindow);

                    saveProhibitedFiles(writer, mainWindow);

                    saveOther(writer, mainWindow);
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

        private static void saveOutputFiles(BinaryWriter writer)
        {
            // Write number of output files
            int count = OutputFiles.Count;
            writer.Write(count);

            // Write each output file
            foreach (string file in OutputFiles)
            {
                writer.Write(file);
            }
        }

        private static void loadUserSettings(BinaryReader reader, MainWindow mainWindow)
        {
            // Clear current list of user settings
            mainWindow.listUserConfigs.Items.Clear();

            // Number of user settings instances
            int count = reader.ReadInt32();

            // For each user settings instance
            for (int i = 0; i < count; i++)
            {
                // Parse user settings instance from binary reader
                UserSettings settings = UserSettings.Parse(reader);

                // Create control from settings
                ControlUserSettings control = new ControlUserSettings(settings);

                // Add user settings to user settings items control
                mainWindow.listUserConfigs.Items.Add(control);
            }
        }

        private static void saveUserSettings(BinaryWriter writer, MainWindow mainWindow)
        {
            // Get number of user settings instances and write
            int count = mainWindow.listUserConfigs.Items.Count;
            writer.Write(count);

            // For each user settings control
            foreach (ControlUserSettings control in mainWindow.listUserConfigs.Items)
            {
                // Get user settings instance
                UserSettings settings = control.Settings;

                // Write user settings to stream
                settings.Write(writer);
            }
        }

        private static void loadGroupSettings(BinaryReader reader, MainWindow mainWindow)
        {
            // Clear current list of group settings
            mainWindow.listGroupConfigs.Items.Clear();

            // Get number of group settings instances
            int count = reader.ReadInt32();

            // Enumerate every instance of group settings
            for (int i = 0; i < count; i++)
            {
                // Get instance of group settings
                GroupSettings settings = GroupSettings.Parse(reader);

                // Create control from settings
                ControlGroupSettings control = new ControlGroupSettings(settings);
                
                // Add instance to group settings items control
                mainWindow.listGroupConfigs.Items.Add(control);
            }
        }

        private static void saveGroupSettings(BinaryWriter writer, MainWindow mainWindow)
        {
            // Get number of group settings instances and write
            int count = mainWindow.listGroupConfigs.Items.Count;
            writer.Write(count);

            // For each group settings control
            foreach (ControlGroupSettings control in mainWindow.listGroupConfigs.Items)
            {
                // Get group settings instance and write
                GroupSettings settings = control.Settings;

                // Write group settings to stream
                settings.Write(writer);
            }
        }

        private static void loadPasswordPolicy(BinaryReader reader, MainWindow mainWindow)
        {
            // Get stored policy
            PasswordPolicy policy = PasswordPolicy.Parse(reader);

            // Write policy settings to window
            policy.WriteToWindow(mainWindow);
        }

        private static void savePasswordPolicy(BinaryWriter writer, MainWindow mainWindow)
        {
            // Get policy from main window
            PasswordPolicy policy = PasswordPolicy.GetFromWindow(mainWindow);

            // Write policy settings to stream
            policy.Write(writer);
        }

        private static void loadLockoutPolicy(BinaryReader reader, MainWindow mainWindow)
        {
            // Get stored policy
            LockoutPolicy policy = LockoutPolicy.Parse(reader);

            // Write policy settings to window
            policy.WriteToWindow(mainWindow);
        }

        private static void saveLockoutPolicy(BinaryWriter writer, MainWindow mainWindow)
        {
            // Get policy from main window
            LockoutPolicy policy = LockoutPolicy.GetFromWindow(mainWindow);

            // Write policy settings to stream
            policy.Write(writer);
        }

        private static void loadAuditPolicy(BinaryReader reader, MainWindow mainWindow)
        {
            // Get stored policy
            AuditPolicy policy = AuditPolicy.Parse(reader);

            // Write policy settings to window
            policy.WriteToWindow(mainWindow);
        }

        private static void saveAuditPolicy(BinaryWriter writer, MainWindow mainWindow)
        {
            // Get policy from main window
            AuditPolicy policy = AuditPolicy.GetFromWindow(mainWindow);

            // Write policy settings to stream
            policy.Write(writer);
        }

        private static void loadUserRights(BinaryReader reader, MainWindow mainWindow)
        {
            // Get count of user rights definitions
            int count = reader.ReadInt32();

            // For number of user rights definitions
            for (int i = 0; i < count; i++)
            {
                // Get constant name
                string constantName = reader.ReadString();

                // Get setting name, only used within scoring report
                string setting = reader.ReadString();

                // Try to find control corresponding to constant name

                // Check first if index of config corresponds to index of item control (so we don't have to loop)
                // This is implemented to allow easier fixations if user rights definitions on windows change in the future
                ControlSettingUserRights control = (ControlSettingUserRights)mainWindow.itemsUserRightsSettings.Items[i];

                bool found = true;

                if (control.UserRightsConstantName != constantName)
                {
                    // Config and index do not correspond, loop items control to try to find correct control
                    found = false;

                    foreach (ControlSettingUserRights check in mainWindow.itemsUserRightsSettings.Items)
                    {
                        if (check.UserRightsConstantName == constantName)
                        {
                            control = check;
                            found = true;
                            break;
                        }
                    }
                }

                // If control wasn't found, create new one to still allow usability of program
                if (!found)
                {
                    control = new ControlSettingUserRights(constantName);
                    control.UserRightsConstantName = constantName;

                    // Add control to items control
                    mainWindow.itemsUserRightsSettings.Items.Add(control);
                }

                // Get and set scoring status
                bool isScored = reader.ReadBoolean();
                control.IsScored = isScored;

                // Get number of identifiers
                int identifiersCount = reader.ReadInt32();

                // For number of identifiers
                for (int j = 0; j < identifiersCount; j++)
                {
                    // Get identifier type and identifier
                    EUserRightsIdentifierType type = (EUserRightsIdentifierType)reader.ReadInt32();
                    string identifier = reader.ReadString();

                    object identifierControl = null;

                    // Create proper control based on type
                    switch (type)
                    {
                        case EUserRightsIdentifierType.Name:
                            identifierControl = new ControlSettingUserRightsName(identifier);
                            break;
                        case EUserRightsIdentifierType.SecurityID:
                            identifierControl = new ControlSettingUserRightsSID(identifier);
                            break;
                    }

                    // Add control to items control
                    control.itemsIdentifiers.Items.Add(identifierControl);
                }
            }
        }

        private static void saveUserRights(BinaryWriter writer, MainWindow mainWindow)
        {
            // Get and write count of user rights definitions
            int count = mainWindow.itemsUserRightsSettings.Items.Count;
            writer.Write(count);

            // For each user rights setting control in items control
            foreach (ControlSettingUserRights control in mainWindow.itemsUserRightsSettings.Items)
            {
                // Get/write constant name
                string constantName = control.UserRightsConstantName;
                writer.Write(constantName);

                // Get/write setting (displayed title)
                string setting = control.Setting;
                writer.Write(setting);

                // Get/write boolean specifying scoring status
                bool isScored = control.IsScored;
                writer.Write(isScored);

                // Get identifications for each member of user rights definition
                // We don't bother converting to a list as it's unnecessary and unoptimized
                IEnumerable<IUserRightsIdentifier> identifiers = control.itemsIdentifiers.Items.Cast<IUserRightsIdentifier>();

                // Write number of identifiers
                writer.Write(identifiers.Count());

                // For each identifier in list, write identifier type and identifier
                foreach (IUserRightsIdentifier identifier in identifiers)
                {
                    writer.Write((Int32)identifier.Type);
                    writer.Write(identifier.Identifier);
                }
            }
        }

        private static void loadSecurityOptions(BinaryReader reader, MainWindow mainWindow)
        {
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
                        secOption = new ControlRegistryComboBox();
                        break;
                    case ESecurityOptionType.RegistryTextRegex:
                        secOption = new ControlRegistryTextRegex();
                        break;
                    case ESecurityOptionType.RegistryRange:
                        secOption = new ControlRegistryRange();
                        break;
                    case ESecurityOptionType.RegistryMultiLine:
                        secOption = new ControlRegistryMultiLine();
                        break;
                    case ESecurityOptionType.SeceditComboBox:
                        secOption = new ControlSeceditComboBox();
                        break;
                    case ESecurityOptionType.SeceditTextRegex:
                        secOption = new ControlSeceditTextRegex();
                        break;
                }

                if (secOption == null)
                {
                    // Uh oh.. corrupted file?
                    throw new Exception("Unknown security option type. Possible configuration file corruption.");
                }

                // Parse information based on type
                secOption.Parse(reader);

                string identifier = secOption.Identifier();

                // Search for matching control to copy information to
                foreach (ISecurityOption control in mainWindow.itemsSecurityOptions.Items.OfType<ISecurityOption>())
                {
                    // Check if control is a match
                    if (type == control.Type && identifier == control.Identifier())
                    {
                        // Copy information to displayed control
                        secOption.CopyTo(control);

                        // Found match, stop searching
                        break;
                    }
                }
            }
        }

        private static void saveSecurityOptions(BinaryWriter writer, MainWindow mainWindow)
        {
            IEnumerable<ISecurityOption> items = mainWindow.itemsSecurityOptions.Items.OfType<ISecurityOption>();

            // Write number of security option settings
            writer.Write(items.Count());

            // Loop over every element of items control and save
            foreach (ISecurityOption secOption in items)
            {
                // Write type and interface
                writer.Write((Int32)secOption.Type);
                secOption.Write(writer);
            }
        }

        private static void saveInstalledPrograms(BinaryWriter writer, MainWindow mainWindow)
        {
            // Get list of controls and filter off unscored
            IEnumerable<ControlSettingProgram> programs = mainWindow.listPrograms.Items.Cast<ControlSettingProgram>()
                .Where(x => x.IsScored);

            // Write number of programs
            writer.Write(programs.Count());

            // Loop over and write each programs details
            foreach (ControlSettingProgram program in programs)
            {
                writer.Write(program.Header);
                writer.Write(program.Installed);
            }
        }

        private static void loadInstalledPrograms(BinaryReader reader, MainWindow mainWindow)
        {
            // Get count of program configs
            int count = reader.ReadInt32();
            
            for (int i = 0; i < count; i++)
            {
                // Get info of program config
                string header = reader.ReadString();
                bool installed = reader.ReadBoolean();

                // Search for control with header matching config
                ControlSettingProgram control = mainWindow.listPrograms.Items.Cast<ControlSettingProgram>()
                    .FirstOrDefault(x => x.Header == header);

                // If no control was found
                if (control == null)
                {
                    // Create new instance of program
                    control = new ControlSettingProgram();

                    // Set properties
                    control.Header = header;
                    control.Installed = installed;

                    // All written programs are scored
                    control.IsScored = true;

                    mainWindow.listPrograms.Items.Add(control);
                }
                else
                {
                    // Set other configs
                    control.IsScored = true;
                    control.Installed = installed;
                }
            }
        }

        private static void loadProhibitedFiles(BinaryReader reader, MainWindow mainWindow)
        {
            mainWindow.itemsProhibitedFiles.Items.Clear();

            // Get count of prohibited files
            int filecount = reader.ReadInt32();

            for (int i = 0; i < filecount; i++)
            {
                // Get File Location
                string fileLocation = reader.ReadString();

                // Create control
                ControlProhibitedFile control = new ControlProhibitedFile();
                control.Path = fileLocation;

                // Add control to items control
                mainWindow.itemsProhibitedFiles.Items.Add(control);
            }
        }

        private static void saveProhibitedFiles(BinaryWriter writer, MainWindow mainWindow)
        {
            // Get/write number of prohibited files
            int count = mainWindow.itemsProhibitedFiles.Items.Count;
            writer.Write(count);

            // Loop over each prohibited file
            foreach (ControlProhibitedFile control in mainWindow.itemsProhibitedFiles.Items)
            {
                // Write paths
                writer.Write(control.Path);
            }
        }

        private static void loadOther(BinaryReader reader, MainWindow mainWindow)
        {
            // Load remote desktop info
            mainWindow.rdpRegistry.Parse(reader);
        }

        private static void saveOther(BinaryWriter writer, MainWindow mainWindow)
        {
            // Save remote desktop info
            mainWindow.rdpRegistry.Write(writer);
        }
    }
}
