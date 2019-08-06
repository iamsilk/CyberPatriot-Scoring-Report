using Microsoft.Win32;

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
