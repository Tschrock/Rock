using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Mobile.Common
{
    public class LaunchPackage
    {
        /// <summary>
        /// Gets or sets the latest version identifier.
        /// </summary>
        /// <value>
        /// The latest version identifier.
        /// </value>
        public int LatestVersionId { get; set; }

        /// <summary>
        /// Gets or sets the latest version settings URL.
        /// </summary>
        /// <value>
        /// The latest version settings URL.
        /// </value>
        public string LatestVersionSettingsUrl { get; set; }

        /// <summary>
        /// Gets or sets the current person.
        /// </summary>
        /// <value>
        /// The current person.
        /// </value>
        public MobilePerson CurrentPerson { get; set; }
    }

    #region POCOs
    public class MobilePerson
    {
        public int PersonAliasId { get; set; }

        public Guid PersonGuid { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhotoUrl { get; set; }

        public string MobilePhone { get; set; }

        public string HomePhone { get; set; }

        public string AuthToken { get; set; }

        public List<Guid> SecurityGroupGuids { get; set; }

        public List<Guid> PersonalizationSegmentGuids { get; set; }
    }
    #endregion
}
