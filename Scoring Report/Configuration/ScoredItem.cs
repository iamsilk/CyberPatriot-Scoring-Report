using Scoring_Report.Configuration.Audit;
using System;
using System.IO;

namespace Scoring_Report.Configuration
{
    public class ScoredItem<T>
    {
        public T Value;

        public bool IsScored = false;

        public ScoredItem()
        {

        }

        public ScoredItem(T value, bool _isScored)
        {
            Value = value;
            IsScored = _isScored;
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

        public static ScoredItem<Range> ParseRange(BinaryReader reader)
        {
            int min = reader.ReadInt32();
            int max = reader.ReadInt32();

            bool isScored = reader.ReadBoolean();

            ScoredItem<Range> scoredItem = new ScoredItem<Range>(new Range(min, max), isScored);
            return scoredItem;
        }

        public static ScoredItem<EAuditSettings> ParseAuditSettings(BinaryReader reader)
        {
            EAuditSettings value = (EAuditSettings)reader.ReadInt32();
            bool isScored = reader.ReadBoolean();

            ScoredItem<EAuditSettings> scoredItem = new ScoredItem<EAuditSettings>(value, isScored);
            return scoredItem;
        }
    }
}
