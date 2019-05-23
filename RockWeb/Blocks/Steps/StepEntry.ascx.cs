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
using System.Web.UI.WebControls;
using Rock;
using Rock.Attribute;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI;

namespace RockWeb.Blocks.Steps
{
    [DisplayName( "Step Entry" )]
    [Category( "Steps" )]
    [Description( "Displays a form to add or edit a step." )]

    #region Block Attributes

    [IntegerField(
        name: "Step Type Id",
        description: "The step type to use to add a new step. Leave blank to use the query string: StepTypeId. The type of the step, if step id is specified, overrides this setting.",
        required: false,
        order: 1,
        key: AttributeKeys.StepType )]

    [LinkedPage(
        name: "Success Page",
        description: "The page to navigate to once the add or edit has completed. Leave blank to navigate to the parent page.",
        required: false,
        order: 2,
        key: AttributeKeys.SuccessPage )]

    #endregion Block Attributes

    public partial class StepEntry : RockBlock
    {
        #region Keys

        /// <summary>
        /// Keys for block attributes
        /// </summary>
        private static class AttributeKeys
        {
            public const string StepType = "StepType";
            public const string SuccessPage = "SuccessPage";
        }

        /// <summary>
        /// Keys for the page parameters
        /// </summary>
        private static class ParameterKeys
        {
            public const string StepTypeId = "StepTypeId";
            public const string StepId = "StepId";
            public const string PersonId = "PersonId";
        }

        #endregion Keys

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !IsPostBack && !ValidateRequiredModels() )
            {
                return;
            }

