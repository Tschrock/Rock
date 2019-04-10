using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Mobile.Common
{
    public class PageReference
    {
        public string Title { get; set; } = string.Empty;

        public Guid PageGuid { get; set; } = Guid.Empty;

        public string IconUrl { get; set; } = string.Empty;
    }
}
