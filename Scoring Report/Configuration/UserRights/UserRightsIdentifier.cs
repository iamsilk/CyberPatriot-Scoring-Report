namespace Scoring_Report.Configuration.UserRights
{
    public class UserRightsIdentifier
    {
        public EUserRightsIdentifierType Type;

        public string Identifier = "";

        // Only used in final output
        private string name;
        public string Name
        {
            get
            {
                switch (Type)
                {
                    case EUserRightsIdentifierType.Name:
                        return Identifier;
                    case EUserRightsIdentifierType.SecurityID:
                        return name;
                }
                return name;
            }
            set
            {
                name = value;
            }
        }
    }
}
