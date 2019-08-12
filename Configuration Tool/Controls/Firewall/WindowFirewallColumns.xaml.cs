using System.Windows;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.Firewall
{
    /// <summary>
    /// Interaction logic for WindowFirewallColumns.xaml
    /// </summary>
    public partial class WindowFirewallColumns : Window
    {
        private DataGrid targetDataGrid;

        public WindowFirewallColumns(DataGrid target)
        {
            InitializeComponent();

            targetDataGrid = target;

            PopulateLists();
        }

        private void PopulateLists()
        {
            foreach (DataGridColumn column in targetDataGrid.Columns)
            {
                if (column.Visibility == Visibility.Visible)
                {
                    displayedColumns.Items.Add(column.Header);
                }
                else
                {
                    availableColumns.Items.Add(column.Header);
                }
            }
        }

        private void ApplyChanges()
        {
            foreach (DataGridColumn column in targetDataGrid.Columns)
            {
                if (displayedColumns.Items.Contains(column.Header))
                    column.Visibility = Visibility.Visible;
                else
                    column.Visibility = Visibility.Hidden;
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            ApplyChanges();

            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            // Get selected item
            string selected = availableColumns.SelectedItem as string;

            // If no selected item, return
            if (selected == null) return;

            // Remove from available columns list
            availableColumns.Items.Remove(selected);

            // Add to displayed columns list
            displayedColumns.Items.Add(selected);
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            // Get selected item
            string selected = displayedColumns.SelectedItem as string;

            // If no selected item, return
            if (selected == null) return;

            // Remove from displayed columns list
            displayedColumns.Items.Remove(selected);

            // Add to available columns list
            availableColumns.Items.Add(selected);
        }
    }
}
