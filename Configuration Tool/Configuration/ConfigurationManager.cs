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
using System.Reflection;

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

        public static List<IConfig> ConfigSections { get; } = new List<IConfig>();

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

            // Clear config sections
            ConfigSections.Clear();

            // Get all types in the executing assembly (Configuration Tool)
            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in allTypes)
            {
                // Filter to parent namespace "Configuration_Tool.Configuration.Sections"
                if (type.Namespace.StartsWith("Configuration_Tool.Configuration.Sections"))
                {
                    // Filter to types with child interface IConfig
                    if (type.GetInterfaces().Contains(typeof(IConfig)))
                    {
                        // Create instance
                        IConfig instance = Activator.CreateInstance(type) as IConfig;

                        // Add to list
                        ConfigSections.Add(instance);
                    }
                }
            }

            // Sort in order of integer value of enumerator 'Type'
            ConfigSections.Sort((x, y) => (x.Type.CompareTo(y.Type)));

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

                bool error = false;

                // Binary reader for parsing of data
                using (BinaryReader reader = new BinaryReader(ConfigFileStream))
                {
                    loadOutputFiles(reader);

                    // Get number of config sections
                    int count = reader.ReadInt32();

                    // For every config section
                    for (int i = 0; i < count; i++)
                    {
                        // Get config section type
                        EConfigType type = (EConfigType)reader.ReadInt32();

                        // Get length of data for specific section
                        int bufferLength = reader.ReadInt32();

                        // Read buffer for section to isolate from others
                        byte[] buffer = reader.ReadBytes(bufferLength);

                        // Find section for loading
                        IConfig config = ConfigSections.FirstOrDefault(x => x.Type == type);

                        // Possibly removed section, skip it
                        if (config == null) continue;

                        // Create isolated binary reader for section loading
                        using (MemoryStream bufferStream = new MemoryStream(buffer))
                        using (BinaryReader sectionReader = new BinaryReader(bufferStream))
                        {
                            // Set main window
                            config.MainWindow = mainWindow;


                            try
                            {
                                // Load config
                                config.Load(sectionReader);
                            }
                            catch { error = true; }
                        }
                    }
                }

                // An error occured during loading, alert user and continue
                if (error)
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
            using (BinaryWriter writer = new BinaryWriter(ConfigFileStream))
            {
                saveOutputFiles(writer);

                // Write number of config sections
                writer.Write(ConfigSections.Count);

                // For each config section
                foreach (IConfig config in ConfigSections)
                {
                    // Write config type
                    writer.Write((Int32)config.Type);

                    // Create isolated binary writer
                    using (MemoryStream bufferStream = new MemoryStream())
                    using (BinaryWriter sectionWriter = new BinaryWriter(bufferStream))
                    {
                        // Set main window
                        config.MainWindow = mainWindow;

                        // Save config to section writer
                        config.Save(sectionWriter);

                        // Get buffer
                        int length = Convert.ToInt32(bufferStream.Length);
                        byte[] buffer = bufferStream.GetBuffer();
                        Array.Resize(ref buffer, length);

                        // Write buffer length and buffer
                        writer.Write(length);
                        writer.Write(buffer);
                    }
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
    }
}
