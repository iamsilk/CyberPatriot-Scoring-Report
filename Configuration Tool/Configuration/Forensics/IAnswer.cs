namespace Configuration_Tool.Configuration.Forensics
{
    public interface IAnswer
    {
        EAnswerType Type { get; }

        string Info { get; set; }
    }
}
