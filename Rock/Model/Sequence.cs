using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Rock.Data;
using Rock.Web.Cache;

namespace Rock.Model
{
    /// <summary>
    /// Represents a sequence in Rock.
    /// </summary>
    [RockDomain( "Sequences" )]
    [Table( "Sequence" )]
    [DataContract]
    public partial class Sequence : Model<Sequence>, IHasActiveFlag
    {
        #region Entity Properties

        /// <summary>
        /// Gets or sets the name of the sequence. This property is required.
        /// </summary>
        [MaxLength( 250 )]
        [DataMember( IsRequired = true )]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description of the sequence.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the attendance association (<see cref="Rock.Model.SequenceStructureType"/>). If not set, this sequence
        /// will not produce attendance records.
        /// </summary>
        [DataMember]
        public SequenceStructureType? StructureType { get; set; }

        /// <summary>
        /// Gets or sets the Id of the Entity associated with attendance for this sequence.
        /// </summary>
        [DataMember]
        public int? StructureEntityId { get; set; }

        /// <summary>
        /// This determines whether the sequence will write attendance records when marking someone as present or
        /// if it will just update the enrolled individual’s map.
        /// </summary>
        [DataMember]
        public bool EnableAttendance { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this sequence requires explicit enrollment. If not set, a person can be
        /// implicitly enrolled through attendance.
        /// </summary>
        [DataMember]
        public bool RequiresEnrollment { get; set; }

        /// <summary>
        /// Gets or sets the timespan that each map bit represents (<see cref="Rock.Model.SequenceOccurenceFrequency"/>).
        /// </summary>
        [DataMember( IsRequired = true )]
        [Required]
        public SequenceOccurenceFrequency OccurenceFrequency { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> associated with the first bit of this sequence.
        /// </summary>
        [DataMember]
        [Required]
        [Column( TypeName = "Date" )]
        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value.Date;
        }
        private DateTime _startDate = RockDateTime.Now;

        /// <summary>
        /// The sequence of bits that represent occurences where attendance was possible. The first bit is representative of the StartDate.
        /// Subsequent bits represent StartDate + (index * Days per OccurenceFrequency).
        /// </summary>
        [DataMember]
        public byte[] OccurenceMap { get; set; }

        #endregion Entity Properties

        #region IHasActiveFlag

        /// <summary>
        /// Gets or sets a flag indicating if this item is active or not.
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; } = true;

        #endregion

        #region Virtual Properties

        /// <summary>
        /// Gets or sets a collection containing the <see cref="SequenceEnrollment">SequenceEnrollments</see> that are of this sequence.
        /// </summary>
        [DataMember]
        public virtual ICollection<SequenceEnrollment> SequenceEnrollments
        {
            get => _sequenceEnrollments ?? ( _sequenceEnrollments = new Collection<SequenceEnrollment>() );
            set => _sequenceEnrollments = value;
        }
        private ICollection<SequenceEnrollment> _sequenceEnrollments;

        /// <summary>
        /// Gets or sets a collection containing the <see cref="SequenceOccurrenceExclusion">SequenceOccurrenceExclusions</see>
        /// that are of this sequence.
        /// </summary>
        [DataMember]
        public virtual ICollection<SequenceOccurrenceExclusion> SequenceOccurrenceExclusions
        {
            get => _sequenceOccurrenceExclusions ?? ( _sequenceOccurrenceExclusions = new Collection<SequenceOccurrenceExclusion>() );
            set => _sequenceOccurrenceExclusions = value;
        }
        private ICollection<SequenceOccurrenceExclusion> _sequenceOccurrenceExclusions;

        #endregion Virtual Properties
    }

    #region Enumerations

    /// <summary>
    /// Represents the attendance association of a <see cref="Sequence"/>.
    /// </summary>
    public enum SequenceStructureType
    {
        /// <summary>
        /// The <see cref="Sequence"/> is associated with attendance to a single group.
        /// </summary>
        Group = 1,

        /// <summary>
        /// The <see cref="Sequence"/> is associated with attendance to groups of a given type.
        /// </summary>
        GroupType = 2,

        /// <summary>
        /// The <see cref="Sequence"/> is associated with attendance specified by a check-in configuration.
        /// </summary>
        CheckInConfig = 3
    }

    /// <summary>
    /// Represents the timespan represented by each of the <see cref="Sequence"/> bits.
    /// </summary>
    public enum SequenceOccurenceFrequency
    {
        /// <summary>
        /// The <see cref="Sequence"/> has bits that represent a day.
        /// </summary>
        Daily = 0,

        /// <summary>
        /// The <see cref="Sequence"/> has bits that represent a week.
        /// </summary>
        Weekly = 1
    }

    #endregion
}
