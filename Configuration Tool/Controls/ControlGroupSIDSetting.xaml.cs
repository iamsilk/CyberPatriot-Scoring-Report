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
    /// Interaction logic for ControlGroupSIDSetting.xaml
    /// </summary>
    public partial class ControlGroupSIDSetting : UserControl
    {
        public MemberSID Member { get; } = new MemberSID();

        public ControlGroupSIDSetting(MemberSID member = null)
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
        }
    }
}
