using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool
{
    public class ScoredItem<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChange(string variable)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(variable));
        }

        private T _value = default(T);
        public T Value
        {
            get { return _value; }
            set
            {
                if (!_value.Equals(value))
                {
                    _value = value;
                    OnChange("Value");
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

        public ScoredItem()
        {

        }

        public ScoredItem(T value, bool isScored)
        {
            _value = value;
            isScored = IsScored;
        }
    }
}
