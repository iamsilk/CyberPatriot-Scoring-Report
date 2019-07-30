using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Configuration_Tool.Controls
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

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)this.Parent;

            itemsControl.Items.Remove(this);
        }
    }
}
