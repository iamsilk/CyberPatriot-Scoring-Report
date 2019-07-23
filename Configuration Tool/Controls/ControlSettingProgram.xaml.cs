using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Configuration_Tool.Controls
{
    /// <summary>
    /// Interaction logic for ControlSettingProgram.xaml
    /// </summary>
    public partial class ControlSettingProgram : UserControl
    {
        public string Header
        {
            get
            {
                return txtHeader.Text;
            }
            set
            {
                txtHeader.Text = value;
            }
        }

        public bool IsScored
        {
            get
            {
                if (!checkBoxScored.IsChecked.HasValue) return false;

                return checkBoxScored.IsChecked.Value;
            }
            set
            {
                checkBoxScored.IsChecked = value;
            }
        }

        public bool Installed
        {
            get
            {
                if (!checkBoxInstalled.IsChecked.HasValue) return false;

                return checkBoxInstalled.IsChecked.Value;
            }
            set
            {
                checkBoxInstalled.IsChecked = value;
            }
        }

        public ControlSettingProgram()
        {
            InitializeComponent();
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

                                    // Check if program is already listed, if so, continue
                                    if (programs.Contains(name)) continue;

                                    // Add user-friendly name to programs list
                                    programs.Add(name);
                                }
                            }
                        }
                    }
                }

                using (RegistryKey users = RegistryKey.OpenBaseKey(RegistryHive.Users, view))
                {
                    // Loop over users subkeys
                    foreach (string strUserKey in users.GetSubKeyNames())
                    {
                        // Get user's registry key
                        using (RegistryKey userKey = users.OpenSubKey(strUserKey))
                        {
                            // Dictionary containing the parent key of programs and the name of the value containing the user-friendly name
                            Dictionary<string, string> localMachineRegs = new Dictionary<string, string>
                            {
                                { @"Software\Microsoft\Windows\CurrentVersion\Uninstall", "DisplayName" },
                                { @"Software\Classes\Installer\Products", "ProductName" }
                            };

                            // Loop over all the user's registries
                            foreach (KeyValuePair<string, string> registry in localMachineRegs)
                            {
                                // Get containing key of programs
                                using (RegistryKey key = userKey.OpenSubKey(registry.Key))
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

                                            // Check if program is already listed, if so, continue
                                            if (programs.Contains(name)) continue;

                                            // Add user-friendly name to programs list
                                            programs.Add(name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Sort alphabetically
            programs.Sort();

            return programs;
        }
    }
}
