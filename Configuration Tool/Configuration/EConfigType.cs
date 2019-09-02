namespace Configuration_Tool.Configuration
{
    public enum EConfigType
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
