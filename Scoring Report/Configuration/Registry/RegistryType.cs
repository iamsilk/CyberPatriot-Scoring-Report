using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Configuration.Registry
{
    public class RegistryType
    {
        public enum RegTypes
        {
            /// <summary>
            /// No type (the stored value, if any)
            /// </summary>
            REG_NONE,

            /// <summary>
            /// A string value, normally stored and exposed in UTF-16LE (when using the Unicode version of Win32 API functions), usually terminated by a NUL character
            /// </summary>
            REG_SZ,

            /// <summary>
            /// An "expandable" string value that can contain environment variables, normally stored and exposed in UTF-16LE, usually terminated by a NUL character
            /// </summary>
            REG_EXPAND_SZ,

            /// <summary>
            /// Binary data (any arbitrary data)
            /// </summary>
            REG_BINARY,

            /// <summary>
            /// A DWORD value, a 32-bit unsigned integer (numbers between 0 and 4,294,967,295 [232 – 1]) (little-endian)
            /// </summary>
            REG_DWORD,

            /// <summary>
            /// A DWORD value, a 32-bit unsigned integer (numbers between 0 and 4,294,967,295 [232 – 1]) (big-endian)
            /// </summary>
            REG_DWORD_BIG_ENDIAN,

            /// <summary>
            /// A symbolic link (UNICODE) to another registry key, specifying a root key and the path to the target key
            /// </summary>
            REG_LINK,

            /// <summary>
            /// A multi-string value, which is an ordered list of non-empty strings, normally stored and exposed in UTF-16LE, each one terminated by a NUL character, the list being normally terminated by a second NUL character.
            /// </summary>
            REG_MULTI_SZ,

            /// <summary>
            /// A resource list (used by the Plug-n-Play hardware enumeration and configuration)
            /// </summary>
            REG_RESOURCE_LIST,

            /// <summary>
            /// A resource descriptor (used by the Plug-n-Play hardware enumeration and configuration)
            /// </summary>
            REG_FULL_RESOURCE_DESCRIPTOR,

            /// <summary>
            /// A resource requirements list (used by the Plug-n-Play hardware enumeration and configuration)
            /// </summary>
            REG_RESOURCE_REQUIREMENTS_LIST,

            /// <summary>
            /// A QWORD value, a 64-bit integer (either big- or little-endian, or unspecified) (introduced in Windows XP)
            /// </summary>
            REG_QWORD
        };
    }
}
