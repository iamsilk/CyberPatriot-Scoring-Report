using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionUsers : ISection
    {
        public string Header => "Users:";

        public SectionDetails GetScore()
        {
            return null;
        }
    }
}
