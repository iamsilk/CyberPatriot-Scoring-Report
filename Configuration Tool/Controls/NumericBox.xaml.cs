using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Configuration_Tool.Controls
{
    /// <summary>
    /// Interaction logic for NumericBox.xaml
    /// </summary>
    public partial class NumericBox : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChange(string variable)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(variable));
        }

        private int number = 0;
        public int Number
        {
            get { return number; }
            set
            {
                if (number != value)
                {
                    number = value;
                    OnChange("Number");
                }
            }
        }
        
        public int Minimum { get; set; } = int.MinValue;
        
        public int Maximum { get; set; } = int.MaxValue;

        public NumericBox()
        {
            InitializeComponent();

            DataContext = this;
        }

        public NumericBox(int num)
        {
            Number = num;
            InitializeComponent();
        }

        public NumericBox(int num, int min, int max)
        {
            Number = num;
            Minimum = min;
            Maximum = max;

            Number = WrapNumber(Number);

            InitializeComponent();
        }

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            if (Number < Maximum) Number++;
        }

        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            if (Number > Minimum) Number--;
        }

        private int WrapNumber(int num)
        {
            if (num > Maximum) return Maximum;

            if (num < Minimum) return Minimum;

            return num;
        }

        private void txtNumber_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int num = 0;
            
            bool parsed = int.TryParse(txtNumber.Text, out num);

            if (!parsed || num > Maximum || num < Minimum)
            {
                e.Handled = true;

                MessageBox.Show("Inserted value is not with valid bounds. Please specify a valid value.");
            }
        }
    }
}
