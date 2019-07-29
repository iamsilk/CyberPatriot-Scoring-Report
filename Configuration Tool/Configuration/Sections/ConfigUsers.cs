using Configuration_Tool.Controls;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigUsers : IConfig
    {
        public EConfigType Type => EConfigType.Users;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Clear current list of user settings
            MainWindow.listUserConfigs.Items.Clear();

            // Number of user settings instances
            int count = reader.ReadInt32();

            // For each user settings instance
            for (int i = 0; i < count; i++)
            {
                // Parse user settings instance from binary reader
                UserSettings settings = UserSettings.Parse(reader);

                // Create control from settings
                ControlUserSettings control = new ControlUserSettings(settings);

                // Add user settings to user settings items control
                MainWindow.listUserConfigs.Items.Add(control);
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Get number of user settings instances and write
            int count = MainWindow.listUserConfigs.Items.Count;
            writer.Write(count);

            // For each user settings control
            foreach (ControlUserSettings control in MainWindow.listUserConfigs.Items)
            {
                // Get user settings instance
                UserSettings settings = control.Settings;

                // Write user settings to stream
                settings.Write(writer);
            }
        }
    }
}
