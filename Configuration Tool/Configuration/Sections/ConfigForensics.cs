using Configuration_Tool.Controls;
using Configuration_Tool.Controls.Forensics;
using System.IO;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigForensics : IConfig
    {
        public EConfigType Type => EConfigType.Forensics;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Clear list of questions
            MainWindow.itemsForensicQuestions.Items.Clear();

            // Read count
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get forensic question control
                ControlForensicQuestion control = ControlForensicQuestion.Parse(reader);

                // Add control to list
                MainWindow.itemsForensicQuestions.Items.Add(control);
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Write count
            writer.Write(MainWindow.itemsForensicQuestions.Items.Count);

            // Write each forensic question
            foreach (ControlForensicQuestion control in MainWindow.itemsForensicQuestions.Items)
                control.Write(writer);
        }
    }
}
