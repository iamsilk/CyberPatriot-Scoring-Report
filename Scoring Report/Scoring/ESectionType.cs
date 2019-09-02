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
        Shares,
        Startup,
        FirewallProfiles,
        FirewallInboundRules,
        FirewallOutboundRules,
        Services,
        Features,
        Forensics,
        CustomRegistry,
        CustomProcessOutput,
        CustomFiles,
        Other = 99,
    }
}
