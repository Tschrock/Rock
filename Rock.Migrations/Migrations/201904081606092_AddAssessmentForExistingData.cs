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
    public partial class AddAssessmentForExistingData : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            Sql( @"
DECLARE @DiscAttributeId INT = ( SELECT TOP 1 [Id] FROM [Attribute] where [Guid]='990275DB-611B-4D2E-94EA-3FFA1186A5E1' )
DECLARE @DiscAssessmentTypeId INT = ( SELECT TOP 1 [Id] FROM [AssessmentType] where [Guid]='A5CB2E3D-118A-41F2-972B-325A328B0B54' )

INSERT INTO Assessment
([PersonAliasId], [AssessmentTypeId], [Status], [CompletedDateTime], [Guid])
SELECT DISTINCT
B.[Id],@DiscAssessmentTypeId,3,[ValueAsDateTime],NEWID()
FROM
	[AttributeValue] A
INNER JOIN
	[PersonAlias] B
ON
	A.[EntityId]=B.[PersonId]
WHERE
	A.[AttributeId]=@DiscAttributeId
	AND
	A.[EntityId] NOT IN (SELECT D.[PersonId] FROM [Assessment] C INNER JOIN [PersonAlias] D ON C.[PersonAliasId] = D.[Id] WHERE C.[AssessmentTypeId]=@DiscAssessmentTypeId)
" );

            Sql( @"
DECLARE @GiftAttributeId INT = ( SELECT TOP 1 [Id] FROM [Attribute] where [Guid]='3668547C-3DC4-450B-B92D-4B98A693A371' )
DECLARE @GiftAssessmentTypeId INT = ( SELECT TOP 1 [Id] FROM [AssessmentType] where [Guid]='B8FBD371-6B32-4BE5-872F-51400D16EC5D' )

INSERT INTO Assessment
([PersonAliasId], [AssessmentTypeId], [Status], [CompletedDateTime], [Guid])
SELECT DISTINCT
B.[Id],@GiftAssessmentTypeId,3,[ValueAsDateTime],NEWID()
FROM
	[AttributeValue] A
INNER JOIN
	[PersonAlias] B
ON
	A.[EntityId]=B.[PersonId]
WHERE
	A.[AttributeId]=@GiftAttributeId
	AND
	A.[EntityId] NOT IN (SELECT D.[PersonId] FROM [Assessment] C INNER JOIN [PersonAlias] D ON C.[PersonAliasId] = D.[Id] WHERE C.[AssessmentTypeId]=@GiftAssessmentTypeId)

" );
        }
        
        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
        }
    }
}
