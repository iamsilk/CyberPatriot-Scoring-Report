namespace Scoring_Report.Configuration.Forensics
{
    public class AnswerRegex : IAnswer
    {
        public EAnswerType Type => EAnswerType.Regex;

        public string Info { get; set; } = "";
    }
}
