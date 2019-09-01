using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.Comparisons
{
    /// <summary>
    /// Interaction logic for ControlComparisonRange.xaml
    /// </summary>
    public partial class ControlComparisonRange : UserControl, IComparison
    {
        public EComparison Type => EComparison.Range;

        public void Load(BinaryReader reader)
        {
            Minimum = reader.ReadInt32();
            Maximum = reader.ReadInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Minimum);
            writer.Write(Maximum);
        }

        public int Maximum
        {
            get
            {
                return numericMax.Number;
            }
            set
            {
                numericMin.Maximum = value;
                numericMax.Number = value;
            }
        }

        public int Minimum
        {
            get
            {
                return numericMin.Number;
            }
            set
            {
                numericMax.Minimum = value;
                numericMin.Number = value;
            }
        }

        public int TotalMaximum
        {
            get
            {
                return numericMax.Maximum;
            }
            set
            {
                numericMax.Maximum = value;
            }
        }

        public int TotalMinimum
        {
            get
            {
                return numericMin.Minimum;
            }
            set
            {
                numericMin.Minimum = value;
            }
        }

        public ControlComparisonRange()
        {
            InitializeComponent();

            numericMin.Minimum = int.MinValue;
            numericMax.Maximum = int.MaxValue;

            numericMax.Minimum = Minimum;
            numericMin.Maximum = Maximum;

            numericMin.PropertyChanged += NumericMin_PropertyChanged;
            numericMax.PropertyChanged += NumericMax_PropertyChanged;
        }

        private void NumericMin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Number")
            {
                numericMax.Minimum = Minimum;
            }
        }

        private void NumericMax_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Number")
            {
                numericMin.Maximum = Maximum;
            }
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
