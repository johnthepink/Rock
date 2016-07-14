// <copyright>
// Copyright 2013 by the Spark Development Network
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
namespace Rock.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    /// <summary>
    ///
    /// </summary>
    public partial class AttributeUpdateFix2 : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            DropColumn("dbo.ContentChannel", "ContentItemPublishingPoint");

            // add pages
            RockMigrationHelper.AddPage( "0B213645-FA4E-44A5-8E4C-B2D8EF054985", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Universal Search Control Panel", "", "7AE403F2-4328-4168-A941-0A506F1AAE14", "fa fa-search-plus" ); // Site:Rock RMS
            RockMigrationHelper.AddPage( "C831428A-6ACD-4D49-9B2D-046D399E3123", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Universal Search Index Components", "", "FF26DBAC-7E4B-4C55-8EE0-2277187D06F3", "fa fa-search-plus" ); // Site:Rock RMS

            // add block types
            RockMigrationHelper.UpdateBlockType( "Universal Search", "A block to search for all indexable entity types in Rock.", "~/Blocks/Cms/UniversalSearch.ascx", "CMS", "FDF1BBFF-7A7B-4F4E-BF34-831203B0FEAC" );
            RockMigrationHelper.UpdateBlockType( "Universal Search Control Panel", "Block to configure Rock's universal search features.", "~/Blocks/Core/UniversalSearchControlPanel.ascx", "Core", "59F03418-0638-48E0-877D-B2F15B52C540" );
           
            // Add Block to Page: Universal Search Control Panel, Site: Rock RMS
            RockMigrationHelper.AddBlock( "7AE403F2-4328-4168-A941-0A506F1AAE14", "", "59F03418-0638-48E0-877D-B2F15B52C540", "Universal Search Control Panel", "Main", "", "", 0, "A8BD8A52-74BF-4E78-AB0E-594C7E04EBD4" );
            // Add Block to Page: Universal Search Index Components, Site: Rock RMS
            RockMigrationHelper.AddBlock( "FF26DBAC-7E4B-4C55-8EE0-2277187D06F3", "", "21F5F466-59BC-40B2-8D73-7314D936C3CB", "Components", "Main", "", "", 0, "ADCBC65E-F4A3-4727-BE6A-800B3B7136C2" );
            // Attrib Value for Block:Components, Attribute:Component Container Page: Universal Search Index Components, Site: Rock RMS
            RockMigrationHelper.AddBlockAttributeValue( "ADCBC65E-F4A3-4727-BE6A-800B3B7136C2", "259AF14D-0214-4BE4-A7BF-40423EA07C99", @"Rock.UniversalSearch.IndexContainer, Rock" );
            // Attrib Value for Block:Components, Attribute:Support Ordering Page: Universal Search Index Components, Site: Rock RMS
            RockMigrationHelper.AddBlockAttributeValue( "ADCBC65E-F4A3-4727-BE6A-800B3B7136C2", "A4889D7B-87AA-419D-846C-3E618E79D875", @"True" );

        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            AddColumn("dbo.ContentChannel", "ContentItemPublishingPoint", c => c.String(maxLength: 250));

            // Remove Block: Universal Search Control Panel, from Page: Universal Search Control Panel, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "A8BD8A52-74BF-4E78-AB0E-594C7E04EBD4" );
            RockMigrationHelper.DeleteBlockType( "59F03418-0638-48E0-877D-B2F15B52C540" ); // Universal Search Control Panel
            RockMigrationHelper.DeleteBlockType( "FDF1BBFF-7A7B-4F4E-BF34-831203B0FEAC" ); // Universal Search
            RockMigrationHelper.DeletePage( "FF26DBAC-7E4B-4C55-8EE0-2277187D06F3" ); //  Page: Universal Search Index Components, Layout: Full Width, Site: Rock RMS
            RockMigrationHelper.DeletePage( "7AE403F2-4328-4168-A941-0A506F1AAE14" ); //  Page: Universal Search Control Panel, Layout: Full Width, Site: Rock RMS
        }
    }
}
