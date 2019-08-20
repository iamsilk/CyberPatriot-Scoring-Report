using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Configuration_Tool.Configuration.Startup
{
    public class StartupInfo
    {
        public string Owner { get; set; } = "";

        public string Name { get; set; } = "";

        public string Command { get; set; } = "";

        public EStartupType StartupType { get; set; }

        public string StartupTypeString
        {
            get
            {
                switch (StartupType)
                {
                    case EStartupType.Registry:
                        return "Registry";
                    case EStartupType.StartupFolder:
                        return "Startup Folder";
                }
                return "Unknown";
            }
        }

        public string Location { get; set; } = "";

        public bool IsScored { get; set; } = false;

        public StartupInfo(string owner, string name, string command, EStartupType startupType, string location, bool isScored)
        {
            Owner = owner;
            Name = name;
            Command = command;
            StartupType = startupType;
            Location = location;
            IsScored = isScored;
        }

        public static StartupInfo Parse(BinaryReader reader)
        {
            // Get all parameters
            string owner = reader.ReadString();
            string name = reader.ReadString();
            string command = reader.ReadString();
            EStartupType type = (EStartupType)reader.ReadInt32();
            string location = reader.ReadString();
            bool isScored = reader.ReadBoolean();

            // Return initialized class
            return new StartupInfo(owner, name, command, type, location, isScored);
        }

        public void Write(BinaryWriter writer)
        {
            // Write all parameters
            writer.Write(Owner);
            writer.Write(Name);
            writer.Write(Command);
            writer.Write((int)StartupType);
            writer.Write(Location);
            writer.Write(IsScored);
        }

        public static void GetStartupInfos(IList<StartupInfo> infos)
        {
            // Clear current list
            infos.Clear();

            string profileListRegPath = @"Software\Microsoft\Windows NT\CurrentVersion\ProfileList";
            string runRegPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

            string startupFolder = @"\Microsoft\Windows\Start Menu\Programs\Startup";

            // Get registry subkey of profile list
            RegistryKey profileList = Registry.LocalMachine.OpenSubKey(profileListRegPath);

            // If subkey exists
            if (profileList != null)
            {
                // For each profile sub key (security id)
                foreach (string sid in profileList.GetSubKeyNames())
                {
                    string profileImagePath = profileList.OpenSubKey(sid).GetValue("ProfileImagePath") as string;

                    // If 'ProfileImagePath' value name doesn't exist, skip
                    if (profileImagePath == null) continue;

                    // If profile path is a user directory
                    if (profileImagePath.Contains("C:\\Users\\"))
                    {
                        // Last part of directory is username (usually)
                        string username = profileImagePath.Split('\\').Last();

                        // Get user's subkey
                        RegistryKey userKey = Registry.Users.OpenSubKey(sid);

                        // If key was successfully retrieved
                        if (userKey != null)
                        {
                            RegistryKey startups = userKey.OpenSubKey(runRegPath);

                            // If key was successfully retrieved
                            if (startups != null)
                            {
                                // For each startup within containing key
                                foreach (string startupName in startups.GetValueNames())
                                {
                                    // Get command
                                    string command = startups.GetValue(startupName).ToString();

                                    // Add startup info to list
                                    infos.Add(new StartupInfo(username, startupName, command, EStartupType.Registry, startups.ToString(), false));
                                }
                            }
                        }

                        // Concat to get profile's startup folder
                        string profileStartupFolder = profileImagePath + @"\AppData\Roaming" + startupFolder;

                        if (Directory.Exists(profileStartupFolder))
                        {
                            // Loop over each file in startup folder
                            foreach (string startupFile in Directory.GetFiles(profileStartupFolder))
                            {
                                // If file is the desktop.ini file, skip
                                if (startupFile.EndsWith("desktop.ini")) continue;

                                // Get name of file
                                string name = Path.GetFileNameWithoutExtension(startupFile);

                                // Add startup info to list
                                infos.Add(new StartupInfo(username, name, startupFile, EStartupType.StartupFolder, startupFile, false));
                            }
                        }
                    }
                }
            }

            // Get program data folder
            string programDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            if (Directory.Exists(programDataFolder + startupFolder))
            {
                // Get all files in shared startup folder
                foreach (string startupFile in Directory.GetFiles(programDataFolder + startupFolder))
                {
                    // If file is the desktop.ini file, skip
                    if (startupFile.EndsWith("desktop.ini")) continue;

                    // Get name of file
                    string name = Path.GetFileNameWithoutExtension(startupFile);

                    // Add startup info to list
                    infos.Add(new StartupInfo("All Users", name, startupFile, EStartupType.StartupFolder, startupFile, false));
                }
            }


            // Read both 32 and 64 bit registries
            RegistryView[] views = new RegistryView[] { RegistryView.Registry32, RegistryView.Registry64 };
            foreach (RegistryView view in views)
            {
                // Get local machine registry key
                using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    // All registry paths to check
                    string[] hklmStartupRegPaths = new string[]
                    {
                        runRegPath,
                        @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Run",
                        @"Software\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows\Run",
                        @"Software\Microsoft\Windows NT\CurrentVersion\Windows\Run"
                    };

                    // Loop over all local machine registries
                    foreach (string registryPath in hklmStartupRegPaths)
                    {
                        // Get key containing startup commands
                        RegistryKey startups = localMachine.OpenSubKey(registryPath);
                        
                        // If key was successfully retrieved
                        if (startups != null)
                        {
                            // For each startup within containing key
                            foreach (string startupName in startups.GetValueNames())
                            {
                                // Get command
                                string command = startups.GetValue(startupName).ToString();

                                // Add startup info to list
                                infos.Add(new StartupInfo("All Users", startupName, command, EStartupType.Registry, startups.ToString(), false));
                            }
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            // Join all parameters except IsScored
            return string.Join(";", Owner, Name, Command, StartupTypeString, Location);
        }
    }
}
