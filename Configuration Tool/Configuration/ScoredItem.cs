using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration
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

        public static ScoredItem<bool> ParseBoolean(BinaryReader reader)
        {
            bool value = reader.ReadBoolean();
            bool isScored = reader.ReadBoolean();

            ScoredItem<bool> scoredItem = new ScoredItem<bool>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<Byte> ParseByte(BinaryReader reader)
        {
            Byte value = reader.ReadByte();
            bool isScored = reader.ReadBoolean();

            ScoredItem<Byte> scoredItem = new ScoredItem<Byte>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<Byte[]> ParseBytes(BinaryReader reader)
        {
            Int32 length = reader.ReadInt32();
            Byte[] value = reader.ReadBytes(length);
            bool isScored = reader.ReadBoolean();

            ScoredItem<Byte[]> scoredItem = new ScoredItem<Byte[]>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<Char> ParseChar(BinaryReader reader)
        {
            Char value = reader.ReadChar();
            bool isScored = reader.ReadBoolean();

            ScoredItem<Char> scoredItem = new ScoredItem<Char>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<Char[]> ParseChars(BinaryReader reader)
        {
            Int32 length = reader.ReadInt32();
            Char[] value = reader.ReadChars(length);
            bool isScored = reader.ReadBoolean();

            ScoredItem<Char[]> scoredItem = new ScoredItem<Char[]>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<Decimal> ParseDecimal(BinaryReader reader)
        {
            Decimal value = reader.ReadDecimal();
            bool isScored = reader.ReadBoolean();

            ScoredItem<Decimal> scoredItem = new ScoredItem<Decimal>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<Double> ParseDouble(BinaryReader reader)
        {
            Double value = reader.ReadDouble();
            bool isScored = reader.ReadBoolean();

            ScoredItem<Double> scoredItem = new ScoredItem<Double>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<Int16> ParseInt16(BinaryReader reader)
        {
            Int16 value = reader.ReadInt16();
            bool isScored = reader.ReadBoolean();

            ScoredItem<Int16> scoredItem = new ScoredItem<Int16>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<Int32> ParseInt32(BinaryReader reader)
        {
            Int32 value = reader.ReadInt32();
            bool isScored = reader.ReadBoolean();

            ScoredItem<Int32> scoredItem = new ScoredItem<Int32>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<Int64> ParseInt64(BinaryReader reader)
        {
            Int64 value = reader.ReadInt64();
            bool isScored = reader.ReadBoolean();

            ScoredItem<Int64> scoredItem = new ScoredItem<Int64>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<SByte> ParseSByte(BinaryReader reader)
        {
            SByte value = reader.ReadSByte();
            bool isScored = reader.ReadBoolean();

            ScoredItem<SByte> scoredItem = new ScoredItem<SByte>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<Single> ParseSingle(BinaryReader reader)
        {
            Single value = reader.ReadSingle();
            bool isScored = reader.ReadBoolean();

            ScoredItem<Single> scoredItem = new ScoredItem<Single>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<String> ParseString(BinaryReader reader)
        {
            String value = reader.ReadString();
            bool isScored = reader.ReadBoolean();

            ScoredItem<String> scoredItem = new ScoredItem<String>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<UInt16> ParseUInt16(BinaryReader reader)
        {
            UInt16 value = reader.ReadUInt16();
            bool isScored = reader.ReadBoolean();

            ScoredItem<UInt16> scoredItem = new ScoredItem<UInt16>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<UInt32> ParseUInt32(BinaryReader reader)
        {
            UInt32 value = reader.ReadUInt32();
            bool isScored = reader.ReadBoolean();

            ScoredItem<UInt32> scoredItem = new ScoredItem<UInt32>(value, isScored);
            return scoredItem;
        }

        public static ScoredItem<UInt64> ParseUInt64(BinaryReader reader)
        {
            UInt64 value = reader.ReadUInt64();
            bool isScored = reader.ReadBoolean();

            ScoredItem<UInt64> scoredItem = new ScoredItem<UInt64>(value, isScored);
            return scoredItem;
        }

        public void Write(BinaryWriter writer)
        {
            switch (Type.GetTypeCode(Value.GetType()))
            {
                case TypeCode.Boolean:
                    writer.Write(Convert.ToBoolean(Value));
                    break;
                case TypeCode.Char:
                    writer.Write(Convert.ToChar(Value));
                    break;
                case TypeCode.SByte:
                    writer.Write(Convert.ToSByte(Value));
                    break;
                case TypeCode.Byte:
                    writer.Write(Convert.ToByte(Value));
                    break;
                case TypeCode.Int16:
                    writer.Write(Convert.ToInt16(Value));
                    break;
                case TypeCode.UInt16:
                    writer.Write(Convert.ToUInt16(Value));
                    break;
                case TypeCode.Int32:
                    writer.Write(Convert.ToInt32(Value));
                    break;
                case TypeCode.UInt32:
                    writer.Write(Convert.ToUInt32(Value));
                    break;
                case TypeCode.Int64:
                    writer.Write(Convert.ToInt64(Value));
                    break;
                case TypeCode.UInt64:
                    writer.Write(Convert.ToUInt64(Value));
                    break;
                case TypeCode.Single:
                    writer.Write(Convert.ToSingle(Value));
                    break;
                case TypeCode.Double:
                    writer.Write(Convert.ToDouble(Value));
                    break;
                case TypeCode.Decimal:
                    writer.Write(Convert.ToDecimal(Value));
                    break;
                case TypeCode.String:
                    writer.Write(Convert.ToString(Value));
                    break;
                case TypeCode.Object:
                    /* If type code of an object is returned, 
                     * value may be an array. In which case 
                     * we must use a different method of 
                     * determining the type
                     */
                    switch(Value.GetType().Name)
                    {
                        case "Byte[]":
                            writer.Write(Value as byte[]);
                            break;
                        case "Char[]":
                            writer.Write(Value as char[]);
                            break;
                    }
                    break;
            }
        }
    }
}
