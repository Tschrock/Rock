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
    ///
    /// </summary>
    public partial class AssessmentsPagesAndBlocks : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            this.CodeGenMigrationsUp();
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            this.CodeGenMigrationsDown();
        }

        private void CodeGenMigrationsUp()
        {
            RockMigrationHelper.AddPage( true, "C831428A-6ACD-4D49-9B2D-046D399E3123", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Assessment Types", "", "CC59F2B4-16B4-47BE-B8A0-E417EABA068F", "fa fa-directions" ); // Site:Rock RMS
            RockMigrationHelper.AddPage( true, "CC59F2B4-16B4-47BE-B8A0-E417EABA068F", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Assessment Type Detail", "", "F3C96663-1079-4F20-BABA-3F3203AFCFF3", "fa fa-directions" ); // Site:Rock RMS

            RockMigrationHelper.UpdateBlockType( "Assessment Type List", "Shows a list of all Assessment Types.", "~/Blocks/Assessments/AssessmentTypeList.ascx", "Assessments", "00A86827-1E0C-4F47-8A6F-82581FA75CED" );
            RockMigrationHelper.UpdateBlockType( "Assessment Type Detail", "Displays the details of the given Assessment Type for editing.", "~/Blocks/Assessments/AssessmentTypeDetail.ascx", "Assessments", "A81AB554-B438-4C7F-9C45-1A9AE2F889C5" );

            // Add Block to Page: Assessment Type Detail Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "F3C96663-1079-4F20-BABA-3F3203AFCFF3".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "A81AB554-B438-4C7F-9C45-1A9AE2F889C5".AsGuid(), "Assessment Type Detail", "Main", @"", @"", 0, "8918560C-B8E0-4ED6-9379-BCC191A57B65" );
            // Add Block to Page: Assessment Types Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "CC59F2B4-16B4-47BE-B8A0-E417EABA068F".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "00A86827-1E0C-4F47-8A6F-82581FA75CED".AsGuid(), "Assessment Type List", "Main", @"", @"", 0, "8D486E88-EB00-40C7-8C66-90B9B92E8823" );

            // Attrib for BlockType: Assessment Type List:Detail Page
            RockMigrationHelper.UpdateBlockTypeAttribute( "00A86827-1E0C-4F47-8A6F-82581FA75CED", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Detail Page", "DetailPage", "", @"", 1, @"", "A0DFC7E8-6E63-403F-9D1B-5E3D3684AD8D" );

            // Attrib Value for Block:Assessment Type List, Attribute:Detail Page Page: Assessment Types, Site: Rock RMS
            RockMigrationHelper.AddBlockAttributeValue( "8D486E88-EB00-40C7-8C66-90B9B92E8823", "A0DFC7E8-6E63-403F-9D1B-5E3D3684AD8D", @"f3c96663-1079-4f20-baba-3f3203afcff3" );
        }

        private void CodeGenMigrationsDown()
        {
            // Attrib for BlockType: Assessment Type List:Detail Page
            RockMigrationHelper.DeleteAttribute( "A0DFC7E8-6E63-403F-9D1B-5E3D3684AD8D" );

            // Remove Block: Assessment Type List, from Page: Assessment Types, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "8D486E88-EB00-40C7-8C66-90B9B92E8823" );
            // Remove Block: Assessment Type Detail, from Page: Assessment Type Detail, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "8918560C-B8E0-4ED6-9379-BCC191A57B65" );

            RockMigrationHelper.DeleteBlockType( "A81AB554-B438-4C7F-9C45-1A9AE2F889C5" ); // Assessment Type Detail
            RockMigrationHelper.DeleteBlockType( "00A86827-1E0C-4F47-8A6F-82581FA75CED" ); // Assessment Type List

            RockMigrationHelper.DeletePage( "F3C96663-1079-4F20-BABA-3F3203AFCFF3" ); //  Page: Assessment Type Detail, Layout: Full Width, Site: Rock RMS
            RockMigrationHelper.DeletePage( "CC59F2B4-16B4-47BE-B8A0-E417EABA068F" ); //  Page: Assessment Types, Layout: Full Width, Site: Rock RMS
        }
    }
}
