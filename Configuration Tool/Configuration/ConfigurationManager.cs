﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Configuration_Tool.Controls;
using System.Windows;

namespace Configuration_Tool.Configuration
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

                // Binary reader for parsing of data
                using (BinaryReader reader = new BinaryReader(ConfigFileStream))
                {
                    loadUserSettings(reader, mainWindow);
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

            // Get stream
            using (ConfigFileStream = new FileStream(CurrentConfigPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(ConfigFileStream))
                {
                    saveUserSettings(writer, mainWindow);
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
            // Get number of user settings instances
            int count = mainWindow.listUserConfigs.Items.Count;

            // Write number of user settings instances
            writer.Write(count);

            // For each user settings instance
            foreach (UserSettings settings in mainWindow.listUserConfigs.Items)
            {
                // Write user settings to stream
                settings.Write(writer);
            }
        }
    }
}
