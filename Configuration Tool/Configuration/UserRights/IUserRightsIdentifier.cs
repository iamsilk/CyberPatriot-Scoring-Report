namespace Configuration_Tool.Configuration.UserRights
{
    public interface IUserRightsIdentifier
    {
        EUserRightsIdentifierType Type { get; }
        string Identifier { get; set; }
    }
}
