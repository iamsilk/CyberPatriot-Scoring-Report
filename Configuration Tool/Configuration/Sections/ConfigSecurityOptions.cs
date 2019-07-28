using Configuration_Tool.Controls;
using Configuration_Tool.Controls.SecOptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigSecurityOptions : IConfig
    {
        public EConfigType Type => EConfigType.SecurityOptions;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Get number of security option settings
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get type of sec option
                ESecurityOptionType type = (ESecurityOptionType)reader.ReadInt32();

                ISecurityOption secOption = null;

                // Initialize secOption based on type
                switch (type)
                {
                    case ESecurityOptionType.RegistryComboBox:
                        secOption = new ControlRegistryComboBox();
                        break;
                    case ESecurityOptionType.RegistryTextRegex:
                        secOption = new ControlRegistryTextRegex();
                        break;
                    case ESecurityOptionType.RegistryRange:
                        secOption = new ControlRegistryRange();
                        break;
                    case ESecurityOptionType.RegistryMultiLine:
                        secOption = new ControlRegistryMultiLine();
                        break;
                    case ESecurityOptionType.SeceditComboBox:
                        secOption = new ControlSeceditComboBox();
                        break;
                    case ESecurityOptionType.SeceditTextRegex:
                        secOption = new ControlSeceditTextRegex();
                        break;
                }

                if (secOption == null)
                {
                    // Uh oh.. corrupted file?
                    throw new Exception("Unknown security option type. Possible configuration file corruption.");
                }

                // Parse information based on type
                secOption.Parse(reader);

                string identifier = secOption.Identifier();

                // Search for matching control to copy information to
                foreach (ISecurityOption control in MainWindow.itemsSecurityOptions.Items.OfType<ISecurityOption>())
                {
                    // Check if control is a match
                    if (type == control.Type && identifier == control.Identifier())
                    {
                        // Copy information to displayed control
                        secOption.CopyTo(control);

                        // Found match, stop searching
                        break;
                    }
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            IEnumerable<ISecurityOption> items = MainWindow.itemsSecurityOptions.Items.OfType<ISecurityOption>();

            // Write number of security option settings
            writer.Write(items.Count());

            // Loop over every element of items control and save
            foreach (ISecurityOption secOption in items)
            {
                // Write type and interface
                writer.Write((Int32)secOption.Type);
                secOption.Write(writer);
            }
        }
    }
}
