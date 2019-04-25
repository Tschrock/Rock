using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
using Rock.Data;

namespace Rock.Model
{
    /// <summary>
    /// Represents a step type in Rock.
    /// </summary>
    [RockDomain( "Steps" )]
    [Table( "StepTypePrerequisite" )]
    [DataContract]
    public partial class StepTypePrerequisite : Model<StepTypePrerequisite>, IOrdered
    {
        #region Entity Properties

        /// <summary>
        /// Gets or sets the Id of the <see cref="StepType"/> to which this prerequisite. This property is required.
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int StepTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="StepType"/> that is a prerequisite. This property is required.
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int PrerequisiteStepTypeId { get; set; }

        #endregion Entity Properties

        #region IOrdered

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        [DataMember]
        public int Order { get; set; }

        #endregion IOrdered

        #region Virtual Properties

        /// <summary>
        /// Gets or sets the Step Type.
        /// </summary>
        [DataMember]
        public virtual StepType StepType { get; set; }

        /// <summary>
        /// Gets or sets the Prerequisite Step Type.
        /// </summary>
        [DataMember]
        public virtual StepType PrerequisiteStepType { get; set; }

        #endregion Virtual Properties

        #region Entity Configuration

        /// <summary>
        /// Step Type Configuration class.
        /// </summary>
        public partial class StepTypePrerequisiteConfiguration : EntityTypeConfiguration<StepTypePrerequisite>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StepTypePrerequisiteConfiguration"/> class.
            /// </summary>
            public StepTypePrerequisiteConfiguration()
            {
                HasRequired( p => p.StepType ).WithMany().HasForeignKey( p => p.StepTypeId ).WillCascadeOnDelete( true );
                HasRequired( p => p.PrerequisiteStepType ).WithMany().HasForeignKey( p => p.PrerequisiteStepTypeId ).WillCascadeOnDelete( true );
            }
        }

        #endregion
    }
}
