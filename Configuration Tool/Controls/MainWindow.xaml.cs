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

            if (commandLineArgs.Length != 0)
            {
                startupParameter = commandLineArgs[0];
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
    }
}
