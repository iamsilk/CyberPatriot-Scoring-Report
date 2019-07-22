using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public class LockoutPolicy
    {
        // Range: 0 - 99999 (mins), Default: None, Relies on Account lockout threshold
        public int AccountLockoutDuration = 0;

        // Range: 0 - 999, Default: 0
        public int AccountLockoutThreshold = 0;

        // Range: 0 - 99999 (mins), Default: None, Relies on Account lockout threshold
        public int ResetLockoutCounterAfter = 0;
    }
}
