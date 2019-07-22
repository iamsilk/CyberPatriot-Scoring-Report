using Configuration_Tool.Configuration;
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

namespace Configuration_Tool.Controls
{
    /// <summary>
    /// Interaction logic for ControlSettingAudit.xaml
    /// </summary>
    public partial class ControlSettingAudit : UserControl
    {
        public ControlSettingAudit()
        {
            InitializeComponent();
        }

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

        public bool IsScored
        {
            get
            {
                if (!checkBoxScored.IsChecked.HasValue) return false;

                return checkBoxScored.IsChecked.Value;
            }
            set
            {
                checkBoxScored.IsChecked = value;
            }
        }

        public bool Success
        {
            get
            {
                if (!checkBoxSuccess.IsChecked.HasValue) return false;

                return checkBoxSuccess.IsChecked.Value;
            }
            set
            {
                checkBoxSuccess.IsChecked = value;
            }
        }

        public bool Failure
        {
            get
            {
                if (!checkBoxFailure.IsChecked.HasValue) return false;

                return checkBoxFailure.IsChecked.Value;
            }
            set
            {
                checkBoxFailure.IsChecked = value;
            }
        }
        
        public EAuditSettings AuditSettings
        {
            get
            {
                EAuditSettings settings = EAuditSettings.Unchanged;

                if (Success) settings |= EAuditSettings.Success;
                if (Failure) settings |= EAuditSettings.Failure;

                return settings;
            }
            set
            {
                // Checks if passed settings contains bit for success
                // If comparison returns 0, value does not contain success
                bool success = (value & EAuditSettings.Success) != 0;

                // Same as success
                bool failure = (value & EAuditSettings.Failure) != 0;

                Success = success;
                Failure = failure;
            }
        }

        public ScoredItem<EAuditSettings> GetScoredItem()
        {
            return new ScoredItem<EAuditSettings>(AuditSettings, IsScored);
        }

        public void SetFromScoredItem(ScoredItem<EAuditSettings> scoredItem)
        {
            AuditSettings = scoredItem.Value;
            IsScored = scoredItem.IsScored;
        }
    }
}
