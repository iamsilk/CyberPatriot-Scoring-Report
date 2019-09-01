using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.Comparisons
{
    /// <summary>
    /// Interaction logic for ControlComparisonSimple.xaml
    /// </summary>
    public partial class ControlComparisonSimple : UserControl, IComparison
    {
        public EComparison Type => EComparison.Simple;

        public string Value
        {
            get { return txtTextMatch.Text; }
            set
            {
                txtTextMatch.Text = value;
            }
        }

        public void Load(BinaryReader reader)
        {
            Value = reader.ReadString();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        public ControlComparisonSimple()
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
