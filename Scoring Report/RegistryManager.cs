using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report
{
    public static class RegistryManager
    {
        public static object GetValue(string key, string valueName)
        {
            return Registry.GetValue(key, valueName, null);
        }
    }
}
