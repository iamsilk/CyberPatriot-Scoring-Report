using Configuration_Tool.Configuration;
using System.Windows.Controls;

namespace Configuration_Tool.Controls
{
    /// <summary>
    /// Interaction logic for ControlSettingNumeric.xaml
    /// </summary>
    public partial class ControlSettingNumeric : UserControl
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

        public int Number
        {
            get
            {
                return numericBox.Number;
            }
            set
            {
                numericBox.Number = value;
            }
        }

        public int Maximum
        {
            get
            {
                return numericBox.Maximum;
            }
            set
            {
                numericBox.Maximum = value;
            }
        }

        public int Minimum
        {
            get
            {
                return numericBox.Minimum;
            }
            set
            {
                numericBox.Minimum = value;
            }
        }

        public ScoredItem<int> GetScoredItem()
        {
            return new ScoredItem<int>(Number, IsScored);
        }

        public void SetFromScoredItem(ScoredItem<int> scoredItem)
        {
            Number = scoredItem.Value;
            IsScored = scoredItem.IsScored;
        }

        public ControlSettingNumeric()
        {
            InitializeComponent();
        }
    }
}
