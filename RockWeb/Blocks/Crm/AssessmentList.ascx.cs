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
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

namespace Rockweb.Blocks.Crm
{
    /// <summary>
    /// Lists all avalable assesments for the individual.
    /// </summary>
    [DisplayName( "Assessment List" )]
    [Category( "CRM" )]
    [Description( "Allows you to view and take any available assessments." )]

    #region Block Attributes

    [BooleanField(
        "Only Show Requested",
        "If enabled, limits the list to show only assessments that have been requested or completed.",
        true,
        order: 0 )]

    [BooleanField(
        "Hide If No Active Requests",
        "If enabled, nothing will be shown if there are not pending (waiting to be taken) assessment requests.",
        false,
        order: 1 )]

    [BooleanField(
        "Hide If No Requests",
        "If enabled, nothing will be shown where there are no requests (pending or completed).",
        false,
        order: 2 )]

    [CodeEditorField(
        "Lava Template",
        "The lava template to use to format the entire block.  <span class='tip tip-lava'></span> <span class='tip tip-html'></span>",
        CodeEditorMode.Html,
        CodeEditorTheme.Rock,
        400,
        true,
        @"<div class='panel-heading panel-default rollover-container clearfix'>
    <div class='panel-heading'>Assessments</div>
    <div class='panel-body'>
            {% for assessmenttype in AssessmentTypes %}
                {% if assessmenttype.LastRequestObject %}
                    {% if assessmenttype.LastRequestObject.Status == 'Complete' %}
                        <div class='panel panel-success'>
                            <div class='panel-heading'> {{ assessmenttype.Title }}</br>
                                Completed: {{ assessmenttype.LastRequestObject.CompletedDate | Date:'M/d/yyyy'}} </br>
                                <a href='{{ assessmenttype.AssessmentResultsPath}}'>View Results</a>
                            </div>
                        </div>
                    {% elseif assessmenttype.LastRequestObject.Status == 'Pending' %}
                        <div class='panel panel-primary'>
                            <div class='panel-heading'> {{ assessmenttype.Title }}</br>
                                Requested: {{assessmenttype.LastRequestObject.Requester}} ({{ assessmenttype.LastRequestObject.RequestedDate | Date:'M/d/yyyy'}})</br>
                                <a href='{{ assessmenttype.AssessmentPath}}'>Start Assessment</a>
                            </div>
                        </div>
                    {% endif %}   
                    {% else %}
                        <div class='panel panel-default'>
                            <div class='panel-heading'> {{ assessmenttype.Title }}</br>
                                Available</br>
                                <a href='{{ assessmenttype.AssessmentPath}}'>Start Assessment</a>
                            </div>
                        </div>
                {% endif %}
            {% endfor %}
    </div>
</div>" )]

#endregion

    public partial class AssessmentList : Rock.Web.UI.RockBlock
    {
        #region Fields

        private bool _onlyShowRequestedOrCompleted = true;
        private bool _hideIfNoActiveRequests = false;
        private bool _hideIfNoRequests = false;

        #endregion

        #region Base Control Methods

        /// <summary>
        /// On initialize
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit( EventArgs e )
        {
            // show hide requested
            _onlyShowRequestedOrCompleted = GetAttributeValue( "OnlyShowRequested" ).AsBoolean();

            // hide if no active requests
            _hideIfNoActiveRequests = GetAttributeValue( "HideIfNoActiveRequests" ).AsBoolean();

            // hide if no requests
            _hideIfNoRequests = GetAttributeValue( "HideIfNoRequests" ).AsBoolean();

            this.BlockUpdated += Block_BlockUpdated;

            base.OnInit( e );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( CurrentPerson == null )
            {
                return;
            }

            if ( !Page.IsPostBack )
            {
                BindData();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            BindData();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Bind the data and merges the Lava fields using the template.
        /// </summary>
        private void BindData()
        {
            lAssessments.Visible = true;
            
            // Gets Assessment types and assessments for each
            RockContext rockContext = new RockContext();
            AssessmentTypeService assessmentTypeService = new AssessmentTypeService( rockContext );

            var allAssessmentsOfEachType = assessmentTypeService.Queryable().AsNoTracking()
                .Where(x => x.IsActive == true )
                .Select( t => new
                    {
                        Title = t.Title,
                        AssessmentPath = t.AssessmentPath,
                        AssessmentResultsPath = t.AssessmentResultsPath,
                        RequiresRequest = t.RequiresRequest,
                        LastRequestObject = t.Assessments
                            .Where( a => a.PersonAlias.Person.Id == CurrentPersonId )
                            .OrderBy( a => a.Status ) // pending first
                            .Select( a => new
                            {
                                RequestedDate = a.RequestedDateTime,
                                CompletedDate = a.CompletedDateTime,
                                Status = a.Status,
                                Requester = a.RequesterPersonAlias.Person.NickName + " " + a.RequesterPersonAlias.Person.LastName
                            } ).OrderByDescending( x => x.CompletedDate ).FirstOrDefault()
                    }
                )
                // order by requested then by pending, completed, then by available to take
                .OrderByDescending( x => x.LastRequestObject ).ThenBy( x => x.LastRequestObject.Status ).ToList();

            // Checks current request types to use against the settings
            bool areThereAnyPendingRequests = false;
            bool areThereAnyRequests = false;

            foreach ( var item in allAssessmentsOfEachType.Where( a => a.LastRequestObject != null ) )
            {
                areThereAnyRequests = true;

                if ( item.LastRequestObject.Status == AssessmentRequestStatus.Pending )
                {
                    areThereAnyPendingRequests = true;
                }
            }
            
            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, CurrentPerson );

            // Decide if anything is going to display
            if ( ( _hideIfNoActiveRequests && !areThereAnyPendingRequests ) ||
                 ( _hideIfNoRequests && !areThereAnyRequests ) )
            {
                lAssessments.Visible = false;
            }
            else
            {
                // Show only the tests requested or completed?...
                if ( _onlyShowRequestedOrCompleted )
                {
                    var onlyRequestedOrCompleted = allAssessmentsOfEachType
                        .Where( x => x.LastRequestObject != null && x.LastRequestObject.Requester != null &&
                        ( x.LastRequestObject.Status == AssessmentRequestStatus.Pending || x.LastRequestObject.CompletedDate != null ) );

                    mergeFields.Add( "AssessmentTypes", onlyRequestedOrCompleted );
                }
                else
                {
                    // ...Otherwise show any allowed, requested or completed requests.
                    var onlyAllowedRequestedOrCompleted = allAssessmentsOfEachType
                        .Where( x => x.RequiresRequest != true ||
                            ( x.LastRequestObject != null && x.LastRequestObject.Status == AssessmentRequestStatus.Pending ) ||
                            ( x.LastRequestObject != null && x.LastRequestObject.CompletedDate != null )
                        );

                    mergeFields.Add( "AssessmentTypes", onlyAllowedRequestedOrCompleted );
                }

                lAssessments.Text = GetAttributeValue( "LavaTemplate" ).ResolveMergeFields( mergeFields, GetAttributeValue( "EnabledLavaCommands" ) );
            }
        }

        #endregion
    }
}