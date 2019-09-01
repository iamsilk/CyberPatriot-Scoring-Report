using Configuration_Tool.Configuration.Users;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.Users
{
    /// <summary>
    /// Interaction logic for ControlUserSettings.xaml
    /// </summary>
    public partial class ControlUserSettings : UserControl
    {
        private string groupName = Guid.NewGuid().ToString();
        public string GroupName => groupName;

        public UserSettings Settings { get; } = new UserSettings();

        public ControlUserSettings(UserSettings settings = null)
        {
            if (settings != null) Settings = settings;

            InitializeComponent();

            DataContext = this;
        }

        private void btnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    MainWindow mainWindow = (MainWindow)window;
                    mainWindow.itemsUserConfig.Items.Remove(this);
                }
            }
        }
    }
}
