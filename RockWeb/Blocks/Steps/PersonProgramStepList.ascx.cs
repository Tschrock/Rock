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
using System.Web.UI.WebControls;
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

            if ( !IsPostBack )
            {
                var rockContext = new RockContext();
                SetProgramDetailsOnBlock( rockContext );
                RenderCards( rockContext );
                RenderGrid( rockContext );                
            }

            SetViewMode( hfIsCardView.Value.AsBoolean() );
        }

        /// <summary>
        /// Show the grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ShowGrid( object sender, EventArgs e )
        {
            SetViewMode( false );
        }

        /// <summary>
        /// Show the card view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ShowCards( object sender, EventArgs e )
        {
            SetViewMode( true );
        }

        /// <summary>
        /// Display either the card or the grid view
        /// </summary>
        /// <param name="isCardView"></param>
        private void SetViewMode( bool isCardView )
        {
            hfIsCardView.Value = isCardView.ToString();
            divCardView.Visible = isCardView;
            divGridView.Visible = !isCardView;
        }

        /// <summary>
        /// Generate the contents of the step type column of the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lStepType_DataBound( object sender, RowEventArgs e )
        {
            var lStepType = sender as Literal;
            var stepGridRow = e.Row.DataItem as StepGridRow;

            lStepType.Text = string.Format( "<i class=\"{0}\"></i> {1}", stepGridRow.StepTypeIconCssClass, stepGridRow.StepTypeName );
        }

        /// <summary>
        /// Generate the contents of the step status column of the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lStepStatus_DataBound( object sender, RowEventArgs e )
        {
            var lStepStatus = sender as Literal;
            var stepGridRow = e.Row.DataItem as StepGridRow;

            if ( stepGridRow.StepStatus == null )
            {
                return;
            }

            lStepStatus.Text = string.Format( "<span class='label label-{0}'>{1}</span>", stepGridRow.StepStatus.StatusColor, stepGridRow.StepStatus.Name );
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
        /// Gets the step types (model) in the order they should be displayed for this program
        /// </summary>
        /// <returns></returns>
        private IOrderedEnumerable<StepType> GetOrderedStepTypes( RockContext rockContext )
        {
            if ( _orderedStepTypes == null )
            {
                var program = GetStepProgram( rockContext );

                if ( program != null )
                {
                    _orderedStepTypes = program.StepTypes.OrderBy( st => st.Order ).ThenBy( st => st.Name );
                }                
            }

            return _orderedStepTypes;
        }
        private IOrderedEnumerable<StepType> _orderedStepTypes;

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
        /// Get a list of the steps that the person has taken within the given step type
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="stepTypeId"></param>
        /// <returns></returns>
        private List<Step> GetPersonStepsOfType( RockContext rockContext, int stepTypeId )
        {
            var defaultValue = new List<Step>();
            var personStepsMap = GetStepTypeToPersonStepMap( rockContext );

            if ( personStepsMap == null )
            {
                return defaultValue;
            }

            List<Step> personStepsOfType = null;
            personStepsMap.TryGetValue( stepTypeId, out personStepsOfType );
            return personStepsOfType ?? defaultValue;
        }

        /// <summary>
        /// Get the person's steps for this program
        /// </summary>
        /// <param name="rockContext"></param>
        /// <returns></returns>
        private Dictionary<int, List<Step>> GetStepTypeToPersonStepMap( RockContext rockContext )
        {
            if ( _personStepsMap == null )
            {
                var person = GetPerson( rockContext );
                var program = GetStepProgram( rockContext );

                if ( person != null && program != null )
                {
                    _personStepsMap = program.StepTypes.ToDictionary(
                        st => st.Id,
                        st => st.Steps
                            .Where( s => s.PersonAlias.PersonId == person.Id )
                            .OrderBy( s => s.CompletedDateTime ?? s.EndDateTime ?? s.StartDateTime ?? DateTime.MinValue )
                            .ToList() );
                }
            }

            return _personStepsMap;
        }
        private Dictionary<int, List<Step>> _personStepsMap;

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

            var stepTypes = GetOrderedStepTypes( rockContext );

            if ( stepTypes == null )
            {
                ShowError( "The step types were not found" );
                return;
            }

            var personStepsMap = GetStepTypeToPersonStepMap( rockContext );

            if ( personStepsMap == null )
            {
                ShowError( "The steps for the person were not found" );
                return;
            }

            var renderedLavaTemplates = new List<string>();

            foreach ( var stepType in stepTypes )
            {
                var personStepsOfType = GetPersonStepsOfType( rockContext, stepType.Id );

                var rendered = stepType.CardLavaTemplate.ResolveMergeFields( new Dictionary<string, object> {
                    { "StepType", stepType },
                    { "Steps", personStepsOfType },
                    { "Person", person },
                    { "Program", program },
                    { "IsComplete", personStepsOfType.Any( s => s.CompletedDateTime.HasValue ) },
                    { "CompletedDateTime", personStepsOfType.Where( s => s.CompletedDateTime.HasValue ).Max( s => s.CompletedDateTime ) },
                    { "StepCount", personStepsOfType.Count }
                } );

                renderedLavaTemplates.Add( rendered );
            }

            rStepTypes.DataSource = from rlt in renderedLavaTemplates select new { RenderedLava = rlt };
            rStepTypes.DataBind();
        }

        /// <summary>
        /// Get data and bind it to the grid to display step records for the given person
        /// </summary>
        /// <param name="rockContext"></param>
        private void RenderGrid( RockContext rockContext )
        {
            var stepTypes = GetOrderedStepTypes( rockContext );

            if ( stepTypes == null )
            {
                ShowError( "The step types were not found" );
                return;
            }

            var rows = new List<StepGridRow>();

            foreach ( var stepType in stepTypes )
            {
                var personStepsOfType = GetPersonStepsOfType( rockContext, stepType.Id );
                rows.AddRange( personStepsOfType.Select( s => new StepGridRow
                {
                    StepTypeName = stepType.Name,
                    CompletedDateTime = s.CompletedDateTime,
                    StepStatus = s.StepStatus,
                    StepTypeIconCssClass = stepType.IconCssClass
                } ) );
            }

            gStepList.DataSource = rows;
            gStepList.DataBind();
        }

        #endregion Control Helpers

        #region Helper Classes

        public class StepGridRow
        {
            public string StepTypeName { get; set; }
            public DateTime? CompletedDateTime { get; set; }
            public StepStatus StepStatus { get; set; }
            public string StepTypeIconCssClass { get; set; }
        }

        #endregion Helper Classes
    }
}