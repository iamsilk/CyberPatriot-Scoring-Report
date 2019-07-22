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
