using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Scoring_Report.Configuration;
using System.Security.Permissions;
using System.Security;
using Scoring_Report.Scoring.Output;
using Scoring_Report.Policies;

namespace Scoring_Report.Scoring
{
    public static class ScoringManager
    {
        public static List<ISection> ScoringSections { get; } = new List<ISection>();

        public static List<SectionDetails> RecentDetails = new List<SectionDetails>();

        public static int MaxScore = 0;

        public static void Setup()
        {
            ScoringSections.Clear();

            // Get all types in the executing assembly (Scoring Report)
            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
            
            foreach (Type type in allTypes)
            {
                // Filter to parent namespace "Scoring_Report.Scoring.Sections"
                if (type.Namespace.StartsWith("Scoring_Report.Scoring.Sections"))
                {
                    // Filter to types with child interface ISection
                    if (type.GetInterfaces().Contains(typeof(ISection)))
                    {
                        // Create instance
                        ISection instance = Activator.CreateInstance(type) as ISection;

                        // Add to list
                        ScoringSections.Add(instance);
                    }
                }
            }

            GetMaxScore();
        }

        public static void GetMaxScore()
        {
            MaxScore = 0;

            foreach (ISection section in ScoringSections)
            {
                MaxScore += section.MaxScore();
            }
        }

        public static void CheckAndOutput()
        {
            // Get score details
            List<SectionDetails> details = CheckScores();

            // Output score details
            OutputManager.OutputToAll(details);
            
        }

        public static List<SectionDetails> CheckScores()
        {
            List<SectionDetails> details = new List<SectionDetails>();

            // Get information we use from secedit for use in sections
            SecurityPolicyManager.GetSeceditInfo();

            // Enumerate all scoring sections
            foreach (ISection section in ScoringSections)
            {
                // Optimization: Check if anything is scored, skip if not
                if (section.MaxScore() == 0) continue;

                // Get scoring details for specific section
                SectionDetails sectionScore = section.GetScore();

                // Add details to list
                details.Add(sectionScore);
            }

            return details;
        }
    }
}
