using Configuration_Tool.Configuration.Groups;
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
        public GroupSettings Settings { get; } = new GroupSettings();

        public ControlGroupSettings(GroupSettings settings = null)
        {
            InitializeComponent();

            if (settings != null)
            {
                Settings = settings;

                // for each member in settings
                foreach (IMember member in Settings.Members)
                {
                    UserControl control;

                    // Get control based on member's id type
                    if (member is MemberSID)
                    {
                        control = new ControlGroupSIDSetting(member as MemberSID);
                    }
                    else if (member is MemberUsername)
                    {
                        control = new ControlGroupUsernameSetting(member as MemberUsername);
                    }
                    else
                    {
                        // How'd we get here...
                        throw new Exception(string.Format("Unknown member identifier type ({0}) for member ({1}) in group ({2})",
                            member.IDType, member.Identifier, settings.GroupName));
                    }

                    // Create container before adding to items control to apply default style
                    Grid container = new Grid();

                    // Add control to grid
                    container.Children.Add(control);

                    // Add grid to items control
                    listUserConfigs.Items.Add(container);
                }
            }
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
