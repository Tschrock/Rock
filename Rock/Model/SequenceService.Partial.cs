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
using System.Text;
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

            // Make sure the enrollment does not already exist for the person
            var rockContext = Context as RockContext;
            var sequenceEnrollmentService = new SequenceEnrollmentService( rockContext );
            var alreadyEnrolled = sequenceEnrollmentService.IsEnrolled( sequence.Id, personAliasId );

            if ( alreadyEnrolled )
            {
                errorMessage = "The enrollment already exists";
                return null;
            }

            // Add the enrollment, matching the occurrence map's length with the same length array of 0/false bits
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
        /// <param name="includeBitMaps">Defaults to false. This may be a costly operation if enabled.</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public SequenceEnrollmentData GetSequenceEnrollmentData( SequenceCache sequence, int personAliasId, out string errorMessage,
            DateTime? startDate = null, DateTime? endDate = null, bool createObjectArray = false, bool includeBitMaps = false )
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

            if ( sequence.OccurenceFrequency == SequenceOccurenceFrequency.Daily && endDate > now )
            {
                errorMessage = "EndDate cannot be in the future";
                return null;
            }

            if ( sequence.OccurenceFrequency == SequenceOccurenceFrequency.Weekly && endDate > now.SundayDate() )
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
            var slideStartUnitsToFuture = GetFrequencyUnitDifference( sequence.StartDate, startDate.Value, sequence.OccurenceFrequency, false );

            // Calculate the number of frequency units that the results are based upon (inclusive)
            var numberOfFrequencyUnits = GetFrequencyUnitDifference( startDate.Value, endDate.Value, sequence.OccurenceFrequency, true );

            // Conform the sequence occurrences to the date range
            var occurrenceMap = ConformMapToDateRange( sequence.OccurenceMap, slideStartUnitsToFuture, numberOfFrequencyUnits );

            // Get the enrollment if it exists
            var rockContext = Context as RockContext;
            var sequenceEnrollmentService = new SequenceEnrollmentService( rockContext );
            var sequenceEnrollment = sequenceEnrollmentService.GetBySequenceAndPersonAlias( sequence.Id, personAliasId );
            var attendanceMap = ConformMapToDateRange( sequenceEnrollment?.AttendanceMap, slideStartUnitsToFuture, numberOfFrequencyUnits );
            var locationId = sequenceEnrollment?.LocationId;

            // Calculate the aggregate exclusion map
            var exclusionMaps = sequence.SequenceOccurrenceExclusions
                .Where( soe => soe.LocationId == locationId )
                .Select( soe => ConformMapToDateRange( soe.ExclusionMap, slideStartUnitsToFuture, numberOfFrequencyUnits ) )
                .ToArray();

            var aggregateExclusionMap = exclusionMaps.Length == 0 ? new bool[numberOfFrequencyUnits] : exclusionMaps[0];

            for ( var i = 1; i < exclusionMaps.Length; i++ )
            {
                OrBitOperation( aggregateExclusionMap, exclusionMaps[i] );
            }

            // Calculate streaks and object array if requested
            var currentStreak = 0;
            var currentStreakStartDate = ( DateTime? ) null;

            var longestStreak = 0;
            var longestStreakStartDate = ( DateTime? ) null;
            var longestStreakEndDate = ( DateTime? ) null;

            var occurrenceCount = 0;
            var attendanceCount = 0;
            var absenceCount = 0;
            var excludedAbsenceCount = 0;

            var objectArray = createObjectArray ? new List<SequenceEnrollmentData.FrequencyUnitData>( numberOfFrequencyUnits ) : null;

            for ( var unitsSinceStart = 0; unitsSinceStart < numberOfFrequencyUnits; unitsSinceStart++ )
            {
                // Use a reverse index to read from the arrays since the bits at the end of the array correspond to the past
                // and the bits at the front are nearer to the present
                var reverseIndex = numberOfFrequencyUnits - unitsSinceStart - 1;

                var hasAttendance = attendanceMap[reverseIndex];
                var hasExclusion = aggregateExclusionMap[reverseIndex];
                var hasOccurrence = occurrenceMap[reverseIndex];

                if ( hasOccurrence )
                {
                    occurrenceCount++;

                    if ( hasAttendance )
                    {
                        attendanceCount++;

                        // If starting a new streak, record the date
                        if ( currentStreak == 0 )
                        {
                            currentStreakStartDate = GetDateOfMapBit( startDate.Value, unitsSinceStart, sequence.OccurenceFrequency );
                        }

                        // Count this attendance toward the current streak
                        currentStreak++;

                        // If this is now the longest streak, update the longest counters
                        if ( currentStreak > longestStreak )
                        {
                            longestStreak = currentStreak;
                            longestStreakStartDate = currentStreakStartDate;
                            longestStreakEndDate = GetDateOfMapBit( startDate.Value, unitsSinceStart, sequence.OccurenceFrequency );
                        }
                    }
                    else if ( hasExclusion )
                    {
                        // Excluded/excused absences don't count toward streaks in a positive nor a negative manner, just ignore other
                        // than this count
                        excludedAbsenceCount++;
                    }
                    else
                    {
                        absenceCount++;

                        // Break the current streak
                        currentStreak = 0;
                        currentStreakStartDate = null;
                    }
                }

                if ( createObjectArray )
                {
                    objectArray.Add( new SequenceEnrollmentData.FrequencyUnitData
                    {
                        DateTime = GetDateOfMapBit( startDate.Value, unitsSinceStart, sequence.OccurenceFrequency ),
                        HasAttendance = hasAttendance,
                        HasExclusion = hasExclusion,
                        HasOccurrence = hasOccurrence
                    } );
                }
            }

            // Create the return object
            return new SequenceEnrollmentData
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                OccurenceMap = includeBitMaps ? GetBitString( occurrenceMap ) : null,
                ExclusionMap = includeBitMaps ? GetBitString( aggregateExclusionMap ) : null,
                AttendanceMap = includeBitMaps ? GetBitString( attendanceMap ) : null,
                OccurenceFrequency = sequence.OccurenceFrequency,
                CurrentStreakCount = currentStreak,
                CurrentStreakStartDate = currentStreakStartDate,
                LongestStreakCount = longestStreak,
                LongestStreakStartDate = longestStreakStartDate,
                LongestStreakEndDate = longestStreakEndDate,
                PerFrequencyUnit = objectArray,
                AbsenceCount = absenceCount,
                AttendanceCount = attendanceCount,
                ExcludedAbsenceCount = excludedAbsenceCount,
                OccurrenceCount = occurrenceCount
            };
        }

        /// <summary>
        /// Notes that the currently logged in person is present. This will update the SequenceEnrollment map and also
        /// Attendance (if enabled).
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="personAliasId"></param>
        /// <param name="errorMessage"></param>
        /// <param name="dateOfAttendance">Defaults to today</param>
        /// <param name="groupId">This is required for marking attendance unless the sequence is a group structure type</param>
        /// <param name="locationId"></param>
        /// <param name="scheduleId"></param>
        public void MarkAttendance( SequenceCache sequence, int personAliasId, out string errorMessage,
            DateTime? dateOfAttendance, int? groupId, int?locationId, int? scheduleId )
        {
            errorMessage = string.Empty;

            // Validate the sequence
            if ( sequence == null )
            {
                errorMessage = "A valid sequence is required";
                return;
            }

            if ( !sequence.IsActive )
            {
                errorMessage = "An active sequence is required";
                return;
            }

            // Override the group id if the sequence is explicit about the group
            if ( sequence.StructureType == SequenceStructureType.Group && sequence.StructureEntityId.HasValue )
            {
                groupId = sequence.StructureEntityId;
            }

            // Apply default values to parameters
            var isDaily = sequence.OccurenceFrequency == SequenceOccurenceFrequency.Daily;

            if ( !dateOfAttendance.HasValue && !isDaily )
            {
                dateOfAttendance = RockDateTime.Now.SundayDate();
            }
            else if ( !dateOfAttendance.HasValue )
            {
                dateOfAttendance = RockDateTime.Now.Date;
            }
            else
            {
                dateOfAttendance = dateOfAttendance.Value.Date;
            }

            // Validate the attendance date
            if ( isDaily && dateOfAttendance < sequence.StartDate.Date )
            {
                errorMessage = "Cannot mark attendance before the sequence began";
                return;
            }

            if ( !isDaily && dateOfAttendance < sequence.StartDate.SundayDate() )
            {
                errorMessage = "Cannot mark attendance before the sequence began";
                return;
            }

            // Get the enrollment if it exists
            var rockContext = Context as RockContext;
            var sequenceEnrollmentService = new SequenceEnrollmentService( rockContext );
            var sequenceEnrollment = sequenceEnrollmentService.GetBySequenceAndPersonAlias( sequence.Id, personAliasId );

            if ( sequenceEnrollment == null && sequence.RequiresEnrollment )
            {
                errorMessage = "This sequence requires enrollment";
                return;
            }

            if ( sequenceEnrollment == null )
            {
                // Enroll the person since they are marking attendance and enrollment is not required
                sequenceEnrollment = Enroll( sequence, personAliasId, out errorMessage );

                if ( !errorMessage.IsNullOrWhiteSpace() )
                {
                    return;
                }

                if ( sequenceEnrollment == null )
                {
                    errorMessage = "The enrollment was not successful but no error was specified";
                    return;
                }
            }

            // Mark attendance on the enrollment attendance map
            var boolMap = GetBoolArray( sequenceEnrollment.AttendanceMap );
            boolMap = SetBit( boolMap, sequence.StartDate, dateOfAttendance.Value, true, sequence.OccurenceFrequency, out errorMessage );

            if ( !errorMessage.IsNullOrWhiteSpace() )
            {
                return;
            }

            // Set the map on the model
            sequenceEnrollment.AttendanceMap = GetByteArray( boolMap );

            // If attendance is enabled and we are able to identify at least the group, then update attendance models
            if ( sequence.EnableAttendance && groupId.HasValue )
            {
                var attendanceService = new AttendanceService( rockContext );
                attendanceService.AddOrUpdate( personAliasId, dateOfAttendance.Value, groupId, locationId, scheduleId, null );
            }
        }

        /// <summary>
        /// Calculate the date represented by a map bit
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="frequencyUnits"></param>
        /// <param name="sequenceOccurenceFrequency"></param>
        /// <returns></returns>
        private DateTime GetDateOfMapBit( DateTime startDate, int frequencyUnits, SequenceOccurenceFrequency sequenceOccurenceFrequency )
        {
            var isDaily = sequenceOccurenceFrequency == SequenceOccurenceFrequency.Daily;
            var daysAfterStart = frequencyUnits * ( isDaily ? 1 : DaysPerWeek );
            var date = startDate.AddDays( daysAfterStart );
            return isDaily ? date.Date : date.SundayDate().Date;
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
        /// Set the bit that corresponds to bitDate. This method works in-place unless the array has to grow.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="startDate"></param>
        /// <param name="bitDate"></param>
        /// <param name="newValue"></param>
        /// <param name="occurenceFrequency"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool[] SetBit( bool[] map, DateTime startDate, DateTime bitDate, bool newValue, SequenceOccurenceFrequency occurenceFrequency, out string errorMessage )
        {
            errorMessage = string.Empty;

            if ( bitDate < startDate )
            {
                errorMessage = "The specified date occurs before the sequence begins";
                return map;
            }

            var unitsFromStart = GetFrequencyUnitDifference( startDate, bitDate, occurenceFrequency, false );

            if ( map == null )
            {
                map = new bool[unitsFromStart + 1];
            }
            else if ( unitsFromStart >= map.Length )
            {
                // Grow the map to accommodate the new value
                var growthNeeded = unitsFromStart - map.Length + 1;
                map = PadLeft( map, growthNeeded );
            }

            map[map.Length - unitsFromStart - 1] = newValue;
            return map;
        }

        /// <summary>
        /// Assumes the arrays are the same length and then ORs the bits in-place on top of array a.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void OrBitOperation( bool[] a, bool[] b )
        {
            for ( var i = 0; i < a.Length; i++ )
            {
                a[i] |= b[i];
            }
        }

        /// <summary>
        /// Copy the bit array into a bit string
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private string GetBitString( bool[] map )
        {
            var stringBuilder = new StringBuilder();

            for ( var i = 0; i < map.Length; i++ )
            {
                stringBuilder.Append( map[i] ? "1" : "0" );
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Get an array of booleans from the byte array
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        private bool[] GetBoolArray( byte[] byteArray )
        {
            if ( byteArray == null )
            {
                return null;
            }

            return byteArray.SelectMany( GetBits ).ToArray();
        }

        /// <summary>
        /// Get the boolean values from the byte
        /// From https://stackoverflow.com/a/2548060
        /// </summary>
        /// <param name="theByte"></param>
        /// <returns></returns>
        private IEnumerable<bool> GetBits( byte theByte )
        {
            for ( int i = 0; i < BitsPerByte; i++ )
            {
                yield return ( theByte & 0x80 ) != 0;
                theByte *= 2;
            }
        }

        /// <summary>
        /// Get the bytes represented by the boolean array
        /// </summary>
        /// <param name="boolArray"></param>
        /// <returns></returns>
        private byte[] GetByteArray( bool[] boolArray )
        {
            if ( boolArray == null )
            {
                return null;
            }

            // Create the resulting byte array. Add 1 instead of checking remainder since 0's padding the
            // left of a number don't mean anything
            var bitCount = boolArray.Length;
            var byteCount = bitCount / BitsPerByte + 1;
            var byteArray = new byte[byteCount];

            // We are walking through each bit in the bool array. Each 8 bools will become a byte. So we need
            // to track the current byte and the current bit within the current byte
            var currentByteIndex = byteCount - 1;
            var currentByteBitIndex = 0;

            // Loop backwards over the bit array since the least significant bit is at the end of the array
            for ( var bitIndex = boolArray.Length - 1; bitIndex >= 0; bitIndex-- )
            {
                if ( boolArray[bitIndex] )
                {
                    byteArray[currentByteIndex] |= ( byte ) ( 1 << currentByteBitIndex );
                }

                currentByteBitIndex++;

                if (currentByteBitIndex >= BitsPerByte)
                {
                    currentByteBitIndex = 0;
                    currentByteIndex--;
                }
            }

            return byteArray;
        }

        /// <summary>
        /// Adjust the map to match some other starting point and length
        /// </summary>
        /// <param name="byteArrayMap"></param>
        /// <param name="slideStartToFutureCount">A positive value means the new start date is after the original (slide the start
        /// date toward the future)</param>
        /// <param name="newLength"></param>
        /// <returns></returns>
        private bool[] ConformMapToDateRange( byte[] byteArrayMap, int slideStartToFutureCount, int newLength )
        {
            if ( byteArrayMap == null || byteArrayMap.Length == 0 )
            {
                return new bool[newLength];
            }

            var map = GetBoolArray( byteArrayMap );

            // This should happen frequently if the sequence start date is used and the end date is left as today so we
            // can skip all the other calculations for speed
            if ( slideStartToFutureCount == 0 && newLength == map.Length )
            {
                return map;
            }

            // Conform the beginning (right-side, least significant) of the map
            if ( slideStartToFutureCount > 0 )
            {
                // If date moved more recent, then we need to remove the bits that are before the new start date
                map = RemoveFromRight( map, slideStartToFutureCount );
            }
            else if ( slideStartToFutureCount < 0 )
            {
                // If date moved less recent, then we need to add the bits that are before the original start date
                map = PadRight( map, 0 - slideStartToFutureCount );
            }

            // Conform the length by adjusting the end (left-side, most significant) of the map
            var slideEndToFutureCount = newLength - map.Length;

            if ( slideEndToFutureCount > 0 )
            {
                // If the ending needs to shift into the future, then add bits to the end (left, most significant)
                map = PadLeft( map, slideEndToFutureCount );
            }
            else if ( slideEndToFutureCount < 0 )
            {
                // If the ending needs to shift into the past, then remove bits from the end (left, most significant)
                map = RemoveFromLeft( map, 0 - slideEndToFutureCount );
            }

            return map;
        }

        /// <summary>
        /// Adds 0's to the end or right side of the map
        /// </summary>
        /// <param name="map"></param>
        /// <param name="count">The number of values to add</param>
        /// <returns></returns>
        private bool[] PadRight( bool[] map, int count )
        {
            if ( count == 0 )
            {
                return map;
            }

            var newLength = map.Length + count;
            var newMap = new bool[newLength];

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
        /// <param name="count">The number of values to add</param>
        /// <returns></returns>
        private bool[] PadLeft( bool[] map, int count )
        {
            if ( count == 0 )
            {
                return map;
            }

            var newLength = map.Length + count;
            var newMap = new bool[newLength];

            for ( var i = 0; i < map.Length; i++ )
            {
                newMap[i + count] = map[i];
            }

            return newMap;
        }

        /// <summary>
        /// Removes bits from the end or right side of the map
        /// </summary>
        /// <param name="map"></param>
        /// <param name="count">The number of values to remove</param>
        /// <returns></returns>
        private bool[] RemoveFromRight( bool[] map, int count )
        {
            if ( count == 0 )
            {
                return map;
            }

            var newLength = map.Length - count;
            var newMap = new bool[newLength];

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
        /// <param name="count">The number of values to remove</param>
        /// <returns></returns>
        private bool[] RemoveFromLeft( bool[] map, int count )
        {
            if ( count == 0 )
            {
                return map;
            }

            var newLength = map.Length - count;
            var newMap = new bool[newLength];

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
        /// The sequence of bits that represent attendance. The least significant bit (rightmost) represents the start date.
        /// </summary>
        public string AttendanceMap { get; set; }

        /// <summary>
        /// The sequence of bits that represent occurrences where attendance was possible.  The least significant bit (rightmost)
        /// represents the start date.
        /// </summary>
        public string OccurenceMap { get; set; }

        /// <summary>
        /// The sequence of bits that represent exclusions.  The least significant bit (rightmost) represents the start date.
        /// </summary>
        public string ExclusionMap { get; set; }

        /// <summary>
        /// Gets or sets the timespan that each map bit represents (<see cref="Rock.Model.SequenceOccurenceFrequency"/>).
        /// </summary>
        public SequenceOccurenceFrequency OccurenceFrequency { get; set; }

        /// <summary>
        /// The date that the current streak began
        /// </summary>
        public DateTime? CurrentStreakStartDate { get; set; }

        /// <summary>
        /// The current number of non excluded occurrences attended in a row
        /// </summary>
        public int CurrentStreakCount { get; set; }

        /// <summary>
        /// The date the longest streak began
        /// </summary>
        public DateTime? LongestStreakStartDate { get; set; }

        /// <summary>
        /// The date the longest streak ended
        /// </summary>
        public DateTime? LongestStreakEndDate { get; set; }

        /// <summary>
        /// The longest number of non excluded occurrences attended in a row
        /// </summary>
        public int LongestStreakCount { get; set; }

        /// <summary>
        /// The number of occurrences within the date range
        /// </summary>
        public int OccurrenceCount { get; set; }

        /// <summary>
        /// The number of attendances on occurrences within the date range
        /// </summary>
        public int AttendanceCount { get; set; }

        /// <summary>
        /// The number of absences on occurrences within the date range
        /// </summary>
        public int AbsenceCount { get; set; }

        /// <summary>
        /// The number of excluded absences on occurrences within the date range
        /// </summary>
        public int ExcludedAbsenceCount { get; set; }

        /// <summary>
        /// A list of object representing days or weeks from start to end containing the date and its attendance, occurrence, and
        /// exclusion data.
        /// </summary>
        public List<FrequencyUnitData> PerFrequencyUnit { get; set; }

        /// <summary>
        /// The object representing a single day or week (frequency unit) in a streak
        /// </summary>
        public class FrequencyUnitData
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