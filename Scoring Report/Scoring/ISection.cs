using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Scoring
{
    public interface ISection
    {
        ESectionType Type { get; }

        string Header { get; }
        int MaxScore();
        SectionDetails GetScore();
        
        void Load(BinaryReader reader);
    }
}
