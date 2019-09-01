using Configuration_Tool.Configuration.Groups;
using Configuration_Tool.Controls;
using Configuration_Tool.Controls.Groups;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigGroups : IConfig
    {
        public EConfigType Type => EConfigType.Groups;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Clear current list of group settings
            MainWindow.itemsGroupConfigs.Items.Clear();

            // Get number of group settings instances
            int count = reader.ReadInt32();

            // Enumerate every instance of group settings
            for (int i = 0; i < count; i++)
            {
                // Get instance of group settings
                GroupSettings settings = GroupSettings.Parse(reader);

                // Create control from settings
                ControlGroupSettings control = new ControlGroupSettings(settings);

                // Add instance to group settings items control
                MainWindow.itemsGroupConfigs.Items.Add(control);
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Get number of group settings instances and write
            int count = MainWindow.itemsGroupConfigs.Items.Count;
            writer.Write(count);

            // For each group settings control
            foreach (ControlGroupSettings control in MainWindow.itemsGroupConfigs.Items)
            {
                // Get group settings instance and write
                GroupSettings settings = control.Settings;

                // Write group settings to stream
                settings.Write(writer);
            }
        }
    }
}
