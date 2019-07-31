using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration_Tool.Controls;

namespace Configuration_Tool.Configuration.Sections
{
    class ConfigShares : IConfig
    {
        public EConfigType Type => EConfigType.Shares;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            MainWindow.itemsShares.Items.Clear();

            // Get count of scored shares
            int sharecount = reader.ReadInt32();

            for (int i = 0; i < sharecount; i++)
            {
                // Get Share Name
                string sharename = reader.ReadString();

                // Get scoring status
                bool isScored = reader.ReadBoolean();

                // Score if exists
                bool exists = reader.ReadBoolean();

                ControlShares control = new ControlShares();

                control.Share = sharename;
                control.IsScored = isScored;
                control.Exists = exists;

                MainWindow.itemsShares.Items.Add(control);
            }
        }
        public void Save(BinaryWriter writer)
        {
            // Get/write number of prohibited files
            int count = MainWindow.itemsShares.Items.Count;
            writer.Write(count);

            // Loop over each prohibited file
            foreach (ControlShares control in MainWindow.itemsShares.Items)
            {
                // Write paths
                writer.Write(control.Share);

                writer.Write(control.IsScored);

                writer.Write(control.Exists);
            }
        }
    }
}
