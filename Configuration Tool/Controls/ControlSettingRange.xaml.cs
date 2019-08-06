using Configuration_Tool.Configuration;
using System.Windows.Controls;

namespace Configuration_Tool.Controls
{
    /// <summary>
    /// Interaction logic for ControlSettingRange.xaml
    /// </summary>
    public partial class ControlSettingRange : UserControl
    {
        public string Header
        {
            get
            {
                return txtHeader.Text;
            }
            set
            {
                txtHeader.Text = value;
            }
        }

        public bool IsScored
        {
            get
            {
                if (!checkBoxScored.IsChecked.HasValue) return false;

                return checkBoxScored.IsChecked.Value;
            }
            set
            {
                checkBoxScored.IsChecked = value;
            }
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

        public ScoredItem<Range> GetScoredItem()
        {
            return new ScoredItem<Range>(new Range(Minimum, Maximum), IsScored);
        }

        public void SetFromScoredItem(ScoredItem<Range> scoredItem)
        {
            Maximum = scoredItem.Value.Max;
            Minimum = scoredItem.Value.Min;
            IsScored = scoredItem.IsScored;
        }
        public ControlSettingRange()
        {
            InitializeComponent();

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
    }
}
