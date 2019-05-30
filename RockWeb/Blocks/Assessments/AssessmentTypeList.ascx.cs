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

namespace RockWeb.Blocks.Assessments
{
    [DisplayName( "Assessment Type List" )]
    [Category( "Assessments" )]
    [Description( "Shows a list of all Assessment Types." )]

    #region Block Attributes

    [LinkedPage(
        "Detail Page",
        Key = AttributeKey.DetailPage,
        Category = AttributeCategory.LinkedPages,
        Order = 1 )]

    #endregion

    public partial class AssessmentTypeList : RockBlockEntityList<AssessmentType>
    {
        #region Attribute Keys

        /// <summary>
        /// Keys to use for Block Attributes
        /// </summary>
        protected static class AttributeKey
        {
            public const string DetailPage = "DetailPage";
        }

        #endregion

        #region Attribute Categories

        /// <summary>
        /// Keys to use for Block Attribute Categories
        /// </summary>
        protected static class AttributeCategory
        {
            public const string LinkedPages = "Linked Pages";
        }

        #endregion

        #region Page Parameter Keys

        /// <summary>
        /// Keys to use for Page Parameters
        /// </summary>
        protected static class PageParameterKey
        {
            public const string AssessmentTypeId = "AssessmentTypeId";
        }

        #endregion

        #region User Preference Keys

        /// <summary>
        /// Keys to use for Filter Settings
        /// </summary>
        protected static class FilterSettingName
        {
            public const string Title = "Title";
            public const string RequiresRequest = "Requires Request";
            public const string IsActive = "Is Active";
        }

        #endregion

        #region Base Control Methods

        protected override void OnConfigureBlock()
        {
            this.ListPanelControl = pnlList;
            this.ListGridControl = gList;
            this.ListFilterControl = rFilter;
            this.ModalAlertControl = mdAlert;

            this.ShowItemAdd = true;
            this.ShowItemDelete = true;
            this.ShowItemSecurity = true;
            this.ShowItemReorder = false;
        }

        protected override string OnGetUrlQueryParameterNameEntityId()
        {
            return PageParameterKey.AssessmentTypeId;
        }

        protected override bool OnDeleteEntity( RockContext dataContext, int entityId )
        {
            var assessmentTypeService = new AssessmentTypeService( dataContext );

            var assessmentType = assessmentTypeService.Get( entityId );

            if ( assessmentType == null )
            {
                this.ShowAlert( "This item could not be found." );
                return false;
            }

            string errorMessage;

            if ( !assessmentTypeService.CanDelete( assessmentType, out errorMessage ) )
            {
                this.ShowAlert( errorMessage, ModalAlertType.Warning );
                return false;
            }

            assessmentTypeService.Delete( assessmentType );

            return true;
        }

        protected override void OnApplyFilterSettings( Dictionary<string, string> settingsKeyValueMap )
        {
            txbTitleFilter.Text = settingsKeyValueMap[FilterSettingName.Title];
            ddlRequiresRequestFilter.SetValue( settingsKeyValueMap[FilterSettingName.RequiresRequest] );
            ddlIsActiveFilter.SetValue( settingsKeyValueMap[FilterSettingName.IsActive] );
        }

        protected override Dictionary<string, string> OnStoreFilterSettings()
        {
            var settings = new Dictionary<string, string>();

            settings[FilterSettingName.Title] = txbTitleFilter.Text;
            settings[FilterSettingName.RequiresRequest] = ddlRequiresRequestFilter.SelectedValue;
            settings[FilterSettingName.IsActive] = ddlIsActiveFilter.SelectedValue;

            return settings;
        }

        protected override string OnGetFilterValueDescription( string filterSettingName )
        {
            if ( filterSettingName == FilterSettingName.Title )
            {
                return string.Format( "Contains \"{0}\"", txbTitleFilter.Text );
            }
            else if ( filterSettingName == FilterSettingName.RequiresRequest )
            {
                return ddlRequiresRequestFilter.SelectedValue;
            }
            else if ( filterSettingName == FilterSettingName.IsActive )
            {
                return ddlIsActiveFilter.SelectedValue;
            }

            return string.Empty;
        }

        protected override object OnGetListDataSource( RockContext dataContext, Dictionary<string, string> filterSettingsKeyValueMap, SortProperty sortProperty )
        {
            var assessmentService = new AssessmentTypeService( dataContext );

            var assessmentTypesQry = assessmentService.Queryable();

            // Filter by: Title
            var name = filterSettingsKeyValueMap[FilterSettingName.Title].ToStringSafe();

            if ( !string.IsNullOrWhiteSpace( name ) )
            {
                assessmentTypesQry = assessmentTypesQry.Where( a => a.Title.Contains( name ) );
            }

            // Filter by: Requires Request
            var requiresRequest = rFilter.GetUserPreference( FilterSettingName.RequiresRequest ).AsBooleanOrNull();

            if ( requiresRequest.HasValue )
            {
                assessmentTypesQry = assessmentTypesQry.Where( a => a.RequiresRequest == requiresRequest.Value );
            }

            // Filter by: Is Active
            var isActive = rFilter.GetUserPreference( FilterSettingName.IsActive ).AsBooleanOrNull();

            if ( isActive.HasValue )
            {
                assessmentTypesQry = assessmentTypesQry.Where( a => a.IsActive == isActive.Value );
            }

            // Apply Sorting.
            if ( sortProperty != null )
            {
                assessmentTypesQry = assessmentTypesQry.Sort( sortProperty );
            }
            else
            {
                assessmentTypesQry = assessmentTypesQry.OrderBy( b => b.Title );
            }

            // Retrieve the Assessment Type data models and create corresponding view models to display in the grid.
            var assessmentTypes = assessmentTypesQry
                .Select( x => new AssessmentTypeListItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    RequiresRequest = x.RequiresRequest,
                    IsActive = x.IsActive,
                    IsSystem = x.IsSystem
                } )
                .ToList();

            return assessmentTypes;
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Represents an entry in the list of items shown by this block.
        /// </summary>
        public class AssessmentTypeListItemViewModel
        {
            public int Id { get; set; }
            public bool IsSystem { get; set; }
            public bool IsActive { get; set; }

            public string Title { get; set; }
            public bool RequiresRequest { get; set; }
        }

        #endregion
    }
}
