using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring
{
    public class SectionDetails
    {
        public int Points = 0;

        public List<string> Output = new List<string>();

        public ISection Section = null;

        public SectionDetails(int points, List<string> output, ISection section)
        {
            Points = points;
            Output = output;
            Section = section;
        }
    }
}
