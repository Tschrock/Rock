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
using System.Collections;
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
        private const byte DaysPerWeek = 7;
        private const byte BitsPerByte = 8;

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
            return groupLocationsQuery.Select( gl => gl.Location )
                .DistinctBy( l => l.Id )
                .AsQueryable();
            ;
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
                .DistinctBy( s => s.Id )
                .AsQueryable();
        }

        /// <summary>
        /// Calculate sequence enrollment data and the streaks it represents
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="personAliasId"></param>
        /// <param name="startDate">Defaults to the sequence start date</param>
        /// <param name="endDate">Defaults to now</param>
        /// <param name="createObjectArray">Defaults to false. This may be a costly operation if enabled.</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public SequenceEnrollmentData GetSequenceEnrollmentData( SequenceCache sequence, int personAliasId, out string errorMessage,
            DateTime? startDate = null, DateTime? endDate = null, bool createObjectArray = false )
        {
            errorMessage = string.Empty;
            var now = RockDateTime.Now;

            // Validate the sequence
            if ( sequence == null )
            {
                errorMessage = "A valid sequence is required";
                return null;
            }

            if ( !sequence.IsActive )
            {
                errorMessage = "An active sequence is required";
                return null;
            }

            // Apply default values to parameters
            if ( !startDate.HasValue )
            {
                startDate = sequence.StartDate;
            }

            if ( !endDate.HasValue )
            {
                endDate = now.Date;
            }

            // Adjust the start and stop dates based on the selected frequency
            if ( sequence.OccurenceFrequency == SequenceOccurenceFrequency.Weekly )
            {
                startDate = startDate.Value.SundayDate();
                endDate = endDate.Value.SundayDate();
            }

            // Validate the parameters
            if ( startDate > now )
            {
                errorMessage = "StartDate cannot be in the future";
                return null;
            }

            if ( endDate > now )
            {
                errorMessage = "EndDate cannot be in the future";
                return null;
            }

            if ( startDate >= endDate )
            {
                errorMessage = "EndDate must be after the StartDate";
                return null;
            }

            // Calculate the difference in start dates from the parameter to what these maps are based upon
            var startDateShiftRightUnits = GetFrequencyUnitDifference( sequence.StartDate, startDate.Value, sequence.OccurenceFrequency, false );

            // Calculate the number of frequency units that the results are based upon (inclusive)
            var numberOfFrequencyUnits = GetFrequencyUnitDifference( startDate.Value, endDate.Value, sequence.OccurenceFrequency, true );

            // Conform the sequence occurrences to the date range
            var occurrenceMap = ConformMapToDateRange( sequence.OccurenceMap, startDateShiftRightUnits, numberOfFrequencyUnits );

            // Get the enrollment if it exists
            var rockContext = Context as RockContext;
            var sequenceEnrollmentService = new SequenceEnrollmentService( rockContext );
            var sequenceEnrollment = sequenceEnrollmentService.GetBySequenceAndPersonAlias( sequence.Id, personAliasId );
            var attendanceMap = ConformMapToDateRange( sequenceEnrollment?.AttendanceMap, startDateShiftRightUnits, numberOfFrequencyUnits );
            var locationId = sequenceEnrollment?.LocationId;

            // Calculate the aggregate exclusion map
            var exclusionMaps = sequence.SequenceOccurrenceExclusions
                .Where( soe => soe.LocationId == locationId )
                .Select( soe => ConformMapToDateRange( soe.ExclusionMap, startDateShiftRightUnits, numberOfFrequencyUnits ) )
                .ToList();

            var aggregateExclusionMap = new BitArray( numberOfFrequencyUnits, false );
            foreach ( var exclusionMap in exclusionMaps )
            {
                aggregateExclusionMap.Or( exclusionMap );
            }

            // Create the return object
            var sequenceEnrollmentData = new SequenceEnrollmentData
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                OccurenceMap = GetByteArray( occurrenceMap ),
                ExclusionMap = GetByteArray( aggregateExclusionMap ),
                AttendanceMap = GetByteArray( attendanceMap ),
                OccurenceFrequency = sequence.OccurenceFrequency
            };

            // Create the object array if requested
            if ( createObjectArray )
            {
                var objectArray = new List<SequenceEnrollmentData.SequenceEnrollmentDataByFrequency>();

                for (var i = 0; i < numberOfFrequencyUnits; i++)
                {
                    var daysAfterStart = i * ( sequence.OccurenceFrequency == SequenceOccurenceFrequency.Daily ? 1 : DaysPerWeek );
                    var date = startDate.Value.AddDays( daysAfterStart );

                    objectArray.Add( new SequenceEnrollmentData.SequenceEnrollmentDataByFrequency {
                        DateTime = sequence.OccurenceFrequency == SequenceOccurenceFrequency.Daily ? date.Date : date.SundayDate().Date,
                        HasAttendance = attendanceMap[i],
                        HasExclusion = aggregateExclusionMap[i],
                        HasOccurrence = occurrenceMap[i]
                    } );
                }

                sequenceEnrollmentData.ByFrequency = objectArray;
            }

            return sequenceEnrollmentData;
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

        /// <summary>
        /// Get the number of frequency units (days or weeks) between the two dates
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="frequencyUnit"></param>
        /// <returns></returns>
        private int GetFrequencyUnitDifference( DateTime startDate, DateTime endDate, SequenceOccurenceFrequency frequencyUnit, bool isInclusive )
        {
            // Calculate the absolute value (Duration()) difference in days
            var numberOfDays = endDate.Date.Subtract( startDate.Date ).Days;

            // Adjust to be inclusive if needed
            if ( isInclusive && numberOfDays >= 0 )
            {
                numberOfDays += 1;
            }
            else if ( isInclusive )
            {
                numberOfDays -= 1;
            }

            // Convert from days to the frequency units
            var numberOfFrequencyUnits = frequencyUnit == SequenceOccurenceFrequency.Daily ?
                numberOfDays :
                numberOfDays / DaysPerWeek;

            return numberOfFrequencyUnits;
        }

        #region Bit Manipulation


        /// <summary>
        /// Copy the bit array into a byte array
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public byte[] GetByteArray( BitArray map )
        {
            var byteArray = new byte[( map.Length - 1 ) / 8 + 1];
            map.CopyTo( byteArray, 0 );
            return byteArray;
        }

        /// <summary>
        /// Adjust the map to match some other starting point and length
        /// </summary>
        /// <param name="byteArrayMap"></param>
        /// <param name="startShiftToFutureCount">A positive value means the new start date is after the original (shift the start
        /// date toward the future)</param>
        /// <param name="newLength"></param>
        /// <returns></returns>
        private BitArray ConformMapToDateRange( byte[] byteArrayMap, int startShiftToFutureCount, int newLength )
        {
            if ( byteArrayMap == null || byteArrayMap.Length == 0 )
            {
                return new BitArray( newLength, false );
            }

            var map = new BitArray( byteArrayMap );

            // This should happen frequently if the sequence start date is used and the end date is left as today so we
            // can skip all the other calculations for speed
            if ( startShiftToFutureCount == 0 && newLength == map.Length )
            {
                return map;
            }

            // Conform the beginning (left-side) of the map
            if ( startShiftToFutureCount > 0 )
            {
                // If date moved right, then we need to remove the bits that are before the new start date
                map = RemoveFromLeft( map, startShiftToFutureCount );
            }            
            else if ( startShiftToFutureCount < 0 )
            {
                // If date moved left, then we need to add the bits that are before the original start date
                map = PadLeft( map, 0 - startShiftToFutureCount );
            }

            // Conform the length by adjusting the end (right-side) of the map
            var endShiftToFutureCount = newLength - map.Length;

            if ( endShiftToFutureCount > 0)
            {
                // If the ending needs to shift into the future, then add bits to the end
                map = PadRight( map, endShiftToFutureCount );
            }
            else if (endShiftToFutureCount < 0)
            {
                // If the ending needs to shift into the past, then remove bits from the end
                map = RemoveFromRight( map, 0 - endShiftToFutureCount );
            }

            return map;
        }

        /// <summary>
        /// Adds 0's to the end or right side of the map
        /// </summary>
        /// <param name="map"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private BitArray PadRight( BitArray map, int count )
        {
            if (count == 0)
            {
                return map;
            }

            var newLength = map.Length + count;
            var newMap = new BitArray( newLength );

            for ( var i = 0; i < map.Length; i++ )
            {
                newMap[i] = map[i];
            }

            return newMap;
        }

        /// <summary>
        /// Adds 0's to the start or left side of the map
        /// </summary>
        /// <param name="map"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private BitArray PadLeft( BitArray map, int count )
        {
            if ( count == 0 )
            {
                return map;
            }

            var newLength = map.Length + count;
            var newMap = new BitArray( newLength );

            for ( var i = count; i < newLength; i++ )
            {
                newMap[i] = map[i];
            }

            return newMap;
        }

        /// <summary>
        /// Removes bits from the end or right side of the map
        /// </summary>
        /// <param name="map"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private BitArray RemoveFromRight( BitArray map, int count )
        {
            if ( count == 0 )
            {
                return map;
            }

            var newLength = map.Length - count;
            var newMap = new BitArray( newLength );

            for ( var i = 0; i < newLength; i++ )
            {
                newMap[i] = map[i];
            }

            return newMap;
        }

        /// <summary>
        /// Removes bits from the start or left side of the map
        /// </summary>
        /// <param name="map"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private BitArray RemoveFromLeft( BitArray map, int count )
        {
            if ( count == 0 )
            {
                return map;
            }

            var newLength = map.Length - count;
            var newMap = new BitArray( newLength );

            for ( var i = 0; i < newLength; i++ )
            {
                newMap[i] = map[i + count];
            }

            return newMap;
        }

        #endregion Bit Manipulation
    }

    /// <summary>
    /// This data transfer object conveys information about a single sequence enrollment as well as streak information that is
    /// calculated based on the sequence occurrence map and sequence exclusion maps.
    /// </summary>
    public class SequenceEnrollmentData
    {
        /// <summary>
        /// The date represented by the first bit in the sequence maps
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date represented by the last bit in the sequence maps
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The sequence of bits that represent attendance. The first bit is representative of the StartDate. Subsequent
        /// bits represent StartDate + (index * Days per OccurenceFrequency). A "1" represents a day or week where the person
        /// had attendance.
        /// </summary>
        public byte[] AttendanceMap { get; set; }

        /// <summary>
        /// The sequence of bits that represent occurrences where attendance was possible. The first bit is representative of the StartDate.
        /// Subsequent bits represent StartDate + (index * Days per OccurenceFrequency). A "1" represents a day or week where attendance
        /// was possible.
        /// </summary>
        public byte[] OccurenceMap { get; set; }

        /// <summary>
        /// The sequence of bits that represent exclusions. The first bit is representative of the StartDate. Subsequent
        /// bits represent StartDate + (index * Days per OccurenceFrequency). A "1" represents a day or week that should be
        /// excluded from the streak calculations.
        /// </summary>
        public byte[] ExclusionMap { get; set; }

        /// <summary>
        /// Gets or sets the timespan that each map bit represents (<see cref="Rock.Model.SequenceOccurenceFrequency"/>).
        /// </summary>
        public SequenceOccurenceFrequency OccurenceFrequency { get; set; }

        /// <summary>
        /// A list of object representing days or weeks from start to end containing the date and its attendance, occurrence, and
        /// exclusion data.
        /// </summary>
        public List<SequenceEnrollmentDataByFrequency> ByFrequency { get; set; }

        /// <summary>
        /// The object representing a single day or week in a streak
        /// </summary>
        public class SequenceEnrollmentDataByFrequency
        {
            /// <summary>
            /// The day or week represented
            /// </summary>
            public DateTime DateTime { get; set; }

            /// <summary>
            /// Did the person have attendance?
            /// </summary>
            public bool HasAttendance { get; set; }

            /// <summary>
            /// Did the sequence have an occurrence?
            /// </summary>
            public bool HasOccurrence { get; set; }

            /// <summary>
            /// Did the sequence have an exclusion?
            /// </summary>
            public bool HasExclusion { get; set; }
        }
    }
}