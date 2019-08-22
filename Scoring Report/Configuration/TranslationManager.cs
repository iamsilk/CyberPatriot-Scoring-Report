using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Scoring_Report
{
    public static class TranslationManager
    {
        // {0} - Translation Header
        // {1} - Joined parameters of translation (delimeter ' ')
        public const string BackupTranslationFormat = "{0} {1}";

        private static Dictionary<string, string> Translations { get; } = new Dictionary<string, string>();

        public static string DefaultTranslationsDirectory { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;

        public const string DefaultTranslationsFile = "Translations.dat";

        public static readonly string DefaultTranslationsPath = Path.Combine(DefaultTranslationsDirectory, DefaultTranslationsFile);

        public static string CurrentTranslationsDirectory { get; private set; } = "";

        public static string CurrentTranslationsPath { get; private set; } = "";

        public static DateTime LastUpdated { get; private set; } = DateTime.MinValue;

        public static void Startup(string startupParameter)
        {
            CurrentTranslationsPath = startupParameter;

            // If no custom translations path is specified
            if (string.IsNullOrWhiteSpace(CurrentTranslationsPath))
            {
                // Set current translations directory/path as default
                CurrentTranslationsDirectory = DefaultTranslationsDirectory;
                CurrentTranslationsPath = DefaultTranslationsPath;
            }
            else
            {
                // Set current translations directory based on given path
                CurrentTranslationsDirectory = Path.GetDirectoryName(CurrentTranslationsPath);
            }

            loadTranslations();
        }

        public static void Loop()
        {
            // Get latest creation time of config file
            DateTime latestWriteTime = File.GetLastWriteTime(CurrentTranslationsPath);

            // If latest was after the last time we updated, load new config
            if (latestWriteTime > LastUpdated)
            {
                loadTranslations();
            }
        }

        public static void LoadTranslations(string translationsPath)
        {
            // If no translations path is specified
            if (string.IsNullOrWhiteSpace(translationsPath))
            {
                // Stop loading non-existent translations
                return;
            }

            CurrentTranslationsPath = translationsPath;
            CurrentTranslationsDirectory = Path.GetDirectoryName(translationsPath);

            loadTranslations();
        }

        private static void loadTranslations()
        {
            // If file/directory doesn't exist
            if (!checkFiles())
            {
                // Stop loading translations
                return;
            }

            // Create stream to go over file's data
            using (FileStream fileStream = File.Open(CurrentTranslationsPath, FileMode.Open, FileAccess.Read))
            {
                LastUpdated = File.GetLastWriteTime(CurrentTranslationsPath);

                // Binary reader for parsing of data
                using (BinaryReader reader = new BinaryReader(fileStream))
                {
                    // Clear translations
                    Translations.Clear();

                    // Get number of translations
                    int count = reader.ReadInt32();

                    for (int i = 0; i < count; i++)
                    {
                        // Get translation header and format
                        string header = reader.ReadString();
                        string format = reader.ReadString();

                        Translations.Add(header, format);
                    }
                }
            }
        }

        private static bool checkFiles()
        {
            // If translations file directory doesn't exist
            if (!Directory.Exists(CurrentTranslationsDirectory))
            {
                // Stop process of loading translations
                return false;
            }

            // If file doesn't exist
            if (!File.Exists(CurrentTranslationsPath))
            {
                // Stop process of loading translations
                return false;
            }

            return true;
        }

        public static string Translate(string format, params object[] parameters)
        {
            // If header/format pair exists
            if (Translations.ContainsKey(format))
            {
                // Return properly formatted translation
                return string.Format(Translations[format], parameters);
            }

            // Join all parameters in one string separated by spaces
            string joined = string.Join(" ", parameters);

            // Return header and parameters in format of fallback
            return string.Format(BackupTranslationFormat, format, joined);
        }
    }
}
