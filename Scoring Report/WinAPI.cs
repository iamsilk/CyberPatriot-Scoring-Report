using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Scoring_Report.Policies;

namespace Scoring_Report
{
    public static class WinAPI
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("kernel32.dll")]
        public extern static bool CloseHandle(IntPtr handle);

        [DllImport("netapi32.dll")]
        public extern static uint NetUserModalsGet(string server, int level, out IntPtr bufPtr);

        [DllImport("advapi32.dll")]
        public extern static uint LsaOpenPolicy(ref string SystemName, ref LSA_OBJECT_ATTRIBUTES ObjectAttributes, uint AccessMask, out IntPtr PolicyHandle);

        [DllImport("advapi32.dll")]
        public static extern uint LsaQueryInformationPolicy(IntPtr PolicyHandle, POLICY_INFORMATION_CLASS InformationClass, out IntPtr Buffer);

        [DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint LsaEnumerateAccountsWithUserRight(IntPtr PolicyHandle, 
            LSA_UNICODE_STRING[] UserRights, out IntPtr EnumerationBuffer, out int CountReturned);

        [DllImport("advapi32")]
        public static extern uint LsaFreeMemory(IntPtr Buffer);

        [DllImport("advapi32")]
        public static extern uint LsaClose(IntPtr PolicyHandle);
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetFileAttributes(string fileName);
    }
}
