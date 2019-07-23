using Microsoft.Win32;
using Scoring_Report.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionInstalledPrograms : ISection
    {
        public string Header => "Installed Programs:";

        public const string Format = "'{0}' was set correctly - {1}";

        public int MaxScore()
        {
            return ConfigurationManager.InstalledPrograms.Count;
        }

        public static List<string> GetPrograms()
        {
            // Read both 32 and 64 bit registries
            RegistryView[] views = new RegistryView[] { RegistryView.Registry32, RegistryView.Registry64 };

            List<string> programs = new List<string>();

            foreach (RegistryView view in views)
            {
                using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    // Dictionary containing the parent key of programs and the name of the value containing the user-friendly name
                    Dictionary<string, string> localMachineRegs = new Dictionary<string, string>
                    {
                        { @"Software\Microsoft\Windows\CurrentVersion\Uninstall", "DisplayName" },
                        { @"Software\Classes\Installer\Products", "ProductName" },
                        { @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall", "DisplayName" }
                    };

                    // Loop over all local machine registries
                    foreach (KeyValuePair<string, string> registry in localMachineRegs)
                    {
                        // Get containing key of programs
                        using (RegistryKey key = localMachine.OpenSubKey(registry.Key))
                        {
                            // If key doesn't exist
                            if (key == null) continue;

                            // Loop over registry keys defining programs
                            foreach (string strSubKey in key.GetSubKeyNames())
                            {
                                // Get handle to sub key
                                using (RegistryKey subKey = key.OpenSubKey(strSubKey))
                                {
                                    string name = subKey.GetValue(registry.Value) as string;

                                    // If name is invalid/unidentifiable
                                    if (string.IsNullOrWhiteSpace(name)) continue;

                                    // Add user-friendly name to programs list
                                    programs.Add(name);
                                }
                            }
                        }
                    }
                }

                using (RegistryKey users = RegistryKey.OpenBaseKey(RegistryHive.Users, view))
                {
                    // Dictionary containing the parent key of programs and the name of the value containing the user-friendly name
                    Dictionary<string, string> localMachineRegs = new Dictionary<string, string>
                    {
                        { @"Software\Microsoft\Windows\CurrentVersion\Uninstall", "DisplayName" },
                        { @"Software\Classes\Installer\Products", "ProductName" }
                    };

                    // Loop over all users registries
                    foreach (KeyValuePair<string, string> registry in localMachineRegs)
                    {
                        // Get containing key of programs
                        using (RegistryKey key = users.OpenSubKey(registry.Key))
                        {
                            // If key doesn't exist
                            if (key == null) continue;

                            // Loop over registry keys defining programs
                            foreach (string strSubKey in key.GetSubKeyNames())
                            {
                                // Get handle to sub key
                                using (RegistryKey subKey = key.OpenSubKey(strSubKey))
                                {
                                    string name = subKey.GetValue(registry.Value) as string;

                                    // If name is invalid/unidentifiable
                                    if (string.IsNullOrWhiteSpace(name)) continue;

                                    // Add user-friendly name to programs list
                                    programs.Add(name);
                                }
                            }
                        }
                    }
                }
            }

            return programs;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Get list of programs on system
            List<string> programs = GetPrograms();

            // Loop over every program config
            foreach (KeyValuePair<string, bool> programConfig in ConfigurationManager.InstalledPrograms)
            {
                bool installed = false;

                // Loop over every program on system
                foreach (string program in programs)
                {
                    // If program and config match, program is installed
                    if (program == programConfig.Key)
                    {
                        installed = true;
                    }
                }

                // If desired install status matches actual status
                if (installed == programConfig.Value)
                {
                    details.Points++;
                    details.Output.Add(string.Format(Format, programConfig.Key, programConfig.Value ? "Installed" : "Uninstalled"));
                }
            }

            return details;
        }
    }
}
