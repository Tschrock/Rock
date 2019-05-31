using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
using Rock.Data;

namespace Rock.Model
{
    /// <summary>
    /// Represents a Sequence Enrollment in Rock.
    /// </summary>
    [RockDomain( "Sequences" )]
    [Table( "SequenceEnrollment" )]
    [DataContract]
    public partial class SequenceEnrollment : Model<SequenceEnrollment>
    {
        #region Entity Properties

        /// <summary>
        /// Gets or sets the Id of the <see cref="Sequence"/> to which this enrollment belongs. This property is required.
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int SequenceId { get; set; }

        /// <summary>
        /// Gets or sets the person alias identifier.
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int PersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> when the person was enrolled in the sequence.
        /// This is not the sequence start date.
        /// </summary>
        [DataMember]
        [Required]
        [Column( TypeName = "Date" )]
        public DateTime EnrollmentDate
        {
            get => _enrollmentDate;
            set => _enrollmentDate = value.Date;
        }
        private DateTime _enrollmentDate = RockDateTime.Now;

        /// <summary>
        /// Gets or sets the location identifier by which the person's exclusions will be sourced.
        /// </summary>
        [DataMember]
        public int? LocationId { get; set; }

        /// <summary>
        /// The sequence of bits that represent attendance. The first bit is representative of the Sequence's StartDate. Subsequent
        /// bits represent StartDate + (index * Days per OccurenceFrequency).
        /// </summary>
        [DataMember]
        public byte[] OccurenceMap { get; set; }

        #endregion Entity Properties

        #region Virtual Properties

        /// <summary>
        /// Gets or sets the Person Alias.
        /// </summary>
        [DataMember]
        public virtual PersonAlias PersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the Sequence.
        /// </summary>
        [DataMember]
        public virtual Sequence Sequence { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        [DataMember]
        public virtual Location Location { get; set; }

        #endregion Virtual Properties

        #region Entity Configuration

        /// <summary>
        /// Sequence Enrollment Configuration class.
        /// </summary>
        public partial class SequenceEnrollmentConfiguration : EntityTypeConfiguration<SequenceEnrollment>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SequenceEnrollmentConfiguration"/> class.
            /// </summary>
            public SequenceEnrollmentConfiguration()
            {
                HasRequired( se => se.Sequence ).WithMany( s => s.SequenceEnrollments ).HasForeignKey( se => se.SequenceId ).WillCascadeOnDelete( true );
                HasRequired( se => se.PersonAlias ).WithMany().HasForeignKey( se => se.PersonAliasId ).WillCascadeOnDelete( true );

                HasOptional( se => se.Location ).WithMany().HasForeignKey( se => se.LocationId ).WillCascadeOnDelete( false );
            }
        }

        #endregion Entity Configuration
    }
}
