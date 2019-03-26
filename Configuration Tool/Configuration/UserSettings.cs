using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration
{
    public class UserSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChange(string variable)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(variable));
        }

        private string username = "";
        public string Username
        {
            get { return username; }
            set
            {
                if (username != value)
                {
                    username = value;
                    OnChange("Username");
                }
            }
        }

        private string securityID;
        public string SecurityID
        {
            get { return securityID; }
            set
            {
                if (securityID != value)
                {
                    securityID = value;
                    OnChange("SecurityID");
                }
            }
        }

        private bool identifiedByUsername = true;
        public bool IdentifiedByUsername
        {
            get { return identifiedByUsername; }
            set
            {
                if (identifiedByUsername != value)
                {
                    identifiedByUsername = value;
                    OnChange("IdentifiedByUsername");
                    OnChange("IdentifiedBySID");
                }
            }
        }
        
        public bool IdentifiedBySID
        {
            get { return !identifiedByUsername; }
            set
            {
                IdentifiedByUsername = !value;
            }
        }

        public ScoredItem<string> Password { get; private set; } = new ScoredItem<string>("", false);

        public ScoredItem<bool> PasswordExpired { get; private set; } = new ScoredItem<bool>(false, false);

        public ScoredItem<bool> PasswordChangeDisabled { get; private set; } = new ScoredItem<bool>(false, false);

        public ScoredItem<bool> PasswordNeverExpires { get; private set; } = new ScoredItem<bool>(false, false);

        public ScoredItem<bool> AccountDisabled { get; private set; } = new ScoredItem<bool>(false, false);

        public ScoredItem<bool> AccountLockedOut { get; private set; } = new ScoredItem<bool>(false, false);

        public UserSettings()
        {

        }
    }
}
