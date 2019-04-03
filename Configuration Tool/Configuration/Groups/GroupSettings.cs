using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration.Groups
{
    public class GroupSettings
    {
        public string GroupName { get; set; } = "";

        public ScoredItem<List<IMember>> Members = new ScoredItem<List<IMember>>();
    }
}
