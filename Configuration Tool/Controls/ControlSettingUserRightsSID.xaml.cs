using Configuration_Tool.Configuration;
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
    /// Interaction logic for ControlSettingUserRightsName.xaml
    /// </summary>
    public partial class ControlSettingUserRightsSID : UserControl, IUserRightsIdentifier, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChange(string variable)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(variable));
        }

        public ControlSettingUserRightsSID()
        {
            InitializeComponent();
        }

        public ControlSettingUserRightsSID(string _identifier)
        {
            identifier = _identifier;

            InitializeComponent();
        }

        public EUserRightsIdentifierType Type => EUserRightsIdentifierType.SecurityID;

        private string identifier = "";
        public string Identifier
        {
            get { return identifier; }
            set
            {
                if (identifier != value)
                {
                    identifier = value;
                    OnChange("Identifier");
                }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Gets items control containing self
            ItemsControl itemsContainer = (ItemsControl)this.Parent;

            // Removes self
            itemsContainer.Items.Remove(this);
        }
    }
}
