namespace Scoring_Report.Configuration.Groups
{
    public class MemberSID : IMember
    {
        public string IDType => "SID";

        public string Identifier { get; set; } = "";
    }
}
