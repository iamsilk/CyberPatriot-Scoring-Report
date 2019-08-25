namespace Scoring_Report.Configuration.Forensics
{
    public interface IAnswer
    {
        EAnswerType Type { get; }

        string Info { get; set; }
    }
}
