using System.Windows.Controls;

namespace Configuration_Tool.Controls.SecOptions
{
    /// <summary>
    /// Interaction logic for ControlNotImplemented.xaml
    /// </summary>
    public partial class ControlNotImplemented : UserControl
    {
        public string Header
        {
            get
            {
                return txtHeader.Text;
            }
            set
            {
                txtHeader.Text = value;
            }
        }

        public ControlNotImplemented()
        {
            InitializeComponent();
        }
    }
}
