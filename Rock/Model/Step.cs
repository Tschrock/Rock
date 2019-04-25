using System;
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
    /// Represents a step in Rock.
    /// </summary>
    [RockDomain( "Steps" )]
    [Table( "Step" )]
    [DataContract]
    public partial class Step : Model<Step>, IOrdered
    {
        #region Entity Properties

        /// <summary>
        /// Gets or sets the Id of the <see cref="StepType"/> to which this step belongs. This property is required.
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int StepTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="StepStatus"/> to which this step belongs.
        /// </summary>
        [DataMember]
        public int? StepStatusId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="PersonAlias"/> that identifies the Person associated with taking this step. This property is required.
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int PersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Campus"/> associated with this step.
        /// </summary>
        [DataMember]
        public int? CampusId { get; set; }
        
        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> associated with the completion of this step.
        /// </summary>
        [DataMember]
        public DateTime? CompletedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> associated with the start of this step.
        /// </summary>
        [DataMember]
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> associated with the end of this step.
        /// </summary>
        [DataMember]
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        [DataMember]
        public string Note { get; set; }

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
        /// Gets or sets the Step Status.
        /// </summary>
        [DataMember]
        public virtual StepStatus StepStatus { get; set; }

        /// <summary>
        /// Gets or sets the Person Alias.
        /// </summary>
        [DataMember]
        public virtual PersonAlias PersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the Campus.
        /// </summary>
        [DataMember]
        public virtual Campus Campus { get; set; }

        #endregion Virtual Properties

        #region Entity Configuration

        /// <summary>
        /// Step Configuration class.
        /// </summary>
        public partial class StepConfiguration : EntityTypeConfiguration<Step>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StepConfiguration"/> class.
            /// </summary>
            public StepConfiguration()
            {
                HasRequired( p => p.StepType ).WithMany().HasForeignKey( p => p.StepTypeId ).WillCascadeOnDelete( true );
                HasRequired( p => p.PersonAlias ).WithMany().HasForeignKey( p => p.PersonAliasId ).WillCascadeOnDelete( true );

                HasOptional( c => c.Campus ).WithMany().HasForeignKey( c => c.CampusId ).WillCascadeOnDelete( false );
                HasOptional( c => c.StepStatus ).WithMany().HasForeignKey( c => c.StepStatusId ).WillCascadeOnDelete( false );
            }
        }

        #endregion
    }
}
