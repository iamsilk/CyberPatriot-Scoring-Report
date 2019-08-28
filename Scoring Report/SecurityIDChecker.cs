using System;
using System.Security.Principal;

namespace Scoring_Report
{
    public static class SecurityIDChecker
    {
        public static bool MatchesConfig(this SecurityIdentifier sid, string config)
        {
            string strSid = sid.Value;

            if (config.Contains("domain"))
            {
                // Split amongst parts
                string[] parts = config.Trim().Split(
                    new string[] { "domain" }, StringSplitOptions.None);

                string rest = strSid;

                // If for whatever reason, there are multiple domain parts in sid, we use this
                for (int i = 0; i < parts.Length; i++)
                {
                    int index = rest.IndexOf(parts[i]);

                    // If first part, index must be zero
                    if (i == 0 && index != 0) break;

                    // If part wasn't found, not equal
                    if (index < 0) break;

                    // Set rest equal to rest of string after specific part
                    rest = rest.Substring(index + parts[i].Length);
                }

                // If all of string has been checked and verified, user was found
                if (rest == "") return true;
            }
            else if (config.Trim() == strSid)
                return true;

            return false;
        }
    }
}
