using System;

namespace Scoring_Report.Policies
{
    public struct LSA_OBJECT_ATTRIBUTES
    {
        public uint Length;
        public IntPtr RootDirectory;
        public string ObjectName;
        public uint Attributes;
        public IntPtr SecurityDescriptor;
        public IntPtr SecurityQualityOfService;
    }
}
