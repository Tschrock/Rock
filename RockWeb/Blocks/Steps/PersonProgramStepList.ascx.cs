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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.Steps
{
    [DisplayName( "Steps" )]
    [Category( "Steps" )]
    [Description( "Renders a page menu based on a root page and lava template." )]

    #region Attributes

    [IntegerField(
        name: "Step Program Id",
        description: "The Id of the Step Program to display. This value can also be a page parameter: StepProgramId. Leave blank to use the page parameter.",
        required: false,
        order: 1,
        key: AttributeKeys.StepProgramId )]

    #endregion Attributes

    public partial class PersonProgramStepList : RockBlock
    {
        private class AttributeKeys
        {
            public const string StepProgramId = "StepProgramId";
        }

        #region Events

        /// <summary>
        /// Handles the OnInit event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            BlockUpdated += PageMenu_BlockUpdated;

            if ( !IsPostBack )
            {
                var rockContext = new RockContext();
                SetProgramDetailsOnBlock( rockContext );
                RenderCards( rockContext );
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

        #endregion Events

        #region Model Helpers

        /// <summary>
        /// Gets the step program (model) that should be displayed in the block
        /// </summary>
        /// <returns></returns>
        private StepProgram GetStepProgram( RockContext rockContext )
        {
            if ( _stepProgram == null )
            {
                var programId = GetAttributeValue( AttributeKeys.StepProgramId ).AsIntegerOrNull();

                if ( programId.HasValue )
                {
                    _stepProgram = new StepProgramService( rockContext ).Get( programId.Value );
                }
                else
                {
                    programId = PageParameter( "StepProgramId" ).AsIntegerOrNull();

                    if ( programId.HasValue )
                    {
                        _stepProgram = new StepProgramService( rockContext ).Get( programId.Value );
                    }
                }
            }

            return _stepProgram;
        }
        private StepProgram _stepProgram;

        /// <summary>
        /// Gets the person model that should be displayed in the block
        /// </summary>
        /// <param name="rockContext"></param>
        /// <returns></returns>
        private Person GetPerson( RockContext rockContext )
        {
            if ( _person == null )
            {
                _person = ContextEntity() as Person;
            }

            if ( _person == null )
            {
                var personId = PageParameter( "PersonId" ).AsIntegerOrNull();

                if ( personId.HasValue )
                {
                    _person = new PersonService( rockContext ).Get( personId.Value );
                }
            }

            return _person;
        }
        private Person _person;

        /// <summary>
        /// Get the person's steps for this program
        /// </summary>
        /// <param name="rockContext"></param>
        /// <returns></returns>
        private Dictionary<int, List<Step>> GetStepsForPerson( RockContext rockContext )
        {
            if ( _steps == null )
            {
                var person = GetPerson( rockContext );
                var program = GetStepProgram( rockContext );

                if ( person != null && program != null )
                {
                    _steps = program.StepTypes.ToDictionary(
                        st => st.Id,
                        st => st.Steps.Where( s => s.PersonAlias.PersonId == person.Id ).ToList() );
                }
            }

            return _steps;
        }
        private Dictionary<int, List<Step>> _steps;

        #endregion Model Helpers

        #region Control Helpers

        /// <summary>
        /// Show an error in the notification box
        /// </summary>
        /// <param name="message"></param>
        private void ShowError( string message )
        {
            nbNotificationBox.NotificationBoxType = NotificationBoxType.Danger;
            nbNotificationBox.Title = "Uh oh...";
            nbNotificationBox.Text = message;
            nbNotificationBox.Visible = true;
        }

        /// <summary>
        /// Set details on the block that come from the program
        /// </summary>
        /// <param name="rockContext"></param>
        private void SetProgramDetailsOnBlock( RockContext rockContext )
        {
            var program = GetStepProgram( rockContext );

            if ( program == null )
            {
                ShowError( "The step program was not found" );
                return;
            }

            lStepProgramName.Text = program.Name;
            iIcon.Attributes["class"] = program.IconCssClass;
        }

        /// <summary>
        /// Render the step cards
        /// </summary>
        /// <param name="rockContext"></param>
        private void RenderCards( RockContext rockContext )
        {
            var program = GetStepProgram( rockContext );

            if ( program == null )
            {
                ShowError( "The step program was not found" );
                return;
            }

            var person = GetPerson( rockContext );

            if ( person == null )
            {
                ShowError( "The person was not found" );
                return;
            }

            var stepTypes = program.StepTypes;

            if ( stepTypes == null )
            {
                ShowError( "The step types were not found" );
                return;
            }

            var personSteps = GetStepsForPerson( rockContext );

            if ( personSteps == null )
            {
                ShowError( "The steps for the person were not found" );
                return;
            }

            var renderedLavaTemplates = new List<string>();

            foreach ( var stepType in stepTypes )
            {
                List<Step> personStepsOfType = null;
                personSteps.TryGetValue( stepType.Id, out personStepsOfType );
                personStepsOfType = personStepsOfType ?? new List<Step>();

                var rendered = stepType.CardLavaTemplate.ResolveMergeFields( new Dictionary<string, object> {
                    { "StepType", stepType },
                    { "Steps", personStepsOfType },
                    { "Person", person },
                    { "Program", program }
                } );

                renderedLavaTemplates.Add( rendered );
            }

            rStepTypes.DataSource = from rlt in renderedLavaTemplates select new { RenderedLava = rlt };
            rStepTypes.DataBind();
        }

        #endregion Control Helpers
    }
}