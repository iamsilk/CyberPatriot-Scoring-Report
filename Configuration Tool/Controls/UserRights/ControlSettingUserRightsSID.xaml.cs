using Configuration_Tool.Configuration.UserRights;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.UserRights
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
