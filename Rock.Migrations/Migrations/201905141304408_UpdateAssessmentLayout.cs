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
    public partial class UpdateAssessmentLayout : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            string updatePageSQL = string.Format( @"

                DECLARE @LayoutId int = ( SELECT [Id] FROM [Layout] WHERE [Guid] = '{0}' )
                
                UPDATE 
	                [Page]
                SET [LayoutId]= @LayoutId
                WHERE
                    [Guid] IN ('{1}','{2}')
               ", "BE15B7BC-6D64-4880-991D-FDE962F91196", "0E6AECD6-675F-4908-9FA3-C7E46040527C", "06410598-3DA4-4710-A047-A518157753AB"
               );

            Sql( updatePageSQL );
            
        }
        
        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
        }
    }
}
