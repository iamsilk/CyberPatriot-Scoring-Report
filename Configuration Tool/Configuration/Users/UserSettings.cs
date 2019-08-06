using System.ComponentModel;
using System.IO;

namespace Configuration_Tool.Configuration.Users
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

        private string securityID = "";
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

        public ScoredItem<bool> Exists { get; private set; } = new ScoredItem<bool>(false, false);

        public ScoredItem<string> Password { get; private set; } = new ScoredItem<string>("", false);

        public ScoredItem<bool> PasswordExpired { get; private set; } = new ScoredItem<bool>(false, false);

        public ScoredItem<bool> PasswordChangeDisabled { get; private set; } = new ScoredItem<bool>(false, false);

        public ScoredItem<bool> PasswordNeverExpires { get; private set; } = new ScoredItem<bool>(false, false);

        public ScoredItem<bool> AccountDisabled { get; private set; } = new ScoredItem<bool>(false, false);

        public ScoredItem<bool> AccountLockedOut { get; private set; } = new ScoredItem<bool>(false, false);

        public UserSettings()
        {

        }

        public static UserSettings Parse(BinaryReader reader)
        {
            UserSettings settings = new UserSettings();

            // Get identification type
            settings.IdentifiedByUsername = reader.ReadBoolean();
            
            // Get username and security identifier
            settings.Username = reader.ReadString();
            settings.SecurityID = reader.ReadString();

            // Get other settings
            settings.Exists = ScoredItem<bool>.ParseBoolean(reader);
            settings.Password = ScoredItem<string>.ParseString(reader);
            settings.PasswordExpired = ScoredItem<bool>.ParseBoolean(reader);
            settings.PasswordChangeDisabled = ScoredItem<bool>.ParseBoolean(reader);
            settings.PasswordNeverExpires = ScoredItem<bool>.ParseBoolean(reader);
            settings.AccountDisabled = ScoredItem<bool>.ParseBoolean(reader);
            settings.AccountLockedOut = ScoredItem<bool>.ParseBoolean(reader);

            return settings;
        }

        public void Write(BinaryWriter writer)
        {
            // Write user identification
            writer.Write(IdentifiedByUsername);

            // Write the username to configuration
            writer.Write(Username);

            // Write the security identifier to configuration
            writer.Write(SecurityID);

            // Write other settings
            Exists.Write(writer);
            Password.Write(writer);
            PasswordExpired.Write(writer);
            PasswordChangeDisabled.Write(writer);
            PasswordNeverExpires.Write(writer);
            AccountDisabled.Write(writer);
            AccountLockedOut.Write(writer);
        }
    }
}
