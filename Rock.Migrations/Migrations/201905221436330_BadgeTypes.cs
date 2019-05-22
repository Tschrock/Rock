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
namespace Rock.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    /// <summary>
    ///
    /// </summary>
    public partial class BadgeTypes : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            CreateTable(
                "dbo.BadgeType",
                c => new
                {
                    Id = c.Int( nullable: false, identity: true ),
                    Name = c.String( nullable: false, maxLength: 100 ),
                    Description = c.String(),
                    ComponentEntityTypeId = c.Int( nullable: false ),
                    SubjectEntityTypeId = c.Int( nullable: false ),
                    IsActive = c.Boolean( nullable: false ),
                    Order = c.Int( nullable: false ),
                    CreatedDateTime = c.DateTime(),
                    ModifiedDateTime = c.DateTime(),
                    CreatedByPersonAliasId = c.Int(),
                    ModifiedByPersonAliasId = c.Int(),
                    Guid = c.Guid( nullable: false ),
                    ForeignId = c.Int(),
                    ForeignGuid = c.Guid(),
                    ForeignKey = c.String( maxLength: 100 ),
                } )
                .PrimaryKey( t => t.Id )
                .ForeignKey( "dbo.EntityType", t => t.ComponentEntityTypeId )
                .ForeignKey( "dbo.PersonAlias", t => t.CreatedByPersonAliasId )
                .ForeignKey( "dbo.PersonAlias", t => t.ModifiedByPersonAliasId )
                .ForeignKey( "dbo.EntityType", t => t.SubjectEntityTypeId )
                .Index( t => t.ComponentEntityTypeId )
                .Index( t => t.SubjectEntityTypeId )
                .Index( t => t.CreatedByPersonAliasId )
                .Index( t => t.ModifiedByPersonAliasId )
                .Index( t => t.Guid, unique: true );

            RockMigrationHelper.UpdateEntityType( "Rock.Model.BadgeType", "Badge Type", "Rock.Model.BadgeType, Rock, Version=1.9.0.0, Culture=neutral, PublicKeyToken=null", true, true, Rock.SystemGuid.EntityType.BADGE_TYPE );

            Sql( @"
INSERT INTO BadgeType (
	[Name]
	,[Description]
	,[ComponentEntityTypeId]
	,[SubjectEntityTypeId]
	,[IsActive]
	,[Order]
	,[CreatedDateTime]
	,[ModifiedDateTime]
	,[CreatedByPersonAliasId]
	,[ModifiedByPersonAliasId]
	,[Guid]
	,[ForeignId]
	,[ForeignGuid]
	,[ForeignKey]
)
SELECT
	[Name]
	,[Description]
	,[EntityTypeId]
	,(SELECT Id FROM EntityType WHERE Name = 'Rock.Model.Person')
	,1
	,[Order]
	,[CreatedDateTime]
	,[ModifiedDateTime]
	,[CreatedByPersonAliasId]
	,[ModifiedByPersonAliasId]
	,[Guid]
	,[ForeignId]
	,[ForeignGuid]
	,[ForeignKey]
FROM
	PersonBadge
WHERE
	Guid NOT IN (SELECT Guid FROM BadgeType);" );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            DropForeignKey( "dbo.BadgeType", "SubjectEntityTypeId", "dbo.EntityType" );
            DropForeignKey( "dbo.BadgeType", "ModifiedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.BadgeType", "CreatedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.BadgeType", "ComponentEntityTypeId", "dbo.EntityType" );
            DropIndex( "dbo.BadgeType", new[] { "Guid" } );
            DropIndex( "dbo.BadgeType", new[] { "ModifiedByPersonAliasId" } );
            DropIndex( "dbo.BadgeType", new[] { "CreatedByPersonAliasId" } );
            DropIndex( "dbo.BadgeType", new[] { "SubjectEntityTypeId" } );
            DropIndex( "dbo.BadgeType", new[] { "ComponentEntityTypeId" } );
            DropTable( "dbo.BadgeType" );
        }
    }
}