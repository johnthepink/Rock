using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.UniversalSearch.IndexModels.Attributes
{
    public class IndexField: System.Attribute
    {
        public IndexFieldType FieldType { get; set; }
    }

    public enum IndexFieldType { Analyzed, NotAnalyzed, NotIndexed}
}
