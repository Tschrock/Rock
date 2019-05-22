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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;

using Rock.Data;
using Rock.Web.Cache;

namespace Rock.Model
{
    /// <summary>
    /// Represents a type or category of binary files in Rock, and configures how binary files of this type are stored and accessed.
    /// </summary>
    [RockDomain( "CRM" )]
    [Table( "BadgeType" )]
    [DataContract]
    public partial class BadgeType : Model<BadgeType>, IOrdered, IHasActiveFlag, ICacheable
    {
        #region Entity Properties

        /// <summary>
        /// Gets or sets the given Name of the BadgeType. This value is an alternate key and is required.
        /// </summary>
        [Required]
        [MaxLength( 100 )]
        [DataMember( IsRequired = true )]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description of the BadgeType.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Id of the badge component
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int ComponentEntityTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the subject's entity type
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int SubjectEntityTypeId { get; set; }

        #region IHasActiveFlag

        /// <summary>
        /// Gets or sets a flag indicating if this item is active or not.
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; } = true;

        #endregion IHasActiveFlag

        #region IOrdered

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        [DataMember]
        public int Order { get; set; }

        #endregion IOrdered


        #endregion Entity Properties

        #region Virtual Properties

        /// <summary>
        /// Gets or sets the storage mode <see cref="Rock.Model.EntityType"/>.
        /// </summary>
        [DataMember]
        public virtual EntityType ComponentEntityType { get; set; }

        /// <summary>
        /// Gets or sets the subject <see cref="Rock.Model.EntityType"/>.
        /// </summary>
        [DataMember]
        public virtual EntityType SubjectEntityType { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region ICacheable

        /// <summary>
        /// Gets the cache object associated with this Entity
        /// </summary>
        /// <returns></returns>
        public IEntityCache GetCacheObject()
        {
            return BadgeTypeCache.Get( Id );
        }

        /// <summary>
        /// Updates any Cache Objects that are associated with this entity
        /// </summary>
        /// <param name="entityState">State of the entity.</param>
        /// <param name="dbContext">The database context.</param>
        public void UpdateCache( EntityState entityState, Rock.Data.DbContext dbContext )
        {
            BadgeTypeCache.UpdateCachedEntity( Id, entityState );
        }

        #endregion

    }

    #region Entity Configuration

    /// <summary>
    /// File Configuration class.
    /// </summary>
    public partial class BadgeTypeConfiguration : EntityTypeConfiguration<BadgeType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadgeTypeConfiguration"/> class.
        /// </summary>
        public BadgeTypeConfiguration()
        {
            HasRequired( bt => bt.SubjectEntityType ).WithMany().HasForeignKey( bt => bt.SubjectEntityTypeId ).WillCascadeOnDelete( false );
            HasRequired( bt => bt.ComponentEntityType ).WithMany().HasForeignKey( bt => bt.ComponentEntityTypeId ).WillCascadeOnDelete( false );
        }
    }

    #endregion
}