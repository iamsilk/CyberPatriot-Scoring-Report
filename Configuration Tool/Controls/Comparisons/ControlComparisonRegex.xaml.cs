using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.Comparisons
{
    /// <summary>
    /// Interaction logic for ControlComparisonRegex.xaml
    /// </summary>
    public partial class ControlComparisonRegex : UserControl, IComparison
    {
        public EComparison Type => EComparison.Regex;

        public string Pattern
        {
            get { return txtRegexMatch.Text; }
            set
            {
                txtRegexMatch.Text = value;
            }
        }

        public RegexOptions Options = RegexOptions.None;

        public void Load(BinaryReader reader)
        {
            Pattern = reader.ReadString();
            Options = (RegexOptions)reader.ReadInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Pattern);
            writer.Write((Int32)Options);
        }

        public ControlComparisonRegex()
        {
            InitializeComponent();
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
