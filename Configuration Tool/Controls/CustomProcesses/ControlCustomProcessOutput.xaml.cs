using Configuration_Tool.Controls.Comparisons;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Configuration_Tool.Controls.CustomProcesses
{
    /// <summary>
    /// Interaction logic for ControlCustomProcessOutput.xaml
    /// </summary>
    public partial class ControlCustomProcessOutput : System.Windows.Controls.UserControl
    {
        public string CustomOutput
        {
            get { return txtCustomOutput.Text; }
            set
            {
                txtCustomOutput.Text = value;
            }
        }

        public string Path
        {
            get { return txtPath.Text; }
            set
            {
                txtPath.Text = value;
            }
        }

        public string Arguments
        {
            get { return txtArguments.Text; }
            set
            {
                txtArguments.Text = value;
            }
        }

        public int Timeout
        {
            get { return numericTimeout.Number; }
            set
            {
                numericTimeout.Number = value;
            }
        }

        public bool Matches
        {
            get
            {
                if (!checkBoxMatches.IsChecked.HasValue)
                    return false;

                return checkBoxMatches.IsChecked.Value;
            }
            set
            {
                checkBoxMatches.IsChecked = value;
            }
        }

        public bool IsScored
        {
            get
            {
                if (!checkBoxIsScored.IsChecked.HasValue)
                    return false;

                return checkBoxIsScored.IsChecked.Value;
            }
            set
            {
                checkBoxIsScored.IsChecked = value;
            }
        }

        public ItemCollection Comparisons => itemsComparisons.Items;

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

        public ControlCustomProcessOutput()
        {
            InitializeComponent();
        }

        public static ControlCustomProcessOutput Parse(BinaryReader reader)
        {
            // Create instance of policy storage
            ControlCustomProcessOutput processOutput = new ControlCustomProcessOutput();

            processOutput.Path = reader.ReadString();
            processOutput.Arguments = reader.ReadString();
            processOutput.Timeout = reader.ReadInt32();

            // Read comparison values
            processOutput.Matches = reader.ReadBoolean();

            int comparisonCount = reader.ReadInt32();

            for (int i = 0; i < comparisonCount; i++)
            {
                IComparison comparison = null;

                EComparison type = (EComparison)reader.ReadInt32();
                switch (type)
                {
                    case EComparison.Simple:
                        comparison = new ControlComparisonSimple();
                        break;
                    case EComparison.Regex:
                        comparison = new ControlComparisonRegex();
                        break;
                    case EComparison.Range:
                        comparison = new ControlComparisonRange();
                        break;
                }

                comparison.Load(reader);

                // Add comparison to list
                processOutput.Comparisons.Add(comparison);
            }

            // Read scoring values
            processOutput.CustomOutput = reader.ReadString();
            processOutput.IsScored = reader.ReadBoolean();

            return processOutput;
        }

        public void Write(BinaryWriter writer)
        {
            // Write properties
            writer.Write(Path);
            writer.Write(Arguments);
            writer.Write(Timeout);

            writer.Write(Matches);

            writer.Write(Comparisons.Count);
            foreach (IComparison comparison in Comparisons)
            {
                writer.Write((Int32)comparison.Type);
                comparison.Write(writer);
            }

            writer.Write(CustomOutput);
            writer.Write(IsScored);
        }

        private void btnAddTextMatch_Click(object sender, RoutedEventArgs e)
        {
            itemsComparisons.Items.Add(new ControlComparisonSimple());
        }

        private void btnAddRegexMatch_Click(object sender, RoutedEventArgs e)
        {
            itemsComparisons.Items.Add(new ControlComparisonRegex());
        }

        private void btnAddExitCodeRange_Click(object sender, RoutedEventArgs e)
        {
            itemsComparisons.Items.Add(new ControlComparisonRange());
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Get items control parent
            ItemsControl parent = (ItemsControl)this.Parent;

            // Remove self from items control
            parent.Items.Remove(this);
        }
    }
}
