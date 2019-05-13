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
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

namespace RockWeb.Blocks.Steps
{
    [DisplayName( "Step Type List" )]
    [Category( "Steps" )]
    [Description( "Shows a list of all step types for a program." )]

    #region Block Attributes

    [StepProgramField(
        "Step Program",
        Key = AttributeKey.StepProgram,
        Description = "Display Step Types from a specified program. If none selected, the block will display the program from the current context.",
        IsRequired = false,
        Order = 1 )]
    [LinkedPage(
        "Detail Page",
        Key = AttributeKey.DetailPage,
        Category = AttributeCategory.LinkedPages,
        Order = 2 )]
    [LinkedPage(
        "Bulk Entry",
        Key = AttributeKey.BulkEntryPage,
        Description = "Linked page that allows for bulk entry of steps for a step type.",
        Category = AttributeCategory.LinkedPages,
        Order = 3 )]

    #endregion Block Attributes

    public partial class StepTypeList : RockBlock, ISecondaryBlock
    {
        #region Attribute Keys

        /// <summary>
        /// Keys to use for Block Attributes
        /// </summary>
        protected static class AttributeKey
        {
            public const string StepProgram = "Programs";
            public const string DetailPage = "DetailPage";
            public const string BulkEntryPage = "BulkEntryPage";
        }

        #endregion Attribute Keys

        #region Attribute Categories

        /// <summary>
        /// Keys to use for Block Attribute Categories
        /// </summary>
        protected static class AttributeCategory
        {
            public const string LinkedPages = "Linked Pages";
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

        #region User Preference Keys

        /// <summary>
        /// Keys to use for Page Parameters
        /// </summary>
        protected static class FilterSettingName
        {
            public const string Name = "Name";
            public const string AllowMultiple = "Allow Multiple";
            public const string SpansTime = "Spans Time";
        }

        #endregion Page Parameter Keys

        #region Base Control Methods

        private StepProgram _program = null;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            this.InitializeStepProgram();

            // Initialize Filter
            if ( !Page.IsPostBack )
            {
                BindFilter();
            }

            rFilter.ApplyFilterClick += rFilter_ApplyFilterClick;
            rFilter.DisplayFilterValue += rFilter_DisplayFilterValue;

            // Initialize Grid
            gStepType.DataKeyNames = new string[] { "Id" };
            gStepType.Actions.AddClick += gStepType_Add;
            gStepType.GridReorder += gStepType_GridReorder;
            gStepType.GridRebind += gStepType_GridRebind;
            gStepType.RowItemText = "Step Type";

            // Initialize Grid: Secured actions
            bool canAddEditDelete = IsUserAuthorized( Authorization.EDIT );
            bool canAdministrate = IsUserAuthorized( Authorization.ADMINISTRATE );

            gStepType.Actions.ShowAdd = canAddEditDelete;
            gStepType.IsDeleteEnabled = canAddEditDelete;

            if ( canAddEditDelete )
            {
                gStepType.RowSelected += gStepType_Edit;
            }

            var securityField = gStepType.ColumnsOfType<SecurityField>().FirstOrDefault();

            securityField.EntityTypeId = EntityTypeCache.Get( typeof( Rock.Model.StepType ) ).Id;

            securityField.Visible = canAdministrate;

            // Initialize Grid: Apply block configuration settings.
            this.ConfigureGridFromBlockSettings();

            // Set up Block Settings change notification.
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upStepTypeList );
        }

        /// <summary>
        /// Configure grid elements that are affected by the current block settings.
        /// </summary>
        private void ConfigureGridFromBlockSettings()
        {
            bool canAddEditDelete = IsUserAuthorized( Authorization.EDIT );

            // Set availability of Bulk Entry button.
            bool allowBulkEntry = canAddEditDelete && GetAttributeValue( AttributeKey.BulkEntryPage ).IsNotNullOrWhiteSpace();

            var bulkEntryColumn = gStepType.ColumnsOfType<LinkButtonField>().FirstOrDefault();

            bulkEntryColumn.Visible = allowBulkEntry;
        }

