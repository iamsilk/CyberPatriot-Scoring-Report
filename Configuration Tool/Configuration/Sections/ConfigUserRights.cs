using Configuration_Tool.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigUserRights : IConfig
    {
        public EConfigType Type => EConfigType.UserRights;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Get count of user rights definitions
            int count = reader.ReadInt32();

            // For number of user rights definitions
            for (int i = 0; i < count; i++)
            {
                // Get constant name
                string constantName = reader.ReadString();

                // Get setting name, only used within scoring report
                string setting = reader.ReadString();

                // Try to find control corresponding to constant name

                // Check first if index of config corresponds to index of item control (so we don't have to loop)
                // This is implemented to allow easier fixations if user rights definitions on windows change in the future
                ControlSettingUserRights control = (ControlSettingUserRights)MainWindow.itemsUserRightsSettings.Items[i];

                bool found = true;

                if (control.UserRightsConstantName != constantName)
                {
                    // Config and index do not correspond, loop items control to try to find correct control
                    found = false;

                    foreach (ControlSettingUserRights check in MainWindow.itemsUserRightsSettings.Items)
                    {
                        if (check.UserRightsConstantName == constantName)
                        {
                            control = check;
                            found = true;
                            break;
                        }
                    }
                }

                // If control wasn't found, create new one to still allow usability of program
                if (!found)
                {
                    control = new ControlSettingUserRights(constantName);
                    control.UserRightsConstantName = constantName;

                    // Add control to items control
                    MainWindow.itemsUserRightsSettings.Items.Add(control);
                }

                // Get and set scoring status
                bool isScored = reader.ReadBoolean();
                control.IsScored = isScored;

                // Get number of identifiers
                int identifiersCount = reader.ReadInt32();

                // For number of identifiers
                for (int j = 0; j < identifiersCount; j++)
                {
                    // Get identifier type and identifier
                    EUserRightsIdentifierType type = (EUserRightsIdentifierType)reader.ReadInt32();
                    string identifier = reader.ReadString();

                    object identifierControl = null;

                    // Create proper control based on type
                    switch (type)
                    {
                        case EUserRightsIdentifierType.Name:
                            identifierControl = new ControlSettingUserRightsName(identifier);
                            break;
                        case EUserRightsIdentifierType.SecurityID:
                            identifierControl = new ControlSettingUserRightsSID(identifier);
                            break;
                    }

                    // Add control to items control
                    control.itemsIdentifiers.Items.Add(identifierControl);
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Get and write count of user rights definitions
            int count = MainWindow.itemsUserRightsSettings.Items.Count;
            writer.Write(count);

            // For each user rights setting control in items control
            foreach (ControlSettingUserRights control in MainWindow.itemsUserRightsSettings.Items)
            {
                // Get/write constant name
                string constantName = control.UserRightsConstantName;
                writer.Write(constantName);

                // Get/write setting (displayed title)
                string setting = control.Setting;
                writer.Write(setting);

                // Get/write boolean specifying scoring status
                bool isScored = control.IsScored;
                writer.Write(isScored);

                // Get identifications for each member of user rights definition
                // We don't bother converting to a list as it's unnecessary and unoptimized
                IEnumerable<IUserRightsIdentifier> identifiers = control.itemsIdentifiers.Items.Cast<IUserRightsIdentifier>();

                // Write number of identifiers
                writer.Write(identifiers.Count());

                // For each identifier in list, write identifier type and identifier
                foreach (IUserRightsIdentifier identifier in identifiers)
                {
                    writer.Write((Int32)identifier.Type);
                    writer.Write(identifier.Identifier);
                }
            }
        }
    }
}
