using Configuration_Tool.Controls.Comparisons;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Configuration_Tool.Controls.CustomFiles
{
    /// <summary>
    /// Interaction logic for ControlCustomFile.xaml
    /// </summary>
    public partial class ControlCustomFile : System.Windows.Controls.UserControl
    {
        public ControlCustomFile()
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

        public ItemCollection Comparisons => itemsComparisons.Items;

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

        public string CustomOutput
        {
            get { return txtCustomOutput.Text; }
            set
            {
                txtCustomOutput.Text = value;
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

        public static ControlCustomFile Parse(BinaryReader reader)
        {
            // Create instance
            ControlCustomFile customFile = new ControlCustomFile();

            customFile.Path = reader.ReadString();
            customFile.Matches = reader.ReadBoolean();

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
                }

                comparison.Load(reader);

                // Add comparison to list
                customFile.Comparisons.Add(comparison);
            }

            // Read scoring values
            customFile.CustomOutput = reader.ReadString();
            customFile.IsScored = reader.ReadBoolean();

            return customFile;
        }

        public void Write(BinaryWriter writer)
        {
            // Write properties
            writer.Write(Path);
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

        private void btnAddSimple_Click(object sender, RoutedEventArgs e)
        {
            itemsComparisons.Items.Add(new ControlComparisonSimple());
        }

        private void btnAddRegex_Click(object sender, RoutedEventArgs e)
        {
            itemsComparisons.Items.Add(new ControlComparisonRegex());
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Get items control parent
            ItemsControl parent = (ItemsControl)this.Parent;

            // Remove self from items control
            parent.Items.Remove(this);
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
    }
}
