using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

namespace Translation_Editor
{
    public static class CustomCommands
    {
        public static readonly RoutedCommand FileOpen = new RoutedCommand("FileOpen", typeof(CustomCommands));
        public static readonly RoutedCommand FileSave = new RoutedCommand("FileSave", typeof(CustomCommands));
        public static readonly RoutedCommand FileSaveAs = new RoutedCommand("FileSaveAs", typeof(CustomCommands));
        public static readonly RoutedCommand ClearTranslations = new RoutedCommand("ClearTranslations", typeof(CustomCommands));
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

            // Check if startup parameter is '--write-defaults'
            if (startupParameter == "--write-defaults")
            {
                TranslationManager.Save();

                Environment.Exit(0);
                return;
            }

            TranslationManager.Startup(startupParameter);
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

            // Check if changes have been saved. If user clicks cancel, return
            if (TranslationManager.CheckSavingChanges(this)) return;

            // Attempt to load chosen file
            TranslationManager.LoadTranslations(filePath);
        }

        private void FileSaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Saves at current/default translations path
            TranslationManager.Save();
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
            TranslationManager.Save(filePath);
        }

        private void ClearTranslationsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Confirm clearing current translations
            MessageBoxResult clearTranslationsOk = MessageBox.Show("Confirm clearing current translations\nNOTE: This will reset EVERYTHING!",
                "Clear Current Translations",
                MessageBoxButton.OKCancel);
            // Return if user did not press ok
            if (clearTranslationsOk != MessageBoxResult.OK) return;

            // Load default translations
            TranslationManager.LoadDefaults();
        }

        private void SaveonExit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TranslationManager.CheckSavingChanges(this))
            {
                e.Cancel = true;
            }
        }
    }
}
