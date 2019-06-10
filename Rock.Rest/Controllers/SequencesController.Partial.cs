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
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using Rock.Data;
using Rock.Model;
using Rock.Rest.Filters;
using Rock.Web.Cache;

namespace Rock.Rest.Controllers
{
    /// <summary>
    /// FinancialScheduledTransactions REST API
    /// </summary>
    public partial class SequencesController
    {
        /// <summary>
        /// Enroll the currently logged-in user into the sequence.
        /// </summary>
        /// <param name="sequenceId"></param>
        /// <param name="personAliasId">Defaults to the current person</param>
        /// <param name="enrollmentDate">Defaults to the current date if omitted</param>
        /// <param name="locationId">Defaults to the person's campus if omitted</param>
        /// <returns></returns>
        [Authenticate, Secured]
        [HttpPost]
        [System.Web.Http.Route( "api/Sequences/Enroll/{sequenceId}" )]
        public virtual HttpResponseMessage Enroll( int sequenceId, [FromUri]int? personAliasId = null, [FromUri] DateTime? enrollmentDate = null, [FromUri] int? locationId = null )
        {
            // Make sure the sequence exists
            var sequence = SequenceCache.Get( sequenceId );

            if ( sequence == null )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.NotFound, "The sequenceId did not resolve" );
                throw new HttpResponseException( errorResponse );
            }

            // If not specified, use the current person alias id
            var rockContext = Service.Context as RockContext;

