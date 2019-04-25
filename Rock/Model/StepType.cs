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
    [Table( "StepType" )]
    [DataContract]
    public partial class StepType : Model<StepType>, IOrdered
    {
        /// TODO
        /// 
        /// AudienceDataViewId
        /// AutoCompleteDataViewId
        /// CardLavaTemplate
        /// MergeTemplateId
        /// MergeTemplateDescriptor

        #region Entity Properties

        /// <summary>
        /// Gets or sets the Id of the <see cref="StepProgram"/> to which this step type belongs. This property is required.
        /// </summary>
        [Required]
        [DataMember( IsRequired = true )]
        public int StepProgramId { get; set; }

        /// <summary>
        /// Gets or sets the name of the step type. This property is required.
        /// </summary>
        [MaxLength( 250 )]
        [DataMember( IsRequired = true )]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description of the step type.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the icon CSS class.
        /// </summary>
        [MaxLength( 100 )]
        [DataMember]
        public string IconCssClass { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this step type allows multiple step records per person.
        /// </summary>
        [DataMember]
        public bool AllowMultiple { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if this item is active or not.
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if this step type happens over time (like being in a group) or is it achievement based (like attended a class).
        /// </summary>
        [DataMember]
        public bool HasEndDate { get; set; } = false;

        /// <summary>
        /// Gets or sets a flag indicating if the number of occurences should be shown on the badge.
        /// </summary>
        [DataMember]
        public bool ShowCountOnBadge { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if this item can be edited by a person.
        /// </summary>
        [DataMember]
        public bool AllowManualEditing { get; set; } = true;

        /// <summary>
        /// Gets or sets the highlight color for badges and cards.
        /// </summary>
        [MaxLength( 100 )]
        [DataMember]
        public string HighlightColor { get; set; }

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
        /// Gets or sets the Step Program.
        /// </summary>
        [DataMember]
        public virtual StepProgram StepProgram { get; set; }

        /// <summary>
        /// Gets or sets a collection containing the <see cref="Step">Steps</see> that are of this step type.
        /// </summary>
        [DataMember]
        public virtual ICollection<Step> Steps
        {
            get => _steps ?? ( _steps = new Collection<Step>() );
            set => _steps = value;
        }
        private ICollection<Step> _steps;

        /// <summary>
        /// Gets or sets a collection containing the <see cref="StepTypePrerequisite">Prerequisites</see> for this step type.
        /// </summary>
        [DataMember]
        public virtual ICollection<StepTypePrerequisite> StepTypePrerequisites
        {
            get => _stepTypePrerequisites ?? ( _stepTypePrerequisites = new Collection<StepTypePrerequisite>() );
            set => _stepTypePrerequisites = value;
        }
        private ICollection<StepTypePrerequisite> _stepTypePrerequisites;

        #endregion Virtual Properties

        #region Entity Configuration

        /// <summary>
        /// Step Type Configuration class.
        /// </summary>
        public partial class StepTypeConfiguration : EntityTypeConfiguration<StepType>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StepTypeConfiguration"/> class.
            /// </summary>
            public StepTypeConfiguration()
            {
                HasRequired( p => p.StepProgram ).WithMany().HasForeignKey( p => p.StepProgramId ).WillCascadeOnDelete( true );
            }
        }

        #endregion
    }
}
