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
    /// <summary>
    /// This script does not add pages or blocks automatically, because the intent is for custom plugins to setup
    /// step programs. Here is a script to add pages and blocks for testing: 
    /// https://gist.github.com/bjwiley2/a800176a96fbcda22a8759cf20f250da
    /// </summary>
    public partial class Steps : RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            ModelsUp();
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            ModelsDown();
        }

        private void ModelsUp()
        {
            CreateTable(
                "dbo.Step",
                c => new
                {
                    Id = c.Int( nullable: false, identity: true ),
                    StepTypeId = c.Int( nullable: false ),
                    StepStatusId = c.Int(),
                    PersonAliasId = c.Int( nullable: false ),
                    CampusId = c.Int(),
                    CompletedDateTime = c.DateTime(),
                    StartDateTime = c.DateTime(),
                    EndDateTime = c.DateTime(),
                    Note = c.String(),
                    Order = c.Int( nullable: false ),
                    CreatedDateTime = c.DateTime(),
                    ModifiedDateTime = c.DateTime(),
                    CreatedByPersonAliasId = c.Int(),
                    ModifiedByPersonAliasId = c.Int(),
                    Guid = c.Guid( nullable: false ),
                    ForeignId = c.Int(),
                    ForeignGuid = c.Guid(),
                    ForeignKey = c.String( maxLength: 100 ),
                    StepType_Id = c.Int(),
                } )
                .PrimaryKey( t => t.Id )
                .ForeignKey( "dbo.Campus", t => t.CampusId )
                .ForeignKey( "dbo.PersonAlias", t => t.CreatedByPersonAliasId )
                .ForeignKey( "dbo.PersonAlias", t => t.ModifiedByPersonAliasId )
                .ForeignKey( "dbo.PersonAlias", t => t.PersonAliasId, cascadeDelete: true )
                .ForeignKey( "dbo.StepType", t => t.StepType_Id )
                .ForeignKey( "dbo.StepStatus", t => t.StepStatusId )
                .ForeignKey( "dbo.StepType", t => t.StepTypeId, cascadeDelete: true )
                .Index( t => t.StepTypeId )
                .Index( t => t.StepStatusId )
                .Index( t => t.PersonAliasId )
                .Index( t => t.CampusId )
                .Index( t => t.CreatedByPersonAliasId )
                .Index( t => t.ModifiedByPersonAliasId )
                .Index( t => t.Guid, unique: true )
                .Index( t => t.StepType_Id );

            CreateTable(
                "dbo.StepStatus",
                c => new
                {
                    Id = c.Int( nullable: false, identity: true ),
                    Name = c.String( nullable: false, maxLength: 50 ),
                    StepProgramId = c.Int( nullable: false ),
                    IsCompleteStatus = c.Boolean( nullable: false ),
                    StatusColor = c.String( maxLength: 100 ),
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
                .ForeignKey( "dbo.PersonAlias", t => t.CreatedByPersonAliasId )
                .ForeignKey( "dbo.PersonAlias", t => t.ModifiedByPersonAliasId )
                .ForeignKey( "dbo.StepProgram", t => t.StepProgramId, cascadeDelete: true )
                .Index( t => t.StepProgramId )
                .Index( t => t.CreatedByPersonAliasId )
                .Index( t => t.ModifiedByPersonAliasId )
                .Index( t => t.Guid, unique: true );

            CreateTable(
                "dbo.StepProgram",
                c => new
                {
                    Id = c.Int( nullable: false, identity: true ),
                    Name = c.String( nullable: false, maxLength: 250 ),
                    Description = c.String(),
                    IconCssClass = c.String( maxLength: 100 ),
                    CategoryId = c.Int( nullable: false ),
                    DefaultListView = c.Int( nullable: false ),
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
                .ForeignKey( "dbo.Category", t => t.CategoryId )
                .ForeignKey( "dbo.PersonAlias", t => t.CreatedByPersonAliasId )
                .ForeignKey( "dbo.PersonAlias", t => t.ModifiedByPersonAliasId )
                .Index( t => t.CategoryId )
                .Index( t => t.CreatedByPersonAliasId )
                .Index( t => t.ModifiedByPersonAliasId )
                .Index( t => t.Guid, unique: true );

            CreateTable(
                "dbo.StepType",
                c => new
                {
                    Id = c.Int( nullable: false, identity: true ),
                    StepProgramId = c.Int( nullable: false ),
                    Name = c.String( nullable: false, maxLength: 250 ),
                    Description = c.String(),
                    IconCssClass = c.String( maxLength: 100 ),
                    AllowMultiple = c.Boolean( nullable: false ),
                    IsActive = c.Boolean( nullable: false ),
                    HasEndDate = c.Boolean( nullable: false ),
                    ShowCountOnBadge = c.Boolean( nullable: false ),
                    AllowManualEditing = c.Boolean( nullable: false ),
                    HighlightColor = c.String( maxLength: 100 ),
                    Order = c.Int( nullable: false ),
                    CreatedDateTime = c.DateTime(),
                    ModifiedDateTime = c.DateTime(),
                    CreatedByPersonAliasId = c.Int(),
                    ModifiedByPersonAliasId = c.Int(),
                    Guid = c.Guid( nullable: false ),
                    ForeignId = c.Int(),
                    ForeignGuid = c.Guid(),
                    ForeignKey = c.String( maxLength: 100 ),
                    StepProgram_Id = c.Int(),
                } )
                .PrimaryKey( t => t.Id )
                .ForeignKey( "dbo.PersonAlias", t => t.CreatedByPersonAliasId )
                .ForeignKey( "dbo.PersonAlias", t => t.ModifiedByPersonAliasId )
                .ForeignKey( "dbo.StepProgram", t => t.StepProgramId, cascadeDelete: true )
                .ForeignKey( "dbo.StepProgram", t => t.StepProgram_Id )
                .Index( t => t.StepProgramId )
                .Index( t => t.CreatedByPersonAliasId )
                .Index( t => t.ModifiedByPersonAliasId )
                .Index( t => t.Guid, unique: true )
                .Index( t => t.StepProgram_Id );

            CreateTable(
                "dbo.StepTypePrerequisite",
                c => new
                {
                    Id = c.Int( nullable: false, identity: true ),
                    StepTypeId = c.Int( nullable: false ),
                    PrerequisiteStepTypeId = c.Int( nullable: false ),
                    Order = c.Int( nullable: false ),
                    CreatedDateTime = c.DateTime(),
                    ModifiedDateTime = c.DateTime(),
                    CreatedByPersonAliasId = c.Int(),
                    ModifiedByPersonAliasId = c.Int(),
                    Guid = c.Guid( nullable: false ),
                    ForeignId = c.Int(),
                    ForeignGuid = c.Guid(),
                    ForeignKey = c.String( maxLength: 100 ),
                    StepType_Id = c.Int(),
                } )
                .PrimaryKey( t => t.Id )
                .ForeignKey( "dbo.PersonAlias", t => t.CreatedByPersonAliasId )
                .ForeignKey( "dbo.PersonAlias", t => t.ModifiedByPersonAliasId )
                .ForeignKey( "dbo.StepType", t => t.PrerequisiteStepTypeId )
                .ForeignKey( "dbo.StepType", t => t.StepTypeId, cascadeDelete: true )
                .ForeignKey( "dbo.StepType", t => t.StepType_Id )
                .Index( t => t.StepTypeId )
                .Index( t => t.PrerequisiteStepTypeId )
                .Index( t => t.CreatedByPersonAliasId )
                .Index( t => t.ModifiedByPersonAliasId )
                .Index( t => t.Guid, unique: true )
                .Index( t => t.StepType_Id );

        }

        private void ModelsDown()
        {
            DropForeignKey( "dbo.Step", "StepTypeId", "dbo.StepType" );
            DropForeignKey( "dbo.Step", "StepStatusId", "dbo.StepStatus" );
            DropForeignKey( "dbo.StepStatus", "StepProgramId", "dbo.StepProgram" );
            DropForeignKey( "dbo.StepType", "StepProgram_Id", "dbo.StepProgram" );
            DropForeignKey( "dbo.StepTypePrerequisite", "StepType_Id", "dbo.StepType" );
            DropForeignKey( "dbo.StepTypePrerequisite", "StepTypeId", "dbo.StepType" );
            DropForeignKey( "dbo.StepTypePrerequisite", "PrerequisiteStepTypeId", "dbo.StepType" );
            DropForeignKey( "dbo.StepTypePrerequisite", "ModifiedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.StepTypePrerequisite", "CreatedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.Step", "StepType_Id", "dbo.StepType" );
            DropForeignKey( "dbo.StepType", "StepProgramId", "dbo.StepProgram" );
            DropForeignKey( "dbo.StepType", "ModifiedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.StepType", "CreatedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.StepProgram", "ModifiedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.StepProgram", "CreatedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.StepProgram", "CategoryId", "dbo.Category" );
            DropForeignKey( "dbo.StepStatus", "ModifiedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.StepStatus", "CreatedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.Step", "PersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.Step", "ModifiedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.Step", "CreatedByPersonAliasId", "dbo.PersonAlias" );
            DropForeignKey( "dbo.Step", "CampusId", "dbo.Campus" );
            DropIndex( "dbo.StepTypePrerequisite", new[] { "StepType_Id" } );
            DropIndex( "dbo.StepTypePrerequisite", new[] { "Guid" } );
            DropIndex( "dbo.StepTypePrerequisite", new[] { "ModifiedByPersonAliasId" } );
            DropIndex( "dbo.StepTypePrerequisite", new[] { "CreatedByPersonAliasId" } );
            DropIndex( "dbo.StepTypePrerequisite", new[] { "PrerequisiteStepTypeId" } );
            DropIndex( "dbo.StepTypePrerequisite", new[] { "StepTypeId" } );
            DropIndex( "dbo.StepType", new[] { "StepProgram_Id" } );
            DropIndex( "dbo.StepType", new[] { "Guid" } );
            DropIndex( "dbo.StepType", new[] { "ModifiedByPersonAliasId" } );
            DropIndex( "dbo.StepType", new[] { "CreatedByPersonAliasId" } );
            DropIndex( "dbo.StepType", new[] { "StepProgramId" } );
            DropIndex( "dbo.StepProgram", new[] { "Guid" } );
            DropIndex( "dbo.StepProgram", new[] { "ModifiedByPersonAliasId" } );
            DropIndex( "dbo.StepProgram", new[] { "CreatedByPersonAliasId" } );
            DropIndex( "dbo.StepProgram", new[] { "CategoryId" } );
            DropIndex( "dbo.StepStatus", new[] { "Guid" } );
            DropIndex( "dbo.StepStatus", new[] { "ModifiedByPersonAliasId" } );
            DropIndex( "dbo.StepStatus", new[] { "CreatedByPersonAliasId" } );
            DropIndex( "dbo.StepStatus", new[] { "StepProgramId" } );
            DropIndex( "dbo.Step", new[] { "StepType_Id" } );
            DropIndex( "dbo.Step", new[] { "Guid" } );
            DropIndex( "dbo.Step", new[] { "ModifiedByPersonAliasId" } );
            DropIndex( "dbo.Step", new[] { "CreatedByPersonAliasId" } );
            DropIndex( "dbo.Step", new[] { "CampusId" } );
            DropIndex( "dbo.Step", new[] { "PersonAliasId" } );
            DropIndex( "dbo.Step", new[] { "StepStatusId" } );
            DropIndex( "dbo.Step", new[] { "StepTypeId" } );
            DropTable( "dbo.StepTypePrerequisite" );
            DropTable( "dbo.StepType" );
            DropTable( "dbo.StepProgram" );
            DropTable( "dbo.StepStatus" );
            DropTable( "dbo.Step" );
        }
    }
}
