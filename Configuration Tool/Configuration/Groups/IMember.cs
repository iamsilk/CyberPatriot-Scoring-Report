namespace Configuration_Tool.Configuration.Groups
{
    public interface IMember
    {
        string IDType { get; }
        string Identifier { get; set; }
    }
}
