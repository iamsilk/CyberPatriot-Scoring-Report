using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring
{
    public enum ESectionType
    {
        Users,
        Groups,
        PasswordPolicy,
        LockoutPolicy,
        AuditPolicy,
        UserRights,
        SecurityOptions,
        InstalledPrograms,
        ProhibitedFiles,
        Other,
    }
}