        /// <summary>
        /// Initialize the parent Step Program of the Steps shown in the block.
        /// </summary>
        private void InitializeStepProgram()
        {
            _program = null;

            // Try to load the Step Program from the cache.
            var programGuid = GetAttributeValue( AttributeKey.StepProgram ).AsGuid();

            int programId = 0;
            string sharedItemKey;

            // If a Step Program is specified in the block settings use it, otherwise use the PageParameters.
            if ( programGuid != Guid.Empty )
            {
                sharedItemKey = string.Format( "{0}:{1}", PageParameterKey.StepProgramId, programGuid );
            }
            else
            {
                programId = PageParameter( PageParameterKey.StepProgramId ).AsInteger();

                sharedItemKey = string.Format( "{0}:{1}", PageParameterKey.StepProgramId, programId );
            }

            if ( !string.IsNullOrEmpty( sharedItemKey ) )
            {
                _program = RockPage.GetSharedItem( sharedItemKey ) as StepProgram;
            }

            // Retrieve the program from the data store and cache for subsequent use.
            if ( _program == null )
            {
                var stepProgramService = new StepProgramService( new RockContext() );

                if ( programGuid != Guid.Empty )
                {
                    _program = stepProgramService.Queryable().Where( g => g.Guid == programGuid ).FirstOrDefault();
                }
                else if ( programId != 0 )
                {
                    _program = stepProgramService.Queryable().Where( g => g.Id == programId ).FirstOrDefault();
                }

                if ( _program != null )
                {
                    RockPage.SaveSharedItem( sharedItemKey, _program );
                }
            }

            // Verify the Step Program is valid.
            if ( _program == null )
            {
                this.SetConfigurationStatusMessage( "There is no Step Program available in this context." );
                return;
            }

            // Check for View permissions.
            if ( !_program.IsAuthorized( Authorization.VIEW, CurrentPerson ) )
            {
                this.SetConfigurationStatusMessage( "Sorry, you are not authorized to view this content." );
                return;
            }

            rFilter.UserPreferenceKeyPrefix = string.Format( "{0}-", _program.Id );
            rFilter.ApplyFilterClick += rFilter_ApplyFilterClick;

            gStepType.DataKeyNames = new string[] { "Id" };
            gStepType.Actions.AddClick += gStepType_Add;
            gStepType.GridRebind += gStepType_GridRebind;
            gStepType.RowItemText = "Step Type";
            gStepType.ExportFilename = _program.Name;
            gStepType.ExportSource = ExcelExportSource.DataSource;

            // Verify authorization to edit either the block or the step program.
            bool canEditBlock = IsUserAuthorized( Authorization.EDIT ) || _program.IsAuthorized( Authorization.EDIT, this.CurrentPerson );

            gStepType.Actions.ShowAdd = canEditBlock;
            gStepType.IsDeleteEnabled = canEditBlock;

            // Reset the configuration status message.
            SetConfigurationStatusMessage( null );
        }

        private void SetConfigurationStatusMessage( string message )
        {
            if ( string.IsNullOrWhiteSpace( message ) )
            {
                nbStepProgramWarning.Visible = false;
                pnlList.Visible = true;
            }
            else
            {
                nbStepProgramWarning.Text = message;
                nbStepProgramWarning.Visible = true;
                pnlList.Visible = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
                BindGrid();
            }

            base.OnLoad( e );
        }

        #endregion Base Control Methods

        #region Control Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            this.InitializeStepProgram();

            this.ConfigureGridFromBlockSettings();

