using Rock.Mobile.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Mobile.Common
{
    /// <summary>POCO for getting the updates to the application.</summary>
    public class UpdatePackage
    {
        /// <summary>Gets or sets the application version identifier.</summary>
        /// <value>The application version identifier.</value>
        public int ApplicationVersionId { get; set; }

        /// <summary>Gets or sets the type of the application.</summary>
        /// <value>The type of the application.</value>
        public ShellType ApplicationType { get; set; } = ShellType.Flyout;

        /// <summary>Gets or sets a value indicating whether [tabs on bottom on android].</summary>
        /// <value>
        ///   <c>true</c> if [tabs on bottom on android]; otherwise, <c>false</c>.</value>
        public bool TabsOnBottomOnAndroid { get; set; } = true;

        /// <summary>Gets or sets the CSS styles.</summary>
        /// <value>The CSS styles.</value>
        public string CssStyles { get; set; } = string.Empty;

        /// <summary>Gets or sets the appearance settings.</summary>
        /// <value>The appearance settings.</value>
        public AppearanceSettings AppearanceSettings { get; set; } = new AppearanceSettings();

        /// <summary>Gets or sets the layouts.</summary>
        /// <value>The layouts.</value>
        public List<Layout> Layouts { get; set; } = new List<Layout>();

        /// <summary>Gets or sets the pages.</summary>
        /// <value>The pages.</value>
        public List<Page> Pages { get; set; } = new List<Page>();

        /// <summary>Gets or sets the blocks.</summary>
        /// <value>The blocks.</value>
        public List<Block> Blocks { get; set; } = new List<Block>();

        /// <summary>Gets or sets the authentication rules.</summary>
        /// <value>The authentication rules.</value>
        public List<Auth> AuthenticationRules { get; set; } = new List<Auth>();
    }

    #region POCOs
    /// <summary>POCO for storing auth rules.</summary>
    public class Auth
    {
        /// <summary>Gets or sets the entity type identifier.</summary>
        /// <value>The entity type identifier.</value>
        public int EntityTypeId { get; set; }

        /// <summary>Gets or sets the entity unique identifier.</summary>
        /// <value>The entity unique identifier.</value>
        public Guid EntityGuid { get; set; }

        /// <summary>Gets or sets the order.</summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>Gets or sets the action.</summary>
        /// <value>The action.</value>
        public string Action { get; set; }

        /// <summary>Gets or sets the allow or deny.</summary>
        /// <value>The allow or deny.</value>
        public string AllowOrDeny { get; set; }

        /// <summary>Gets or sets the group unique identifier.</summary>
        /// <value>The group unique identifier.</value>
        public Guid GroupGuid { get; set; }

        /// <summary>Gets or sets the special role.</summary>
        /// <value>The special role.</value>
        public int SpecialRole { get; set; }

        /// <summary>Gets or sets the authentication unique identifier.</summary>
        /// <value>The authentication unique identifier.</value>
        public Guid AuthGuid { get; set; }
    }

    /// <summary>POCO for storing a block.</summary>
    public class Block
    {
        /// <summary>Gets or sets the block unique identifier.</summary>
        /// <value>The block unique identifier.</value>
        public Guid BlockGuid { get; set; }

        /// <summary>Gets or sets the zone.</summary>
        /// <value>The zone.</value>
        public string Zone { get; set; }

        /// <summary>Gets or sets the order.</summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>Gets or sets the page unique identifier.</summary>
        /// <value>The page unique identifier.</value>
        public Guid PageGuid { get; set; }

        /// <summary>Gets or sets the type of the block.</summary>
        /// <value>The type of the block.</value>
        public string BlockType { get; set; }
    }

    /// <summary>POCO for storing a page.</summary>
    public class Page
    {
        /// <summary>Gets or sets the page unique identifier.</summary>
        /// <value>The page unique identifier.</value>
        public Guid PageGuid { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Title { get; set; }

        /// <summary>Gets or sets a value indicating whether [display in nav].</summary>
        /// <value>
        ///   <c>true</c> if [display in nav]; otherwise, <c>false</c>.</value>
        public bool DisplayInNav { get; set; }

        /// <summary>Gets or sets the icon URL.</summary>
        /// <value>The icon URL.</value>
        public string IconUrl { get; set; }

        /// <summary>Gets or sets the layout unique identifier.</summary>
        /// <value>The layout unique identifier.</value>
        public Guid LayoutGuid { get; set; }

        /// <summary>Gets or sets the parent page unique identifier.</summary>
        /// <value>The parent page unique identifier.</value>
        public Guid? ParentPageGuid { get; set; }

        /// <summary>Gets or sets the depth level of the heirarchy.</summary>
        /// <value>The depth level.</value>
        public int DepthLevel { get; set; }

        /// <summary>Gets or sets the order.</summary>
        /// <value>The order.</value>
        public int Order { get; set; }
    }

    /// <summary>POCO for storing a layout</summary>
    public class Layout {
        /// <summary>Gets or sets the layout unique identifier.</summary>
        /// <value>The layout unique identifier.</value>
        public Guid LayoutGuid { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the layout xaml.</summary>
        /// <value>The layout xaml.</value>
        public string LayoutXaml { get; set; }
    }

    /// <summary>POCO for storing Appearance Settings</summary>
    public class AppearanceSettings {
        /// <summary>Gets or sets the color of the bar background.</summary>
        /// <value>The color of the bar background.</value>
        public string BarBackgroundColor { get; set; }

        /// <summary>Gets or sets the color of the bar text.</summary>
        /// <value>The color of the bar text.</value>
        public string BarTextColor { get; set; }

        /// <summary>Gets or sets the logo URL.</summary>
        /// <value>The logo URL.</value>
        public string LogoUrl { get; set; }
    }
    #endregion
}
 