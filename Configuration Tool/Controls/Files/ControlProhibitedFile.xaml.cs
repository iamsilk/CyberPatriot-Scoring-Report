using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Configuration_Tool.Controls.Files
{
    /// <summary>
    /// Interaction logic for ControlProhibitedFile.xaml
    /// </summary>
    public partial class ControlProhibitedFile : System.Windows.Controls.UserControl
    {
        public ControlProhibitedFile()
        {
            InitializeComponent();
        }

        public string Path
        {
            get { return txtPath.Text; }
            set
            {
                txtPath.Text = value;
            }
        }

        private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.FileName = Path;
                dialog.Filter = "All Files|*.*";
                dialog.Multiselect = false;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    Path = dialog.FileName;
                }
            }
        }
        
        private void btnBrowseDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (Directory.Exists(Path))
                {
                    dialog.SelectedPath = Path;
                }

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    Path = dialog.SelectedPath;
                }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)this.Parent;

            itemsControl.Items.Remove(this);
        }
    }
}
