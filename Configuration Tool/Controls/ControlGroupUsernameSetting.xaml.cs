using Configuration_Tool.Configuration.Groups;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Configuration_Tool.Controls
{
    /// <summary>
    /// Interaction logic for ControlGroupUsernameSetting.xaml
    /// </summary>
    public partial class ControlGroupUsernameSetting : UserControl
    {
        public MemberUsername Member { get; } = new MemberUsername();

        public ControlGroupUsernameSetting(MemberUsername member = null)
        {
            InitializeComponent();

            if (member != null) Member = member;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Gets parent grid
            Grid container = (Grid)this.Parent;

            // Gets items control containing grid
            ItemsControl itemsContainer = (ItemsControl)container.Parent;

            // Removes self
            itemsContainer.Items.Remove(container);

            // Parent of ItemsControl > ScrollViewer
            // Parent of ScrollViewer > Grid2
            // Parent of Grid2 > Grid1
            // Parent of Grid1 > ControlGroupSettings
            ControlGroupSettings control =
                (ControlGroupSettings)((Grid)((Grid)((ScrollViewer)itemsContainer.Parent).Parent).Parent).Parent;

            // Remove self from configuration
            control.Settings.Members.Remove(Member);
        }
    }
}
