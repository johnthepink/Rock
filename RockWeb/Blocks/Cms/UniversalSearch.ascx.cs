// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.UniversalSearch;
using Rock.UniversalSearch.IndexModels;
using System.Data.Entity;
using Nest;
using Rock.UniversalSearch.IndexComponents;
using Newtonsoft.Json.Linq;
using Rock.UniversalSearch.IndexModels;

namespace RockWeb.Blocks.Cms
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Universal Search" )]
    [Category( "CMS" )]
    [Description( "A block to search for all indexable entity types in Rock." )]
    
    public partial class UniversalSearch : Rock.Web.UI.RockBlock
    {
        #region Fields

        // used for private variables

        #endregion

        #region Properties

        // used for public / protected properties

        #endregion

        #region Base Control Methods

        //  overrides of the base RockBlock methods (i.e. OnInit, OnLoad)

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            var sm = ScriptManager.GetCurrent( Page );
            sm.Navigate += sm_Navigate;

            if ( !Page.IsPostBack )
            {
                var entities = new EntityTypeService( new RockContext() ).Queryable().AsNoTracking().ToList();

                var indexableEntities = entities.Where( i => i.IsIndexingSupported == true ).ToList();

                cblEntities.DataTextField = "FriendlyName";
                cblEntities.DataValueField = "Id";
                cblEntities.DataSource = indexableEntities;
                cblEntities.DataBind();
            }
        }

        private void sm_Navigate( object sender, HistoryEventArgs e )
        {
            var state = e.State["search"];
            Search( state );
            tbSearch.Text = state;
        }

        #endregion

        #region Events

        // handlers called by the controls on your block

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {

        }

        protected void btnSearch_Click( object sender, EventArgs e )
        {
            this.AddHistory( "search", tbSearch.Text, "you searched, I saw you" );
            Search( tbSearch.Text );
        }

        private void Search(string term )
        {
            lResults.Text = string.Empty;

            //var client = IndexContainer.GetActiveComponent();

            ElasticSearch search = new ElasticSearch();
            ElasticClient _client = search.Client;

            //ISearchResponse<dynamic> results = null;
            List<int> entities = cblEntities.SelectedValuesAsInt;

            // START EXACT SEARCH
            var searchDescriptor = new SearchDescriptor<dynamic>().AllIndices();

            if ( entities == null || entities.Count == 0 )
            {
                searchDescriptor = searchDescriptor.AllTypes();
            }
            else
            {
                var entityTypes = new List<string>();
                foreach ( var entityId in entities )
                {
                    // get entities search model name
                    var entityType = new EntityTypeService( new RockContext() ).Get( entityId );
                    entityTypes.Add( entityType.GetIndexModelType.Name.ToLower() );
                }

                searchDescriptor = searchDescriptor.Type( string.Join(",", entityTypes) ); // todo: considter adding indexmodeltype to the entity cache
            }
            searchDescriptor = searchDescriptor.Query( q => q.QueryString( s => s.Query( term ) ) );//.Type(",personindex,"); //<- comma delimited list

            var rawResults = _client.Search<dynamic>( searchDescriptor );
            // END EXACT SEARCH

            // START FUZZY SEARCH
            /*var rawResults = _client.Search<dynamic>( d =>
                                   d.AllIndices().AllTypes()
                                   .Query( q =>
                                       q.Fuzzy( f => f.Value( term ) )
                                   )
                                   .Explain( true ) // todo remove before flight 
                               );*/
            // END FUZZY SEARCH

            var results = TempGetResults( rawResults );


            //var results = client.Search( term, SearchType.ExactMatch, cblEntities.SelectedValuesAsInt );

            foreach ( var result in results as IEnumerable<SearchResultModel> )
            {
                var formattedResult = result.Document.FormatSearchResult( CurrentPerson );

                if ( formattedResult.IsViewAllowed )
                {
                    lResults.Text += string.Format( "<strong>Score</strong> {0} - <i class='{2}'></i> Result: {1} <br /><pre style='xdisplay: none;'>{3}</pre><p>", result.Score, formattedResult.FormattedResult, result.Document.IconCssClass, result.Explain );
                }
            }
        }

        #endregion

        private List<SearchResultModel> TempGetResults( ISearchResponse<dynamic> results)
        {
            
            List<SearchResultModel> searchResults = new List<SearchResultModel>();
            if ( results != null )
            {
                foreach ( var hit in results.Hits )
                {
                    var searchResult = new SearchResultModel();
                    searchResult.Score = hit.Score;
                    searchResult.Type = hit.Type;
                    searchResult.Index = hit.Index;
                    searchResult.EntityId = hit.Id.AsInteger();

                    try
                    {
                        if ( hit.Source != null )
                        {
                            var typeName = (string)((JObject)hit.Source)["indexModelType"] + ",Rock";
                            Type indexModelType = Type.GetType( typeName );

                            if ( indexModelType != null )
                            {
                                searchResult.Document = (IndexModelBase)((JObject)hit.Source).ToObject( indexModelType ); // return the source document as the derived type
                            }
                            else
                            {
                                searchResult.Document = ((JObject)hit.Source).ToObject<IndexModelBase>(); // return the source document as the base type
                            }
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



        protected void btnTest_Click( object sender, EventArgs e )
        {
            ElasticSearch search = new ElasticSearch();
            ElasticClient _client = search.Client;

            /*var indexDescriptor = new TypeMappingDescriptor<PersonIndex>();
            indexDescriptor.Properties( ps => ps.String( s => s.Name( "FirstName" ).Boost( 5 ) ) );
            indexDescriptor.Properties( ps => ps.String( s => s.Name( "LastName" ).Boost( 10 ) ) );

            var test3 = new
            test3.Name = "FirstName";

            var typeMapping = new TypeMapping();
            typeMapping.Properties.Add(;

            var indexRequest = new CreateIndexRequest();
            indexRequest.Mappings.Add( "personindex", indexDescriptor );
            var response3 = _client.CreateIndex(indexRequest);*/

            /*var response = _client.CreateIndex( "personindex", t => t
                 .Mappings( ms => ms.Map<PersonIndex>( m => m.Properties( ps => ps
                                    .String( s => s.Name( "FirstName" ) )
                                    .String( s => s.Name( "LastName" ).Boost(5) )
                                    .String( s => s.Name( "IconCssClass" ).Index(FieldIndexOption.No))
                                    )
                                )
                            )
                        );*/


            /*var createIndexRequest = new CreateIndexRequest("personindex");
            createIndexRequest.Mappings = new Mappings();

            var typeMapping = new TypeMapping();
            typeMapping.Properties = new Properties();
            typeMapping.Properties.Add( "firstName", new StringProperty() { Name = "FirstName", Boost = 2 } );
            typeMapping.Properties.Add( "lastName", new StringProperty() { Name = "LastName", Boost = 4 } );
            typeMapping.Properties.Add( "iconCssClass", new StringProperty() { Name = "IconCssClass", Index = FieldIndexOption.No } );

            createIndexRequest.Mappings.Add( "personindex", typeMapping );

            var response = _client.CreateIndex( createIndexRequest );*/

            /*var putMapRequest = new PutMappingRequest("personindex", "personindex");
            putMapRequest.Properties = new Properties();
            putMapRequest.Properties.Add( "firstName", new StringProperty() { Name = "FirstName", Boost = 2 } );
            putMapRequest.Properties.Add( "lastName", new StringProperty() { Name = "LastName", Boost = 15 } );
            putMapRequest.Properties.Add( "iconCssClass", new StringProperty() { Name = "IconCssClass", Index = FieldIndexOption.No} );
            var putResponse = _client.Map( putMapRequest );*/



            var response2 = _client.Map<PersonIndex>( m => m.Index("personindex").Properties( ps => ps
                                     .String( s => s.Name( c => c.FirstName ) )
                                     .String( s => s.Name( c => c.LastName ).Boost( 15 ) ) ) );

            var result = _client.GetMapping<PersonIndex>();

            //http://stackoverflow.com/questions/35350490/create-index-with-multi-field-mapping-syntax-with-nest-2-x
        }
    }
}