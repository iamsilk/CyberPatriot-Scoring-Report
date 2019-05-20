using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring
{
    public static class ScoringManager
    {
        public static List<ISection> ScoringSections { get; } = new List<ISection>();

        
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
        }
    }
}
