using System.ComponentModel;

namespace Configuration_Tool.Configuration.Groups
{
    public class MemberUsername : IMember, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChange(string variable)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(variable));
        }

        public string IDType => "Username";

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
    }
}
