using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Scoring_Report.Configuration.Registry.RegistryHives;
using static Scoring_Report.Configuration.Registry.RegistryType;

namespace Scoring_Report.Configuration.Registry
{
    public class CustomRegKey
    {
        public Hives Hive = Hives.HKEY_LOCAL_MACHINE;

        public RegTypes RegType = RegTypes.REG_NONE;

        public string KeyPath = string.Empty;

        public string ValueName = string.Empty;

        public string Value = string.Empty;

        public CustomRegKey()
        {

        }

        public bool RegistryKeyExists()
        {
            try
            {
            }
            catch
            {
                return false;
            }
        }
    }
}
