//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
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


namespace Rock.Client
{
    /// <summary>
    /// Base client model for ContentChannelItem that only includes the non-virtual fields. Use this for PUT/POSTs
    /// </summary>
    public partial class ContentChannelItemEntity
    {
        /// <summary />
        public int Id { get; set; }

        /// <summary />
        public int? ApprovedByPersonAliasId { get; set; }

        /// <summary />
        public DateTime? ApprovedDateTime { get; set; }

        /// <summary />
        public string Content { get; set; }

        /// <summary />
        public int ContentChannelId { get; set; }

        /// <summary />
        public int ContentChannelTypeId { get; set; }

        /// <summary />
        public DateTime? ExpireDateTime { get; set; }

        /// <summary />
        public Guid? ForeignGuid { get; set; }

        /// <summary />
        public string ForeignKey { get; set; }

        /// <summary>
        /// If the ModifiedByPersonAliasId is being set manually and should not be overwritten with current user when saved, set this value to true
        /// </summary>
        public bool ModifiedAuditValuesAlreadyUpdated { get; set; }

        /// <summary />
        public string Permalink { get; set; }

        /// <summary />
        public int Priority { get; set; }

        /// <summary />
        public DateTime StartDateTime { get; set; }

        /// <summary />
        public Rock.Client.Enums.ContentChannelItemStatus Status { get; set; }

        /// <summary />
        public string Title { get; set; }

        /// <summary>
        /// Leave this as NULL to let Rock set this
        /// </summary>
        public DateTime? CreatedDateTime { get; set; }

        /// <summary>
        /// This does not need to be set or changed. Rock will always set this to the current date/time when saved to the database.
        /// </summary>
        public DateTime? ModifiedDateTime { get; set; }

        /// <summary>
        /// Leave this as NULL to let Rock set this
        /// </summary>
        public int? CreatedByPersonAliasId { get; set; }

        /// <summary>
        /// If you need to set this manually, set ModifiedAuditValuesAlreadyUpdated=True to prevent Rock from setting it
        /// </summary>
        public int? ModifiedByPersonAliasId { get; set; }

        /// <summary />
        public Guid Guid { get; set; }

        /// <summary />
        public int? ForeignId { get; set; }

        /// <summary>
        /// Copies the base properties from a source ContentChannelItem object
        /// </summary>
        /// <param name="source">The source.</param>
        public void CopyPropertiesFrom( ContentChannelItem source )
        {
            this.Id = source.Id;
            this.ApprovedByPersonAliasId = source.ApprovedByPersonAliasId;
            this.ApprovedDateTime = source.ApprovedDateTime;
            this.Content = source.Content;
            this.ContentChannelId = source.ContentChannelId;
            this.ContentChannelTypeId = source.ContentChannelTypeId;
            this.ExpireDateTime = source.ExpireDateTime;
            this.ForeignGuid = source.ForeignGuid;
            this.ForeignKey = source.ForeignKey;
            this.ModifiedAuditValuesAlreadyUpdated = source.ModifiedAuditValuesAlreadyUpdated;
            this.Permalink = source.Permalink;
            this.Priority = source.Priority;
            this.StartDateTime = source.StartDateTime;
            this.Status = source.Status;
            this.Title = source.Title;
            this.CreatedDateTime = source.CreatedDateTime;
            this.ModifiedDateTime = source.ModifiedDateTime;
            this.CreatedByPersonAliasId = source.CreatedByPersonAliasId;
            this.ModifiedByPersonAliasId = source.ModifiedByPersonAliasId;
            this.Guid = source.Guid;
            this.ForeignId = source.ForeignId;

        }
    }

    /// <summary>
    /// Client model for ContentChannelItem that includes all the fields that are available for GETs. Use this for GETs (use ContentChannelItemEntity for POST/PUTs)
    /// </summary>
    public partial class ContentChannelItem : ContentChannelItemEntity
    {
        /// <summary />
        public ContentChannel ContentChannel { get; set; }

        /// <summary />
        public ContentChannelType ContentChannelType { get; set; }

        /// <summary>
        /// NOTE: Attributes are only populated when ?loadAttributes is specified. Options for loadAttributes are true, false, 'simple', 'expanded' 
        /// </summary>
        public Dictionary<string, Rock.Client.Attribute> Attributes { get; set; }

        /// <summary>
        /// NOTE: AttributeValues are only populated when ?loadAttributes is specified. Options for loadAttributes are true, false, 'simple', 'expanded' 
        /// </summary>
        public Dictionary<string, Rock.Client.AttributeValue> AttributeValues { get; set; }
    }
}
