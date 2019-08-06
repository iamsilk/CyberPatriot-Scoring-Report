using Configuration_Tool.Configuration.Groups;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.Groups
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

            // Create member
            MemberSID member = new MemberSID();

            // Add member to group setting's list
            Settings.Members.Add(member);

            // Create SID setting element
            ControlGroupSIDSetting control = new ControlGroupSIDSetting(member);

            // Add element to grid (to apply default style)
            grid.Children.Add(control);

            // Add element to user config items control
            listUserConfigs.Items.Add(grid);
        }

        private void btnAddUsername_Click(object sender, RoutedEventArgs e)
        {
            // Create containing grid
            Grid grid = new Grid();

            // Create member
            MemberUsername member = new MemberUsername();

            // Add member to group setting's list
            Settings.Members.Add(member);

            // Create Username setting element
            ControlGroupUsernameSetting control = new ControlGroupUsernameSetting(member);

            // Add element to grid (to apply default style)
            grid.Children.Add(control);

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
