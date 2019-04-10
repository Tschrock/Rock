using Rock.Mobile.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Mobile.Common
{
    /// <summary>POCO class to store information about the device.</summary>
    public class DeviceData
    {
        /// <summary>Gets or sets the type of the device.</summary>
        /// <value>The type of the device.</value>
        public DeviceType DeviceType { get; set; }

        /// <summary>Gets or sets the manufacturer.</summary>
        /// <value>The manufacturer.</value>
        public string Manufacturer { get; set; }

        /// <summary>Gets or sets the model.</summary>
        /// <value>The model.</value>
        public string Model { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the version string.</summary>
        /// <value>The version string.</value>
        public string VersionString { get; set; }

        /// <summary>Gets or sets the device platform.</summary>
        /// <value>The device platform.</value>
        public DevicePlatform DevicePlatform { get; set; }
    }
}
