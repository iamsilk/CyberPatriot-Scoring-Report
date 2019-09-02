using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace Translation_Editor
{
    public static class TranslationManager
    {
        private static Dictionary<string, string> DefaultTranslations { get; } = new Dictionary<string, string>()
        {
            { "True",   "True" },
            { "False",  "False" },
            { "Enabled",    "Enabled" },
            { "Disabled",   "Disabled" },
            { "Delimiter",  ", " },

            { "SectionUsers",               "Users:" },
            { "SecurityID",                 "SID" },
            { "Username",                   "Username" },
            { "UserExists",                 "User {3} - Exists on local machine ({2})" },
            { "PasswordChanged",            "User {3} - Password changed from default" },
            { "PasswordExpired",            "User {3} - Password must be changed at next logon set to {2}" },
            { "PasswordChangeDisabled",     "User {3} - Password change disabled set to {2}" },
            { "PasswordNeverExpires",       "User {3} - Password never expires set to {2}" },
            { "AccountDisabled",            "User {3} - Account disabled set to {2}" },
            { "AccountLockedOut",           "User {3} - Account locked out set to {2}" },

            { "SectionGroups",  "Groups:" },
            { "Group",          "Group '{0}' correctly configured - {1}" },

            { "SectionPasswordPolicy",  "Password Policy:" },
            { "EnforcePasswordHistory", "'Enforce password history' set correctly - {0} passwords remembered" },
            { "MaxPasswordAge",         "'Maximum password age' set correctly - {0} days" },
            { "MinPasswordAge",         "'Minimum password age' set correctly - {0} days" },
            { "MinPasswordLength",      "'Minimum password length' set correctly - {0} characters" },
            { "PasswordComplexity",     "'Password must meet complexity requirements' set correctly - {0}" },
            { "ReversibleEncryption",   "'Store passwords using reversible encryption' set correctly - {0}" },

            { "SectionLockoutPolicy",       "Account Lockout Policy:" },
            { "AccountLockoutDuration",     "'Account lockout duration' set correctly - {0} minutes" },
            { "AccountLockoutThreshold",    "'Account lockout threshold' set correctly - {0} invalid logon attempts" },
            { "ResetLockoutCounterAfter",   "'Reset account lockout counter after' set correctly - {0} minutes" },

            { "SectionAuditPolicy", "Audit Policy:" },
            { "AuditUnchanged",     "Unchanged" },
            { "AuditSuccess",       "Success" },
            { "AuditFailure",       "Failure" },
            { "AuditBoth",          "Success, Failure" },
            { "AuditPolicy",        "'{0}' set correctly - {1}" },

            { "SectionUserRights",  "User Rights Assignment:" },
            { "UserRights",         "'{0}' set correctly - {1}" },

            { "SectionSecurityOptions",     "Security Options:" },
            { "SecurityOptions",            "'{0}' set correctly - {1}" },

            { "SectionInstalledPrograms",   "Installed Programs:" },
            { "Installed",                  "Installed" },
            { "Uninstalled",                "Uninstalled" },
            { "InstalledPrograms",          "'{0}' set correctly - {1}" },

            { "SectionProhibitedFiles", "Prohibited Files:" },
            { "ProhibitedFiles",        "File '{0}' has been deleted" },

            { "SectionShares",  "Shares:" },
            { "Exists",         "Exists" },
            { "Deleted",        "Deleted" },
            { "Shares",         "Share '{0}' has been set properly - {1}" },

            { "SectionStartup", "Startup:" },
            { "Startup",        "Startup '{1}' has been removed." },

            { "SectionFirewallProfiles",        "Firewall - Profiles:" },
            { "SectionFirewallInboundRules",    "Firewall - Inbound Rules:" },
            { "SectionFirewallOutboundRules",   "Firewall - Outbound Rules:" },
            { "On",                         "On" },
            { "Off",                        "Off" },
            { "Allow",                      "Allow" },
            { "Block",                      "Block" },
            { "BlockAll",                   "Block all connections" },
            { "Yes",                        "Yes" },
            { "No",                         "No" },
            { "Domain",                     "Domain" },
            { "Private",                    "Private" },
            { "Public",                     "Public" },
            { "All",                        "All" },
            { "AllowConnection",            "Allow the connection" },
            { "BlockConnection",            "Block the connection" },
            { "FirewallProfileProperty",    "{0} - '{1}' has been set properly - '{2}'" },
            { "FirewallInboundRule",        "Rule '{0}' has been removed" },
            { "FirewallOutboundRule",       "Rule '{0}' has been removed" },

            { "SectionServices",    "Services:" },
            { "Service",            "Service '{0}' has been configured properly: Status - {1}, Startup - {2}" },

            { "SectionFeatures",            "Windows Features:" },
            { "WindowsFeatureInstalled",    "Feature '{0}' has been installed" },
            { "WindowsFeatureNotInstalled", "Feature '{0}' has been uninstalled" },

            { "SectionForensics",   "Forensic Questions:" },
            { "AnswerCorrect",    "'{0}' is correct" },

            { "SectionCustomRegistry", "Custom Registry:" },
            { "RegistryKeyCustomOutput", "{0}" },

            { "SectionCustomProcessOutput", "Custom Processes:" },
            { "ProcessCustomOutput", "{0}" },

            { "SectionCustomFiles", "Custom Files:" },
            { "CustomFilesCustomOutput", "{0}" },

            { "SectionOther",   "Other:" },
            { "RemoteDesktop",  "Remote Desktop allowance set correctly - {0}" },
            { "HostFile",       "Host file contains only default entries" },
        };

        public static BindingList<Translation> Translations { get; } = new BindingList<Translation>(
            DefaultTranslations.Select(x => new Translation(x.Key, x.Value)).ToList());

        public static string DefaultTranslationsDirectory { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;

        public const string DefaultTranslationsFile = "Translations.dat";

        public static readonly string DefaultTranslationsPath = Path.Combine(DefaultTranslationsDirectory, DefaultTranslationsFile);

        public static string CurrentTranslationsDirectory { get; private set; } = DefaultTranslationsDirectory;

        public static string CurrentTranslationsPath { get; private set; } = DefaultTranslationsPath;

        public static byte[] DefaultTranslationsBytes { get; set; } = null;

        public static byte[] CurrentTranslationsBytes { get; set; } = null;

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

            // Cache default translations
            CacheDefaults();

            loadTranslations();
        }

        public static MainWindow FindMainWindow()
        {
            // For each window in current application
            foreach (Window window in Application.Current.Windows)
            {
                // If window is main window (should only be one)
                if (window is MainWindow)
                {
                    // Return found window
                    return (MainWindow)window;
                }
            }

            return null;
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

            // Get all bytes from translations file
            CurrentTranslationsBytes = File.ReadAllBytes(CurrentTranslationsPath);

            // Create stream to go over file's data
            using (MemoryStream bufferStream = new MemoryStream(CurrentTranslationsBytes))
            {
                // Get main window
                MainWindow mainWindow = FindMainWindow();

                // If main window wasn't found
                if (mainWindow == null)
                {
                    // Don't know what happened here :/
                    throw new Exception("Couldn't find main window while loading translations.");
                }

                LoadFromStream(bufferStream);
            }
        }

        public static void LoadFromStream(Stream stream)
        {
            // Binary reader for parsing of data
            using (BinaryReader reader = new BinaryReader(stream))
            {
                if (LoadingDefaults)
                {
                    // Clear translations
                    Translations.Clear();
                }

                // Get number of translations
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    // Get translation
                    Translation translation = Translation.Parse(reader);

                    if (LoadingDefaults)
                    {
                        // If loading defaults, just add and skip the searching
                        Translations.Add(translation);
                        continue;
                    }

                    // Search for matching header
                    Translation match = Translations.FirstOrDefault(x => x.Header == translation.Header);

                    // If no match was found
                    if (match == null)
                    {
                        // Add translation to list
                        Translations.Add(translation);
                    }
                    else
                    {
                        // Set format to read format from config
                        match.Format = translation.Format;
                    }
                }
            }
        }

        public static void Save(string translationsPath = "")
        {
            // If path is specified
            if (!string.IsNullOrWhiteSpace(translationsPath))
            {
                // Set current translation path
                CurrentTranslationsPath = translationsPath;

                // Set current translation directory
                CurrentTranslationsDirectory = Path.GetDirectoryName(translationsPath);
            }

            saveTranslations();
        }

        private static void saveTranslations()
        {
            // If translations directory doesn't exist
            if (!Directory.Exists(CurrentTranslationsDirectory))
            {
                Directory.CreateDirectory(CurrentTranslationsDirectory);
            }

            // Get main window
            MainWindow mainWindow = FindMainWindow();

            // If main window wasn't found
            if (mainWindow == null)
            {
                // Don't know what happened here :/
                throw new Exception("Couldn't find main window while saving translations.");
            }

            // Setup memory stream to save buffer for later comparisons on exiting/loading
            using (MemoryStream bufferStream = new MemoryStream())
            {
                // Save to buffer stream
                int length = SaveToStream(bufferStream, mainWindow);

                // Get and resize buffer to true length
                byte[] buffer = bufferStream.GetBuffer();
                Array.Resize(ref buffer, length);

                // Store buffer in global variable
                CurrentTranslationsBytes = buffer;
            }

            File.WriteAllBytes(CurrentTranslationsPath, CurrentTranslationsBytes);
        }

        public static int SaveToStream(Stream stream, MainWindow mainWindow)
        {
            // Update all bindings
            mainWindow.UpdateBindingSources();

            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Write number of translations
                writer.Write(Translations.Count);

                // Write each translation
                foreach (Translation translation in Translations)
                    translation.Write(writer);

                return (int)stream.Length;
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

        public static void CacheDefaults()
        {
            // Get main window
            MainWindow mainWindow = FindMainWindow();

            // If main window wasn't found
            if (mainWindow == null)
            {
                // Don't know what happened here :/
                throw new Exception("Couldn't find main window while caching default translations.");
            }

            // Create memory stream to cache final buffer
            using (MemoryStream bufferStream = new MemoryStream())
            {
                // Save to buffer stream
                int length = SaveToStream(bufferStream, mainWindow);

                // Get buffer and resize to true length
                byte[] buffer = bufferStream.GetBuffer();
                Array.Resize(ref buffer, length);

                // Store buffer in global variable
                DefaultTranslationsBytes = buffer;
            }
        }

        public static bool LoadingDefaults = false;

        public static void LoadDefaults()
        {
            // Get main window
            MainWindow mainWindow = FindMainWindow();

            // If main window wasn't found
            if (mainWindow == null)
            {
                // Don't know what happened here :/
                throw new Exception("Couldn't find main window while loading default translations.");
            }
            
            LoadingDefaults = true;

            // Create memory stream of default translations buffer
            using (MemoryStream bufferStream = new MemoryStream(DefaultTranslationsBytes))
            {
                // Load from default buffer stream
                LoadFromStream(bufferStream);
            }

            LoadingDefaults = false;
        }

        // Used for efficient byte array comparison
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        // Returns true if action is cancelled
        public static bool CheckSavingChanges(MainWindow mainWindow)
        {
            if (CurrentTranslationsBytes == null)
            {
                // No translations were loaded, started as default
                CurrentTranslationsBytes = DefaultTranslationsBytes;
            }

            MessageBoxResult save = MessageBoxResult.No;

            using (MemoryStream bufferStream = new MemoryStream())
            {
                int length = SaveToStream(bufferStream, mainWindow);

                // Get and resize buffer to true length
                byte[] buffer = bufferStream.GetBuffer();
                Array.Resize(ref buffer, length);

                // If cached translations and current translations are inequal, they have unsaved changes
                if (length != CurrentTranslationsBytes.Length ||
                    memcmp(CurrentTranslationsBytes, buffer, CurrentTranslationsBytes.Length) != 0)
                {
                    save = MessageBox.Show("Would you like to save changes?",
                        "Save Translations",
                        MessageBoxButton.YesNoCancel);

                    if (save == MessageBoxResult.Yes)
                    {
                        // Save changes
                        CurrentTranslationsBytes = buffer;

                        File.WriteAllBytes(CurrentTranslationsPath, CurrentTranslationsBytes);
                    }

                    return save == MessageBoxResult.Cancel;
                }
            }

            return false;
        }

        private static void saveTranslations(BinaryWriter writer)
        {
            // Write number of translations
            writer.Write(Translations.Count);

            // Loop over each translation
            foreach (Translation translation in Translations)
            {
                // Write translation
                translation.Write(writer);
            }
        }
    }
}
