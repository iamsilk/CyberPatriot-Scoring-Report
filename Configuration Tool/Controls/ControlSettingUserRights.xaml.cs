using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
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
    /// Interaction logic for ControlSettingUserRights.xaml
    /// </summary>
    public partial class ControlSettingUserRights : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChange(string variable)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(variable));
        }

        public string Setting { get; set; } = "Default Group Policy Setting Name";

        public string UserRightsConstantName { get; set; } = "";

        private bool isScored = false;
        public bool IsScored
        {
            get { return isScored; }
            set
            {
                if (value != isScored)
                {
                    isScored = value;
                    OnChange("IsScored");
                }
            }
        }

        public static List<string> LocalMachineGroupNames = null;

        public static void GetLocalGroups()
        {
            // If list is null, local groups already loaded
            if (LocalMachineGroupNames != null) return;

            LocalMachineGroupNames = new List<string>();

            // Create instance for communicating with active directory
            using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
            {
                // Create searcher for active directory
                using (PrincipalSearcher searcher = new PrincipalSearcher(new GroupPrincipal(context)))
                {
                    // For each group on the machine
                    foreach (GroupPrincipal group in searcher.FindAll())
                    {
                        LocalMachineGroupNames.Add(group.Name);
                    }
                }
            }
        }

        private void constructor()
        {
            InitializeComponent();

            GetLocalGroups();

            // For each group name on local machine
            foreach (string group in LocalMachineGroupNames)
            {
                comboBoxIdentifier.Items.Add(group);
            }
        }

        public ControlSettingUserRights()
        {
            constructor();
        }

        public ControlSettingUserRights(string setting)
        {
            Setting = setting;

            constructor();
        }

        private void btnAddName_Click(object sender, RoutedEventArgs e)
        {
            // Create user rights name control
            ControlSettingUserRightsName control = new ControlSettingUserRightsName();

            // Set text of control to specified text
            control.Identifier = comboBoxIdentifier.Text;

            // Add to item control
            itemsIdentifiers.Items.Add(control);
        }

        private void btnAddSID_Click(object sender, RoutedEventArgs e)
        {
            // Create user rights SID control
            ControlSettingUserRightsSID control = new ControlSettingUserRightsSID();

            // Set text of control to specified text
            control.Identifier = comboBoxIdentifier.Text;

            // Add to item control
            itemsIdentifiers.Items.Add(control);
        }
    }
}