            if ( !personAliasId.HasValue )
            {
                personAliasId = GetPersonAliasId( rockContext );

                if ( !personAliasId.HasValue )
                {
                    var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.BadRequest, "The personAliasId for the current user did not resolve" );
                    throw new HttpResponseException( errorResponse );
                }
            }

            // Create the enrollment
            var sequenceService = Service as SequenceService;
            var sequenceEnrollment = sequenceService.Enroll( sequence, personAliasId.Value, out var errorMessage, enrollmentDate, locationId );

            if ( !errorMessage.IsNullOrWhiteSpace() )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.BadRequest, errorMessage );
                throw new HttpResponseException( errorResponse );
            }

            if ( sequenceEnrollment == null )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.InternalServerError, "The enrollment was not successful but no error was specified" );
                throw new HttpResponseException( errorResponse );
            }

            // Save to the DB and tell the user the new id
            rockContext.SaveChanges();
            return ControllerContext.Request.CreateResponse( HttpStatusCode.Created, sequenceEnrollment.Id );
        }

        /// <summary>
        /// Returns a listing of locations, including geofence data, for the sequence. These locations are determined from the
        /// structure type of the sequence.
        /// </summary>
        /// <param name="sequenceId"></param>
        /// <returns></returns>
        [Authenticate, Secured]
        [HttpGet]
        [EnableQuery]
        [System.Web.Http.Route( "api/Sequences/Locations/{sequenceId}" )]
        public virtual IQueryable<Location> GetLocations( int sequenceId )
        {
            // Make sure the sequence exists
            var sequence = SequenceCache.Get( sequenceId );

            if ( sequence == null )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.NotFound, "The sequenceId did not resolve" );
                throw new HttpResponseException( errorResponse );
            }

            // Get the locations from the service
            var sequenceService = Service as SequenceService;
            var locations = sequenceService.GetLocations( sequence, out var errorMessage );

            if ( !errorMessage.IsNullOrWhiteSpace() )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.BadRequest, errorMessage );
                throw new HttpResponseException( errorResponse );
            }

            if ( locations == null )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.InternalServerError, "The location retrieval was not successful but no error was specified" );
                throw new HttpResponseException( errorResponse );
            }

            return locations;
        }

        /// <summary>
        /// Returns a listing of schedules, including iCal data, for the sequence and specified location.
        /// </summary>
        /// <param name="sequenceId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [Authenticate, Secured]
        [HttpGet]
        [EnableQuery]
        [System.Web.Http.Route( "api/Sequences/LocationSchedules/{sequenceId}/{locationId}" )]
        public virtual IQueryable<Schedule> GetLocationSchedules( int sequenceId, int locationId )
        {
            // Make sure the sequence exists
            var sequence = SequenceCache.Get( sequenceId );

            if ( sequence == null )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.NotFound, "The sequenceId did not resolve" );
                throw new HttpResponseException( errorResponse );
            }

            // Get the schedules from the service
            var sequenceService = Service as SequenceService;
            var schedules = sequenceService.GetLocationSchedules( sequence, locationId, out var errorMessage );

            if ( !errorMessage.IsNullOrWhiteSpace() )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.BadRequest, errorMessage );
                throw new HttpResponseException( errorResponse );
            }

            if ( schedules == null )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.InternalServerError, "The schedule retrieval was not successful but no error was specified" );
                throw new HttpResponseException( errorResponse );
            }

            return schedules;
        }

        /// <summary>
        /// Returns the currently logged-in user or the person indicated's streak information.
        /// </summary>
        /// <param name="sequenceId"></param>
        /// <param name="personAliasId">Defaults to the current person</param>
        /// <param name="startDate">Defaults to the sequence start date</param>
        /// <param name="endDate">Defaults to now</param>
        /// <param name="createObjectArray">Defaults to false. This may be a costly operation if enabled.</param>
        /// <param name="includeBitMaps">Defaults to false. This may be a costly operation if enabled.</param>
        /// <returns></returns>
        [Authenticate, Secured]
        [HttpGet]
        [System.Web.Http.Route( "api/Sequences/EnrollmentStreak/{sequenceId}" )]
        public virtual SequenceEnrollmentData GetEnrollmentStreak( int sequenceId,
            [FromUri]int? personAliasId = null, [FromUri]DateTime? startDate = null, [FromUri]DateTime? endDate = null,
            [FromUri]bool createObjectArray = false, [FromUri]bool includeBitMaps = false )
        {
            // Make sure the sequence exists
            var sequence = SequenceCache.Get( sequenceId );

            if ( sequence == null )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.NotFound, "The sequenceId did not resolve" );
                throw new HttpResponseException( errorResponse );
            }

            // If not specified, use the current person alias id
            if ( !personAliasId.HasValue )
            {
                var rockContext = Service.Context as RockContext;
                personAliasId = GetPersonAliasId( rockContext );

                if ( !personAliasId.HasValue )
                {
                    var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.BadRequest, "The personAliasId for the current user did not resolve" );
                    throw new HttpResponseException( errorResponse );
                }
            }

            // Get the data from the service
            var sequenceService = Service as SequenceService;
            var sequenceEnrollmentData = sequenceService.GetSequenceEnrollmentData( sequence, personAliasId.Value, out var errorMessage, startDate, endDate, createObjectArray, includeBitMaps );

            if ( !errorMessage.IsNullOrWhiteSpace() )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.BadRequest, errorMessage );
                throw new HttpResponseException( errorResponse );
            }

            if ( sequenceEnrollmentData == null )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.InternalServerError, "The sequence data retrieval was not successful but no error was specified" );
                throw new HttpResponseException( errorResponse );
            }

            return sequenceEnrollmentData;
        }

        /// <summary>
        /// Notes that the person is present. This will update the SequenceEnrollment map and also attendance (if enabled).
        /// </summary>
        /// <param name="sequenceId"></param>
        /// <param name="personAliasId">Defaults to the current person</param>
        /// <param name="dateOfAttendance">Defaults to now</param>
        /// <returns></returns>
        [Authenticate, Secured]
        [HttpPost]
        [System.Web.Http.Route( "api/Sequences/MarkAttendance/{sequenceId}" )]
        public virtual HttpResponseMessage MarkAttendance( int sequenceId, [FromUri]int? personAliasId = null, [FromUri]DateTime? dateOfAttendance = null )
        {
            // Make sure the sequence exists
            var sequence = SequenceCache.Get( sequenceId );

            if ( sequence == null )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.NotFound, "The sequenceId did not resolve" );
                throw new HttpResponseException( errorResponse );
            }

            // If not specified, use the current person alias id
            var rockContext = Service.Context as RockContext;

            if ( !personAliasId.HasValue )
            {                
                personAliasId = GetPersonAliasId( rockContext );

                if ( !personAliasId.HasValue )
                {
                    var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.BadRequest, "The personAliasId for the current user did not resolve" );
                    throw new HttpResponseException( errorResponse );
                }
            }

            // Get the data from the service
            var sequenceService = Service as SequenceService;
            sequenceService.MarkAttendance( sequence, personAliasId.Value, out var errorMessage, dateOfAttendance );

            if ( !errorMessage.IsNullOrWhiteSpace() )
            {
                var errorResponse = ControllerContext.Request.CreateErrorResponse( HttpStatusCode.BadRequest, errorMessage );
                throw new HttpResponseException( errorResponse );
            }

            // Save to the DB
            rockContext.SaveChanges();
            return ControllerContext.Request.CreateResponse( HttpStatusCode.OK );
        }
    }
}
