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
using System.Data.Entity;
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
    [Description( "Displays step records for a person in a step program." )]

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

        private class FilterKeys
        {
            public const string StepTypeName = "StepTypeName";
        }

        #region Events

        /// <summary>
        /// Handles the OnInit event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            gfGridFilter.ApplyFilterClick += gfGridFilter_ApplyFilterClick;
            gfGridFilter.ClearFilterClick += gfGridFilter_ClearFilterClick;
            gfGridFilter.DisplayFilterValue += gfGridFilter_DisplayFilterValue;

            gStepList.DataKeyNames = new[] { "id" };
            gStepList.GridRebind += gStepList_GridRebind;

            if ( !IsPostBack )
            {
                SetProgramDetailsOnBlock();
                RenderCardView();
                RenderGridView();                
            }

            SetViewMode( hfIsCardView.Value.AsBoolean() );
        }

        /// <summary>
        /// Handle the rebind event for the step list grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gStepList_GridRebind( object sender, GridRebindEventArgs e )
        {
            RenderGridView();
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
            pnlCardView.Visible = isCardView;
            pnlGridView.Visible = !isCardView;
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

        /// <summary>
        /// The click event of the add step buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddStep( object sender, CommandEventArgs e )
        {
            var stepTypeId = e.CommandArgument.ToStringSafe().AsIntegerOrNull();
            // TODO
        }

        /// <summary>
        /// Event when the user clicks to delete a step from the card view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rSteps_Edit( object sender, CommandEventArgs e )
        {
            var stepId = e.CommandArgument.ToStringSafe().AsInteger();
            // TODO
        }

        /// <summary>
        /// Event when the user clicks to delete a step from the card view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rSteps_Delete( object sender, CommandEventArgs e )
        {
            var stepId = e.CommandArgument.ToStringSafe().AsInteger();
            DeleteStep( stepId );
            RenderCardView();
        }

        /// <summary>
        /// Handle the event where the user wants to delete a step from the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gStepList_Delete( object sender, RowEventArgs e )
        {
            var stepId = e.RowKeyId;
            DeleteStep( stepId );
            RenderGridView();
        }

        protected void rAddStepButtons_ItemDataBound( Object Sender, RepeaterItemEventArgs e )
        {
            if ( e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem )
            {
                return;
            }

            var addStepButton = e.Item.DataItem as AddStepButton;

            if ( !addStepButton.IsEnabled )
            {
                var linkButton = e.Item.FindControl( "lbAddStep" ) as LinkButton;
                linkButton.Attributes["disabled"] = "disabled";
            }
        }

        protected void rStepTypeCards_ItemDataBound( Object Sender, RepeaterItemEventArgs e )
        {
            if ( e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem )
            {
                return;
            }

            var cardData = e.Item.DataItem as CardData;
            var stepTypeId = cardData.StepType.Id;
            var hasMetPrerequisites = HasMetPrerequisites( stepTypeId );

            var pnlStepRecords = e.Item.FindControl( "pnlStepRecords" ) as Panel;
            var pnlPrereqs = e.Item.FindControl( "pnlPrereqs" ) as Panel;

            pnlStepRecords.Visible = hasMetPrerequisites;
            pnlPrereqs.Visible = !hasMetPrerequisites;

            if ( hasMetPrerequisites )
            {
                var steps = GetPersonStepsOfType( stepTypeId );

                var data = steps.Select( s => new CardStepData
                {
                    StepId = s.Id,
                    StatusHtml = string.Format( "{0}<br /><small>{1}</small>",
                        s.StepStatus != null ? s.StepStatus.Name : string.Empty,
                        s.CompletedDateTime.HasValue ? s.CompletedDateTime.Value.ToShortDateString() : string.Empty )
                } );

                var rSteps = e.Item.FindControl( "rSteps" ) as Repeater;
                rSteps.DataSource = data;
                rSteps.DataBind();
            }
            else
            {
                var prereqs = GetPrerequisiteStepTypes( stepTypeId );

                var rPrereqs = e.Item.FindControl( "rPrereqs" ) as Repeater;
                rPrereqs.DataSource = prereqs;
                rPrereqs.DataBind();
            }
        }

        #endregion Events

        #region GridFilter Events

        /// <summary>
        /// Handles the DisplayFilterValue event of the gfGridFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Rock.Web.UI.Controls.GridFilter.DisplayFilterValueArgs"/> instance containing the event data.</param>
        protected void gfGridFilter_DisplayFilterValue( object sender, GridFilter.DisplayFilterValueArgs e )
        {
            // TODO - Keep adding filters. Some will need transformed here

            /*
            switch ( e.Key )
            {
                case FilterKeys.StepTypeName:
                {
                    
                }
            }
            */
        }

        /// <summary>
        /// Handles the ApplyFilterClick event of the gfGridFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gfGridFilter_ApplyFilterClick( object sender, EventArgs e )
        {
            gfGridFilter.SaveUserPreference( FilterKeys.StepTypeName, tbStepTypeName.Text );            
            RenderGridView();
        }

        /// <summary>
        /// Handles the ClearFilterClick event of the gfGridFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gfGridFilter_ClearFilterClick( object sender, EventArgs e )
        {
            gfGridFilter.DeleteUserPreferences();
            BindFilter();
        }

        /// <summary>
        /// Binds the filter.
        /// </summary>
        private void BindFilter()
        {
            var stepTypeNameFilter = gfGridFilter.GetUserPreference( FilterKeys.StepTypeName );
            tbStepTypeName.Text = !string.IsNullOrWhiteSpace( stepTypeNameFilter ) ? stepTypeNameFilter : string.Empty;
        }

        #endregion GridFilter Events

        #region Model Helpers

        /// <summary>
        /// Gets the step program (model) that should be displayed in the block
        /// </summary>
        /// <returns></returns>
        private StepProgram GetStepProgram()
        {
            if ( _stepProgram == null )
            {
                var rockContext = GetRockContext();
                var programId = GetAttributeValue( AttributeKeys.StepProgramId ).AsIntegerOrNull();

                if ( !programId.HasValue )
                {
                    programId = PageParameter( "StepProgramId" ).AsIntegerOrNull();
                }

                if ( programId.HasValue )
                {
                    _stepProgram = new StepProgramService( rockContext ).Queryable()
                        .AsNoTracking()
                        .FirstOrDefault( sp => sp.Id == programId.Value && sp.IsActive );
                }
            }

            return _stepProgram;
        }
        private StepProgram _stepProgram;

        /// <summary>
        /// Gets the step types (model) for this program
        /// </summary>
        /// <returns></returns>
        private IEnumerable<StepType> GetStepTypes()
        {
            if ( _stepTypes == null )
            {
                var program = GetStepProgram();

                if ( program != null )
                {
                    _stepTypes = program.StepTypes.Where( st => st.IsActive );
                }                
            }

            return _stepTypes;
        }
        private IEnumerable<StepType> _stepTypes;

        /// <summary>
        /// Apply standard ordering for step types to a step type query
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        private IOrderedEnumerable<StepType> OrderStepTypes( IEnumerable<StepType> qry )
        {
            return qry.OrderBy( st => st.Order ).ThenBy( st => st.Name );
        }

        /// <summary>
        /// Gets the person model that should be displayed in the block
        /// </summary>
        /// <returns></returns>
        private Person GetPerson()
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
                    var rockContext = GetRockContext();
                    _person = new PersonService( rockContext ).Get( personId.Value );
                }
            }

            return _person;
        }
        private Person _person;

        /// <summary>
        /// Get a list of the steps that the person has taken within the given step type
        /// </summary>
        /// <param name="stepTypeId"></param>
        /// <returns></returns>
        private List<Step> GetPersonStepsOfType( int stepTypeId )
        {
            var defaultValue = new List<Step>();
            var personStepsMap = GetStepTypeToPersonStepMap();

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
        /// <returns></returns>
        private Dictionary<int, List<Step>> GetStepTypeToPersonStepMap()
        {
            if ( _personStepsMap == null )
            {
                var person = GetPerson();
                var program = GetStepProgram();

                if ( person != null && program != null )
                {
                    _personStepsMap = program.StepTypes.Where( st => st.IsActive ).ToDictionary(
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

        /// <summary>
        /// Get a list of the prerequisites for the given step type id
        /// </summary>
        /// <param name="stepTypeId"></param>
        /// <returns></returns>
        private List<StepType> GetPrerequisiteStepTypes( int stepTypeId )
        {
            var rockContext = GetRockContext();
            var service = new StepTypePrerequisiteService( rockContext );

            return service.Queryable()
                .AsNoTracking()
                .Include( stp => stp.PrerequisiteStepType )
                .Where( stp => stp.StepTypeId == stepTypeId && stp.PrerequisiteStepType.IsActive )
                .Select( stp => stp.PrerequisiteStepType )
                .ToList();
        }

        /// <summary>
        /// Has the person met the prereqs for the given step type
        /// </summary>
        /// <param name="stepTypeId"></param>
        /// <returns></returns>
        private bool HasMetPrerequisites( int stepTypeId )
        {
            var preReqs = GetPrerequisiteStepTypes( stepTypeId );

            if ( !preReqs.Any() )
            {
                return true;
            }

            foreach ( var preReq in preReqs )
            {
                var steps = GetPersonStepsOfType( preReq.Id );

                if ( !steps.Any() || steps.All( s => !s.IsComplete ) )
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Can a step of the given step type be added for the person. Checks for active, prereqs, and if the step
        /// type allows more than one step record
        /// </summary>
        /// <param name="stepType"></param>
        /// <returns></returns>
        private bool CanAddStep( StepType stepType )
        {
            if ( !stepType.IsActive || !HasMetPrerequisites(stepType.Id))
            {
                return false;
            }

            if ( stepType.AllowMultiple )
            {
                return true;
            }

            var exisitingSteps = GetPersonStepsOfType( stepType.Id );
            return !exisitingSteps.Any();
        }

        /// <summary>
        /// Delete the step with the given Id and then re-render the lists of steps
        /// </summary>
        /// <param name="stepId"></param>
        private void DeleteStep( int stepId )
        {
            var rockContext = GetRockContext();
            var service = new StepService( rockContext );
            var step = service.Get( stepId );
            string errorMessage;

            if ( step == null )
            {
                return;
            }

            if ( !service.CanDelete( step, out errorMessage ) )
            {
                ShowError( errorMessage );
                return;
            }

            service.Delete( step );
            rockContext.SaveChanges();

            RenderGridView();
            RenderCardView();
        }

        /// <summary>
        /// Get the rock context
        /// </summary>
        /// <returns></returns>
        private RockContext GetRockContext()
        {
            if ( _rockContext == null )
            {
                _rockContext = new RockContext();
            }

            return _rockContext;
        }
        private RockContext _rockContext;

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
        private void SetProgramDetailsOnBlock()
        {
            var program = GetStepProgram();

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
        private void RenderCardView()
        {
            var program = GetStepProgram();

            if ( program == null )
            {
                ShowError( "The step program was not found" );
                return;
            }

            var person = GetPerson();

            if ( person == null )
            {
                ShowError( "The person was not found" );
                return;
            }

            var stepTypes = OrderStepTypes( GetStepTypes() );

            if ( stepTypes == null )
            {
                ShowError( "The step types were not found" );
                return;
            }

            var cardsData = new List<CardData>();

            foreach ( var stepType in stepTypes )
            {
                var personStepsOfType = GetPersonStepsOfType( stepType.Id );

                var rendered = stepType.CardLavaTemplate.ResolveMergeFields( new Dictionary<string, object> {
                    { "StepType", stepType },
                    { "Steps", personStepsOfType },
                    { "Person", person },
                    { "Program", program },
                    { "IsComplete", personStepsOfType.Any( s => s.IsComplete ) },
                    { "CompletedDateTime", personStepsOfType.Where( s => s.CompletedDateTime.HasValue ).Max( s => s.CompletedDateTime ) },
                    { "StepCount", personStepsOfType.Count }
                } );

                cardsData.Add( new CardData
                {
                    StepType = stepType,
                    RenderedLava = rendered
                } );
            }

            rStepTypeCards.DataSource = cardsData;
            rStepTypeCards.DataBind();
        }

        /// <summary>
        /// Get data and bind it to the grid to display step records for the given person
        /// </summary>
        private void RenderGridView()
        {
            var stepTypesQry = GetStepTypes();

            if ( stepTypesQry == null )
            {
                ShowError( "The step types were not found" );
                return;
            }

            // Apply filters
            var stepTypeNameFilter = gfGridFilter.GetUserPreference( FilterKeys.StepTypeName );
            if ( !string.IsNullOrEmpty( stepTypeNameFilter ) )
            {
                stepTypesQry = stepTypesQry.Where( st => st.Name.Contains( stepTypeNameFilter ) );
            }

            // Order the step types
            var orderedStepTypes = OrderStepTypes( stepTypesQry );
            var rows = new List<StepGridRow>();
            var addButtons = new List<AddStepButton>();

            foreach ( var stepType in stepTypesQry )
            {
                var personStepsOfType = GetPersonStepsOfType( stepType.Id );
                rows.AddRange( personStepsOfType.Select( s => new StepGridRow
                {
                    Id = s.Id,
                    StepTypeName = stepType.Name,
                    CompletedDateTime = s.CompletedDateTime,
                    StepStatus = s.StepStatus,
                    StepTypeIconCssClass = stepType.IconCssClass
                } ) );

                var addButtonIsEnabled = CanAddStep( stepType );

                addButtons.Add( new AddStepButton
                {
                    StepTypeId = stepType.Id,
                    IsEnabled = CanAddStep( stepType ),
                    ButtonContents = string.Format( "<i class=\"{0}\"></i> &nbsp; {1}", stepType.IconCssClass, stepType.Name )
                } );
            }

            gStepList.DataSource = rows;
            gStepList.DataBind();

            rAddStepButtons.DataSource = addButtons;
            rAddStepButtons.DataBind();
        }

        #endregion Control Helpers

        #region Helper Classes

        public class StepGridRow
        {
            public int Id { get; set; }
            public string StepTypeName { get; set; }
            public DateTime? CompletedDateTime { get; set; }
            public StepStatus StepStatus { get; set; }
            public string StepTypeIconCssClass { get; set; }
        }

        public class AddStepButton
        {
            public int StepTypeId { get; set; }
            public bool IsEnabled { get; set; }
            public string ButtonContents { get; set; }
        }

        public class CardData
        {
            public StepType StepType { get; set; }
            public string RenderedLava { get; set; }
        }

        public class CardStepData
        {
            public int StepId { get; set; }
            public string StatusHtml { get; set; }
        }

        #endregion Helper Classes
    }
}