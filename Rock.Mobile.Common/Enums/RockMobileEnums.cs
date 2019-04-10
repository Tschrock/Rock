using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Mobile.Common.Enums
{
    /// <summary>The type of application shell.</summary>
    public enum ShellType
    {
        Blank = 0,
        Flyout = 1,
        Tabbed = 2
    }

    /// <summary>The device type running the application (Phone, Tablet, Watch).</summary>
    public enum DeviceType
    {
        Phone = 0,
        Tablet = 1,
        Watch = 2,
        Unknown = 9999
    }

    /// <summary>The device platform (iOS, Android)</summary>
    public enum DevicePlatform
    {
        iOS = 0,
        Android = 1,
        Other = 9999
    }
}
