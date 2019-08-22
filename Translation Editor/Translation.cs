using System.ComponentModel;
using System.IO;

namespace Translation_Editor
{
    public class Translation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChange(string variable)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(variable));
        }

        private string header = "";
        public string Header
        {
            get { return header; }
            set
            {
                if (value != header)
                {
                    header = value;
                    OnChange("Header");
                }
            }
        }

        private string format = "";
        public string Format
        {
            get { return format; }
            set
            {
                if (value != format)
                {
                    format = value;
                    OnChange("Format");
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Header);
            writer.Write(Format);
        }

        public static Translation Parse(BinaryReader reader)
        {
            Translation translation = new Translation(
                reader.ReadString(),
                reader.ReadString());

            return translation;
        }

        public Translation(string _header, string _format)
        {
            Header = _header;
            Format = _format;
        }
    }
}
