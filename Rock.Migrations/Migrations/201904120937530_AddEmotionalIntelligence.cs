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
    public partial class AddEmotionalIntelligence : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "EQ: Self Aware", "core_EQSelfAware", "", "", 0, "", SystemGuid.Attribute.PERSON_EQ_CONSTRUCTS_SELF_AWARE );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_EQ_CONSTRUCTS_SELF_AWARE );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "EQ: Self Regulate", "core_EQSelfRegulate", "", "", 0, "", SystemGuid.Attribute.PERSON_EQ_CONSTRUCTS_SELF_REGULATE );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_EQ_CONSTRUCTS_SELF_REGULATE );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "EQ: Others Aware", "core_EQOthersAware", "", "", 0, "", SystemGuid.Attribute.PERSON_EQ_CONSTRUCTS_OTHERS_AWARE );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_EQ_CONSTRUCTS_OTHERS_AWARE );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "EQ: Others Regulate", "core_EQOthersRegulate", "", "", 0, "", SystemGuid.Attribute.PERSON_EQ_CONSTRUCTS_OTHERS_REGULATE );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_EQ_CONSTRUCTS_OTHERS_REGULATE );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "EQ: In Problem Solving", "core_EQProblemSolving", "", "", 0, "", SystemGuid.Attribute.PERSON_EQ_SCALES_IN_PROBLEM_SOLVING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_EQ_SCALES_IN_PROBLEM_SOLVING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "EQ: Under Stress", "core_EQUnderStress", "", "", 0, "", SystemGuid.Attribute.PERSON_EQ_SCALES_UNDER_STRESS );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_EQ_SCALES_UNDER_STRESS );
        }

        /// <summary>
        /// Add security to attributes
        /// </summary>
        private void AddSecurityToAttributes( string attributeGuid )
        {
            RockMigrationHelper.AddSecurityAuthForAttribute(
               attributeGuid,
               0,
               Rock.Security.Authorization.VIEW,
               true,
               Rock.SystemGuid.Group.GROUP_STAFF_MEMBERS,
               ( int ) Rock.Model.SpecialRole.None,
               Guid.NewGuid().ToString()
               );
            RockMigrationHelper.AddSecurityAuthForAttribute(
               attributeGuid,
               1,
               Rock.Security.Authorization.VIEW,
               true,
               Rock.SystemGuid.Group.GROUP_STAFF_LIKE_MEMBERS,
               ( int ) Rock.Model.SpecialRole.None,
               Guid.NewGuid().ToString()
               );
            RockMigrationHelper.AddSecurityAuthForAttribute(
                attributeGuid,
                2,
                Rock.Security.Authorization.VIEW,
                false,
                null,
                ( int ) Rock.Model.SpecialRole.AllUsers,
                Guid.NewGuid().ToString()
             );

            RockMigrationHelper.AddSecurityAuthForAttribute(
               attributeGuid,
               0,
               Rock.Security.Authorization.EDIT,
               true,
               Rock.SystemGuid.Group.GROUP_STAFF_MEMBERS,
               ( int ) Rock.Model.SpecialRole.None,
               Guid.NewGuid().ToString()
               );
            RockMigrationHelper.AddSecurityAuthForAttribute(
               attributeGuid,
               1,
               Rock.Security.Authorization.EDIT,
               true,
               Rock.SystemGuid.Group.GROUP_STAFF_LIKE_MEMBERS,
               ( int ) Rock.Model.SpecialRole.None,
               Guid.NewGuid().ToString()
               );
            RockMigrationHelper.AddSecurityAuthForAttribute(
                attributeGuid,
                2,
                Rock.Security.Authorization.EDIT,
                false,
                null,
                ( int ) Rock.Model.SpecialRole.AllUsers,
                Guid.NewGuid().ToString()
             );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
        }
    }
}
