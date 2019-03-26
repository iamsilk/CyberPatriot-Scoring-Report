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
using Configuration_Tool.Configuration;

namespace Configuration_Tool.Controls
{
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

            ConfigurationManager.Startup(startupParameter);
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

        private void btnFileOpen_Click(object sender, RoutedEventArgs e)
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

        private void btnFileSave_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.Save();
        }
    }
}
