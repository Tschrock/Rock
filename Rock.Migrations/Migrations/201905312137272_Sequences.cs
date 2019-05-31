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
    public partial class Sequences : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            CreateTable(
                "dbo.SequenceOccurrenceExclusion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SequenceId = c.Int(nullable: false),
                        LocationId = c.Int(),
                        ExclusionMap = c.Binary(),
                        CreatedDateTime = c.DateTime(),
                        ModifiedDateTime = c.DateTime(),
                        CreatedByPersonAliasId = c.Int(),
                        ModifiedByPersonAliasId = c.Int(),
                        Guid = c.Guid(nullable: false),
                        ForeignId = c.Int(),
                        ForeignGuid = c.Guid(),
                        ForeignKey = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PersonAlias", t => t.CreatedByPersonAliasId)
                .ForeignKey("dbo.Location", t => t.LocationId)
                .ForeignKey("dbo.PersonAlias", t => t.ModifiedByPersonAliasId)
                .ForeignKey("dbo.Sequence", t => t.SequenceId, cascadeDelete: true)
                .Index(t => t.SequenceId)
                .Index(t => t.LocationId)
                .Index(t => t.CreatedByPersonAliasId)
                .Index(t => t.ModifiedByPersonAliasId)
                .Index(t => t.Guid, unique: true);
            
            CreateTable(
                "dbo.Sequence",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        StructureType = c.Int(),
                        StructureEntityId = c.Int(),
                        EnableAttendance = c.Boolean(nullable: false),
                        RequiresEnrollment = c.Boolean(nullable: false),
                        OccurenceFrequency = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false, storeType: "date"),
                        OccurenceMap = c.Binary(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(),
                        ModifiedDateTime = c.DateTime(),
                        CreatedByPersonAliasId = c.Int(),
                        ModifiedByPersonAliasId = c.Int(),
                        Guid = c.Guid(nullable: false),
                        ForeignId = c.Int(),
                        ForeignGuid = c.Guid(),
                        ForeignKey = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PersonAlias", t => t.CreatedByPersonAliasId)
                .ForeignKey("dbo.PersonAlias", t => t.ModifiedByPersonAliasId)
                .Index(t => t.CreatedByPersonAliasId)
                .Index(t => t.ModifiedByPersonAliasId)
                .Index(t => t.Guid, unique: true);
            
            CreateTable(
                "dbo.SequenceEnrollment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SequenceId = c.Int(nullable: false),
                        PersonAliasId = c.Int(nullable: false),
                        EnrollmentDate = c.DateTime(nullable: false, storeType: "date"),
                        LocationId = c.Int(),
                        OccurenceMap = c.Binary(),
                        CreatedDateTime = c.DateTime(),
                        ModifiedDateTime = c.DateTime(),
                        CreatedByPersonAliasId = c.Int(),
                        ModifiedByPersonAliasId = c.Int(),
                        Guid = c.Guid(nullable: false),
                        ForeignId = c.Int(),
                        ForeignGuid = c.Guid(),
                        ForeignKey = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PersonAlias", t => t.CreatedByPersonAliasId)
                .ForeignKey("dbo.Location", t => t.LocationId)
                .ForeignKey("dbo.PersonAlias", t => t.ModifiedByPersonAliasId)
                .ForeignKey("dbo.PersonAlias", t => t.PersonAliasId, cascadeDelete: true)
                .ForeignKey("dbo.Sequence", t => t.SequenceId, cascadeDelete: true)
                .Index(t => t.SequenceId)
                .Index(t => t.PersonAliasId)
                .Index(t => t.LocationId)
                .Index(t => t.CreatedByPersonAliasId)
                .Index(t => t.ModifiedByPersonAliasId)
                .Index(t => t.Guid, unique: true);
            
        }
        
        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            DropForeignKey("dbo.SequenceOccurrenceExclusion", "SequenceId", "dbo.Sequence");
            DropForeignKey("dbo.SequenceEnrollment", "SequenceId", "dbo.Sequence");
            DropForeignKey("dbo.SequenceEnrollment", "PersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.SequenceEnrollment", "ModifiedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.SequenceEnrollment", "LocationId", "dbo.Location");
            DropForeignKey("dbo.SequenceEnrollment", "CreatedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.Sequence", "ModifiedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.Sequence", "CreatedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.SequenceOccurrenceExclusion", "ModifiedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.SequenceOccurrenceExclusion", "LocationId", "dbo.Location");
            DropForeignKey("dbo.SequenceOccurrenceExclusion", "CreatedByPersonAliasId", "dbo.PersonAlias");
            DropIndex("dbo.SequenceEnrollment", new[] { "Guid" });
            DropIndex("dbo.SequenceEnrollment", new[] { "ModifiedByPersonAliasId" });
            DropIndex("dbo.SequenceEnrollment", new[] { "CreatedByPersonAliasId" });
            DropIndex("dbo.SequenceEnrollment", new[] { "LocationId" });
            DropIndex("dbo.SequenceEnrollment", new[] { "PersonAliasId" });
            DropIndex("dbo.SequenceEnrollment", new[] { "SequenceId" });
            DropIndex("dbo.Sequence", new[] { "Guid" });
            DropIndex("dbo.Sequence", new[] { "ModifiedByPersonAliasId" });
            DropIndex("dbo.Sequence", new[] { "CreatedByPersonAliasId" });
            DropIndex("dbo.SequenceOccurrenceExclusion", new[] { "Guid" });
            DropIndex("dbo.SequenceOccurrenceExclusion", new[] { "ModifiedByPersonAliasId" });
            DropIndex("dbo.SequenceOccurrenceExclusion", new[] { "CreatedByPersonAliasId" });
            DropIndex("dbo.SequenceOccurrenceExclusion", new[] { "LocationId" });
            DropIndex("dbo.SequenceOccurrenceExclusion", new[] { "SequenceId" });
            DropTable("dbo.SequenceEnrollment");
            DropTable("dbo.Sequence");
            DropTable("dbo.SequenceOccurrenceExclusion");
        }
    }
}
