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

namespace Rock.Attribute
{
    /// <summary>
    /// Field Attribute to select 0 or more Assessments stored as a comma-delimited list of Assessments.Guid
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
    public class AssessmentTypesFieldAttribute : SelectFieldAttribute
    {
        private const string INCLUDE_INACTIVE_KEY = "includeInactive";

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentTypesFieldAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <param name="defaultAssessmentGuids">The default Assessment guids.</param>
        /// <param name="category">The category.</param>
        /// <param name="order">The order.</param>
        /// <param name="key">The key.</param>
        public AssessmentTypesFieldAttribute( string name = "Assessments", string description = "", bool required = true, string defaultAssessmentGuids = "", string category = "", int order = 0, string key = null )
            : base( name, description, required, defaultAssessmentGuids, category, order, key, typeof( Rock.Field.Types.AssessmentTypesFieldType ).FullName )
        {
            FieldConfigurationValues.Add( INCLUDE_INACTIVE_KEY, new Field.ConfigurationValue( "False" ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentTypesFieldAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <param name="defaultAssessmentGuids">The default Assessment guids.</param>
        /// <param name="includeInactive">if set to <c>true</c> [include inactive].</param>
        /// <param name="category">The category.</param>
        /// <param name="order">The order.</param>
        /// <param name="key">The key.</param>
        public AssessmentTypesFieldAttribute( string name = "Assessment", string description = "", bool required = true, string defaultAssessmentGuids = "", bool includeInactive = false, string category = "", int order = 0, string key = null )
            : base( name, description, required, defaultAssessmentGuids, category, order, key, typeof( Rock.Field.Types.AssessmentTypesFieldType ).FullName )
        {
            FieldConfigurationValues.Add( INCLUDE_INACTIVE_KEY, new Field.ConfigurationValue( includeInactive.ToString() ) );
        }
    }
}