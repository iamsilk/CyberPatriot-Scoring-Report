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
using Microsoft.Win32;
using Configuration_Tool.Configuration;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Configuration_Tool.Controls
{
    public static class CustomCommands
    {
        public static readonly RoutedCommand FileOpen = new RoutedCommand("FileOpen", typeof(CustomCommands));
        public static readonly RoutedCommand FileSave = new RoutedCommand("FileSave", typeof(CustomCommands));
        public static readonly RoutedCommand FileSaveAs = new RoutedCommand("FileSaveAs", typeof(CustomCommands));
        public static readonly RoutedCommand RemoveConfigTool = new RoutedCommand("RemoveConfigTool", typeof(CustomCommands));
        public static readonly RoutedCommand ClearConfiguration = new RoutedCommand("ClearConfiguration", typeof(CustomCommands));
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string startupParameter = "";

            string[] commandLineArgs = Environment.GetCommandLineArgs();

            if (commandLineArgs.Length > 1)
            {
                // Skip first command line argument, as it is the current directory
                startupParameter = commandLineArgs[1];
            }

            PopulateProgramsList();

            ConfigurationManager.Startup(startupParameter);

#if !DEBUG
            // Setup Scoring Report process
            using (Process process = new Process())
            {
                process.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scoring Report.exe");

                if (File.Exists(process.StartInfo.FileName))
                {
                    // Specify argument stating to install the Scoring Report service
                    process.StartInfo.Arguments = "/i";

                    // Start process and wait for exit
                    process.Start();
                    process.WaitForExit();

                    // 0x80004005 is the error code showing the service is already installed
                    if (process.ExitCode != 0 && (uint)process.ExitCode != 0x80004005)
                    {
                        MessageBox.Show(string.Format("There was an issue installing the Scoring Report service. Exit code: 0x{0:X}", process.ExitCode));
                    }
                }
            }
#endif
        }

        private void btnAddUserConfig_Click(object sender, RoutedEventArgs e)
        {
            ControlUserSettings userSettings = new ControlUserSettings();

            listUserConfigs.Items.Add(userSettings);
        }

        private void btnAddGroupConfig_Click(object sender, RoutedEventArgs e)
        {
            ControlGroupSettings groupSettings = new ControlGroupSettings();

            listGroupConfigs.Items.Add(groupSettings);
        }

        private void FileOpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            // Sets filter and default file extension for file dialog
            fileDialog.DefaultExt = ".dat";
            fileDialog.Filter = "DAT Files (*.dat)|*.dat|All Files (*.*)|*.*";

            // Show file dialog
            bool? fileChosen = fileDialog.ShowDialog();

            // If result has no value or value is false
            if (!fileChosen.HasValue || !fileChosen.Value)
            {
                // Stop attempting load
                return;
            }

            // Get chosen file path
            string filePath = fileDialog.FileName;

            // Attempt to load chosen file
            ConfigurationManager.LoadConfig(filePath);
        }

        private void FileSaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Saves at current/default configuration path
            ConfigurationManager.Save();
        }

        private void FileSaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();

            // Sets filter and default file extension for file dialog
            fileDialog.DefaultExt = ".dat";
            fileDialog.Filter = "DAT Files (*.dat)|*.dat|All Files (*.*)|*.*";

            // Show file dialog
            bool? fileChosen = fileDialog.ShowDialog();

            // If result has no value or value is false
            if (!fileChosen.HasValue || !fileChosen.Value)
            {
                // Stop attempting save
                return;
            }

            // Get chosen file path
            string filePath = fileDialog.FileName;

            // Save at chosen file path
            ConfigurationManager.Save(filePath);
        }

        private void ClearConfigurationCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Confirm clearing current configuration
            MessageBoxResult clearConfigurationOk = MessageBox.Show("Confirm clearing current configuration\nNOTE: This will reset EVERYTHING!",
                "Clear Current Configuration",
                MessageBoxButton.OKCancel);
            // Return if user did not press ok
            if (clearConfigurationOk != MessageBoxResult.OK) return;

            // Reset the Password and Lockout Policies
            // Set IsChecked to false and restore default values
            settingAccountLockoutDuration.IsScored = false;
            settingAccountLockoutDuration.Maximum = 99999;
            settingAccountLockoutDuration.Maximum = 99999;
            settingAccountLockoutThreshold.IsScored = false;
            settingAccountLockoutThreshold.Maximum = 999;
            settingAccountLockoutThreshold.Minimum = 0;
            settingEnforcePasswordHistory.IsScored = false;
            settingEnforcePasswordHistory.Maximum = 24;
            settingEnforcePasswordHistory.Minimum = 0;
            settingMaxPasswordAge.IsScored = false;
            settingMaxPasswordAge.Maximum = 999;
            settingMaxPasswordAge.Minimum = 0;
            settingMinPasswordAge.IsScored = false;
            settingMinPasswordAge.Maximum = 998;
            settingMinPasswordAge.Minimum = 0;
            settingMinPasswordLen.IsScored = false;
            settingMinPasswordLen.Maximum = 20;
            settingMinPasswordLen.Minimum = 0;
            settingPasswordComplexity.IsScored = false;
            settingPasswordComplexity.comboBox.Text = "Enabled";
            settingResetLockoutCounterAfter.IsScored = false;
            settingResetLockoutCounterAfter.Maximum = 99999;
            settingResetLockoutCounterAfter.Minimum = 0;
            settingReversibleEncryption.IsScored = false;
            settingReversibleEncryption.comboBox.Text = "Disabled";

            // Reset Audit Policies
            // Looping through each audit policy
            foreach (ControlSettingAudit control in itemsAuditPolicy.Items.Cast<ControlSettingAudit>())
            {
                // Set control from blank scored item
                control.SetFromScoredItem(HeaderSettingPairs[control.Header]);
            }

            // Reset User Rights Assignment
            foreach (ControlSettingUserRights control in itemsUserRightsSettings.Items.Cast<ControlSettingUserRights>())
            {
                // Set IsScored to false
                control.IsScored = false;
                // Clear all set items
                control.itemsIdentifiers.Items.Clear();
                // Clear the input box
                control.comboBoxIdentifier.Text = string.Empty;
            }

            // Reset Security Options
            // Loop through each type of options and
            // Set IsScored to false
            foreach (var item in itemsSecurityOptions.Items)
            {
                string value = item.GetType().ToString();
                Console.WriteLine(value);
                value = value.Replace("Configuration_Tool.Controls.SecOptions.", "");
                if (value == "ControlRegistryComboBox")
                {
                    ((SecOptions.ControlRegistryComboBox)item).IsScored = false;
                }
                if (value == "ControlRegistryMultiLine")
                {
                    ((SecOptions.ControlRegistryMultiLine)item).IsScored = false;
                }
                if (value == "ControlRegistryRange")
                {
                    ((SecOptions.ControlRegistryRange)item).IsScored = false;
                }
                if (value == "ControlRegistryTextRegex")
                {
                    ((SecOptions.ControlRegistryTextRegex)item).IsScored = false;
                }
                if (value == "ControlSeceditComboBox")
                {
                    ((SecOptions.ControlSeceditComboBox)item).IsScored = false;
                }
                if (value == "ControlSeceditTextRegex")
                {
                    ((SecOptions.ControlSeceditTextRegex)item).IsScored = false;
                }
            }

            // Clear user configurations
            listUserConfigs.Items.Clear();

            // Clear group configurations
            listGroupConfigs.Items.Clear();

            // Clear and repopulate programs list
            listPrograms.Items.Clear();
            PopulateProgramsList();

            // Clear prohibited files configurations
            itemsProhibitedFiles.Items.Clear();
        }

        private void RemoveConfigToolCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Confirm wanting to remove configuration tool
            MessageBoxResult result = MessageBox.Show("Are you sure you want to prepare this system for competitors and remove the configuration tool?",
                "Confirmation Required", MessageBoxButton.YesNo);

            // If "Yes" wasn't pressed, return
            if (result != MessageBoxResult.Yes) return;

            // We use a batch file to remove as we can't delete ourselves as the program runs
            File.WriteAllLines("remove.bat", new string[] {
                "@echo off",
                "echo Removing Configuration Tool and other files...",
                "ping 127.0.0.1 > nul", // Waits, gives enough time for Configuration Tool to close
                "del /F \"%public%\\Desktop\\Configuration Tool.lnk\"",
                "del /F \"Configuration Tool.exe\"",
                "del remove.bat"
            });

            // Run removal program
            Process.Start("remove.bat");

            // Terminate self
            Environment.Exit(0);
        }

        private void btnOutputFileAdd_Click(object sender, RoutedEventArgs e)
        {
            // If input is empty or only whitespace
            if (string.IsNullOrWhiteSpace(txtOutputFileInput.Text))
            {
                return;
            }

            // Get trimmed (removed outer whitespace) input
            string file = txtOutputFileInput.Text.Trim();

            // We will not check if the file exists because the 
            // output file may not be created until it is scored

            // If file is already in output list
            if (ConfigurationManager.OutputFiles.Contains(file))
            {
                // Show message box dialog alerting user
                MessageBox.Show("The file attempted to be added is already in the list");
                return;
            }

            // Add file to output file list
            ConfigurationManager.OutputFiles.Add(file);
        }

        private void btnOutputFileBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            // Set filter to all files
            fileDialog.Filter = "All Files (*.*)|*.*";

            // Allow multiple selections
            fileDialog.Multiselect = true;

            // Show file dialog
            bool? chosen = fileDialog.ShowDialog();

            // If file(s) was chosen
            if (chosen.HasValue && chosen.Value)
            {
                // Get chosen files
                string[] files = fileDialog.FileNames;

                string alreadyAdded = "";

                // For each file
                foreach (string file in files)
                {
                    // Trim any whitespace
                    string trimmed = file.Trim();

                    // Check if output file is already added
                    if (ConfigurationManager.OutputFiles.Contains(trimmed))
                    {
                        // Add path to variable for a later alert
                        alreadyAdded += trimmed + Environment.NewLine;
                    }
                    else
                    {
                        // Add to list
                        ConfigurationManager.OutputFiles.Add(trimmed);
                    }
                }

                // If there were files which were already added
                if (alreadyAdded != "")
                {
                    // Trim off final newline that was added
                    alreadyAdded.TrimEnd(Environment.NewLine.ToCharArray());

                    // Show message box dialog alerting the user
                    MessageBox.Show("The following files were already added to the list of outputs:"
                        + Environment.NewLine + alreadyAdded);
                }
            }
        }

        private void btnOutputFileRemove_Click(object sender, RoutedEventArgs e)
        {
            // Get list of selected items to be removed
            IList list = listOutputFiles.SelectedItems;

            // If selected items list is empty or null, return
            if (list == null || list.Count == 0) return;

            // Cast IList to list of strings ToList() will copy the
            // list instead of making a reference, allowing removal
            List<string> files = list.Cast<string>().ToList();

            // For each selected file
            foreach (string file in files)
            {
                // Remove from list box
                ConfigurationManager.OutputFiles.Remove(file);
            }
        }

        private void PopulateProgramsList()
        {
            // Get list of programs
            List<string> programs = ControlSettingProgram.GetPrograms();

            // Loop over list of programs
            foreach (string program in programs)
            {
                ControlSettingProgram control = new ControlSettingProgram();
                control.Header = program;

                listPrograms.Items.Add(control);
            }
        }

        private void btnAddPath_Click(object sender, RoutedEventArgs e)
        {
            ControlProhibitedFile control = new ControlProhibitedFile();

            itemsProhibitedFiles.Items.Add(control);
        }

        private void SaveonExit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult saveonExit = MessageBox.Show("Would you like to save the current configuration?",
                "Save Configuration",
                MessageBoxButton.YesNo);
            if (saveonExit == MessageBoxResult.Yes)
            {
                ConfigurationManager.Save();
            }
        }

        public Dictionary<string, ScoredItem<EAuditSettings>> HeaderSettingPairs = new Dictionary<string, ScoredItem<EAuditSettings>>()
        {
            { "Audit account logon events", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit account management", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit directory service access", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit logon events", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit object access", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit policy change", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit privilege use", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit process tracking", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) },
            { "Audit system events", new ScoredItem<EAuditSettings>(EAuditSettings.Unchanged, false) }
        };
    }
}
