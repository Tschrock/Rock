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
            int completeStatusValue = Rock.Model.AssessmentRequestStatus.Complete.ConvertToInt();
            Sql( $@"
-- Convert existing DISC test to an Assessment record
DECLARE @DiscLastSaveDateAttributeId INT = ( SELECT TOP 1 [Id] FROM [Attribute] where [Guid]='{SystemGuid.Attribute.PERSON_DISC_LAST_SAVE_DATE}' )
DECLARE @DiscAssessmentTypeId INT = ( SELECT TOP 1 [Id] FROM [AssessmentType] where [Guid]='{SystemGuid.AssessmentType.DISC}' )

INSERT INTO Assessment
([PersonAliasId], [AssessmentTypeId], [Status], [CompletedDateTime], [Guid])
SELECT DISTINCT
B.[Id],@DiscAssessmentTypeId,{completeStatusValue},[ValueAsDateTime],NEWID()
FROM
	[AttributeValue] A
INNER JOIN
	[PersonAlias] B
ON
	A.[EntityId] = B.[PersonId]
WHERE
	A.[AttributeId]=@DiscLastSaveDateAttributeId
	AND	A.[EntityId] NOT IN (SELECT D.[PersonId] FROM [Assessment] C INNER JOIN [PersonAlias] D ON C.[PersonAliasId] = D.[Id] WHERE C.[AssessmentTypeId]=@DiscAssessmentTypeId)
	AND A.[ValueAsDateTime] IS NOT NULL
" );

            Sql( $@"
-- Convert existing Spiritual Gifts test to an Assessment record
DECLARE @GiftLastSaveDateAttributeId INT = ( SELECT TOP 1 [Id] FROM [Attribute] where [Guid]='{SystemGuid.Attribute.PERSON_SPIRITUAL_GIFTS_LAST_SAVE_DATE}' )
DECLARE @GiftAssessmentTypeId INT = ( SELECT TOP 1 [Id] FROM [AssessmentType] where [Guid]='{SystemGuid.AssessmentType.GIFTS}' )

INSERT INTO Assessment
([PersonAliasId], [AssessmentTypeId], [Status], [CompletedDateTime], [Guid])
SELECT DISTINCT
B.[Id],@GiftAssessmentTypeId,{completeStatusValue},[ValueAsDateTime],NEWID()
FROM
	[AttributeValue] A
INNER JOIN
	[PersonAlias] B
ON
	A.[EntityId] = B.[PersonId]
WHERE
	A.[AttributeId]=@GiftLastSaveDateAttributeId
	AND	A.[EntityId] NOT IN (SELECT D.[PersonId] FROM [Assessment] C INNER JOIN [PersonAlias] D ON C.[PersonAliasId] = D.[Id] WHERE C.[AssessmentTypeId]=@GiftAssessmentTypeId)
	AND A.[ValueAsDateTime] IS NOT NULL
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
