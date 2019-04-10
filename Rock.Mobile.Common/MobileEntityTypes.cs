using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Mobile.Common
{
    public static class MobileEntityTypes
    {
        /// <summary>Gets the entity types for mobile apps.</summary>
        /// <value>The entity types.</value>
        public static Dictionary<string,int> EntityTypes
        {
            get
            {
                return _entityTypes;
            }
        }
        private static Dictionary<string, int> _entityTypes = new Dictionary<string, int>();


        static MobileEntityTypes()
        {
            // Load entity types
            _entityTypes.Add( "Rock.Mobile.Model.ApplicationSetting", 1 );
            _entityTypes.Add( "Rock.Mobile.Model.ApplicationPage", 2 );
            _entityTypes.Add( "Rock.Mobile.Model.ApplicationBlock", 3 );
        }
    }
}
