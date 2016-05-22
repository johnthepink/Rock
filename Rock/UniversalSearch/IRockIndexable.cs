using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.UniversalSearch.IndexModels;

namespace Rock.UniversalSearch
{
    interface IRockIndexable
    {
        /// <summary>
        /// Bulks the index documents.
        /// </summary>
        void BulkIndexDocuments();

        /// <summary>
        /// Deletes the indexed documents.
        /// </summary>
        void DeleteIndexedDocuments();

        /// <summary>
        /// Indexes the name of the model.
        /// </summary>
        /// <returns></returns>
        Type IndexModelName();
    }
}
