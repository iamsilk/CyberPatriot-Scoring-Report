using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration.Groups
{
    public class MemberSID : IMember, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChange(string variable)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(variable));
        }

        public string IDType => "SID";

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
