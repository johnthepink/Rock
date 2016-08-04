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
        /// Indexes the document.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        void IndexDocument( int id );

        /// <summary>
        /// Deletes the indexed document.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        void DeleteIndexedDocument( int id );

        /// <summary>
        /// Deletes the indexed documents.
        /// </summary>
        void DeleteIndexedDocuments();

        /// <summary>
        /// Indexes the name of the model.
        /// </summary>
        /// <returns></returns>
        Type IndexModelType();
    }
}
