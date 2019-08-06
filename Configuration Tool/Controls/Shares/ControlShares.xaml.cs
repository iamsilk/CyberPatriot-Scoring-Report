using System.Windows;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.Shares
{
    /// <summary>
    /// Interaction logic for ControlShares.xaml
    /// </summary>
    public partial class ControlShares : System.Windows.Controls.UserControl
    {
        public ControlShares()
        {
            InitializeComponent();
        }


        public string Share
        {
            get { return sharePath.Text; }
            set
            {
                sharePath.Text = value;
            }
        }

        public bool Exists
        {
            get
            {
                if (!shareExists.IsChecked.HasValue) return false;
                return shareExists.IsChecked.Value;
            }
            set
            {
                shareExists.IsChecked = value;
            }
        }

        public bool IsScored
        {
            get
            {
                if (!isScored.IsChecked.HasValue) return false;
                return isScored.IsChecked.Value;
            }
            set
            {
                isScored.IsChecked = value;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)this.Parent;

            itemsControl.Items.Remove(this);
        }
    }
}
