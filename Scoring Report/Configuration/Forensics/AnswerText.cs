namespace Scoring_Report.Configuration.Forensics
{
    public class AnswerText : IAnswer
    {
        public EAnswerType Type => EAnswerType.Text;

        public string Info { get; set; } = "";
    }
}
