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
    [DisplayName( "Step Program Detail" )]
    [Category( "Steps" )]
    [Description( "Displays the details of the given Step Program for editing." )]

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

    #endregion Block Attributes

    public partial class StepProgramDetail : RockBlock, IDetailBlock
    {
        #region Attribute Keys

        /// <summary>
        /// Keys to use for Block Attributes
        /// </summary>
        protected static class AttributeKey
        {
            public const string ShowChart = "Show Chart";
            public const string ChartStyle = "Chart Style";
            public const string SlidingDateRange = "SlidingDateRange";
            public const string CombineChartSeries = "CombineChartSeries";
        }

        #endregion Attribute Keys

        #region Page Parameter Keys

        /// <summary>
        /// Keys to use for Page Parameters
        /// </summary>
        protected static class PageParameterKey
        {
            public const string StepProgramId = "ProgramId";
        }

        #endregion Page Parameter Keys

        #region Properties

        private List<StepStatus> StatusesState { get; set; }
        private List<StepWorkflowTrigger> WorkflowsState { get; set; }

        #endregion Properties

        #region Private Variables

        private RockBlockNotificationManager _notificationManager;

        #endregion Private Variables

        #region Control Methods

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            string json;

            json = ViewState["StatusesState"] as string ?? string.Empty;

            this.StatusesState = JsonConvert.DeserializeObject<List<StepStatus>>( json );

            this.StatusesState = this.StatusesState ?? new List<StepStatus>();

            json = ViewState["WorkflowsState"] as string ?? string.Empty;

            this.WorkflowsState = JsonConvert.DeserializeObject<List<StepWorkflowTrigger>>( json );

            this.WorkflowsState = this.WorkflowsState ?? new List<StepWorkflowTrigger>();
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            // Create a custom contract resolver to specifically ignore some complex properties that add significant amounts of unnecessary ViewState data.
            var resolver = new Rock.Utility.DynamicPropertyMapContractResolver();

            resolver.IgnoreProperty( typeof( StepStatus ), "StepProgram", "Steps", "UrlEncodedKey" );

            var jsonSetting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = resolver,
            };

            ViewState["StatusesState"] = JsonConvert.SerializeObject( StatusesState, Formatting.None, jsonSetting );
            ViewState["WorkflowsState"] = JsonConvert.SerializeObject( WorkflowsState, Formatting.None, jsonSetting );

            return base.SaveViewState();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            InitializeSettingsNotification( upStepProgram );

            _notificationManager = new RockBlockNotificationManager( this, nbBlockStatus, pnlContent );

            InitializeStatusesGrid();
            InitializeWorkflowsGrid();
            InitializeActionButtons();
            InitializeChartFilter();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                var stepProgramId = PageParameter( PageParameterKey.StepProgramId ).AsInteger();

                this.ShowDetail( stepProgramId );
            }
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

            int? stepProgramId = PageParameter( pageReference, PageParameterKey.StepProgramId ).AsIntegerOrNull();
            if ( stepProgramId != null )
            {
                var stepProgram = new StepProgramService( new RockContext() ).Get( stepProgramId.Value );

                if ( stepProgram != null )
                {
                    breadCrumbs.Add( new BreadCrumb( stepProgram.Name, pageReference ) );
                }
                else
                {
                    breadCrumbs.Add( new BreadCrumb( "New Step Program", pageReference ) );
                }
            }
            else
            {
                // don't show a breadcrumb if we don't have a pageparam to work with
            }

            return breadCrumbs;
        }

        /// <summary>
        /// Initialize the Statuses grid.
        /// </summary>
        private void InitializeStatusesGrid()
        {
            gStatuses.DataKeyNames = new string[] { "Guid" };
            gStatuses.Actions.ShowAdd = true;
            gStatuses.Actions.AddClick += gStatuses_Add;
            gStatuses.GridRebind += gStatuses_GridRebind;
            gStatuses.GridReorder += gStatuses_GridReorder;
        }

        /// <summary>
        /// Initialize the Workflows grid.
        /// </summary>
        private void InitializeWorkflowsGrid()
        {
            gWorkflows.DataKeyNames = new string[] { "Guid" };
            gWorkflows.Actions.ShowAdd = true;
            gWorkflows.Actions.AddClick += gWorkflows_Add;
            gWorkflows.GridRebind += gWorkflows_GridRebind;
        }

        /// <summary>
        /// Initialize the action buttons that affect the entire record.
        /// </summary>
        private void InitializeActionButtons()
        {
            btnDelete.Attributes["onclick"] = string.Format( "javascript: return Rock.dialogs.confirmDelete(event, '{0}', 'All associated Step Types and Step Participants will also be deleted!');", StepProgram.FriendlyTypeName );

            btnSecurity.EntityTypeId = EntityTypeCache.Get( typeof( Rock.Model.StepProgram ) ).Id;
        }

        /// <summary>
        /// Set the initial value of controls
        /// </summary>
        private void IntializeChartFilter()
        {
            // Set the default Date Range from the block settings.
            drpSlidingDateRange.DelimitedValues = GetAttributeValue( AttributeKey.SlidingDateRange ) ?? "-1||";
        }

        /// <summary>
        /// Initialize handlers for block configuration changes.
        /// </summary>
        /// <param name="triggerPanel"></param>
        private void InitializeSettingsNotification( UpdatePanel triggerPanel )
        {
            // Set up Block Settings change notification.
            this.BlockUpdated += Block_BlockUpdated;

            this.AddConfigurationUpdateTrigger( triggerPanel );
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
            this.StartEditMode();
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            this.SaveRecord();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnCancel_Click( object sender, EventArgs e )
        {
            this.CancelEditMode();
        }

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            var currentstepProgram = GetStepProgram();

            if ( currentstepProgram != null )
            {
                ShowReadonlyDetails( currentstepProgram );
            }
            else
            {
                string stepProgramId = PageParameter( PageParameterKey.StepProgramId );
                if ( !string.IsNullOrWhiteSpace( stepProgramId ) )
                {
                    ShowDetail( stepProgramId.AsInteger() );
                }
                else
                {
                    pnlDetails.Visible = false;
                }
            }
        }

        #endregion

        #region StepStatus Events

        /// <summary>
        /// Handles the Delete event of the gStatuses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gStatuses_Delete( object sender, RowEventArgs e )
        {
            var rowGuid = ( Guid ) e.RowKeyValue;

            StatusesState.RemoveEntity( rowGuid );

            BindStepStatusesGrid();
        }

        /// <summary>
        /// Handles the Click event of the btnAddStepStatus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnAddStepStatus_Click( object sender, EventArgs e )
        {
            StepStatus stepStatus = null;

            var guid = hfStepProgramAddStepStatusGuid.Value.AsGuid();

            if ( !guid.IsEmpty() )
            {
                stepStatus = StatusesState.FirstOrDefault( l => l.Guid.Equals( guid ) );
            }

            if ( stepStatus == null )
            {
                stepStatus = new StepStatus();
            }

            stepStatus.Name = tbStepStatusName.Text;
            stepStatus.IsActive = cbIsActive.Checked;
            stepStatus.IsCompleteStatus = cbIsCompleted.Checked;
            stepStatus.StatusColor = cpStatus.Text;

            if ( !stepStatus.IsValid )
            {
                return;
            }

            if ( StatusesState.Any( a => a.Guid.Equals( stepStatus.Guid ) ) )
            {
                StatusesState.RemoveEntity( stepStatus.Guid );
            }

            StatusesState.Add( stepStatus );
            BindStepStatusesGrid();
            HideDialog();
        }

        /// <summary>
        /// Handles the GridRebind event of the gStatuses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void gStatuses_GridRebind( object sender, EventArgs e )
        {
            BindStepStatusesGrid();
        }

        /// <summary>
        /// Handles the GridReorder event of the gStatuses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridReorderEventArgs" /> instance containing the event data.</param>
        void gStatuses_GridReorder( object sender, GridReorderEventArgs e )
        {
            var movedItem = StatusesState.Where( ss => ss.Order == e.OldIndex ).FirstOrDefault();

            if ( movedItem != null )
            {
                if ( e.NewIndex < e.OldIndex )
                {
                    // Moved up
                    foreach ( var otherItem in StatusesState.Where( ss => ss.Order < e.OldIndex && ss.Order >= e.NewIndex ) )
                    {
                        otherItem.Order++;
                    }
                }
                else
                {
                    // Moved Down
                    foreach ( var otherItem in StatusesState.Where( ss => ss.Order > e.OldIndex && ss.Order <= e.NewIndex ) )
                    {
                        otherItem.Order--;
                    }
                }

                movedItem.Order = e.NewIndex;
            }

            BindStepStatusesGrid();
        }

        /// <summar>ymod
        /// Handles the Add event of the gStatuses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void gStatuses_Add( object sender, EventArgs e )
        {
            gStatuses_ShowEdit( Guid.Empty );
        }

        /// <summary>
        /// Handles the Edit event of the gStatuses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gStatuses_Edit( object sender, RowEventArgs e )
        {
            Guid StepStatusGuid = ( Guid ) e.RowKeyValue;
            gStatuses_ShowEdit( StepStatusGuid );
        }

        /// <summary>
        /// gs the statuses_ show edit.
        /// </summary>
        /// <param name="StepStatusGuid">The connection status unique identifier.</param>
        protected void gStatuses_ShowEdit( Guid stepStatusGuid )
        {
            var stepStatus = StatusesState.FirstOrDefault( l => l.Guid.Equals( stepStatusGuid ) );

            if ( stepStatus != null )
            {
                tbStepStatusName.Text = stepStatus.Name;
                cbIsActive.Checked = stepStatus.IsActive;
                cpStatus.Value = stepStatus.StatusColor;
                cbIsCompleted.Checked = stepStatus.IsCompleteStatus;
            }
            else
            {
                tbStepStatusName.Text = string.Empty;
                cbIsActive.Checked = true;
                cbIsCompleted.Checked = false;
            }

            hfStepProgramAddStepStatusGuid.Value = stepStatusGuid.ToString();

            ShowDialog( "StepStatuses", true );
        }

        /// <summary>
        /// Binds the connection statuses grid.
        /// </summary>
        private void BindStepStatusesGrid()
        {
            SetStepStatusStateOrder();

            gStatuses.DataSource = StatusesState;

            gStatuses.DataBind();
        }

        /// <summary>
        /// Sets the attribute list order.
        /// </summary>
        /// <param name="attributeList">The attribute list.</param>
        private void SetStepStatusStateOrder()
        {
            if ( StatusesState != null )
            {
                if ( StatusesState.Any() )
                {
                    StatusesState = StatusesState.OrderBy( ss => ss.Order ).ThenBy( ss => ss.Name ).ToList();

                    for ( var i = 0; i < StatusesState.Count; i++ )
                    {
                        StatusesState[i].Order = i;
                    }

                    SaveViewState();
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
            StepWorkflowTrigger workflowTrigger = null;
            var guid = hfAddStepWorkflowGuid.Value.AsGuid();
            if ( !guid.IsEmpty() )
            {
                workflowTrigger = WorkflowsState.FirstOrDefault( l => l.Guid.Equals( guid ) );
            }

            if ( workflowTrigger == null )
            {
                workflowTrigger = new StepWorkflowTrigger();
            }
            try
            {
                workflowTrigger.WorkflowType = new WorkflowTypeService( new RockContext() ).Get( wpWorkflowType.SelectedValueAsId().Value );
            }
            catch { }

            workflowTrigger.WorkflowTypeId = wpWorkflowType.SelectedValueAsId().Value;
            workflowTrigger.TriggerType = ddlTriggerType.SelectedValueAsEnum<StepWorkflowTrigger.WorkflowTriggerCondition>();
            workflowTrigger.TypeQualifier = String.Format( "|{0}|{1}|", ddlPrimaryQualifier.SelectedValue, ddlSecondaryQualifier.SelectedValue );
            workflowTrigger.StepProgramId = 0;

            if ( !workflowTrigger.IsValid )
            {
                return;
            }
            if ( WorkflowsState.Any( a => a.Guid.Equals( workflowTrigger.Guid ) ) )
            {
                WorkflowsState.RemoveEntity( workflowTrigger.Guid );
            }

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
            WorkflowsState.RemoveEntity( rowGuid );

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
        /// Gs the workflows_ show edit.
        /// </summary>
        /// <param name="stepWorkflowGuid">The connection workflow unique identifier.</param>
        protected void gWorkflows_ShowEdit( Guid stepWorkflowGuid )
        {
            var workflowTrigger = WorkflowsState.FirstOrDefault( l => l.Guid.Equals( stepWorkflowGuid ) );

            if ( workflowTrigger != null )
            {
                wpWorkflowType.SetValue( workflowTrigger.WorkflowTypeId );
                ddlTriggerType.SelectedValue = workflowTrigger.TriggerType.ToString();
            }


            hfAddStepWorkflowGuid.Value = stepWorkflowGuid.ToString();
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
            using ( var rockContext = new RockContext() )
            {
                var qualifierValues = new String[2];

                var workflowTrigger = WorkflowsState.FirstOrDefault( l => l.Guid.Equals( hfAddStepWorkflowGuid.Value.AsGuid() ) );

                var sStepWorkflowTriggerType = ddlTriggerType.SelectedValueAsEnum<StepWorkflowTrigger.WorkflowTriggerCondition>();

                int stepProgramId = int.Parse( hfStepProgramId.Value );

                switch ( sStepWorkflowTriggerType )
                {
                    case StepWorkflowTrigger.WorkflowTriggerCondition.Manual:
                        {
                            ddlPrimaryQualifier.Visible = false;
                            ddlPrimaryQualifier.Items.Clear();
                            ddlSecondaryQualifier.Visible = false;
                            ddlSecondaryQualifier.Items.Clear();
                            break;
                        }
                    case StepWorkflowTrigger.WorkflowTriggerCondition.StatusChanged:
                        {
                            var statusList = new StepStatusService( rockContext ).Queryable().Where( s => s.StepProgramId == stepProgramId ).ToList();
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
                            break;
                        }
                }

                if ( workflowTrigger != null )
                {
                    if ( workflowTrigger.TriggerType == sStepWorkflowTriggerType )
                    {
                        qualifierValues = workflowTrigger.TypeQualifier.SplitDelimitedValues();
                        if ( ddlPrimaryQualifier.Visible && qualifierValues.Length > 0 )
                        {
                            ddlPrimaryQualifier.SelectedValue = qualifierValues[0];
                        }

                        if ( ddlSecondaryQualifier.Visible && qualifierValues.Length > 1 )
                        {
                            ddlSecondaryQualifier.SelectedValue = qualifierValues[1];
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Binds the connection workflows grid.
        /// </summary>
        private void BindStepWorkflowsGrid()
        {
            if ( WorkflowsState != null )
            {
                SetStepWorkflowListOrder( WorkflowsState );
                gWorkflows.DataSource = WorkflowsState.Select( c => new
                {
                    c.Id,
                    c.Guid,
                    WorkflowType = c.WorkflowType.Name,
                    Trigger = c.TriggerType.ConvertToString()
                } ).ToList();
            }

            gWorkflows.DataBind();
        }

        /// <summary>
        /// Sets the connection workflow list order.
        /// </summary>
        /// <param name="stepWorkflowList">The connection workflow list.</param>
        private void SetStepWorkflowListOrder( List<StepWorkflowTrigger> stepWorkflowList )
        {
            if ( stepWorkflowList != null )
            {
                if ( stepWorkflowList.Any() )
                {
                    stepWorkflowList.OrderBy( c => c.WorkflowType.Name ).ThenBy( c => c.TriggerType.ConvertToString() ).ToList();
                }
            }
        }

        private void LoadWorkflowTriggerTypesSelectionList()
        {
            ddlTriggerType.Items.Add( new ListItem( "Program Completed", StepWorkflowTrigger.WorkflowTriggerCondition.IsComplete.ToString() ) );
            ddlTriggerType.Items.Add( new ListItem( "Status Changed", StepWorkflowTrigger.WorkflowTriggerCondition.StatusChanged.ToString() ) );
            ddlTriggerType.Items.Add( new ListItem( "Manual", StepWorkflowTrigger.WorkflowTriggerCondition.Manual.ToString() ) );
        }

        #endregion

        #endregion

        #region Internal Methods

        /// <summary>
        /// Cancel edit mode and return to read-only mode.
        /// </summary>
        private void CancelEditMode()
        {
            if ( hfStepProgramId.Value.Equals( "0" ) )
            {
                NavigateToParentPage();
            }
            else
            {
                ShowReadonlyDetails( this.GetStepProgram() );
            }
        }

        /// <summary>
        /// Enter record edit mode.
        /// </summary>
        private void StartEditMode()
        {
            var rockContext = new RockContext();

            var stepProgram = this.GetStepProgram( null, rockContext );

            ShowEditDetails( stepProgram );
        }

        /// <summary>
        /// Delete the current record.
        /// </summary>
        private void DeleteRecord()
        {
            var rockContext = new RockContext();

            var stepProgramService = new StepProgramService( rockContext );
            var authService = new AuthService( rockContext );

            var stepProgram = this.GetStepProgram( null, rockContext );

            if ( stepProgram != null )
            {
                if ( !stepProgram.IsAuthorized( Authorization.ADMINISTRATE, this.CurrentPerson ) )
                {
                    mdDeleteWarning.Show( "You are not authorized to delete this item.", ModalAlertType.Information );
                    return;
                }

                var stepTypes = stepProgram.StepTypes.ToList();
                var stepTypeService = new StepTypeService( rockContext );

                foreach ( var stepType in stepTypes )
                {
                    string errorMessageStepType;
                    if ( !stepTypeService.CanDelete( stepType, out errorMessageStepType ) )
                    {
                        mdDeleteWarning.Show( errorMessageStepType, ModalAlertType.Information );
                        return;
                    }

                    stepTypeService.Delete( stepType );
                }

                rockContext.SaveChanges();

                string errorMessage;
                if ( !stepProgramService.CanDelete( stepProgram, out errorMessage ) )
                {
                    mdDeleteWarning.Show( errorMessage, ModalAlertType.Information );
                    return;
                }

                stepProgramService.Delete( stepProgram );
                rockContext.SaveChanges();
            }

            NavigateToParentPage();
        }

        /// <summary>
        /// Save the current record.
        /// </summary>
        /// <returns></returns>
        private void SaveRecord()
        {
            StepProgram stepProgram;

            var rockContext = new RockContext();

            var stepProgramService = new StepProgramService( rockContext );
            var stepStatusService = new StepStatusService( rockContext );
            var attributeService = new AttributeService( rockContext );
            var qualifierService = new AttributeQualifierService( rockContext );

            int stepProgramId = int.Parse( hfStepProgramId.Value );

            if ( stepProgramId == 0 )
            {
                stepProgram = new StepProgram();

                stepProgramService.Add( stepProgram );
            }
            else
            {
                stepProgram = stepProgramService.Queryable()
                                                .Include( x => x.StepTypes )
                                                .Include( x => x.StepStatuses )
                                                .Where( c => c.Id == stepProgramId )
                                                .FirstOrDefault();

                var uiWorkflows = WorkflowsState.Select( l => l.Guid );

                foreach ( var trigger in stepProgram.StepWorkflowTriggers.Where( l => !uiWorkflows.Contains( l.Guid ) ).ToList() )
                {
                    stepProgram.StepWorkflowTriggers.Remove( trigger );
                }

                var uiStatuses = StatusesState.Select( r => r.Guid );

                foreach ( var StepStatus in stepProgram.StepStatuses.Where( r => !uiStatuses.Contains( r.Guid ) ).ToList() )
                {
                    stepProgram.StepStatuses.Remove( StepStatus );
                    stepStatusService.Delete( StepStatus );
                }
            }

            stepProgram.Name = tbName.Text;
            stepProgram.IsActive = cbActive.Checked;
            stepProgram.Description = tbDescription.Text;
            stepProgram.IconCssClass = tbIconCssClass.Text;

            stepProgram.CategoryId = cpCategory.SelectedValueAsInt();

            stepProgram.DefaultListView = rblDefaultListView.SelectedValue.ConvertToEnum<StepProgram.ViewMode>( StepProgram.ViewMode.Cards );

            // Update Statuses
            foreach ( var stepStatusState in this.StatusesState )
            {
                var stepStatus = stepProgram.StepStatuses.Where( a => a.Guid == stepStatusState.Guid ).FirstOrDefault();

                if ( stepStatus == null )
                {
                    stepStatus = new StepStatus();
                    stepProgram.StepStatuses.Add( stepStatus );
                }

                stepStatus.CopyPropertiesFrom( stepStatusState );
                stepStatus.StepProgramId = stepProgram.Id;
            }

            // Update Workflow Triggers
            foreach ( var stateTrigger in WorkflowsState )
            {
                var workflowTrigger = stepProgram.StepWorkflowTriggers.Where( a => a.Guid == stateTrigger.Guid ).FirstOrDefault();

                if ( workflowTrigger == null )
                {
                    workflowTrigger = new StepWorkflowTrigger();

                    stepProgram.StepWorkflowTriggers.Add( workflowTrigger );
                }
                else
                {
                    stateTrigger.Id = workflowTrigger.Id;
                    stateTrigger.Guid = workflowTrigger.Guid;
                }

                workflowTrigger.TriggerType = stateTrigger.TriggerType;
                workflowTrigger.WorkflowTypeId = stateTrigger.WorkflowTypeId;
                workflowTrigger.WorkflowName = stateTrigger.WorkflowName;

                workflowTrigger.StepProgramId = stepProgramId;
                workflowTrigger.StepTypeId = null;
            }

            if ( !stepProgram.IsValid )
            {
                // Controls will render the error messages
                return;
            }

            try
            {
                rockContext.SaveChanges();

                stepProgram = stepProgramService.Get( stepProgram.Id );

                if ( stepProgram == null )
                {
                    throw new Exception( "This record is no longer valid, please reload your data." );
                }

                if ( !stepProgram.IsAuthorized( Authorization.VIEW, CurrentPerson ) )
                {
                    stepProgram.AllowPerson( Authorization.VIEW, CurrentPerson, rockContext );
                }

                if ( !stepProgram.IsAuthorized( Authorization.EDIT, CurrentPerson ) )
                {
                    stepProgram.AllowPerson( Authorization.EDIT, CurrentPerson, rockContext );
                }

                if ( !stepProgram.IsAuthorized( Authorization.ADMINISTRATE, CurrentPerson ) )
                {
                    stepProgram.AllowPerson( Authorization.ADMINISTRATE, CurrentPerson, rockContext );
                }

            }
            catch ( Exception ex )
            {
                _notificationManager.ShowException( ex );
                return;
            }

            // If the save was successful, reload the page using the new record Id.
            var qryParams = new Dictionary<string, string>();
            qryParams[PageParameterKey.StepProgramId] = stepProgram.Id.ToString();

            NavigateToPage( RockPage.Guid, qryParams );
        }

        /// <summary>
        /// Shows the controls needed for edit mode.
        /// </summary>
        /// <param name="stepProgramId">The Site Program identifier.</param>
        public void ShowDetail( int stepProgramId )
        {
            this.LoadWorkflowTriggerTypesSelectionList();

            pnlDetails.Visible = false;

            StepProgram stepProgram = null;
            using ( var rockContext = new RockContext() )
            {
                if ( !stepProgramId.Equals( 0 ) )
                {
                    stepProgram = GetStepProgram( stepProgramId, rockContext );
                    pdAuditDetails.SetEntity( stepProgram, ResolveRockUrl( "~" ) );
                }

                if ( stepProgram == null )
                {
                    stepProgram = new StepProgram { Id = 0 };
                    // hide the panel drawer that show created and last modified dates
                    pdAuditDetails.Visible = false;
                }

                // Admin rights are required to edit a Step Program. Edit rights only allow adding/removing items.
                bool adminAllowed = UserCanAdministrate || stepProgram.IsAuthorized( Authorization.ADMINISTRATE, CurrentPerson );
                pnlDetails.Visible = true;
                hfStepProgramId.Value = stepProgram.Id.ToString();
                lIcon.Text = string.Format( "<i class='{0}'></i>", stepProgram.IconCssClass );
                bool readOnly = false;

                if ( !adminAllowed )
                {
                    readOnly = true;

                    _notificationManager.ShowMessageEditModeDisallowed( StepProgram.FriendlyTypeName );
                }

                rblDefaultListView.Items.Clear();
                rblDefaultListView.Items.Add( new ListItem( "Cards", StepProgram.ViewMode.Cards.ToString() ) );
                rblDefaultListView.Items.Add( new ListItem( "Grid", StepProgram.ViewMode.Grid.ToString() ) );

                if ( readOnly )
                {
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                    btnSecurity.Visible = false;
                    ShowReadonlyDetails( stepProgram );
                }
                else
                {
                    btnEdit.Visible = true;
                    btnDelete.Visible = true;
                    btnSecurity.Visible = true;

                    btnSecurity.Title = "Secure " + stepProgram.Name;
                    btnSecurity.EntityId = stepProgram.Id;

                    if ( !stepProgramId.Equals( 0 ) )
                    {
                        ShowReadonlyDetails( stepProgram );
                    }
                    else
                    {
                        ShowEditDetails( stepProgram );
                    }
                }
            }
        }

        /// <summary>
        /// Shows the edit details.
        /// </summary>
        /// <param name="stepProgram">Type of the connection.</param>
        private void ShowEditDetails( StepProgram stepProgram )
        {
            if ( stepProgram == null )
            {
                stepProgram = new StepProgram();
                stepProgram.IconCssClass = "fa fa-compress";
            }
            if ( stepProgram.Id == 0 )
            {
                lReadOnlyTitle.Text = ActionTitle.Add( StepProgram.FriendlyTypeName ).FormatAsHtmlTitle();
            }
            else
            {
                lReadOnlyTitle.Text = stepProgram.Name.FormatAsHtmlTitle();
            }

            SetEditMode( true );

            tbName.Text = stepProgram.Name;
            cbActive.Checked = stepProgram.IsActive;
            tbDescription.Text = stepProgram.Description;
            tbIconCssClass.Text = stepProgram.IconCssClass;

            cpCategory.SetValue( stepProgram.CategoryId );

            rblDefaultListView.SelectedValue = stepProgram.DefaultListView.ToString();

            WorkflowsState = stepProgram.StepWorkflowTriggers.ToList();
            StatusesState = stepProgram.StepStatuses.ToList();

            BindStepWorkflowsGrid();
            BindStepStatusesGrid();
        }

        /// <summary>
        /// Shows the readonly details.
        /// </summary>
        /// <param name="stepProgram">Type of the connection.</param>
        private void ShowReadonlyDetails( StepProgram stepProgram )
        {
            SetEditMode( false );

            hfStepProgramId.SetValue( stepProgram.Id );

            WorkflowsState = null;
            StatusesState = null;

            lReadOnlyTitle.Text = stepProgram.Name.FormatAsHtmlTitle();

            // Create the read-only description text.
            var descriptionListMain = new DescriptionList();

            descriptionListMain.Add( "Description", stepProgram.Description );

            lStepProgramDescription.Text = descriptionListMain.Html;

            // Configure Label: Inactive
            hlInactive.Visible = !stepProgram.IsActive;

            // Configure Label: Category
            if ( stepProgram.Category != null )
            {
                hlCategory.Text = stepProgram.Category.Name;
            }

            this.RefreshChart();
        }

        /// <summary>
        /// Gets the step program data model displayed by this page.
        /// </summary>
        /// <param name="stepProgramId">The connection type identifier.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <returns></returns>
        private StepProgram GetStepProgram( int? stepProgramId = null, RockContext rockContext = null )
        {
            if ( stepProgramId == null )
            {
                stepProgramId = hfStepProgramId.ValueAsInt();
            }

            string key = string.Format( "StepProgram:{0}", stepProgramId );

            var stepProgram = RockPage.GetSharedItem( key ) as StepProgram;

            if ( stepProgram == null )
            {
                rockContext = rockContext ?? new RockContext();

                stepProgram = new StepProgramService( rockContext ).Queryable()
                    .Where( c => c.Id == stepProgramId )
                    .FirstOrDefault();

                RockPage.SaveSharedItem( key, stepProgram );
            }

            return stepProgram;
        }

        private int GetActiveStepProgramId()
        {
            return hfStepProgramId.ValueAsInt();
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
                case "STEPSTATUSES":
                    dlgStepStatuses.Show();
                    break;
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
                case "STEPSTATUSES":
                    dlgStepStatuses.Hide();
                    break;
                case "STEPWORKFLOWS":
                    dlgStepWorkflow.Hide();
                    break;
            }

            hfActiveDialog.Value = string.Empty;
        }

        #endregion

        #region Step Activity Chart

        /// <summary>
        /// Initialize the chart by applying block configuration settings.
        /// </summary>
        private void InitializeChartFilter()
        {
            // Set the default Date Range from the block settings.
            var dateRangeSettings = this.GetAttributeValue( AttributeKey.SlidingDateRange );

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

        /// <summary>
        /// Refresh the chart using the current filter settings.
        /// </summary>
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
        /// Gets the data set required for the chart.
        /// </summary>
        /// <returns></returns>
        public FlotChartDataSet<TimeSeriesChartDataPoint> GetChartData( DateTime? startDate = null, DateTime? endDate = null )
        {
            var dataContext = new RockContext();

            var stepService = new StepService( dataContext );

            var programId = GetActiveStepProgramId();

            // Get all of the active Steps associated with the current program, grouped by Step Type.
            var stepsCompletedQuery = stepService.Queryable()
                .Where( x => x.StepType.StepProgramId == programId && x.StepType.IsActive && x.CompletedDateTime != null );

            if ( startDate != null )
            {
                startDate = startDate.Value.Date;

                stepsCompletedQuery = stepsCompletedQuery.Where( x => x.StartDateTime >= startDate );
            }

            if ( endDate != null )
            {
                var compareDate = endDate.Value.Date.AddDays( 1 );

                stepsCompletedQuery = stepsCompletedQuery.Where( x => x.CompletedDateTime < compareDate );
            }

            // Materialize the result so we can use a newly-constructed DateTime object in the Group function.
            var steps = stepsCompletedQuery.ToList();

            var stepTypeDataPoints = steps
                .GroupBy( x => new { Month = new DateTime( x.CompletedDateTime.Value.Year, x.CompletedDateTime.Value.Month, 1 ), SeriesName = x.StepType.Name, Order = x.StepType.Order } )
                .Select( x => new TimeSeriesChartDataPoint
                {
                    SeriesName = x.Key.SeriesName,
                    DateTime = x.Key.Month,
                    SortKey = x.Key.Order.ToString(),
                    Value = x.Count()
                } )
                .ToList();

            var dataSource = new FlotChartDataSet<TimeSeriesChartDataPoint>();

            dataSource.DataPoints.AddRange( stepTypeDataPoints );

            return dataSource;
        }

        #endregion

    }
}