using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration.Groups
{
    public class GroupSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnChange(string variable)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(variable));
        }

        private string groupName = "";
        public string GroupName
        {
            get { return groupName; }
            set
            {
                if (groupName != value)
                {
                    groupName = value;
                    OnChange("GroupName");
                }
            }
        }

        private bool isScored = false;
        public bool IsScored
        {
            get { return isScored; }
            set
            {
                if (isScored != value)
                {
                    isScored = value;
                    OnChange("IsScored");
                }
            }
        }

        public List<IMember> Members = new List<IMember>();

        public GroupSettings()
        {

        }
    }
}
