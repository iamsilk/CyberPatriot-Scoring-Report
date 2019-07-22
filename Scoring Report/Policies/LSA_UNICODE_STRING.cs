using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LSA_UNICODE_STRING
    {
        public ushort Length;
        public ushort MaximumLength;
        public string Buffer;

        public LSA_UNICODE_STRING(string str)
        {
            Buffer = str;
            Length = (ushort)(str.Length * 2);
            MaximumLength = (ushort)(Length + 2);
        }
    }
}
