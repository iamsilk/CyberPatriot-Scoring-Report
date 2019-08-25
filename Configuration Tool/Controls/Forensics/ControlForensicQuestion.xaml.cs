using Configuration_Tool.Configuration.Forensics;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Configuration_Tool.Controls.Forensics
{
    /// <summary>
    /// Interaction logic for ControlForensicQuestion.xaml
    /// </summary>
    public partial class ControlForensicQuestion : System.Windows.Controls.UserControl
    {
        public ControlForensicQuestion()
        {
            InitializeComponent();
        }

        public string Path
        {
            get { return txtPath.Text; }
            set
            {
                txtPath.Text = value;
            }
        }

        public string AnswerPrefix
        {
            get { return txtAnswerPrefix.Text; }
            set
            {
                txtAnswerPrefix.Text = value;
            }
        }

        public string Title
        {
            get { return txtTitle.Text; }
            set
            {
                txtTitle.Text = value;
            }
        }

        private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.FileName = Path;
                dialog.Filter = "All Files|*.*";
                dialog.Multiselect = false;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    Path = dialog.FileName;
                }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)this.Parent;

            itemsControl.Items.Remove(this);
        }

        private void btnAddTextMatch_Click(object sender, RoutedEventArgs e)
        {
            ControlAnswerText control = new ControlAnswerText();

            listAnswers.Items.Add(control);
        }

        private void btnAddRegexMatch_Click(object sender, RoutedEventArgs e)
        {
            ControlAnswerRegex control = new ControlAnswerRegex();

            listAnswers.Items.Add(control);
        }

        public static ControlForensicQuestion Parse(BinaryReader reader)
        {
            ControlForensicQuestion control = new ControlForensicQuestion();

            control.Path = reader.ReadString();
            control.AnswerPrefix = reader.ReadString();
            control.Title = reader.ReadString();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                IAnswer answer = null;

                EAnswerType type = (EAnswerType)reader.ReadInt32();
                string info = reader.ReadString();

                switch (type)
                {
                    case EAnswerType.Text:
                        answer = new ControlAnswerText();
                        break;
                    case EAnswerType.Regex:
                        answer = new ControlAnswerRegex();
                        break;
                }

                if (answer == null) continue;

                answer.Info = info;

                control.listAnswers.Items.Add(answer);
            }

            return control;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Path);
            writer.Write(AnswerPrefix);
            writer.Write(Title);

            writer.Write(listAnswers.Items.Count);

            foreach (IAnswer answer in listAnswers.Items)
            {
                writer.Write((Int32)answer.Type);
                writer.Write(answer.Info);
            }
        }
    }
}
