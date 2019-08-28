using Scoring_Report.Configuration;
using Scoring_Report.Configuration.UserRights;
using Scoring_Report.Policies;
using Scoring_Report.Policies.UserRights;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionUserRights : ISection
    {
        public ESectionType Type => ESectionType.UserRights;

        public string Header => TranslationManager.Translate("SectionUserRights");

        public static List<UserRightsDefinition> UserRightsDefinitions { get; } = new List<UserRightsDefinition>();

        public static UserRightsAssignment SystemPolicy => SecurityPolicyManager.Settings.LocalPolicies.UserRightsAssignment;

        public int MaxScore()
        {
            // Return number of scored user rights definitions
            return UserRightsDefinitions.Count;
        }

        public string GetNameFromSID(SecurityIdentifier identifier)
        {
            // Translates SID to name from local groups/users or accessible domain info
            string name = identifier.Translate(typeof(NTAccount)).ToString();

            // Gets last section of name, for example, without
            // this something may be like 'BUILTIN\\Backup Operators'
            name = name.Split('\\').Last();

            return name;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            SecurityPolicyManager.GetUserRightsAssignment();

            // For each config definition
            foreach (UserRightsDefinition definition in UserRightsDefinitions)
            {
                // Create copy of dictionary. Uses more memory but optimizes
                // checking as we can remove user rights after checking them
                Dictionary<string, List<SecurityIdentifier>> tempDict = 
                    new Dictionary<string, List<SecurityIdentifier>>(SystemPolicy.UserRightsSetting);

                string foundUserRights = null;
                bool configCorrect = true;

                // For each retrieved system user rights definition
                foreach (KeyValuePair<string, List<SecurityIdentifier>> userRights in tempDict)
                {
                    // Check if config and system define the same user rights
                    if (definition.ConstantName != userRights.Key) continue;

                    foundUserRights = definition.ConstantName;

                    // If config and user rights identifiers count do not match,
                    // it is incorrectly configured. Break the loop and set as incorrect
                    if (definition.Identifiers.Count != userRights.Value.Count)
                    {
                        configCorrect = false;
                        break;
                    }

                    // Loop over sids first so we're not converting 
                    // identifiers to usernames multiple times for each
                    foreach (SecurityIdentifier identifier in userRights.Value)
                    {
                        // Cache name to be compared with later checked identifiers
                        string name = null;

                        bool foundIdentifier = false;

                        foreach (UserRightsIdentifier cfgId in definition.Identifiers)
                        {
                            switch (cfgId.Type)
                            {
                                case EUserRightsIdentifierType.Name:
                                    // If name has not been found yet, retrieve it
                                    if (name == null)
                                    {
                                        name = GetNameFromSID(identifier);
                                    }

                                    // If name and config name match, match is found
                                    if (name == cfgId.Identifier)
                                    {
                                        foundIdentifier = true;
                                        
                                        // Save name for possible output
                                        cfgId.Name = name;
                                    }
                                    break;
                                case EUserRightsIdentifierType.SecurityID:
                                    // If Security IDs match, match is found
                                    if (identifier.MatchesConfig(cfgId.Identifier))
                                    {
                                        foundIdentifier = true;

                                        // If name has not been found yet, retrieve it
                                        if (name == null)
                                        {
                                            name = GetNameFromSID(identifier);
                                        }

                                        // Save name for possible output
                                        cfgId.Name = name;
                                    }

                                    break;
                            }

                            // If we found the match, break the loop
                            // as there is no need to keep searching
                            if (foundIdentifier) break;
                        }

                        // If no match was found, the config and
                        // system config do not match, break the loop
                        if (!foundIdentifier)
                        {
                            configCorrect = false;
                            break;
                        }
                    }

                    // If process got here,
                    // break the loop as a match was found
                    break;
                }

                // If user rights was found
                if (foundUserRights != null)
                {
                    // If configured correctly, increment points and give output
                    if (configCorrect)
                    {
                        // Create list for every name, used to give user the proper output
                        IEnumerable<string> names = definition.Identifiers.Select(x => x.Name);

                        details.Points++;
                        details.Output.Add(TranslationManager.Translate("UserRights", definition.Setting, 
                            string.Join(TranslationManager.Translate("Delimiter"), names)));
                    }

                    tempDict.Remove(foundUserRights);
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Clear current user rights definitions
            UserRightsDefinitions.Clear();

            // Get count of user rights definitions
            int count = reader.ReadInt32();

            // For number of user rights definitions
            for (int i = 0; i < count; i++)
            {
                // Get constant name
                string constantName = reader.ReadString();

                // Get setting
                string setting = reader.ReadString();

                // Get and set scoring status
                bool isScored = reader.ReadBoolean();

                // Create instance of definition object
                UserRightsDefinition userRights = new UserRightsDefinition(constantName, setting);

                // Get number of identifiers
                int identifiersCount = reader.ReadInt32();

                // For number of identifiers
                for (int j = 0; j < identifiersCount; j++)
                {
                    // Get identifier type and identifier
                    EUserRightsIdentifierType type = (EUserRightsIdentifierType)reader.ReadInt32();
                    string strIdentifier = reader.ReadString();

                    // Initialize storage for identifier
                    UserRightsIdentifier identifier = new UserRightsIdentifier();
                    identifier.Type = type;
                    identifier.Identifier = strIdentifier;

                    // Add identifier to list
                    userRights.Identifiers.Add(identifier);
                }

                // Optimization, only add scored items
                if (isScored)
                {
                    UserRightsDefinitions.Add(userRights);
                }
            }
        }
    }
}
