using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Configuration.Groups
{
    public class GroupSettings
    {
        public string GroupName = "";

        public bool IsScored = false;

        public List<IMember> Members = new List<IMember>();

        public GroupSettings()
        {

        }

        public static GroupSettings Parse(BinaryReader reader)
        {
            GroupSettings settings = new GroupSettings();

            // Get group name and scoring status
            settings.GroupName = reader.ReadString();
            settings.IsScored = reader.ReadBoolean();

            // Get number of members
            int count = reader.ReadInt32();

            // Create list with count, optimized to allocate memory at construction
            settings.Members = new List<IMember>(count);

            // Enumerate and read every member of group
            for (int i = 0; i < count; i++)
            {
                // Get id type and identifier of member
                string idType = reader.ReadString();
                string identifier = reader.ReadString();

                // Using switch statement instead of if/else if... to allow 
                // for easy adding of other identification types in the future
                
                IMember member;
                switch (idType)
                {
                    // Create instance of member, depending on id type
                    case "SID":
                        member = new MemberSID();
                        break;
                    case "Username":
                        member = new MemberUsername();
                        break;
                    default:
                        // How'd we get here?
                        throw new Exception(string.Format("Unknown identifier type ({0}) for identifier ({1}) when parsing for group ({2})", idType, identifier, settings.GroupName));
                }

                // Set identifier for member
                member.Identifier = identifier;

                // Add member to group settings
                settings.Members.Add(member);
            }

            // Return instance
            return settings;
        }

        public void Write(BinaryWriter writer)
        {
            // Write group name and scoring status
            writer.Write(GroupName);
            writer.Write(IsScored);

            // Write count of members
            writer.Write(Members.Count);

            // Write all members
            foreach (IMember member in Members)
            {
                // Write id type and identifier
                writer.Write(member.IDType);
                writer.Write(member.Identifier);
            }
        }
    }
}
