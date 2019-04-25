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
using System.ComponentModel;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;

namespace RockWeb.Blocks.Steps
{
    [DisplayName( "Steps" )]
    [Category( "Steps" )]
    [Description( "Renders a page menu based on a root page and lava template." )]

    public partial class PersonProgramStepList : RockBlock
    {
        /// <summary>
        /// Handles the OnInit event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            BlockUpdated += PageMenu_BlockUpdated;

            if (!IsPostBack)
            {
                var rockContext = new RockContext();
                var program = GetStepProgram( rockContext );

                lStepProgramName.Text = program.Name;
            }
        }

        /// <summary>
        /// Handles the BlockUpdated event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void PageMenu_BlockUpdated( object sender, EventArgs e )
        {
        }

        private int GetStepProgramId()
        {
            return 1;
        }

        private StepProgram GetStepProgram( RockContext rockContext )
        {
            var id = GetStepProgramId();
            return ( new StepProgramService( rockContext ) ).Get( id );
        }
    }
}