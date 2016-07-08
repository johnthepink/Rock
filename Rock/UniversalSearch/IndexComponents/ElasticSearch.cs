using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.UniversalSearch.IndexModels;

namespace Rock.UniversalSearch.IndexComponents
{
    [Description( "ElasticSearch Universal Search Index" )]
    [Export( typeof( IndexComponent ) )]
    [ExportMetadata( "ComponentName", "ElasticSearch" )]

    [TextField( "Node URL", "The URL of the ElasticSearch node (http://myserver:9200)", true, key: "NodeUrl" )]
    public class ElasticSearch : IndexComponent
    {
        private ElasticClient _client;

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public override bool IsConnected
        {
            get
            {
                if ( _client != null )
                {
                    var results = _client.ClusterState();

                    if (results != null )
                    {
                        return results.IsValid;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public ElasticClient Client
        {
            get
            {
                return _client;
            }
        }

        /// <summary>
        /// Gets the index location.
        /// </summary>
        /// <value>
        /// The index location.
        /// </value>
        public override string IndexLocation
        {
            get
            {
                return GetAttributeValue( "NodeUrl" );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticSearch" /> class.
        /// </summary>
        public ElasticSearch()
        {
            var node = new Uri( GetAttributeValue( "NodeUrl" ) );
            var config = new ConnectionSettings( node );
            config.DisableDirectStreaming();
            _client = new ElasticClient( config );
        }

        /// <summary>
        /// Indexes the document.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="document">The document.</param>
        public override void IndexDocument<T>( T document, string indexName = null, string mappingType = null )
        {
            if (indexName == null )
            {
                indexName = document.GetType().Name.ToLower();
            }

            if (mappingType == null )
            {
                mappingType = document.GetType().Name.ToLower();
            }

            _client.IndexAsync<T>( document, c => c.Index( indexName ).Type( mappingType ) );
        }


        /// <summary>
        /// Deletes the type of the documents by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName">Name of the index.</param>
        public override void DeleteDocumentsByType<T>( string indexName = null )
        {
            if ( indexName == null )
            {
                indexName = typeof( T ).Name.ToLower();
            }

            _client.DeleteByQueryAsync<T>(indexName, typeof( T ).Name.ToLower(), d => d.MatchAll() );
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">The document.</param>
        /// <param name="indexName">Name of the index.</param>
        public override void DeleteDocument<T>( T document, string indexName = null )
        {
            if ( indexName == null )
            {
                indexName = document.GetType().Name.ToLower();
            }

            _client.Delete<T>( document, d => d.Index( indexName ) );
        }

        public override void CreateIndex<T>(string indexName = null)
        {
            Type typeParameterType = typeof( T );
            object instance = Activator.CreateInstance( typeParameterType );

            if (instance is IndexModelBase )
            {
                var model = (IndexModelBase)instance;

                var response = _client.CreateIndex( "personindex", t => t
                 .Mappings( ms => ms.Map<PersonIndex>( m => m.Properties( ps => ps
                                    .String( s => s.Name( c => c.FirstName ) )
                                    .String( s => s.Name( c => c.LastName ).Boost( 5 ) )
                                    .String( s => s.Name( c => c.IconCssClass ).Index( FieldIndexOption.No ) )
                                    )
                                )
                            )
                        );
            }
        }

        /// <summary>
        /// Deletes the index.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        public override void DeleteIndex( string indexName )
        {
            _client.DeleteIndex( indexName );
        }

        public override IEnumerable<SearchResultModel> Search( string query, SearchType searchType = SearchType.ExactMatch, List<int> entities = null )
        {
            ISearchResponse<dynamic> results = null;
            List<SearchResultModel> searchResults = new List<SearchResultModel>();

            if (searchType == SearchType.ExactMatch )
            {
                var searchDescriptor = new SearchDescriptor<dynamic>().AllIndices();

                if (entities == null || entities.Count == 0 )
                {
                    searchDescriptor = searchDescriptor.AllTypes();
                }
                else
                {
                    foreach( var entityId in entities )
                    {
                        // get entities search model name
                        var entityType = new EntityTypeService( new RockContext() ).Get( entityId );
                        searchDescriptor = searchDescriptor.Type(entityType.GetIndexModelType);
                    }
                }

                searchDescriptor = searchDescriptor.Query( q => q.QueryString( s => s.Query( query ) ) );

                results = _client.Search<dynamic>( searchDescriptor );
            }
            else
            {
                results = _client.Search<dynamic>( d => 
                                    d.AllIndices().AllTypes()
                                    .Query( q => 
                                        q.Fuzzy( f => f.Value( query ) ) 
                                    )
                                    .Explain( true ) // todo remove before flight 
                                );
            }

            //var presults = _client.Search<PersonIndex>( s => s.AllIndices().Query( q => q.QueryString( qs => qs.Query( query ) ) ) );

            // normallize the results to rock search results
            if (results != null )
            {
                foreach(var hit in results.Hits )
                {
                    var searchResult = new SearchResultModel();
                    searchResult.Score = hit.Score;
                    searchResult.Type = hit.Type;
                    searchResult.Index = hit.Index;
                    searchResult.EntityId = hit.Id.AsInteger();

                    try {
                        if ( hit.Source != null )
                        {

                            /*Type indexModelType = Type.GetType( (string)((JObject)hit.Source)["indexModelType"] );

                            if ( indexModelType != null )
                            {
                                searchResult.Document = (IndexModelBase)((JObject)hit.Source).ToObject( indexModelType ); // return the source document as the derived type
                            }
                            else
                            {
                                searchResult.Document = ((JObject)hit.Source).ToObject<IndexModelBase>(); // return the source document as the base type
                            }*/
                        }

                        if ( hit.Explanation != null )
                        {
                            searchResult.Explain = hit.Explanation.ToJson();
                        }

                        searchResults.Add( searchResult );
                    }
                    catch { } // ignore if the result if an exception resulted (most likely cause is getting a result from a non-rock index)
                }
            }

            return searchResults;

            
        }
    }
}


// forbidden characters in field names _ . , #

// cluster state: http://localhost:9200/_cluster/state?filter_nodes=false&filter_metadata=true&filter_routing_table=true&filter_blocks=true&filter_indices=true