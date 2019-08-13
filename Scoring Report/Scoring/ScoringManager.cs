using Scoring_Report.Policies;
using Scoring_Report.Scoring.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            // Sort in order of integer value of enumerator 'Type'
            ScoringSections.Sort((x, y) => (x.Type.CompareTo(y.Type)));
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

            // Reset max score
            MaxScore = 0;

            // Enumerate all scoring sections
            foreach (ISection section in ScoringSections)
            {
                // Get max score
                int maxScore = section.MaxScore();

                // Optimization: Check if anything is scored, skip if not
                if (maxScore == 0) continue;

                // Increment total max score by section's
                MaxScore += maxScore;

                // Get scoring details for specific section
                SectionDetails sectionScore = section.GetScore();

                // Add details to list
                details.Add(sectionScore);
            }

            return details;
        }
    }
}
