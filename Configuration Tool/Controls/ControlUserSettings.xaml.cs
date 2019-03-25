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
    /// Interaction logic for ControlUserSettings.xaml
    /// </summary>
    public partial class ControlUserSettings : UserControl
    {
        private string groupName = Guid.NewGuid().ToString();
        public string GroupName => groupName;

        public UserSettings Settings { get; } = new UserSettings();

        public ControlUserSettings()
        {
            InitializeComponent();
        }

        private void btnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    MainWindow mainWindow = (MainWindow)window;
                    mainWindow.listUserConfigs.Items.Remove(this);
                }
            }
        }
    }
}
