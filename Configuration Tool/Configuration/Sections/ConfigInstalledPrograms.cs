using Configuration_Tool.Controls;
using Configuration_Tool.Controls.Programs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigInstalledPrograms : IConfig
    {
        public EConfigType Type => EConfigType.InstalledPrograms;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Get count of program configs
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get info of program config
                string header = reader.ReadString();
                bool installed = reader.ReadBoolean();

                // Search for control with header matching config
                ControlSettingProgram control = MainWindow.itemsPrograms.Items.Cast<ControlSettingProgram>()
                    .FirstOrDefault(x => x.Header == header);

                // If no control was found
                if (control == null)
                {
                    // Create new instance of program
                    control = new ControlSettingProgram();

                    // Set properties
                    control.Header = header;
                    control.Installed = installed;

                    // All written programs are scored
                    control.IsScored = true;

                    MainWindow.itemsPrograms.Items.Add(control);
                }
                else
                {
                    // Set other configs
                    control.IsScored = true;
                    control.Installed = installed;
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Get list of controls and filter off unscored
            IEnumerable<ControlSettingProgram> programs = MainWindow.itemsPrograms.Items.Cast<ControlSettingProgram>()
                .Where(x => x.IsScored);

            // Write number of programs
            writer.Write(programs.Count());

            // Loop over and write each programs details
            foreach (ControlSettingProgram program in programs)
            {
                writer.Write(program.Header);
                writer.Write(program.Installed);
            }
        }
    }
}
