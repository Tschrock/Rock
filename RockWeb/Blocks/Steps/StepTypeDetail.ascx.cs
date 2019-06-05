﻿// <copyright>
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
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Rock;
using Rock.Attribute;
using Rock.Chart;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.Steps
{
    [DisplayName( "Step Type Detail" )]
    [Category( "Steps" )]
    [Description( "Displays the details of the given Step Type for editing." )]

    #region Block Attributes

    [BooleanField
        ( "Show Chart",
          Key = AttributeKey.ShowChart,
          DefaultValue = "true",
          Order = 0 )]
    [DefinedValueField
        ( Rock.SystemGuid.DefinedType.CHART_STYLES,
         "Chart Style",
         Key = AttributeKey.ChartStyle,
         DefaultValue = Rock.SystemGuid.DefinedValue.CHART_STYLE_ROCK,
         Order = 1 )]
    [SlidingDateRangeField
        ( "Default Chart Date Range",
          Key = AttributeKey.SlidingDateRange,
          DefaultValue = "Current||Year||",
          EnabledSlidingDateRangeTypes = "Last,Previous,Current,DateRange",
          Order = 2 )]
    [CategoryField(
        "Data View Categories",
        Key = AttributeKey.DataViewCategories,
        Description = "The categories from which the Audience and Autocomplete data view options can be selected. If empty, all data views will be available.",
        AllowMultiple = true,
        EntityTypeName = "Rock.Model.DataView",
        EntityTypeQualifierColumn = "",
        EntityTypeQualifierValue = "",
        IsRequired = false,
        DefaultValue = "",
        Category = "",
        Order = 7 )]

    #endregion Block Attributes

    public partial class StepTypeDetail : RockBlock, IDetailBlock
    {
        #region Attribute Keys

        /// <summary>
        /// Keys to use for Block Attributes
        /// </summary>
        protected static class AttributeKey
        {
            public const string ShowChart = "ShowChart";
            public const string ChartStyle = "ChartStyle";
            public const string SlidingDateRange = "SlidingDateRange";
            public const string DataViewCategories = "DataViewCategories";
        }

        #endregion Attribute Keys

        #region Page Parameter Keys

        /// <summary>
        /// Keys to use for Page Parameters
        /// </summary>
        protected static class PageParameterKey
        {
            public const string StepTypeId = "StepTypeId";
            public const string StepProgramId = "ProgramId";
        }

        #endregion Page Parameter Keys

        #region Properties

        private List<StepWorkflowTriggerViewModel> WorkflowsState { get; set; }

        #endregion

        #region Private Variables

        private int _stepProgramId = 0;
        private int _stepTypeId = 0;
        private RockContext _dataContext = null;
        private bool _blockContextIsValid = false;

        #endregion Private Variables

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            this.InitializeBlockNotification( nbBlockStatus, pnlDetails );
            this.InitializeSettingsNotification( upStepType );

            _blockContextIsValid = this.InitializeBlockContext();

            if ( !_blockContextIsValid )
            {
                return;
            }

            dvpAutocomplete.EntityTypeId = EntityTypeCache.Get( typeof( Rock.Model.Person ) ).Id;
            dvpAutocomplete.CategoryGuids = GetAttributeValue( AttributeKey.DataViewCategories ).SplitDelimitedValues().AsGuidList();

            dvpAudience.EntityTypeId = EntityTypeCache.Get( typeof( Rock.Model.Person ) ).Id;
            dvpAudience.CategoryGuids = GetAttributeValue( AttributeKey.DataViewCategories ).SplitDelimitedValues().AsGuidList();

            gWorkflows.DataKeyNames = new string[] { "Guid" };
            gWorkflows.Actions.ShowAdd = true;
            gWorkflows.Actions.AddClick += gWorkflows_Add;
            gWorkflows.GridRebind += gWorkflows_GridRebind;

            btnDelete.Attributes["onclick"] = string.Format( "javascript: return Rock.dialogs.confirmDelete(event, '{0}', 'This will also delete the associated step participants.');", StepType.FriendlyTypeName );
            btnSecurity.EntityTypeId = EntityTypeCache.Get( typeof( Rock.Model.StepType ) ).Id;

            this.InitializeChart();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !_blockContextIsValid )
            {
                return;
            }

            if ( !Page.IsPostBack )
            {
                this.ShowDetail( _stepTypeId );
            }
        }

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            var json = ViewState["WorkflowsState"] as string ?? string.Empty;

            this.WorkflowsState = JsonConvert.DeserializeObject<List<StepWorkflowTriggerViewModel>>( json ) ?? new List<StepWorkflowTriggerViewModel>();
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            var jsonSetting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject( WorkflowsState, Formatting.None, jsonSetting );

            ViewState["WorkflowsState"] = json;

            return base.SaveViewState();
        }

        /// <summary>
        /// Returns breadcrumbs specific to the block that should be added to navigation
        /// based on the current page reference.  This function is called during the page's
        /// oninit to load any initial breadcrumbs
        /// </summary>
        /// <param name="pageReference">The page reference.</param>
        /// <returns></returns>
        public override List<BreadCrumb> GetBreadCrumbs( PageReference pageReference )
        {
            var breadCrumbs = new List<BreadCrumb>();

            int? stepTypeId = PageParameter( pageReference, PageParameterKey.StepTypeId ).AsIntegerOrNull();
            if ( stepTypeId != null )
            {
                var stepType = new StepTypeService( this.GetDataContext() ).Get( stepTypeId.Value );

                if ( stepType != null )
                {
                    breadCrumbs.Add( new BreadCrumb( stepType.Name, pageReference ) );
                }
                else
                {
                    breadCrumbs.Add( new BreadCrumb( "New Step Type", pageReference ) );
                }
            }
            else
            {
                // don't show a breadcrumb if we don't have a pageparam to work with
            }

            return breadCrumbs;
        }

        /// <summary>
        /// Navigate to the step program page
        /// </summary>
        private void GoToStepProgramPage()
        {
            NavigateToParentPage( new Dictionary<string, string> { { PageParameterKey.StepProgramId, _stepProgramId.ToString() } } );
        }

        #endregion

        #region Events

        #region Control Events

        protected void btnRefreshChart_Click( object sender, EventArgs e )
        {
            this.RefreshChart();
        }

        /// <summary>
        /// Handles the Click event of the btnEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnEdit_Click( object sender, EventArgs e )
        {
            var stepType = this.GetStepType();

            ShowEditDetails( stepType );
        }

        /// <summary>
        /// Handles the Click event of the btnDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnDelete_Click( object sender, EventArgs e )
        {
            this.DeleteRecord();
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            var recordId = this.SaveRecord();

            if ( recordId <= 0 )
            {
                return;
            }

            // Update the query string for this page and reload.
            var qryParams = new Dictionary<string, string>();
            qryParams[PageParameterKey.StepTypeId] = recordId.ToString();

            NavigateToPage( RockPage.Guid, qryParams );
        }

        /// <summary>
        /// Save the current record.
        /// </summary>
        /// <returns>The Id of the new record, or -1 if the process could not be completed.</returns>
        private int SaveRecord()
        {
            StepType stepType;

            var rockContext = this.GetDataContext();

            var stepTypeService = new StepTypeService( rockContext );
            var stepWorkflowService = new StepWorkflowService( rockContext );
            var stepWorkflowTriggerService = new StepWorkflowTriggerService( rockContext );

            int stepTypeId = int.Parse( hfStepTypeId.Value );

            if ( stepTypeId == 0 )
            {
                stepType = new StepType();

                stepType.StepProgramId = _stepProgramId;

                stepTypeService.Add( stepType );
            }
            else
            {
                stepType = stepTypeService.Queryable()
                                          .Include( x => x.StepWorkflowTriggers )
                                          .Where( c => c.Id == stepTypeId )
                                          .FirstOrDefault();
            }

            // Workflow Triggers: Remove deleted triggers.
            var uiWorkflows = WorkflowsState.Select( l => l.Guid );

            var deletedTriggers = stepType.StepWorkflowTriggers.Where( l => !uiWorkflows.Contains( l.Guid ) ).ToList();

            foreach ( var trigger in deletedTriggers )
            {
                // Remove the Step workflows associated with this trigger.
                var stepWorkflows = stepWorkflowService.Queryable().Where( w => w.StepWorkflowTriggerId == trigger.Id );

                foreach ( var requestWorkflow in stepWorkflows )
                {
                    stepWorkflowService.Delete( requestWorkflow );
                }

                // Remove the trigger.
                stepType.StepWorkflowTriggers.Remove( trigger );

                stepWorkflowTriggerService.Delete( trigger );
            }

            // Workflow Triggers: Update modified triggers.
            foreach ( var stateTrigger in WorkflowsState )
            {
                var workflowTrigger = stepType.StepWorkflowTriggers.Where( a => a.Guid == stateTrigger.Guid ).FirstOrDefault();

                if ( workflowTrigger == null )
                {
                    workflowTrigger = new StepWorkflowTrigger();

                    workflowTrigger.StepProgramId = stepType.StepProgramId;

                    stepType.StepWorkflowTriggers.Add( workflowTrigger );
                }

                workflowTrigger.Guid = stateTrigger.Guid;
                workflowTrigger.WorkflowTypeId = stateTrigger.WorkflowTypeId;
                workflowTrigger.TriggerType = stateTrigger.TriggerType;
                workflowTrigger.TypeQualifier = stateTrigger.TypeQualifier;
                workflowTrigger.WorkflowTypeId = stateTrigger.WorkflowTypeId;
                workflowTrigger.WorkflowName = stateTrigger.WorkflowTypeName;
            }

            // Update Basic properties
            stepType.Name = tbName.Text;
            stepType.IsActive = cbIsActive.Checked;
            stepType.Description = tbDescription.Text;
            stepType.IconCssClass = tbIconCssClass.Text;

            stepType.HighlightColor = cpHighlight.Value;
            stepType.ShowCountOnBadge = cbShowBadgeCount.Checked;
            stepType.HasEndDate = cbHasDuration.Checked;
            stepType.AllowMultiple = cbAllowMultiple.Checked;

            // Update Pre-requisites
            var uiPrerequisiteStepTypeIds = cblPrerequsities.SelectedValuesAsInt;

            var stepTypes = stepTypeService.Queryable().ToList();

            var removePrerequisiteStepTypes = stepType.StepTypePrerequisites.Where( x => !uiPrerequisiteStepTypeIds.Contains( x.PrerequisiteStepTypeId ) ).ToList();

            var prerequisiteService = new StepTypePrerequisiteService( rockContext );

            foreach ( var prerequisiteStepType in removePrerequisiteStepTypes )
            {
                stepType.StepTypePrerequisites.Remove( prerequisiteStepType );

                prerequisiteService.Delete( prerequisiteStepType );
            }

            var existingPrerequisiteStepTypeIds = stepType.StepTypePrerequisites.Select( x => x.PrerequisiteStepTypeId ).ToList();

            var addPrerequisiteStepTypeIds = stepTypes.Where( x => uiPrerequisiteStepTypeIds.Contains( x.Id )
                                                                 && !existingPrerequisiteStepTypeIds.Contains( x.Id ) )
                                                      .Select( x => x.Id )
                                                      .ToList();

            foreach ( var prerequisiteStepTypeId in addPrerequisiteStepTypeIds )
            {
                var newPrerequisite = new StepTypePrerequisite();

                newPrerequisite.StepTypeId = stepType.Id;
                newPrerequisite.PrerequisiteStepTypeId = prerequisiteStepTypeId;

                stepType.StepTypePrerequisites.Add( newPrerequisite );
            }

            // Update Advanced Settings
            stepType.AutoCompleteDataViewId = dvpAutocomplete.SelectedValueAsId();
            stepType.AudienceDataViewId = dvpAudience.SelectedValueAsId();

            stepType.MergeTemplateId = mtpMergeTemplate.SelectedValueAsId();
            stepType.MergeTemplateDescriptor = tbMergeDescriptor.Text;

            stepType.AllowManualEditing = cbAllowEdit.Checked;

            stepType.CardLavaTemplate = ceCardTemplate.Text;

            if ( !stepType.IsValid )
            {
                // Controls will render the error messages
                return -1;
            }

            rockContext.SaveChanges();

            return stepType.Id;
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnCancel_Click( object sender, EventArgs e )
        {
            if ( hfStepTypeId.Value.Equals( "0" ) )
            {
                GoToStepProgramPage();
            }
            else
            {
                ShowReadonlyDetails( this.GetStepType() );
            }
        }

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            var currentstepType = GetStepType();

            if ( currentstepType != null )
            {
                ShowReadonlyDetails( currentstepType );
            }
            else
            {
                string stepTypeId = PageParameter( PageParameterKey.StepTypeId );
                if ( !string.IsNullOrWhiteSpace( stepTypeId ) )
                {
                    ShowDetail( stepTypeId.AsInteger() );
                }
                else
                {
                    pnlDetails.Visible = false;
                }
            }
        }

        #endregion

        #region StepWorkflow Events

        /// <summary>
        /// Handles the SaveClick event of the dlgStepWorkflow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void dlgStepWorkflow_SaveClick( object sender, EventArgs e )
        {
            StepWorkflowTriggerViewModel workflowTrigger = null;

            var guid = hfAddStepWorkflowGuid.Value.AsGuid();
            if ( !guid.IsEmpty() )
            {
                workflowTrigger = WorkflowsState.FirstOrDefault( l => l.Guid.Equals( guid ) );
            }

            if ( workflowTrigger == null )
            {
                workflowTrigger = new StepWorkflowTriggerViewModel();

                workflowTrigger.Guid = Guid.NewGuid();
            }

            workflowTrigger.WorkflowTypeId = wpWorkflowType.SelectedValueAsId().Value;
            workflowTrigger.TriggerType = ddlTriggerType.SelectedValueAsEnum<StepWorkflowTrigger.WorkflowTriggerCondition>();

            var qualifierSettings = new StepWorkflowTrigger.StatusChangeTriggerSettings
            {
                FromStatusId = ddlPrimaryQualifier.SelectedValue.AsIntegerOrNull(),
                ToStatusId = ddlSecondaryQualifier.SelectedValue.AsIntegerOrNull()
            };

            workflowTrigger.TypeQualifier = qualifierSettings.ToSelectionString();

            var dataContext = this.GetDataContext();

            var workflowTypeService = new WorkflowTypeService( dataContext );

            var workflowTypeId = wpWorkflowType.SelectedValueAsId().GetValueOrDefault( 0 );

            var workflowType = workflowTypeService.Queryable().AsNoTracking().FirstOrDefault( x => x.Id == workflowTypeId );

            workflowTrigger.WorkflowTypeName = ( workflowType == null ) ? "(Unknown)" : workflowType.Name;

            WorkflowsState.Add( workflowTrigger );

            BindStepWorkflowsGrid();

            HideDialog();
        }

        /// <summary>
        /// Handles the Delete event of the gWorkflows control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gWorkflows_Delete( object sender, RowEventArgs e )
        {
            Guid rowGuid = ( Guid ) e.RowKeyValue;

            var workflowTypeStateObj = WorkflowsState.Where( g => g.Guid.Equals( rowGuid ) ).FirstOrDefault();
            if ( workflowTypeStateObj != null )
            {
                WorkflowsState.Remove( workflowTypeStateObj );
            }

            BindStepWorkflowsGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gWorkflows control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void gWorkflows_GridRebind( object sender, EventArgs e )
        {
            BindStepWorkflowsGrid();
        }

        /// <summary>
        /// Handles the Edit event of the gWorkflows control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gWorkflows_Edit( object sender, RowEventArgs e )
        {
            Guid stepWorkflowGuid = ( Guid ) e.RowKeyValue;
            gWorkflows_ShowEdit( stepWorkflowGuid );
        }

        /// <summary>
        /// Show the edit dialog for the specified Workflow Trigger.
        /// </summary>
        /// <param name="triggerGuid">The workflow trigger unique identifier.</param>
        protected void gWorkflows_ShowEdit( Guid triggerGuid )
        {
            var workflowTrigger = WorkflowsState.FirstOrDefault( l => l.Guid.Equals( triggerGuid ) );

            if ( workflowTrigger != null )
            {
                wpWorkflowType.SetValue( workflowTrigger.WorkflowTypeId );
                ddlTriggerType.SelectedValue = workflowTrigger.TriggerType.ToString();
            }
            else
            {
                // Set default values
                wpWorkflowType.SetValue( null );
                ddlTriggerType.SelectedValue = StepWorkflowTrigger.WorkflowTriggerCondition.IsComplete.ToString();
            }

            hfAddStepWorkflowGuid.Value = triggerGuid.ToString();
            ShowDialog( "StepWorkflows", true );
            UpdateTriggerQualifiers();
        }

        /// <summary>
        /// Handles the Add event of the gWorkflows control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void gWorkflows_Add( object sender, EventArgs e )
        {
            gWorkflows_ShowEdit( Guid.Empty );
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlTriggerType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ddlTriggerType_SelectedIndexChanged( object sender, EventArgs e )
        {
            UpdateTriggerQualifiers();
        }

        /// <summary>
        /// Updates the trigger qualifiers.
        /// </summary>
        private void UpdateTriggerQualifiers()
        {
            var rockContext = this.GetDataContext();

            var workflowTrigger = WorkflowsState.FirstOrDefault( l => l.Guid.Equals( hfAddStepWorkflowGuid.Value.AsGuid() ) );

            var sStepWorkflowTriggerType = ddlTriggerType.SelectedValueAsEnum<StepWorkflowTrigger.WorkflowTriggerCondition>();

            if ( sStepWorkflowTriggerType == StepWorkflowTrigger.WorkflowTriggerCondition.StatusChanged )
            {
                // Populate the selection lists for "To Status" and "From Status".
                var stepType = this.GetStepType();

                var statusList = new StepStatusService( rockContext ).Queryable().Where( s => s.StepProgramId == stepType.StepProgramId ).ToList();

                ddlPrimaryQualifier.Label = "From";
                ddlPrimaryQualifier.Visible = true;
                ddlPrimaryQualifier.Items.Clear();
                ddlPrimaryQualifier.Items.Add( new ListItem( string.Empty, string.Empty ) );

                foreach ( var status in statusList )
                {
                    ddlPrimaryQualifier.Items.Add( new ListItem( status.Name, status.Id.ToString().ToUpper() ) );
                }

                ddlSecondaryQualifier.Label = "To";
                ddlSecondaryQualifier.Visible = true;
                ddlSecondaryQualifier.Items.Clear();
                ddlSecondaryQualifier.Items.Add( new ListItem( string.Empty, string.Empty ) );

                foreach ( var status in statusList )
                {
                    ddlSecondaryQualifier.Items.Add( new ListItem( status.Name, status.Id.ToString().ToUpper() ) );
                }
            }
            else
            {
                ddlPrimaryQualifier.Visible = false;
                ddlPrimaryQualifier.Items.Clear();
                ddlSecondaryQualifier.Visible = false;
                ddlSecondaryQualifier.Items.Clear();
            }

            // Set the qualifier values.
            if ( workflowTrigger != null )
            {
                if ( workflowTrigger.TriggerType == sStepWorkflowTriggerType )
                {
                    var qualifierSettings = new StepWorkflowTrigger.StatusChangeTriggerSettings( workflowTrigger.TypeQualifier );

                    ddlPrimaryQualifier.SelectedValue = qualifierSettings.FromStatusId.ToStringSafe();
                    ddlSecondaryQualifier.SelectedValue = qualifierSettings.ToStatusId.ToStringSafe();
                }
            }

        }

        /// <summary>
        /// Binds the workflow triggers grid.
        /// </summary>
        private void BindStepWorkflowsGrid()
        {
            if ( WorkflowsState != null )
            {
                SetStepWorkflowListOrder( WorkflowsState );

                // Set the description for the trigger.
                var stepService = new StepWorkflowTriggerService( new RockContext() );

                foreach ( var workflowTrigger in WorkflowsState )
                {
                    var qualifierSettings = new StepWorkflowTrigger.StatusChangeTriggerSettings( workflowTrigger.TypeQualifier );

                    workflowTrigger.TriggerDescription = stepService.GetTriggerSettingsDescription( workflowTrigger.TriggerType, qualifierSettings );
                }

                gWorkflows.DataSource = WorkflowsState;
            }


            gWorkflows.DataBind();
        }

        /// <summary>
        /// Sets the workflow triggers list order.
        /// </summary>
        /// <param name="stepWorkflowList">The workflow trigger list.</param>
        private void SetStepWorkflowListOrder( List<StepWorkflowTriggerViewModel> stepWorkflowList )
        {
            if ( stepWorkflowList != null )
            {
                if ( stepWorkflowList.Any() )
                {
                    stepWorkflowList.OrderBy( c => c.WorkflowTypeName ).ThenBy( c => c.TriggerType.ConvertToString() ).ToList();
                }
            }
        }

        #endregion

        #endregion

        #region Internal Methods

        /// <summary>
        /// Retrieve a singleton data context for data operations in this block.
        /// </summary>
        /// <returns></returns>
        private RockContext GetDataContext()
        {
            if ( _dataContext == null )
            {
                _dataContext = new RockContext();
            }

            return _dataContext;
        }

        /// <summary>
        /// Initialize handlers for block configuration changes.
        /// </summary>
        /// <param name="triggerPanel"></param>
        private void InitializeSettingsNotification( UpdatePanel triggerPanel )
        {
            // Set up Block Settings change notification.
            BlockUpdated += Block_BlockUpdated;

            AddConfigurationUpdateTrigger( triggerPanel );
        }

        /// <summary>
        /// Initialize the essential context in which this block is operating.
        /// </summary>
        /// <returns>True, if the block context is valid.</returns>
        private bool InitializeBlockContext()
        {
            _stepProgramId = PageParameter( PageParameterKey.StepProgramId ).AsInteger();
            _stepTypeId = PageParameter( PageParameterKey.StepTypeId ).AsInteger();

            if ( _stepProgramId == 0
                 && _stepTypeId == 0 )
            {
                ShowNotification( "A new Step cannot be added because there is no Step Program available in this context.", NotificationBoxType.Danger, true );

                return false;
            }

            return true;
        }

        /// <summary>
        /// Populate the selection list for Workflow Trigger Types.
        /// </summary>
        private void LoadWorkflowTriggerTypesSelectionList()
        {
            ddlTriggerType.Items.Add( new ListItem( "Step Completed", StepWorkflowTrigger.WorkflowTriggerCondition.IsComplete.ToString() ) );
            ddlTriggerType.Items.Add( new ListItem( "Status Changed", StepWorkflowTrigger.WorkflowTriggerCondition.StatusChanged.ToString() ) );
            ddlTriggerType.Items.Add( new ListItem( "Manual", StepWorkflowTrigger.WorkflowTriggerCondition.Manual.ToString() ) );
        }

        /// <summary>
        /// Populate the selection list for Prerequisite Steps.
        /// </summary>
        private void LoadPrerequisiteStepsList()
        {
            var dataContext = this.GetDataContext();

            // Load available Prerequisite Steps, being any other Step Types in this Step Program that are active.
            var stepType = this.GetStepType();

            int programId = 0;

            if ( stepType != null )
            {
                programId = stepType.StepProgramId;
            }

            if ( programId == 0 )
            {
                programId = _stepProgramId;
            }

            var stepsService = new StepTypeService( dataContext );

            var prerequisiteStepTypes = stepsService.Queryable()
                .Include( x => x.StepProgram )
                .Where( x => x.StepProgramId == programId && x.Id != _stepTypeId && x.IsActive )
                .ToList();

            cblPrerequsities.DataSource = prerequisiteStepTypes;

            cblPrerequsities.DataBind();
        }

        /// <summary>
        /// Shows the detail panel containing the main content of the block.
        /// </summary>
        /// <param name="stepTypeId">The entity id of the item to be shown.</param>
        public void ShowDetail( int stepTypeId )
        {
            pnlDetails.Visible = false;

            var rockContext = this.GetDataContext();

            // Get the Step Type data model
            var stepType = this.GetStepType( stepTypeId );

            if ( stepType.Id != 0 )
            {
                pdAuditDetails.SetEntity( stepType, ResolveRockUrl( "~" ) );
            }
            else
            {
                // hide the panel drawer that show created and last modified dates
                pdAuditDetails.Visible = false;
            }

            // Admin rights are required to edit a Step Type. Edit rights only allow adding/removing items.
            bool adminAllowed = UserCanAdministrate || stepType.IsAuthorized( Authorization.ADMINISTRATE, CurrentPerson );
            pnlDetails.Visible = true;
            hfStepTypeId.Value = stepType.Id.ToString();
            lIcon.Text = string.Format( "<i class='{0}'></i>", stepType.IconCssClass );
            bool readOnly = false;

            nbEditModeMessage.Text = string.Empty;
            if ( !adminAllowed )
            {
                readOnly = true;
                nbEditModeMessage.Text = EditModeMessage.ReadOnlyEditActionNotAllowed( StepProgram.FriendlyTypeName );
            }

            if ( readOnly )
            {
                btnEdit.Visible = false;
                btnDelete.Visible = false;
                btnSecurity.Visible = false;
                ShowReadonlyDetails( stepType );
            }
            else
            {
                btnEdit.Visible = true;
                btnDelete.Visible = true;
                btnSecurity.Visible = true;

                btnSecurity.Title = "Secure " + stepType.Name;
                btnSecurity.EntityId = stepType.Id;

                if ( !stepTypeId.Equals( 0 ) )
                {
                    ShowReadonlyDetails( stepType );
                }
                else
                {
                    ShowEditDetails( stepType );
                }
            }

        }

        /// <summary>
        /// Shows the edit details.
        /// </summary>
        /// <param name="stepType">The entity instance to be displayed.</param>
        private void ShowEditDetails( StepType stepType )
        {
            if ( stepType == null )
            {
                stepType = new StepType();
                stepType.IconCssClass = "fa fa-compress";
            }
            if ( stepType.Id == 0 )
            {
                lReadOnlyTitle.Text = ActionTitle.Add( StepType.FriendlyTypeName ).FormatAsHtmlTitle();
            }
            else
            {
                lReadOnlyTitle.Text = stepType.Name.FormatAsHtmlTitle();
            }

            SetEditMode( true );

            this.LoadPrerequisiteStepsList();
            this.LoadWorkflowTriggerTypesSelectionList();

            // General properties
            tbName.Text = stepType.Name;
            cbIsActive.Checked = stepType.IsActive;
            tbDescription.Text = stepType.Description;

            tbIconCssClass.Text = stepType.IconCssClass;
            cpHighlight.Text = stepType.HighlightColor;

            cbAllowMultiple.Checked = stepType.AllowMultiple;
            cbHasDuration.Checked = stepType.HasEndDate;
            cbShowBadgeCount.Checked = stepType.ShowCountOnBadge;

            // Pre-requisites
            if ( stepType.StepTypePrerequisites != null )
            {
                cblPrerequsities.SetValues( stepType.StepTypePrerequisites.Select( x => x.PrerequisiteStepTypeId ) );
            }

            // Advanced Settings
            dvpAutocomplete.SetValue( stepType.AutoCompleteDataViewId );
            dvpAudience.SetValue( stepType.AudienceDataViewId );

            mtpMergeTemplate.SetValue( stepType.MergeTemplateId );
            tbMergeDescriptor.Text = stepType.MergeTemplateDescriptor;

            cbAllowEdit.Checked = stepType.AllowManualEditing;

            ceCardTemplate.Text = stepType.CardLavaTemplate;

            // Workflow Triggers
            WorkflowsState = new List<StepWorkflowTriggerViewModel>();

            foreach ( var trigger in stepType.StepWorkflowTriggers )
            {
                var newItem = new StepWorkflowTriggerViewModel( trigger );

                WorkflowsState.Add( newItem );
            }

            BindStepWorkflowsGrid();
        }

        /// <summary>
        /// Shows the readonly details.
        /// </summary>
        /// <param name="stepType">The entity instance to be displayed.</param>
        private void ShowReadonlyDetails( StepType stepType )
        {
            SetEditMode( false );

            hfStepTypeId.SetValue( stepType.Id );

            WorkflowsState = null;

            lReadOnlyTitle.Text = stepType.Name.FormatAsHtmlTitle();

            // Create the read-only description text.
            var descriptionListMain = new DescriptionList();

            descriptionListMain.Add( "Description", stepType.Description );

            lStepTypeDescription.Text = descriptionListMain.Html;

            // Configure Label: Inactive
            hlInactive.Visible = !stepType.IsActive;

            this.RefreshChart();
        }

        /// <summary>
        /// Delete the current record.
        /// </summary>
        private void DeleteRecord()
        {
            var rockContext = this.GetDataContext();

            var stepTypeService = new StepTypeService( rockContext );

            var stepType = this.GetStepType( forceLoadFromContext: true );

            if ( stepType != null )
            {                
                if ( !stepType.IsAuthorized( Authorization.ADMINISTRATE, this.CurrentPerson ) )
                {
                    mdDeleteWarning.Show( "You are not authorized to delete this item.", ModalAlertType.Information );
                    return;
                }

                string errorMessage;

                if ( !stepTypeService.CanDelete( stepType, out errorMessage ) )
                {
                    mdDeleteWarning.Show( errorMessage, ModalAlertType.Information );
                    return;
                }

                stepTypeService.Delete( stepType );
                rockContext.SaveChanges();
            }

            GoToStepProgramPage();
        }

        /// <summary>
        /// Gets the specified Step Type data model, or the current model if none is specified.
        /// </summary>
        /// <param name="stepType">The entity id of the instance to be retrieved.</param>
        /// <returns></returns>
        private StepType GetStepType( int? stepTypeId = null, bool forceLoadFromContext = false )
        {
            if ( stepTypeId == null )
            {
                stepTypeId = hfStepTypeId.ValueAsInt();
            }

            string key = string.Format( "StepType:{0}", stepTypeId );

            StepType stepType = null;

            if ( !forceLoadFromContext )
            {
                stepType = RockPage.GetSharedItem( key ) as StepType;
            }

            if ( stepType == null )
            {
                var rockContext = this.GetDataContext();

                stepType = new StepTypeService( rockContext ).Queryable()
                    .Where( c => c.Id == stepTypeId )
                    .FirstOrDefault();

                if ( stepType == null )
                {
                    stepType = new StepType { Id = 0 };
                }

                RockPage.SaveSharedItem( key, stepType );
            }

            if ( _stepProgramId == default( int ) )
            {
                _stepProgramId = stepType.StepProgramId;
            }

            return stepType;
        }

        private int GetActiveStepTypeId()
        {
            return hfStepTypeId.ValueAsInt();
        }

        /// <summary>
        /// Sets the edit mode.
        /// </summary>
        /// <param name="editable">if set to <c>true</c> [editable].</param>
        private void SetEditMode( bool editable )
        {
            pnlEditDetails.Visible = editable;
            pnlViewDetails.Visible = !editable;

            this.HideSecondaryBlocks( editable );
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void ShowDialog( string dialog, bool setValues = false )
        {
            hfActiveDialog.Value = dialog.ToUpper().Trim();
            ShowDialog( setValues );
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void ShowDialog( bool setValues = false )
        {
            switch ( hfActiveDialog.Value )
            {
                case "STEPWORKFLOWS":
                    dlgStepWorkflow.Show();
                    break;
            }
        }

        /// <summary>
        /// Hides the dialog.
        /// </summary>
        private void HideDialog()
        {
            switch ( hfActiveDialog.Value )
            {
                case "STEPWORKFLOWS":
                    dlgStepWorkflow.Hide();
                    break;
            }

            hfActiveDialog.Value = string.Empty;
        }

        #endregion

        #region Step Activity Chart

        private void InitializeChart()
        {
            // Set the default Date Range from the block settings.
            var dateRangeSettings = GetAttributeValue( AttributeKey.SlidingDateRange );

            if ( !string.IsNullOrEmpty( dateRangeSettings ) )
            {
                drpSlidingDateRange.DelimitedValues = dateRangeSettings;
            }

            if ( drpSlidingDateRange.SlidingDateRangeMode == SlidingDateRangePicker.SlidingDateRangeType.All )
            {
                // Default to current year
                drpSlidingDateRange.SlidingDateRangeMode = SlidingDateRangePicker.SlidingDateRangeType.Current;
                drpSlidingDateRange.TimeUnit = SlidingDateRangePicker.TimeUnitType.Year;
            }
        }

        private void RefreshChart()
        {
            var chartDateRange = SlidingDateRangePicker.CalculateDateRangeFromDelimitedValues( drpSlidingDateRange.DelimitedValues ?? "-1||" );

            var chartData = this.GetChartData( chartDateRange.Start, chartDateRange.End );

            this.ConfigureChart( chartData, chartDateRange.Start, chartDateRange.End );

            this.BindChart( chartData, chartDateRange.Start, chartDateRange.End );
        }

        /// <summary>
        /// Configures the scale and appearance of the chart according to the dataset being displayed.
        /// </summary>
        /// <param name="chartData"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        private void ConfigureChart( FlotChartDataSet<TimeSeriesChartDataPoint> chartData, DateTime? startDate = null, DateTime? endDate = null )
        {
            lcSteps.StartDate = startDate ?? chartData.DataPoints.Min( x => x.DateTime );
            lcSteps.EndDate = endDate ?? chartData.DataPoints.Max( x => x.DateTime );

            lcSteps.Options.series = new SeriesOptions( false, true, false );

            lcSteps.Options.yaxis = new AxisOptions
            {
                minTickSize = 1,
                tickFormatter = "function (val, axis) { return val | 0 }"
            };

            lcSteps.Options.xaxis = new AxisOptions
            {
                mode = AxisMode.time,
                timeformat = "%b-%y",
                tickSize = new string[] { "1", "month" }
            };

            lcSteps.Options.grid = new GridOptions { hoverable = true, clickable = false };
        }

        /// <summary>
        /// Bind the necessary data to the graph widgets.
        /// </summary>
        /// <param name="chartDataJson"></param>
        private void BindChart( FlotChartDataSet<TimeSeriesChartDataPoint> chartData, DateTime? startDate = null, DateTime? endDate = null )
        {
            lcSteps.Visible = GetAttributeValue( AttributeKey.ShowChart ).AsBooleanOrNull() ?? true;

            var chartDateRange = SlidingDateRangePicker.CalculateDateRangeFromDelimitedValues( drpSlidingDateRange.DelimitedValues ?? "-1||" );

            lcSteps.StartDate = chartDateRange.Start;
            lcSteps.EndDate = chartDateRange.End;

            lcSteps.ChartData = chartData.GetRockChartJsonData();
        }

        /// <summary>
        /// Gets the steps started and completed within the specified date period.
        /// </summary>
        /// <returns></returns>
        public FlotChartDataSet<TimeSeriesChartDataPoint> GetChartData( DateTime? startDate = null, DateTime? endDate = null )
        {
            var dataContext = this.GetDataContext();

            var stepService = new StepService( dataContext );

            // Get the Steps associated with the current Step Type.
            var stepsStartedQuery = stepService.Queryable()
                .Where( x => x.StepTypeId == _stepTypeId && x.StepType.IsActive && x.StartDateTime != null && x.CompletedDateTime == null );

            var stepsCompletedQuery = stepService.Queryable()
                .Where( x => x.StepTypeId == _stepTypeId && x.StepType.IsActive && x.CompletedDateTime != null );

            if ( startDate != null )
            {
                startDate = startDate.Value.Date;

                stepsStartedQuery = stepsStartedQuery.Where( x => x.StartDateTime >= startDate );
                stepsCompletedQuery = stepsCompletedQuery.Where( x => x.CompletedDateTime >= startDate );
            }

            if ( endDate != null )
            {
                var compareDate = endDate.Value.Date.AddDays( 1 );

                stepsStartedQuery = stepsStartedQuery.Where( x => x.StartDateTime < compareDate );
                stepsCompletedQuery = stepsCompletedQuery.Where( x => x.CompletedDateTime < compareDate );
            }

            var chartData = new FlotChartDataSet<TimeSeriesChartDataPoint>();

            var startedSeriesData = stepsStartedQuery.ToList()
                .GroupBy( x => new DateTime( x.StartDateTime.Value.Year, x.StartDateTime.Value.Month, 1 ) )
                .Select( x => new TimeSeriesChartDataPoint
                {
                    SeriesName = "Started",
                    DateTime = x.Key,
                    Value = x.Count(),
                    SortKey = "1"
                } );

            var completedSeriesData = stepsCompletedQuery.ToList()
                .GroupBy( x => new DateTime( x.CompletedDateTime.Value.Year, x.CompletedDateTime.Value.Month, 1 ) )
                .Select( x => new TimeSeriesChartDataPoint
                {
                    SeriesName = "Completed",
                    DateTime = x.Key,
                    Value = x.Count(),
                    SortKey = "2"
                } );

            var allSeriesData = startedSeriesData.Union( completedSeriesData ).OrderBy( x => x.SortKey ).ThenBy( x => x.DateTime );

            chartData.DataPoints.AddRange( allSeriesData );

            return chartData;
        }

        #endregion

        #region Support Classes

        [Serializable]
        private class StepWorkflowTriggerViewModel
        {
            public int Id { get; set; }
            public Guid Guid { get; set; }
            public string WorkflowTypeName { get; set; }

            public int? StepTypeId { get; set; }
            public int WorkflowTypeId { get; set; }

            public StepWorkflowTrigger.WorkflowTriggerCondition TriggerType { get; set; }
            public string TypeQualifier { get; set; }

            public string TriggerDescription { get; set; }

            public StepWorkflowTriggerViewModel()
            {
                //
            }

            public StepWorkflowTriggerViewModel( StepWorkflowTrigger trigger )
            {
                Id = trigger.Id;
                Guid = trigger.Guid;
                StepTypeId = trigger.StepTypeId;
                TriggerType = trigger.TriggerType;
                TypeQualifier = trigger.TypeQualifier;

                if ( trigger.WorkflowType != null )
                {
                    WorkflowTypeId = trigger.WorkflowType.Id;
                    WorkflowTypeName = trigger.WorkflowType.Name;
                }
            }
        }

        #endregion

        #region Block Notifications

        private NotificationBox _notificationControl;
        private Control _detailContainerControl;

        /// <summary>
        /// Initialize block-level notification message handlers for block configuration changes.
        /// </summary>
        /// <param name="triggerPanel"></param>
        private void InitializeBlockNotification( NotificationBox notificationControl, Control detailContainerControl )
        {
            _notificationControl = notificationControl;
            _detailContainerControl = detailContainerControl;

            this.ClearBlockNotification();
        }

        /// <summary>
        /// Reset the notification message for the block.
        /// </summary>
        public void ClearBlockNotification()
        {
            _notificationControl.Visible = false;
            _detailContainerControl.Visible = true;
        }

        /// <summary>
        /// Show a notification message for the block.
        /// </summary>
        /// <param name="notificationControl"></param>
        /// <param name="message"></param>
        /// <param name="notificationType"></param>
        public void ShowNotification( string message, NotificationBoxType notificationType = NotificationBoxType.Info, bool hideBlockContent = false )
        {
            _notificationControl.Text = message;
            _notificationControl.NotificationBoxType = notificationType;

            _notificationControl.Visible = true;
            _detailContainerControl.Visible = !hideBlockContent;
        }

        #endregion
    }
}