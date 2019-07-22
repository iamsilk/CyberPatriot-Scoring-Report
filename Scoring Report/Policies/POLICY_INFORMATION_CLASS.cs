using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public enum POLICY_INFORMATION_CLASS
    {
        PolicyAuditLogInformation = 1,
        PolicyAuditEventsInformation,
        PolicyPrimaryDomainInformation,
        PolicyPdAccountInformation,
        PolicyAccountDomainInformation,
        PolicyLsaServerRoleInformation,
        PolicyReplicaSourceInformation,
        PolicyDefaultQuotaInformation,
        PolicyModificationInformation,
        PolicyAuditFullSetInformation,
        PolicyAuditFullQueryInformation,
        PolicyDnsDomainInformation,
        PolicyDnsDomainInformationInt,
        PolicyLocalAccountDomainInformation,
        PolicyMachineAccountInformation,
        PolicyLastEntry
    }
}