            if ( !IsPostBack )
            {
                ShowEditDetails();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            var rockContext = GetRockContext();
            var service = new StepService( rockContext );
            var step = GetStep();
            var stepType = GetStepType();
            var person = GetPerson();
            var isPersonSelectable = IsPersonSelectable();

            // If the person is allowed to be selected and the person is missing, query for it
            if ( isPersonSelectable && ppPerson.PersonId.HasValue && person == null )
            {
                var personService = new PersonService( rockContext );
                person = personService.Get( ppPerson.PersonId.Value );
            }

            // Person is the only required field for the step
            if ( person == null )
            {
                ShowError( "The person is required to save a step record." );
            }

            // If the step is null, then the aim is to create a new step
            if ( step == null )
            {
                step = new Step
                {
                    StepTypeId = stepType.Id,
                    PersonAliasId = person.PrimaryAliasId.Value
                };
                service.Add( step );
            }

            // Update the step properties. Person cannot be changed (only set when the step is added)
            step.StartDateTime = rdpStartDate.SelectedDate;
            step.EndDateTime = stepType.HasEndDate ? rdpEndDate.SelectedDate : null;
            step.StepStatusId = rsspStatus.SelectedValueAsId();

            // Update the completed date time, which is based on the start, end, and status
            if ( !step.StepStatusId.HasValue )
            {
                step.CompletedDateTime = null;
            }
            else
            {
                var stepStatusService = new StepStatusService( rockContext );
                var stepStatus = stepStatusService.Get( step.StepStatusId.Value );

                if ( stepStatus == null || !stepStatus.IsCompleteStatus )
                {
                    step.CompletedDateTime = null;
                }
                else
                {
                    step.CompletedDateTime = step.EndDateTime ?? step.StartDateTime;
                }
            }

            // Save the step record
            rockContext.SaveChanges();

            // Save the step attributes from the attribute controls
            step.LoadAttributes( rockContext );
            avcAttributes.GetEditValues( step );
            step.SaveAttributeValues( rockContext );

            GoToSuccessPage( step.Id );
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnCancel_Click( object sender, EventArgs e )
        {
            GoToSuccessPage( null );
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Validate that the models required to add or to edit are present
        /// </summary>
        private bool ValidateRequiredModels()
        {
            var step = GetStep();

            if ( step != null )
            {
                // Edit only requires the step
                return true;
            }

            // Add requires step type            
            var stepType = GetStepType();

            if ( stepType == null )
            {
                ShowError( "A step type is required to add a step" );
                return false;
            }

            return true;
        }

        /// <summary>
        /// Display an error in the browser window
        /// </summary>
        /// <param name="message"></param>
        private void ShowError( string message )
        {
            nbMessage.Text = message;
            nbMessage.Visible = true;
        }

        /// <summary>
        /// Shows the edit details.
        /// </summary>
        private void ShowEditDetails()
        {
            var stepType = GetStepType();

            if ( stepType == null )
            {
                return;
            }

            rsspStatus.StepProgramId = stepType.StepProgramId;

            lStepTypeTitle.Text = string.Format( "{0} {1}",
                stepType.IconCssClass.IsNotNullOrWhiteSpace() ?
                    string.Format( @"<i class=""{0}""></i>", stepType.IconCssClass ) :
                    string.Empty,
                stepType.Name );

            rdpEndDate.Visible = stepType.HasEndDate;
            rdpStartDate.Label = stepType.HasEndDate ? "Start Date" : "Date";

            var step = GetStep();
            if ( step != null )
            {
                rdpStartDate.SelectedDate = step.StartDateTime;
                rdpEndDate.SelectedDate = step.EndDateTime;
                rsspStatus.SelectedValue = step.StepStatusId.ToStringSafe();
            }

            BuildDynamicControls();
            InitializePersonPicker();
        }

        /// <summary>
        /// Build the dynamic controls based on the attributes
        /// </summary>
        private void BuildDynamicControls()
        {
            var stepEntityTypeId = EntityTypeCache.GetId( typeof( Step ) );
            var excludedAttributes = AttributeCache.All()
                .Where( a => a.EntityTypeId == stepEntityTypeId )
                .Where( a => a.Key == "Order" || a.Key == "Active" );
            avcAttributes.ExcludedAttributes = excludedAttributes.ToArray();

            var stepType = GetStepType();
            var step = GetStep() ?? new Step { StepTypeId = stepType.Id };

            step.LoadAttributes();
            avcAttributes.AddEditControls( step );
        }

        #endregion

        #region Attribute Helpers

        /// <summary>
        /// Redirect to the success page, or if it is not set, then go to the parent page
        /// </summary>
        private void GoToSuccessPage( int? newStepId )
        {
            var page = GetAttributeValue( AttributeKeys.SuccessPage );
            var parameters = new Dictionary<string, string>();
            var person = GetPerson();
            var step = GetStep();
            var stepType = GetStepType();

            if ( person != null )
            {
                parameters.Add( ParameterKeys.PersonId, person.Id.ToString() );
            }

            if ( newStepId.HasValue && newStepId > 0 )
            {
                parameters.Add( ParameterKeys.StepId, newStepId.Value.ToString() );
            }
            else if ( step != null )
            {
                parameters.Add( ParameterKeys.StepId, step.Id.ToString() );
            }

            if ( stepType != null )
            {
                parameters.Add( ParameterKeys.StepTypeId, stepType.Id.ToString() );
            }

            if ( page.IsNullOrWhiteSpace() )
            {
                NavigateToParentPage( parameters );
            }
            else
            {
                NavigateToLinkedPage( AttributeKeys.SuccessPage, parameters );
            }
        }

        #endregion Attribute Helpers

        #region Model Getters

        /// <summary>
        /// Get the step model
        /// </summary>
        /// <returns></returns>
        private Step GetStep()
        {
            if ( _step == null )
            {
                var stepId = PageParameter( ParameterKeys.StepId ).AsIntegerOrNull();

                if ( stepId.HasValue )
                {
                    var rockContext = GetRockContext();
                    var service = new StepService( rockContext );
                    _step = service.Get( stepId.Value );
                }
            }

            return _step;
        }
        private Step _step = null;

        /// <summary>
        /// Get the step type model
        /// </summary>
        /// <returns></returns>
        private StepType GetStepType()
        {
            if ( _stepType == null )
            {
                var step = GetStep();

                if ( step != null )
                {
                    _stepType = step.StepType;
                }
                else
                {
                    var stepTypeId = GetAttributeValue( AttributeKeys.StepType ).AsIntegerOrNull() ??
                        PageParameter( ParameterKeys.StepTypeId ).AsIntegerOrNull();

                    if ( stepTypeId.HasValue )
                    {
                        var rockContext = GetRockContext();
                        var service = new StepTypeService( rockContext );

                        _stepType = service.Queryable()
                            .AsNoTracking()
                            .FirstOrDefault( st => st.Id == stepTypeId.Value && st.IsActive );
                    }
                }
            }

            return _stepType;
        }
        private StepType _stepType = null;

        /// <summary>
        /// Get the person. 1st source is the step, 2nd is the query param, 3rd is the context
        /// </summary>
        /// <returns></returns>
        private Person GetPerson()
        {
            if ( _person == null )
            {
                var step = GetStep();

                if ( step != null && step.PersonAlias != null && step.PersonAlias.Person != null )
                {
                    _person = step.PersonAlias.Person;
                }
                else
                {
                    var personId = PageParameter( ParameterKeys.PersonId ).AsIntegerOrNull();

                    if ( personId.HasValue )
                    {
                        var rockContext = GetRockContext();
                        var service = new PersonService( rockContext );
                        _person = service.Get( personId.Value );
                    }
                    else
                    {
                        _person = ContextEntity() as Person;
                    }
                }
            }

            return _person;
        }
        private Person _person = null;

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
        private RockContext _rockContext = null;

        #endregion Model Getters

        #region Control Helpers

        /// <summary>
        /// Initialize the person picker. Show or hide it based on if the person is selectable
        /// </summary>
        private void InitializePersonPicker()
        {
            var isSelectable = IsPersonSelectable();
            ppPerson.Visible = isSelectable;
            ppPerson.Required = isSelectable;
        }

        /// <summary>
        /// Returns true if this block is adding a new step and the Person is not set by context or page parameter
        /// </summary>
        /// <returns></returns>
        private bool IsPersonSelectable()
        {
            var person = GetPerson();
            var step = GetStep();
            return step == null && person == null;
        }

        #endregion Control Helpers
    }
}