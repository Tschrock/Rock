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
using System.Data.Entity;
using System.Linq;
using Rock.Data;
using Rock.Web.Cache;

namespace Rock.Model
{
    /// <summary>
    /// Service/Data access class for <see cref="Sequence"/> entity objects.
    /// </summary>
    public partial class SequenceService
    {
        /// <summary>
        /// Enroll the person into the sequence
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="personAliasId"></param>
        /// <param name="locationId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public SequenceEnrollment Enroll( SequenceCache sequence, int personAliasId, out string errorMessage, DateTime? enrollmentDate = null, int? locationId = null )
        {
            errorMessage = string.Empty;

            // Validate the parameters
            if ( sequence == null )
            {
                errorMessage = "A valid sequence is required";
                return null;
            }

            if ( personAliasId == default( int ) )
            {
                errorMessage = "A valid personAliasId is required";
                return null;
            }

            if ( !sequence.IsActive )
            {
                errorMessage = "An active sequence is required";
                return null;
            }

            if ( enrollmentDate.HasValue && enrollmentDate > RockDateTime.Now )
            {
                errorMessage = "The enrollmentDate cannot be in the future";
                return null;
            }

            var rockContext = Context as RockContext;

            if ( locationId.HasValue )
            {
                var locationService = new LocationService( rockContext );
                var location = locationService.Get( locationId.Value );

                if ( location == null )
                {
                    errorMessage = "The locationId is not valid";
                    return null;
                }
            }

            // Make sure the enrollment does not already exist for the person
            var sequenceEnrollmentService = new SequenceEnrollmentService( rockContext );
            var alreadyEnrolled = sequenceEnrollmentService.Queryable()
                .AsNoTracking()
                .Any( se => se.PersonAliasId == personAliasId && se.SequenceId == sequence.Id );

            if ( alreadyEnrolled )
            {
                errorMessage = "The enrollment already exists";
                return null;
            }

            // Add the enrollment, matching the occurence map's length with the same length array of 0/false bits
            var sequenceEnrollment = new SequenceEnrollment
            {
                SequenceId = sequence.Id,
                PersonAliasId = personAliasId,
                LocationId = locationId,
                EnrollmentDate = enrollmentDate ?? RockDateTime.Now,
                AttendanceMap = new byte[sequence.OccurenceMap?.Length ?? 0]
            };

            sequenceEnrollmentService.Add( sequenceEnrollment );
            return sequenceEnrollment;
        }

        /// <summary>
        /// Return the locations associated with the sequence structure type
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public IQueryable<Location> GetLocations( SequenceCache sequence, out string errorMessage )
        {
            errorMessage = string.Empty;

            var defaultReturnValue = new List<Location>().AsQueryable();
            var errorReturnValue = ( IQueryable<Location> ) null;

            // Validate the parameters
            if ( sequence == null )
            {
                errorMessage = "A valid sequence is required";
                return errorReturnValue;
            }

            if ( !sequence.IsActive )
            {
                errorMessage = "An active sequence is required";
                return errorReturnValue;
            }

            // If the structure information is not complete, it is not possible to get locations
            if ( !sequence.StructureEntityId.HasValue || !sequence.StructureType.HasValue )
            {
                return defaultReturnValue;
            }

            // Calculate the group locations depending on the structure type
            var groupLocationsQuery = GetGroupLocationsQuery( sequence.StructureType.Value, sequence.StructureEntityId.Value );
            return groupLocationsQuery.Select( gl => gl.Location ).Distinct();
        }

        /// <summary>
        /// Get the schedules for the sequence at the given location
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="locationId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public IQueryable<Schedule> GetLocationSchedules( SequenceCache sequence, int locationId, out string errorMessage )
        {
            errorMessage = string.Empty;

            var defaultReturnValue = new List<Schedule>().AsQueryable();
            var errorReturnValue = ( IQueryable<Schedule> ) null;

            // Validate the parameters
            if ( sequence == null )
            {
                errorMessage = "A valid sequence is required";
                return errorReturnValue;
            }

            if ( !sequence.IsActive )
            {
                errorMessage = "An active sequence is required";
                return errorReturnValue;
            }

            // If the structure information is not complete, it is not possible to get locations
            if ( !sequence.StructureEntityId.HasValue || !sequence.StructureType.HasValue )
            {
                return defaultReturnValue;
            }

            // Calculate the schedules for the group locations within the structure
            var groupLocationsQuery = GetGroupLocationsQuery( sequence.StructureType.Value, sequence.StructureEntityId.Value );
            return groupLocationsQuery.Where( gl => gl.LocationId == locationId )
                .SelectMany( gl => gl.Schedules )
                .Where( s => s.IsActive )
                .Distinct();
        }

        /// <summary>
        /// Get the group locations that are contained with the structure
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="structureEntityId"></param>
        /// <returns></returns>
        private IQueryable<GroupLocation> GetGroupLocationsQuery( SequenceStructureType structureType, int structureEntityId )
        {
            var rockContext = Context as RockContext;
            var groupLocationService = new GroupLocationService( rockContext );

            var query = groupLocationService.Queryable()
                .AsNoTracking()
                .Where( gl =>
                    gl.Group.IsActive &&
                    gl.Location.IsActive );

            switch ( structureType )
            {
                case SequenceStructureType.CheckInConfig:
                    return query.Where( gl => gl.Group.GroupType.ParentGroupTypes.Any( gt => gt.Id == structureEntityId ) );
                case SequenceStructureType.Group:
                    return query.Where( gl => gl.Group.Id == structureEntityId );
                case SequenceStructureType.GroupType:
                    return query.Where( gl => gl.Group.GroupTypeId == structureEntityId );
                case SequenceStructureType.GroupTypePurpose:
                    return query.Where( gl => gl.Group.GroupType.GroupTypePurposeValueId == structureEntityId );
                default:
                    throw new NotImplementedException( string.Format( "Getting group locations for the SequenceStructureType '{0}' is not implemented", structureType ) );
            }
        }
    }
}