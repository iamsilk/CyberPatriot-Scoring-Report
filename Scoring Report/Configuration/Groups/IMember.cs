namespace Scoring_Report.Configuration.Groups
{
    public interface IMember
    {
        string IDType { get; }
        string Identifier { get; set; }
    }
}
