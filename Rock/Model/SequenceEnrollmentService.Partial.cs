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
using System.Linq;

namespace Rock.Model
{
    /// <summary>
    /// Service/Data access class for <see cref="SequenceEnrollment"/> entity objects.
    /// </summary>
    public partial class SequenceEnrollmentService
    {
        /// <summary>
        /// Get the person's enrollment in the sequence
        /// </summary>
        /// <param name="sequenceId"></param>
        /// <param name="personAliasId"></param>
        /// <returns></returns>
        public SequenceEnrollment GetBySequenceAndPersonAlias( int sequenceId, int personAliasId )
        {
            return Queryable().FirstOrDefault( se => se.SequenceId == sequenceId && se.PersonAliasId == personAliasId );
        }
    }
}