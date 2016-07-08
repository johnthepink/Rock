using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.UniversalSearch.IndexModels.Attributes
{
    public class IndexBoost: System.Attribute
    {
        public int Level { get; set; }
    }
}
