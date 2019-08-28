using Scoring_Report.Configuration;
using Scoring_Report.Configuration.Groups;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionGroups : ISection
    {
        public ESectionType Type => ESectionType.Groups;

        public string Header => TranslationManager.Translate("SectionGroups");

        public static List<GroupSettings> Groups { get; } = new List<GroupSettings>();

        public int MaxScore()
        {
            // Set max to 0
            int max = 0;

            // For each group settings in list
            foreach (GroupSettings group in Groups)
            {
                // If group is scored, increment max
                if (group.IsScored) max++;
            }

            return max;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // If no configuration for this section, return empty details
            if (Groups.Count == 0) return details;

            // Create instance for communicating with active directory
            using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
            {
                // Create searcher for active directory
                using (PrincipalSearcher searcher = new PrincipalSearcher(new GroupPrincipal(context)))
                {
                    // For each group on the machine
                    foreach (GroupPrincipal group in searcher.FindAll())
                    {
                        // For each group config
                        foreach (GroupSettings settings in Groups)
                        {
                            // If enumerated settings is not for specified group, skip
                            if (group.Name != settings.GroupName)
                            {
                                continue;
                            }

                            // If number of members of group and config are inequal, skip
                            if (group.Members.Count != settings.Members.Count)
                            {
                                continue;
                            }

                            // Copy the list so we may remove elements
                            List<UserPrincipal> membersGroup = new List<UserPrincipal>(group.Members.Cast<UserPrincipal>());

                            bool groupsMatch = true;

                            // For each member config
                            foreach (IMember memberConfig in settings.Members)
                            {
                                // Member found that matches current config
                                UserPrincipal foundMember = null;

                                // For each member in the group
                                foreach (UserPrincipal member in membersGroup)
                                {
                                    bool matches = false;

                                    // Determine the id type and get identifier
                                    switch (memberConfig.IDType)
                                    {
                                        case "Username":
                                            matches = member.SamAccountName == memberConfig.Identifier; 
                                            break;
                                        case "SID":
                                            matches = member.Sid.MatchesConfig(memberConfig.Identifier);
                                            break;
                                    }

                                    // If identifier matches config
                                    if (matches)
                                    {
                                        foundMember = member;
                                        break;
                                    }
                                }

                                // If no match was found, groups do not match
                                if (foundMember == null)
                                {
                                    groupsMatch = false;
                                    break;
                                }

                                // Member was found, remove from list
                                // for optimization and possible duplicates
                                membersGroup.Remove(foundMember);
                            }

                            // If groups match, output and give point
                            if (groupsMatch)
                            {
                                details.Points++;

                                // Get list of members' names separated by commas
                                string members = string.Join(TranslationManager.Translate("Delimiter"), group.Members.Cast<UserPrincipal>().Select(x => x.SamAccountName));

                                details.Output.Add(TranslationManager.Translate("Group", settings.GroupName, members));
                            }
                        }
                    }
                }
            }
            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Clear current list of group settings
            Groups.Clear();

            // Get number of group settings instances
            int count = reader.ReadInt32();

            // Enumerate every instance of group settings
            for (int i = 0; i < count; i++)
            {
                // Get instance of group settings
                GroupSettings settings = GroupSettings.Parse(reader);

                // Add group settings to list
                Groups.Add(settings);
            }
        }
    }
}