            BindGrid();
        }

        #endregion Control Events

        #region Filter Events

        /// <summary>
        /// Handles the ApplyFilterClick event of the rFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void rFilter_ApplyFilterClick( object sender, EventArgs e )
        {
            rFilter.SaveUserPreference( FilterSettingName.Name, txbNameFilter.Text );
            rFilter.SaveUserPreference( FilterSettingName.AllowMultiple, ddlAllowMultipleFilter.SelectedValue );
            rFilter.SaveUserPreference( FilterSettingName.SpansTime, ddlHasDurationFilter.SelectedValue );

            BindGrid();
        }

        /// <summary>
        /// Handles the ClearFilterClick event of the rFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void rFilter_ClearFilterClick( object sender, EventArgs e )
        {
            rFilter.DeleteUserPreferences();

            BindFilter();
        }

        /// <summary>
        /// ts the filter display filter value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        protected void rFilter_DisplayFilterValue( object sender, GridFilter.DisplayFilterValueArgs e )
        {
            rFilter.SaveUserPreference( FilterSettingName.Name, txbNameFilter.Text );
            rFilter.SaveUserPreference( FilterSettingName.AllowMultiple, ddlAllowMultipleFilter.SelectedValue );
            rFilter.SaveUserPreference( FilterSettingName.SpansTime, ddlHasDurationFilter.SelectedValue );

            if ( e.Key == FilterSettingName.Name )
            {
                e.Value = string.Format( "Contains \"{0}\"", txbNameFilter.Text );
            }
            else if ( e.Key == FilterSettingName.AllowMultiple )
            {
                e.Value = ddlAllowMultipleFilter.SelectedValue;
            }
            else if ( e.Key == FilterSettingName.SpansTime )
            {
                e.Value = ddlHasDurationFilter.SelectedValue;
            }
            else
            {
                e.Value = string.Empty;
            }
        }

        #endregion

        #region Grid Events

        /// <summary>
        /// Handles the Add event of the gStepType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gStepType_Add( object sender, EventArgs e )
        {
            NavigateToLinkedPage( AttributeKey.DetailPage, "StepTypeId", 0 );
        }

        /// <summary>
        /// Handles the Edit event of the gStepType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gStepType_Edit( object sender, RowEventArgs e )
        {
            NavigateToLinkedPage( AttributeKey.DetailPage, "StepTypeId", e.RowKeyId );
        }

        /// <summary>
        /// Handles the Edit event of the gStepType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gStepType_BulkEntry( object sender, RowEventArgs e )
        {
            NavigateToLinkedPage( AttributeKey.BulkEntryPage, "StepTypeId", e.RowKeyId );
        }

        /// <summary>
        /// Handles the Delete event of the gStepType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gStepType_Delete( object sender, RowEventArgs e )
        {
            var rockContext = new RockContext();

            var stepTypeService = new StepTypeService( rockContext );

            var stepProgram = stepTypeService.Get( e.RowKeyId );

            if ( stepProgram == null )
            {
                mdGridWarning.Show( "This item could not be found.", ModalAlertType.Information );
                return;
            }

            string errorMessage;

            if ( !stepTypeService.CanDelete( stepProgram, out errorMessage ) )
            {
                mdGridWarning.Show( errorMessage, ModalAlertType.Information );
                return;
            }

            stepTypeService.Delete( stepProgram );

            rockContext.SaveChanges();

            BindGrid();
        }

        /// <summary>
        /// Handles the GridReorder event of the gStepType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridReorderEventArgs" /> instance containing the event data.</param>
        void gStepType_GridReorder( object sender, GridReorderEventArgs e )
        {
            var rockContext = new RockContext();
            var service = new StepTypeService( rockContext );
            var stepPrograms = service.Queryable().OrderBy( b => b.Order );

            service.Reorder( stepPrograms.ToList(), e.OldIndex, e.NewIndex );
            rockContext.SaveChanges();

            BindGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gStepType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void gStepType_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        #endregion Grid Events

        #region Internal Methods

        /// <summary>
        /// Binds the filter.
        /// </summary>
        private void BindFilter()
        {
            txbNameFilter.Text = rFilter.GetUserPreference( FilterSettingName.Name );
            ddlAllowMultipleFilter.SetValue( rFilter.GetUserPreference( FilterSettingName.AllowMultiple ) );
            ddlHasDurationFilter.SetValue( rFilter.GetUserPreference( FilterSettingName.SpansTime ) );
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            if ( _program == null )
            {
                return;
            }

            var stepTypesQry = new StepTypeService( new RockContext() )
                .Queryable();

            // Filter by: Step Programs
            stepTypesQry = stepTypesQry.Where( x => x.StepProgramId == _program.Id );

            // Filter by: Name
            var name = rFilter.GetUserPreference( FilterSettingName.Name ).ToStringSafe();

            if ( !string.IsNullOrWhiteSpace( name ) )
            {
                stepTypesQry = stepTypesQry.Where( a => a.Name.Contains( name ) );
            }

            // Filter by: Allow Multiple
            var allowMultiple = rFilter.GetUserPreference( FilterSettingName.AllowMultiple ).AsBooleanOrNull();

            if ( allowMultiple.HasValue )
            {
                stepTypesQry = stepTypesQry.Where( a => a.AllowMultiple == allowMultiple.Value );
            }

            // Filter by: Has Duration
            var hasDuration = rFilter.GetUserPreference( FilterSettingName.SpansTime ).AsBooleanOrNull();

            if ( hasDuration.HasValue )
            {
                stepTypesQry = stepTypesQry.Where( a => a.HasEndDate == hasDuration.Value );
            }

            // Sort by: Order
            stepTypesQry = stepTypesQry.OrderBy( b => b.Order );

            // Retrieve the Step Type data models and create corresponding view models to display in the grid.
            var stepTypes = stepTypesQry.ToList();

            gStepType.DataSource = stepTypes.Select( x => StepTypeListItemViewModel.NewFromDataModel( x ) ).ToList();

            gStepType.DataBind();
        }

        #endregion Internal Methods

        #region Helper Classes

        /// <summary>
        /// Represents an entry in the list of Step Programs shown on this page.
        /// </summary>
        public class StepTypeListItemViewModel
        {
            public StepType Program { get; set; }

            public int Id { get; set; }
            public string Name { get; set; }
            public string IconCssClass { get; set; }
            public bool AllowMultipleInstances { get; set; }
            public bool HasDuration { get; set; }
            public int StepCompletedCount { get; set; }

            public static StepTypeListItemViewModel NewFromDataModel( StepType stepProgram )
            {
                var newItem = new StepTypeListItemViewModel();

                newItem.Id = stepProgram.Id;
                newItem.Name = stepProgram.Name;
                newItem.IconCssClass = stepProgram.IconCssClass;

                newItem.AllowMultipleInstances = stepProgram.AllowMultiple;
                newItem.HasDuration = stepProgram.HasEndDate;

                // TODO: Calculate total steps taken.
                newItem.StepCompletedCount = 777;

                return newItem;
            }
        }

        #endregion Helper Classes

        #region ISecondaryBlock

        /// <summary>
        /// Sets the visible.
        /// </summary>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        public void SetVisible( bool visible )
        {
            pnlContent.Visible = visible;
        }

        #endregion
    }
}