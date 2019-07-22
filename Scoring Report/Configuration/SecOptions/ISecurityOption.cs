using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Configuration.SecOptions
{
    public interface ISecurityOption
    {
        ESecurityOptionType Type { get; }

        bool IsScored { get; set; }
        void Parse(BinaryReader reader);
    }
}
