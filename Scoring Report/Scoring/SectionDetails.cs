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

        public string Output = "";

        public SectionDetails(int points, string output)
        {
            Points = points;
            Output = output;
        }
    }
}
