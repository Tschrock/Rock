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
using System.Runtime.Serialization;
using Rock.Data;
using Rock.Model;
using Rock.PersonProfile;

namespace Rock.Web.Cache
{
    /// <summary>
    /// Information about a BadgeType that is required by the rendering engine.
    /// This information will be cached by the engine
    /// </summary>
    [Serializable]
    [DataContract]
    public class BadgeTypeCache : ModelCache<BadgeTypeCache, BadgeType>
    {
        #region Entity Properties

        /// <summary>
        /// Gets or sets the given Name of the BadgeType. This value is an alternate key and is required.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description of the BadgeType.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Id of the badge component
        /// </summary>
        [DataMember]
        public int ComponentEntityTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the subject's entity type
        /// </summary>
        [DataMember]
        public int SubjectEntityTypeId { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this item is active or not.
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        [DataMember]
        public int Order { get; set; }

        /// <summary>
        /// Gets the Entity Type.
        /// </summary>
        public EntityTypeCache SubjectEntityType
        {
            get => EntityTypeCache.Get( SubjectEntityTypeId );
        }

        /// <summary>
        /// Gets the Entity Type.
        /// </summary>
        public EntityTypeCache ComponentEntityType
        {
            get => EntityTypeCache.Get( ComponentEntityTypeId );
        }

        /// <summary>
        /// Gets the badge component.
        /// </summary>
        public virtual BadgeComponent BadgeComponent
        {
            get => ComponentEntityType != null ? BadgeContainer.GetComponent( ComponentEntityType.Name ) : null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies from model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void SetFromEntity( IEntity entity )
        {
            base.SetFromEntity( entity );

            var BadgeType = entity as BadgeType;
            if ( BadgeType == null )
                return;

            Name = BadgeType.Name;
            Description = BadgeType.Description;
            ComponentEntityTypeId = BadgeType.ComponentEntityTypeId;
            SubjectEntityTypeId = BadgeType.SubjectEntityTypeId;
            Order = BadgeType.Order;
            IsActive = BadgeType.IsActive;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}