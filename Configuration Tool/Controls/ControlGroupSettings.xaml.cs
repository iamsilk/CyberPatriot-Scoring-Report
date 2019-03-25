using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Configuration_Tool.Controls
{
    /// <summary>
    /// Interaction logic for ControlGroupSettings.xaml
    /// </summary>
    public partial class ControlGroupSettings : UserControl
    {
        public ControlGroupSettings()
        {
            InitializeComponent();
        }

        private void btnAddSID_Click(object sender, RoutedEventArgs e)
        {
            // Create containing grid
            Grid grid = new Grid();

            // Create SID setting element
            grid.Children.Add(new ControlGroupSIDSetting());

            // Add element to user config items control
            listUserConfigs.Items.Add(grid);
        }

        private void btnAddUsername_Click(object sender, RoutedEventArgs e)
        {
            // Create containing grid
            Grid grid = new Grid();

            // Create Username setting element
            grid.Children.Add(new ControlGroupUsernameSetting());

            // Add element to user config items control
            listUserConfigs.Items.Add(grid);
        }

        private void btnRemoveConfig_Click(object sender, RoutedEventArgs e)
        {
            // Get parent container (items control element)
            ItemsControl parentContainer = (ItemsControl)this.Parent;

            // Remove self from parent container
            parentContainer.Items.Remove(this);
        }
    }
}
