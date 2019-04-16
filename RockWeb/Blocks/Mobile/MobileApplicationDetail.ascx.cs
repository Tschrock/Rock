using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Rock;
using Rock.Constants;
using Rock.Data;
using Rock.Mobile.Common;
using Rock.Mobile.Common.Enums;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

[DisplayName( "Mobile Application Detail" )]
[Category( "Mobile" )]
[Description( "Presents the details of mobile application detail." )]
public partial class Blocks_Mobile_MobileApplicationDetail : RockBlock, IDetailBlock
{
    #region AttributeKeys
    protected static class AttributeKey
    {
        public const string DefaultFileType = "DefaultFileType";
    }
    #endregion

    private List<PageInfo> _pageWithDepth = new List<PageInfo>();

    // Need blocks and Pages added to packages for auth rules
    private List<SourceBlockInfo> _blocksAddedToPackages = new List<SourceBlockInfo>();
    private List<SourePageInfo> _pagesAddedToPackages = new List<SourePageInfo>();
    protected override void OnInit( EventArgs e )
    {
        base.OnInit( e );
        btnApplicationDelete.Attributes["onclick"] = string.Format( "javascript: return Rock.dialogs.confirmDelete(event, 'Application');" );
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnLoad( EventArgs e )
    {
        bool isEdit = ViewState["IsEdit"] == null ? false : ViewState["IsEdit"].ToString().AsBoolean();
        this.lbTabLayout.Enabled = isEdit;
        this.lbTabPages.Enabled = isEdit;

        base.OnLoad( e );
        if ( !IsPostBack )
        {
            if ( !TabSelected )
            {
                ShowDetail( PageParameter( "siteId" ).AsInteger() );

                if ( !IsUserAuthorized( Authorization.EDIT ) )
                {
                    btnEdit.Visible = false;
                    btnApplicationDelete.Visible = false;
                    btnPublish.Visible = false;
                }
            }
        }
    }

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether [tab selected].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [tab selected]; otherwise, <c>false</c>.
    /// </value>
    private bool TabSelected
    {
        get
        {
            return hfTabSelected.Value.AsBoolean();
        }
        set
        {
            hfTabSelected.Value = value.ToString();
        }
    }
    private bool CancelEdit = false;
    private List<Rock.Model.Layout> ExistingLayouts { get; set; }
    private List<Rock.Model.Page> ExistingPages { get; set; }

    #endregion

    #region Data Structures

    /// <summary>
    /// Used to keeping a list of Blocks added
    /// in support adding Auth
    /// </summary>
    public class SourceBlockInfo
    {
        public PackageType PackageType { get; set; }
        public Rock.Model.Block Block { get; set; }
    }

    /// <summary>
    /// Used to keep a list of Pages that were added
    /// in support of adding Auth
    /// </summary>
    public class SourePageInfo
    {
        public PackageType PackageType { get; set; }
        public Rock.Model.Page Page { get; set; }
    }

    /// <summary>
    /// Used Set Page and associated Depth
    /// In support of Publish Process
    /// </summary>
    public class PageInfo
    {
        public Rock.Model.Page Page { get; set; }
        public int Depth { get; set; }
    }

    /// <summary>
    /// Used to process package Information
    /// </summary>
    public class PackageSourceInfo
    {
        public Rock.Model.Layout Layout { get; set; }
        public ICollection<Rock.Model.Page> LayoutPages { get; set; }
        public ICollection<Rock.Model.Block> Pageblocks { get; set; }
        public ICollection<Rock.Model.Block> Blocks { get; set; }
    }

    /// <summary>
    /// Local Enum to determine package type
    /// </summary>
    public enum PackageType
    {
        Phone = 0,
        Tablet = 1
    }

    #endregion

    #region Methods
    /// <summary>
    /// Binds the title bar.
    /// </summary>
    /// <param name="site">The site.</param>
    private void BindTitleBar( Site site )
    {
        this.lReadOnlyTitle.Text = site.Name;
        var latetestVersion = site.LatestVersionDateTime;
        if ( latetestVersion != null )
        {
            this.ltVersion.Text = string.Format( "<span class='label label-info'>Latest Version{0}</span>", ( ( DateTime ) site.LatestVersionDateTime ).ToString( @"MM/dd/yyyy hh:mm tt" ) );
        }
    }

    #region Application Tab Methods

    /// <summary>
    /// Shows the detail.
    /// </summary>
    /// <param name="siteId">The site identifier.</param>
    public void ShowDetail( int siteId )
    {
        pnlApplicationDetails.Visible = true;
        var liApplication = this.FindControl( "liApplication" ) as HtmlGenericControl;
        liApplication.Attributes.Add( "class", "active" );

        InitializeRadioButtonLists();

        Guid fileTypeGuid = GetAttributeValue( AttributeKey.DefaultFileType ).AsGuid();
        imgIcon.BinaryFileTypeGuid = fileTypeGuid;

        Site site = null;
        if ( siteId == 0 )
        {
            DisplayDeleteAndPublishButton( false );
        }

        if ( !siteId.Equals( 0 ) )
        {
            site = new SiteService( new RockContext() ).Get( siteId );
            BindTitleBar( site );
            InitializeApplicationTabReadOnlyView( siteId );
        }

        if ( site == null )
        {
            site = new Site { Id = 0, SiteType = SiteType.Mobile };
            site.SiteDomains = new List<SiteDomain>();
        }

        hfSiteId.Value = site.Id.ToString();

        tbApplicationName.ReadOnly = site.IsSystem;
        tbApplicationName.Text = site.Name;

        tbDescription.ReadOnly = site.IsSystem;
        tbDescription.Text = site.Description;

        cbIsActive.Checked = site.IsActive;

        imgIcon.BinaryFileId = site.SiteLogoBinaryFileId;
        imgPreviewThumbnail.BinaryFileId = site.ThumbnailFileId;

        BindAdditionalSettings( site );
        UpdateExistingLayouts( site.Id );
        BuildLayoutMenue();
        BuildLayoutDisplay();
        LoadLayoutDropDown();
        BuildPageMenu();
    }

    /// <summary>
    /// Displays the delete and publish button.
    /// </summary>
    /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
    private void DisplayDeleteAndPublishButton( bool isVisible )
    {
        btnApplicationDelete.Visible = isVisible;
        btnPublish.Visible = isVisible;
    }

    /// <summary>
    /// Enables the page tab.
    /// </summary>
    /// <param name="enabled">if set to <c>true</c> [enabled].</param>
    private void EnablePageTab( bool enabled )
    {
        this.lbTabPages.Enabled = enabled;
    }

    /// <summary>
    /// Initializes the application tab read only view.
    /// </summary>
    /// <param name="site">The site.</param>
    private void InitializeApplicationTabReadOnlyView( int? siteId )
    {
        var site = SiteCache.Get( ( int ) siteId );
        var additionalSettings = GetAdditionalSettingFromSite( site.AdditionalSettings );
        if ( additionalSettings != null )
        {
            if ( additionalSettings.ShellType != null )
            {
                this.ltApplicationType.Text = string.Format( "</br><span>{0}</span></br>", additionalSettings.ShellType.ConvertToString() );
            }
        }

        // Layout information
        this.ExistingLayouts = new LayoutService( new RockContext() ).GetBySiteId( ( int ) siteId ).ToList();
        BuildLayoutDisplay();

        // Setup Image
        if ( site.SiteLogoBinaryFileId != null )
        {
            ltIconImage.Text = string.Format( "<a href='{0}'><img src='/GetImage.ashx?id={1}' height='100' width='100' /></a>", site.SiteLogoBinaryFileUrl, site.SiteLogoBinaryFileId );
        }

        if ( site.ThumbnailFileId != null )
        {
            ltMobileImage.Text = string.Format( "<a href='{0}'><img src='/GetImage.ashx?id={1}' height='auto' width='100' /></a>", site.ThumbnailFileUrl, site.ThumbnailFileId );
        }

    }

    /// <summary>
    /// Initializes the RadioButton lists.
    /// </summary>
    private void InitializeRadioButtonLists()
    {
        var shellTypes = Enum.GetValues( typeof( ShellType ) );
        foreach ( var shellType in shellTypes )
        {
            rblApplicationType.Items.Add( new ListItem { Text = shellType.ToString(), Selected = false } );
        }

        var tabLocations = Enum.GetValues( typeof( TabLocation ) );

        foreach ( var tabLocationType in tabLocations )
        {
            rblAndroidTabLocation.Items.Add( new ListItem { Text = tabLocationType.ToString(), Selected = false } );
        }
    }
    /// <summary>
    /// Updates the additional settings.
    /// </summary>
    /// <param name="site">The site.</param>
    private void UpdateAdditionalSettings( Site site )
    {
        bool hasAdditionalSetting = false;
        var selecteShellType = rblApplicationType.SelectedValueAsEnumOrNull<ShellType>();
        var selectedTab = rblAndroidTabLocation.SelectedValueAsEnumOrNull<TabLocation>();

        var additionalSettings = new AdditionalSettings();

        if ( selecteShellType != null )
        {
            additionalSettings.ShellType = ( ShellType ) selecteShellType;
            hasAdditionalSetting = true;
        }

        // We only show Tab Android Tab location when Shell Type is Tabb
        if ( selectedTab != null && additionalSettings.ShellType == ShellType.Tabbed )
        {
            additionalSettings.TabLocation = ( TabLocation ) selectedTab;
            hasAdditionalSetting = true;
        }

        if ( ceCssPreHtml.Text.IsNotNullOrWhiteSpace() )
        {
            additionalSettings.CssStyle = ceCssPreHtml.Text;
            hasAdditionalSetting = true;
        }

        if ( hasAdditionalSetting )
        {
            var serializedAdditionalSetting = JsonConvert.SerializeObject( additionalSettings ).ToString();
            site.AdditionalSettings = serializedAdditionalSetting;
        }
    }

    /// <summary>
    /// De serialize and binds the additional settings.
    /// </summary>
    /// <param name="site">The site.</param>
    private void BindAdditionalSettings( Site site )
    {
        if ( site.AdditionalSettings.IsNotNullOrWhiteSpace() )
        {
            var additionalSetting = GetAdditionalSettingFromSite( site.AdditionalSettings );

            if ( additionalSetting != null )
            {
                // Css styles
                if ( additionalSetting.CssStyle.IsNotNullOrWhiteSpace() )
                {
                    ceCssPreHtml.Text = additionalSetting.CssStyle;
                }

                // Tab Location
                if ( additionalSetting.TabLocation != null )
                {
                    rblAndroidTabLocation.SelectedValue = additionalSetting.TabLocation.ConvertToString();
                }

                if ( additionalSetting.ShellType != null )
                {
                    rblApplicationType.SelectedValue = additionalSetting.ShellType.ConvertToString();
                    if ( additionalSetting.ShellType == ShellType.Tabbed )
                    {
                        if ( additionalSetting.TabLocation != null )
                        {
                            rblAndroidTabLocation.SelectedValue = additionalSetting.TabLocation.ConvertToString();
                        }
                        rblAndroidTabLocation.Visible = true;
                    }
                    else
                    {
                        rblAndroidTabLocation.Visible = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the additional setting from site.
    /// </summary>
    /// <param name="additionalSettings">The additional settings.</param>
    /// <returns></returns>
    private AdditionalSettings GetAdditionalSettingFromSite( string additionalSettings )
    {
        if ( additionalSettings.IsNotNullOrWhiteSpace() )
        {
            return JsonConvert.DeserializeObject<AdditionalSettings>( additionalSettings );
        }
        return null;
    }

    #endregion

    #region Layout Tab Methods

    /// <summary>
    /// Enables the layout tab.
    /// </summary>
    /// <param name="enabled">if set to <c>true</c> [enabled].</param>
    private void EnableLayoutTab( bool enabled )
    {
        this.lbTabLayout.Enabled = enabled;
    }

    /// <summary>
    /// Builds the layout display.
    /// </summary>
    private void BuildLayoutDisplay()
    {
        if ( this.ExistingLayouts != null && this.ExistingLayouts.Count > 0 )
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( "<ul id='ulLayouts'>" );
            foreach ( var layout in this.ExistingLayouts )
            {
                sb.AppendLine( string.Format( "<li id='li_{0}' ><label class='control-label'>{1}</label></li>", layout.Id, layout.Name ) );
            }

            sb.AppendLine( "</ul>" );

            this.ltLayouts.Text = sb.ToString();
            btnPublish.Visible = true;
        }
        else
        {
            this.ltLayouts.Text = string.Empty;
            btnPublish.Visible = false;
        }
    }

    /// <summary>
    /// Displays the pages tab.
    /// </summary>
    private void DisplayPagesTab()
    {
        pnlPages.Visible = true;
        pnlLayout.Visible = false;
        pnlApplicationDetails.Visible = false;
        pnlApplicationEditDetails.Visible = false;
        EnablePageTab( true );
    }

    /// <summary>
    /// Resets the layout for add.
    /// </summary>
    private void ResetLayoutForAdd()
    {
        hfLayoutId.Value = string.Empty;
        tbLayoutName.Text = string.Empty;
        tbLayoutDescription.Text = string.Empty;
        cePhoneLayoutXaml.Text = string.Empty;
        ceTabletLayoutXaml.Text = string.Empty;
        tbLayoutName.Focus();
    }

    /// <summary>
    /// Deletes the current layout.
    /// </summary>
    private void DeleteCurrentLayout()
    {
        if ( hfLayoutId.Value.AsIntegerOrNull() != null )
        {
            var rockContext = new RockContext();
            var layoutService = new LayoutService( rockContext );
            var currentLayout = layoutService.Get( hfLayoutId.Value.AsInteger() );
            if ( currentLayout != null )
            {
                var siteId = currentLayout.SiteId;
                layoutService.Delete( currentLayout );
                rockContext.SaveChanges();
                UpdateExistingLayouts( siteId );
                BuildLayoutMenue();
                SetToFirstLayout();
            }
        }
    }

    /// <summary>
    /// Sets to first layout.
    /// </summary>
    private void SetToFirstLayout()
    {
        if ( this.ExistingLayouts.Count == 0 )
        {
            ResetLayoutForAdd();
            return;
        }

        var firstLayout = this.ExistingLayouts.First();
        LoadSelectedLayout( firstLayout.Guid );
    }

    private void UpdateExistingLayouts( int siteId )
    {
        var layoutService = new LayoutService( new RockContext() );
        this.ExistingLayouts = layoutService.GetBySiteId( ( int ) siteId ).Select( l => l ).ToList();
    }
    /// <summary>
    /// Displays the layout tab.
    /// </summary>
    private void DisplayLayoutTab( int? layoutId, int? siteId )
    {
        var layoutService = new LayoutService( new RockContext() );

        if ( !siteId.HasValue )
        {
            siteId = hfSiteId.Value.AsInteger();
        }

        UpdateExistingLayouts( ( int ) siteId );

        Rock.Model.Layout layout = null;

        if ( layoutId != null && !layoutId.Equals( 0 ) )
        {
            layout = layoutService.Get( ( int ) layoutId );
        }

        if ( layout == null && siteId.HasValue )
        {
            var site = SiteCache.Get( siteId.Value );

            if ( site != null )
            {
                layout = new Rock.Model.Layout { Id = 0 };
                layout.SiteId = siteId.Value;
            }
        }

        if ( layout != null )
        {
            hfSiteId.Value = layout.SiteId.ToString();
            hfLayoutId.Value = layout.Id.ToString();
        }

        pnlLayout.Visible = true;
        pnlApplicationDetails.Visible = false;
        pnlApplicationEditDetails.Visible = false;
        pnlPages.Visible = false;
    }

    /// <summary>
    /// Builds the layout menue.
    /// </summary>
    private void BuildLayoutMenue()
    {
        rptLayoutMenu.DataSource = this.ExistingLayouts;
        rptLayoutMenu.DataBind();

        if ( rptLayoutMenu.Items.Count > 0 )
        {
            var layoutId = this.ExistingLayouts.First().Id;
            var selectedLayout = rptLayoutMenu.Items[0].FindControl( string.Format( "layoutItem_{0}", layoutId ) ) as LinkButton;
            if ( selectedLayout != null )
            {
                selectedLayout.Focus();
            }
            SetToFirstLayout();
            // If we have at least one show add button
            DisplayLayoutAddButton( true );
            lbtnDeleteCurrentLayout.Visible = true;
            btnPublish.Visible = true;
        }
        else
        {
            // This is either the first one or all have been deleted
            DisplayLayoutAddButton( false );
            lbtnDeleteCurrentLayout.Visible = false;
            btnPublish.Visible = false;
        }
    }

    /// <summary>
    /// Displays the layout add button.
    /// </summary>
    /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
    private void DisplayLayoutAddButton( bool isVisible )
    {
        lbtnAddLayout.Visible = isVisible;
    }

    #endregion

    #region Page Tab Methods

    public void ResetPageTabForAdd()
    {
        tbPageName.Text = string.Empty;
        cbDisplayInNavigation.Checked = false;
        ddlPageLayout.SelectedIndex = 0;
    }

    /// <summary>
    /// Loads the layout drop down.
    /// </summary>
    private void LoadLayoutDropDown()
    {
        if ( this.ExistingLayouts == null )
        {
            return;
        }
        ddlPageLayout.Items.Clear();
        ddlPageLayout.Items.Add( new ListItem( string.Empty, None.IdValue ) );

        foreach ( var layout in this.ExistingLayouts )
        {
            ddlPageLayout.Items.Add( new ListItem( layout.Name, layout.Id.ToString() ) );
        }
    }

    /// <summary>
    /// Builds the page menu.
    /// </summary>
    private void BuildPageMenu()
    {
        var pageService = new PageService( new RockContext() );
        var sitePages = pageService.GetBySiteId( hfSiteId.ValueAsInt() ).ToList();
        this.ExistingPages = sitePages;

        lbtnAddPage.Visible = sitePages.Count > 0;
        rptPageMenue.DataSource = sitePages;
        rptPageMenue.DataBind();
        SetSelectedFirstPage();
    }

    private void LoadSelectedPage( Guid pageGuid )
    {
        var page = PageCache.Get( pageGuid );
        if ( page == null )
        {
            return;
        }

        this.tbPageName.Text = page.PageTitle;
        this.cbDisplayInNavigation.Checked = page.DisplayInNavWhen == DisplayInNavWhen.WhenAllowed;
        ddlPageLayout.SelectedValue = page.LayoutId.ToString();
        hfPageId.Value = page.Id.ToString();
    }

    private void SetSelectedFirstPage()
    {
        if ( this.ExistingPages == null )
        {
            return;
        }

        var firstPage = this.ExistingPages.First();
        LoadSelectedPage( firstPage.Guid );
    }

    #endregion

    /// <summary>
    /// Displays the application tab.
    /// </summary>
    private void DisplayApplicationTab( bool showDetail )
    {
        pnlApplicationDetails.Visible = showDetail;
        pnlApplicationEditDetails.Visible = !showDetail;
        pnlLayout.Visible = false;
        pnlPages.Visible = false;
    }

    /// <summary>
    /// Clears the active tabs.
    /// </summary>
    private void ClearActiveTabs()
    {
        var liApplication = this.FindControl( "liApplication" ) as HtmlGenericControl;
        liApplication.Attributes.Remove( "class" );

        var liLayout = this.FindControl( "liLayout" ) as HtmlGenericControl;
        liLayout.Attributes.Remove( "class" );

        var liPages = this.FindControl( "liPages" ) as HtmlGenericControl;
        liPages.Attributes.Remove( "class" );

    }
    /// <summary>
    /// Publishes the packages.
    /// </summary>
    private void PublishPackages()
    {
        mdConfirmPublish.Hide();

        var rockContext = new RockContext();
        var siteService = new SiteService( rockContext );
        var layoutService = new LayoutService( rockContext );
        var pageService = new PageService( rockContext );
        var blockService = new BlockService( rockContext );
        var binaryFileService = new BinaryFileService( rockContext );

        var siteId = hfSiteId.Value.AsIntegerOrNull();
        if ( siteId == null && siteId == 0 )
        {
            return;
        }

        List<PackageSourceInfo> phonePackageSourceInfos = new List<PackageSourceInfo>();
        List<PackageSourceInfo> tabletPackageSourceInfos = new List<PackageSourceInfo>();

        var publishSite = siteService.Get( ( int ) siteId );
        if ( publishSite != null )
        {
            var packageSourceInfos = layoutService.GetBySiteId( ( int ) siteId ).Select( l =>
            new PackageSourceInfo
            {
                Layout = l,
                LayoutPages = l.Pages,
                Blocks = l.Blocks
            } ).ToList();

            var existingLayouts = packageSourceInfos.Select( p => p.Layout ).ToList();
            var allPages = pageService.GetBySiteId( siteId );

            BuildListOfPagesAndDepth( allPages );

            this.ExistingLayouts = existingLayouts;
            foreach ( var packageSource in packageSourceInfos )
            {
                var layout = packageSource.Layout;
                if ( layout.LayoutMobilePhone.IsNotNullOrWhiteSpace() )
                {
                    phonePackageSourceInfos.Add( packageSource );
                }

                if ( layout.LayoutMobileTablet.IsNotNullOrWhiteSpace() )
                {
                    tabletPackageSourceInfos.Add( packageSource );
                }
            }

            var phonePackage = new UpdatePackage();
            var tabletPackage = new UpdatePackage();

            var latestVersionDate = RockDateTime.Now;

            // Unix time stamp UTC - epoc-time of 1970-01-01.
            var unixTimestamp = ( Int32 ) ( latestVersionDate.ToUniversalTime().Subtract( new DateTime( 1970, 1, 1 ) ) ).TotalSeconds;

            phonePackage.ApplicationVersionId = unixTimestamp;
            tabletPackage.ApplicationVersionId = unixTimestamp;

            var appSetting = this.GetAdditionalSettingFromSite( publishSite.AdditionalSettings );

            if ( appSetting != null )
            {
                phonePackage.ApplicationType = ( ShellType ) appSetting.ShellType;
                tabletPackage.ApplicationVersionId = unixTimestamp;

                if ( appSetting.CssStyle.IsNotNullOrWhiteSpace() )
                {
                    phonePackage.CssStyles = appSetting.CssStyle;
                }

                if ( appSetting.ShellType == ShellType.Tabbed )
                {
                    phonePackage.TabsOnBottomOnAndroid = appSetting.TabLocation == TabLocation.Bottom;
                }
            }

            // Package Phone
            foreach ( var mobilePackageSource in phonePackageSourceInfos )
            {
                AddLayoutAndPagesToPackage( phonePackage, mobilePackageSource, PackageType.Phone );
            }

            var pageGuids = phonePackage.Pages.Select( p => p.PageGuid ).ToList();
            var sourcePages = allPages.Where( p => pageGuids.Contains( p.Guid ) ).Select( p => p ).ToList();

            // Apply Auth rules for Phone
            AddAuthForPagesAndBlocks( phonePackage, PackageType.Phone );

            //Package Tablet
            foreach ( var tabletPackageSource in tabletPackageSourceInfos )
            {
                AddLayoutAndPagesToPackage( tabletPackage, tabletPackageSource, PackageType.Tablet );
            }

            // Apply Auth rules for Tablet
            AddAuthForPagesAndBlocks( tabletPackage, PackageType.Tablet );

            var binaryFileType = new BinaryFileTypeService( rockContext ).Get( Rock.SystemGuid.BinaryFiletype.DEFAULT.AsGuid() );

            var tablePackageFile = JsonConvert.SerializeObject( tabletPackage );

            //  var phonePackageBytes = this.JsonStringToByteArray( JsonConvert.SerializeObject( phonePackage ) );
            var tabletPackageBytes = Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( tabletPackage ) );

            byte[] phonePackageBytes = Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( phonePackage ) );

            // Add Packages to Binary File Data
            rockContext.WrapTransaction( () =>
            {
                BinaryFile phonePackagebinaryFile = null;
                BinaryFile tabletPackagebinaryFile = null;

                bool phoneUpdate = false;
                bool tabletUpdate = false;
                // If phone package already exists we will updated 
                if ( publishSite.ConfigurationMobileTabletFileId.HasValue )
                {
                    phonePackagebinaryFile = binaryFileService.Get( ( int ) publishSite.ConfigurationMobilePhoneFileId );
                    phoneUpdate = true;
                }
                else
                {
                    // Otherwise create new
                    phonePackagebinaryFile = new BinaryFile();
                }

                // Phone Binary File  
                phonePackagebinaryFile.BinaryFileTypeId = binaryFileType.Id;
                phonePackagebinaryFile.IsTemporary = false;
                phonePackagebinaryFile.MimeType = "application/json";
                phonePackagebinaryFile.FileSize = phonePackageBytes.Length;
                phonePackagebinaryFile.ContentStream = new MemoryStream( phonePackageBytes );
                phonePackagebinaryFile.FileName = "ConfigurationMobilePhone";
                if ( !phoneUpdate )
                {
                    binaryFileService.Add( phonePackagebinaryFile );
                }

                // Tablet Binary File
                if ( publishSite.ConfigurationMobileTabletFileId.HasValue )
                {
                    tabletPackagebinaryFile = binaryFileService.Get( ( int ) publishSite.ConfigurationMobileTabletFileId );
                    tabletUpdate = true;
                }
                else
                {
                    tabletPackagebinaryFile = new BinaryFile();
                }

                tabletPackagebinaryFile.BinaryFileTypeId = binaryFileType.Id;
                tabletPackagebinaryFile.IsTemporary = false;
                tabletPackagebinaryFile.MimeType = "application/json";
                tabletPackagebinaryFile.FileSize = phonePackageBytes.Length;
                tabletPackagebinaryFile.ContentStream = new MemoryStream( tabletPackageBytes );
                tabletPackagebinaryFile.FileName = "ConfigurationMobileTablet";
                if ( !tabletUpdate )
                {
                    binaryFileService.Add( tabletPackagebinaryFile );

                }
                rockContext.SaveChanges();

                // update Site latest Version date
                publishSite.LatestVersionDateTime = latestVersionDate;
                publishSite.ConfigurationMobilePhoneFileId = phonePackagebinaryFile.Id;
                publishSite.ConfigurationMobileTabletFileId = tabletPackagebinaryFile.Id;

                rockContext.SaveChanges();

            } );
        }
    }

    /// <summary>
    /// Adds the authentication for pages and blocks.
    /// </summary>
    /// <param name="package">The package.</param>
    /// <param name="packageType">Type of the package.</param>
    private void AddAuthForPagesAndBlocks( UpdatePackage package, PackageType packageType )
    {
        var pageEntityTypeId = EntityTypeCache.Get( Rock.SystemGuid.EntityType.PAGE.AsGuid() ).Id;
        var blockEntityTypeId = EntityTypeCache.Get( Rock.SystemGuid.EntityType.BLOCK.AsGuid() ).Id;

        // Auth 
        var rockContext = new RockContext();
        var authService = new AuthService( rockContext );

        var addedPageIds = _pagesAddedToPackages.Where( p => p.PackageType == packageType ).Select( p => p.Page.Id ).ToList();
        var sourcePages = _pagesAddedToPackages.Where( p => p.PackageType == packageType ).Select( p => p.Page ).ToList();

        // Page Auths rules for groups or special roles should be added
        var pageAuths = authService.GetByEntityIds( pageEntityTypeId, addedPageIds )
            .Where( a => a.GroupId != null || a.SpecialRole != SpecialRole.None ).AsNoTracking();

        foreach ( var auth in pageAuths )
        {
            var mobilePageAuth = new Rock.Mobile.Common.Auth
            {
                Action = auth.Action,
                AllowOrDeny = auth.AllowOrDeny,
                AuthGuid = auth.Guid,
                EntityGuid = sourcePages.Where( p => p.Id == auth.EntityId ).Select( p => p.Guid ).FirstOrDefault(),
                EntityTypeId = auth.EntityTypeId,
                Order = auth.Order,
                SpecialRole = auth.SpecialRole.ConvertToInt(),
            };

            if ( auth.Group != null )
            {
                mobilePageAuth.GroupGuid = auth.Group.Guid;
            }

            package.AuthenticationRules.Add( mobilePageAuth );
        }

        //Blocks rules for groups or special roles should be added
        var addedBlockIds = _blocksAddedToPackages.Select( b => b.Block.Id ).ToList();
        var blockAuths = authService.GetByEntityIds( blockEntityTypeId, addedBlockIds )
          .Where( a => a.GroupId != null || a.SpecialRole != SpecialRole.None ).AsNoTracking();

        var blocksAddToPackage = _blocksAddedToPackages.Where( b => b.PackageType == packageType ).Select( b => b.Block ).ToList();

        foreach ( var blockAuth in blockAuths )
        {
            var mobileBlockAuth = new Rock.Mobile.Common.Auth
            {
                Action = blockAuth.Action,
                AllowOrDeny = blockAuth.AllowOrDeny,
                AuthGuid = blockAuth.Guid,
                EntityGuid = blocksAddToPackage.Where( b => b.Id == blockAuth.EntityId ).Select( b => b.Guid ).FirstOrDefault(),
                EntityTypeId = blockEntityTypeId,
                Order = blockAuth.Order,
                SpecialRole = blockAuth.SpecialRole.ConvertToInt()
            };

            if ( blockAuth.Group != null )
            {
                mobileBlockAuth.GroupGuid = blockAuth.Group.Guid;
            }

            package.AuthenticationRules.Add( mobileBlockAuth );
        }
    }

    /// <summary>
    /// Sets the application tab active.
    /// </summary>
    private void SetApplicationTabActive()
    {
        lbTab_SelectedClick( this.lbTabApplication, null );
    }
    /// <summary>
    /// Loads the selected layout.
    /// </summary>
    /// <param name="selectedLayoutId">The selected layout identifier.</param>
    private void LoadSelectedLayout( Guid selectedLayoutGuid )
    {
        var selectedLayout = LayoutCache.Get( selectedLayoutGuid );
        if ( selectedLayout != null )
        {
            this.tbLayoutName.Text = selectedLayout.Name;
            this.tbLayoutDescription.Text = selectedLayout.Description;
            this.cePhoneLayoutXaml.Text = selectedLayout.LayoutMobilePhone;
            this.ceTabletLayoutXaml.Text = selectedLayout.LayoutMobileTablet;
            this.hfLayoutId.Value = selectedLayout.Id.ToString();
        }
    }

    /// <summary>
    /// Builds the list of pages and depth.
    /// Since a layout assigned may only refer to child pages
    /// We need a list of all pages from the root to get full depth
    /// </summary>
    /// <param name="allPages">All pages.</param>
    private void BuildListOfPagesAndDepth( IOrderedQueryable<Rock.Model.Page> allPages )
    {
        var rootpages = allPages.Where( p => p.ParentPageId == null ).ToList();
        foreach ( var rootPage in rootpages )
        {
            _pageWithDepth.Add( new PageInfo { Page = rootPage, Depth = 0 } );
            AssignPageDepth( rootPage, 0 );
        }
    }

    /// <summary>
    /// Adds the layout and pages to package.
    /// </summary>
    /// <param name="package">The package.</param>
    /// <param name="mobilePackageSource">The mobile package source.</param>
    /// <param name="packageType">Type of the package.</param>
    private void AddLayoutAndPagesToPackage( UpdatePackage package, PackageSourceInfo mobilePackageSource, PackageType packageType )
    {
        var layout = mobilePackageSource.Layout;

        if ( packageType == PackageType.Phone )
        {
            if ( layout.LayoutMobilePhone.IsNotNullOrWhiteSpace() )
            {
                package.Layouts.Add( new Rock.Mobile.Common.Layout { LayoutGuid = layout.Guid, LayoutXaml = layout.LayoutMobilePhone, Name = layout.Name } );
            }
        }
        if ( packageType == PackageType.Tablet )
        {
            if ( layout.LayoutMobileTablet.IsNotNullOrWhiteSpace() )
            {

                package.Layouts.Add( new Rock.Mobile.Common.Layout { LayoutGuid = layout.Guid, LayoutXaml = layout.LayoutMobileTablet, Name = layout.Name } );
            }
        }

        foreach ( var page in mobilePackageSource.LayoutPages )
        {
            AddPageAndBlocksToPackageSourceWithDepth( package, layout, page, packageType );
        }
    }

    /// <summary>
    /// Adds the page and blocks to package source with depth.
    /// </summary>
    /// <param name="phonePackage">The phone package.</param>
    /// <param name="layout">The layout.</param>
    /// <param name="page">The page.</param>
    /// <param name="packageType">Type of the package.</param>
    private void AddPageAndBlocksToPackageSourceWithDepth( UpdatePackage phonePackage, Rock.Model.Layout layout, Rock.Model.Page page, PackageType packageType )
    {
        if ( page.Pages.Count == 0 )
        {
            var depth = _pageWithDepth.Where( p => p.Page.Id == page.Id ).Select( d => d.Depth ).FirstOrDefault();
            // TODO: Map Display In Nav, IConUrl
            var existsInPackage = phonePackage.Pages.Where( p => p.PageGuid == page.Guid && p.LayoutGuid == layout.Guid ).Any();
            if ( existsInPackage )
            {
                return;
            }

            _pagesAddedToPackages.Add( new SourePageInfo { PackageType = packageType, Page = page } );

            phonePackage.Pages.Add( new Rock.Mobile.Common.Page
            {
                DepthLevel = depth,
                PageGuid = page.Guid,
                ParentPageGuid = page.ParentPage.Guid,
                LayoutGuid = layout.Guid,
                Order = page.Order,
                Title = page.PageTitle
            } );

            foreach ( var block in page.Blocks )
            {
                _blocksAddedToPackages.Add( new SourceBlockInfo { PackageType = packageType, Block = block } );

                phonePackage.Blocks.Add( new Rock.Mobile.Common.Block
                {
                    BlockGuid = block.Guid,
                    BlockType = block.BlockType.ToString(),
                    Order = block.Order,
                    PageGuid = block.Page.Guid,
                    Zone = block.Zone
                } );
            }

            return;
        }

        foreach ( var childPage in page.Pages )
        {
            var childDepth = _pageWithDepth.Where( p => p.Page.Id == childPage.Id ).Select( d => d.Depth ).FirstOrDefault();
            var childExistsInPackage = phonePackage.Pages.Where( p => p.PageGuid == childPage.Guid && p.LayoutGuid == layout.Guid ).Any();

            _pagesAddedToPackages.Add( new SourePageInfo { PackageType = packageType, Page = childPage } );
            if ( !childExistsInPackage )
            {
                phonePackage.Pages.Add( new Rock.Mobile.Common.Page
                {
                    DepthLevel = childDepth,
                    PageGuid = childPage.Guid,
                    ParentPageGuid = childPage.ParentPage.Guid,
                    LayoutGuid = layout.Guid,
                    Order = childPage.Order,
                    Title = childPage.PageTitle,
                } );

                foreach ( var block in childPage.Blocks )
                {
                    _blocksAddedToPackages.Add( new SourceBlockInfo { PackageType = packageType, Block = block } );
                    phonePackage.Blocks.Add( new Rock.Mobile.Common.Block
                    {
                        BlockGuid = block.Guid,
                        BlockType = block.BlockType.ToString(),
                        Order = block.Order,
                        PageGuid = block.Page.Guid,
                        Zone = block.Zone
                    } );
                }
            }

            AddPageAndBlocksToPackageSourceWithDepth( phonePackage, layout, childPage, packageType );
        }
    }

    /// <summary>
    /// Assigns the page depth.
    /// </summary>
    /// <param name="currentPage">The current page.</param>
    /// <param name="currentLevel">The current level.</param>
    private void AssignPageDepth( Rock.Model.Page currentPage, int currentLevel )
    {
        currentLevel += 1;
        if ( currentPage.Pages == null )
        {

            _pageWithDepth.Add( new PageInfo { Page = currentPage, Depth = currentLevel } );
            return;
        }

        foreach ( var child in currentPage.Pages )
        {
            _pageWithDepth.Add( new PageInfo { Page = child, Depth = currentLevel } );
            AssignPageDepth( child, currentLevel );
        }
    }

    #endregion

    #region Events

    #region Application Tab Events

    /// <summary>
    /// Handles the Click event of the btnApplicationDelete control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnApplicationDelete_Click( object sender, EventArgs e )
    {
        bool canDelete = false;

        var rockContext = new RockContext();
        SiteService siteService = new SiteService( rockContext );
        Site site = siteService.Get( hfSiteId.Value.AsInteger() );
        LayoutService layoutService = new LayoutService( rockContext );
        if ( site != null )
        {
            var layoutQry = layoutService.Queryable()
                   .Where( l =>
                       l.SiteId == site.Id );
            layoutService.DeleteRange( layoutQry );
            rockContext.SaveChanges( true );

            string errorMessage;
            canDelete = siteService.CanDelete( site, out errorMessage, includeSecondLvl: true );
            if ( !canDelete )
            {
                // mdDeleteWarning.Show( errorMessage, ModalAlertType.Alert );
                return;
            }

            siteService.Delete( site );
            rockContext.SaveChanges();
        }

        NavigateToParentPage();
    }

    /// <summary>
    /// Handles the SelectedClick event of the Tab control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbTab_SelectedClick( object sender, EventArgs e )
    {
        var isEdit = ViewState["IsEdit"] == null ? false : ViewState["IsEdit"].ToString().AsBoolean();

        var tab = sender as LinkButton;
        var tabSelected = tab.Text;
        switch ( tabSelected )
        {
            case "Application":
                ClearActiveTabs();
                DisplayApplicationTab( !isEdit );
                break;
            case "Layout":
                if ( !isEdit )
                {
                    return;
                }
                ClearActiveTabs();
                DisplayLayoutTab( hfLayoutId.Value.AsIntegerOrNull(), hfSiteId.Value.AsInteger() );
                break;
            case "Pages":
                if ( !isEdit )
                {
                    return;
                }
                ClearActiveTabs();
                DisplayPagesTab();
                break;
            default:
                break;
        }
        var parent = tab.Parent as HtmlGenericControl;
        parent.Attributes.Add( "class", "active" );
        this.TabSelected = true;
    }
    /// <summary>
    /// Handles the SelectedIndexChanged event of the rblApplicationType control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void rblApplicationType_SelectedIndexChanged( object sender, EventArgs e )
    {
        var list = sender as RockRadioButtonList;
        if ( list.SelectedValue == ShellType.Tabbed.ToString() )
        {
            rblAndroidTabLocation.Visible = true;
        }
        else
        {
            rblAndroidTabLocation.Visible = false;
        }
    }

    /// <summary>
    /// Handles the Click event of the btnEdit control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnEdit_Click( object sender, EventArgs e )
    {
        ViewState["IsEdit"] = true;

        pnlApplicationDetails.Visible = false;
        pnlApplicationEditDetails.Visible = true;

        if ( hfSiteId.Value.AsIntegerOrNull() == null && hfSiteId.Value.AsInteger() == 0 )
        {
            return;
        }

        EnableLayoutTab( true );
        EnablePageTab( true );
    }

    /// <summary>
    /// Handles the Click event of the btnSaveMobileDetails control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnSaveMobileDetails_Click( object sender, EventArgs e )
    {
        Site site;

        if ( Page.IsValid )
        {
            var rockContext = new RockContext();
            PageService pageService = new PageService( rockContext );
            SiteService siteService = new SiteService( rockContext );
            SiteDomainService siteDomainService = new SiteDomainService( rockContext );

            var newApplication = false;

            var siteId = hfSiteId.Value.AsInteger();

            if ( siteId == 0 )
            {
                newApplication = true;
                site = new Rock.Model.Site() { SiteType = SiteType.Mobile };
                siteService.Add( site );
            }
            else
            {
                site = siteService.Get( siteId );
            }

            site.Name = tbApplicationName.Text;
            site.Description = tbDescription.Text;

            int? existingSiteLogoId = null;
            if ( site.SiteLogoBinaryFileId != imgIcon.BinaryFileId )
            {
                existingSiteLogoId = site.FavIconBinaryFileId;
                site.SiteLogoBinaryFileId = imgIcon.BinaryFileId;
            }

            int? existingThumbnailId = null;
            if ( site.ThumbnailFileId != imgPreviewThumbnail.BinaryFileId )
            {
                existingThumbnailId = site.ThumbnailFileId;
                site.ThumbnailFileId = imgPreviewThumbnail.BinaryFileId;
            }

            UpdateAdditionalSettings( site );
            site.LatestVersionDateTime = RockDateTime.Now;

            rockContext.WrapTransaction( () =>
            {
                rockContext.SaveChanges();

                if ( existingSiteLogoId.HasValue )
                {
                    BinaryFileService binaryFileService = new BinaryFileService( rockContext );
                    var binaryFile = binaryFileService.Get( existingSiteLogoId.Value );
                    if ( binaryFile != null )
                    {
                        // marked the old images as IsTemporary so they will get cleaned up later
                        binaryFile.IsTemporary = true;
                        rockContext.SaveChanges();
                    }
                }

                if ( existingThumbnailId.HasValue )
                {
                    BinaryFileService binaryFileService = new BinaryFileService( rockContext );
                    var binaryFile = binaryFileService.Get( existingThumbnailId.Value );
                    if ( binaryFile != null )
                    {
                        // marked the old images as IsTemporary so they will get cleaned up later
                        binaryFile.IsTemporary = true;
                        rockContext.SaveChanges();
                    }
                }

                if ( newApplication )
                {
                    //    Rock.Security.Authorization.CopyAuthorization( RockPage.Layout.Site, site, rockContext, Authorization.EDIT );
                    //    Rock.Security.Authorization.CopyAuthorization( RockPage.Layout.Site, site, rockContext, Authorization.ADMINISTRATE );
                    //    Rock.Security.Authorization.CopyAuthorization( RockPage.Layout.Site, site, rockContext, Authorization.APPROVE );
                }

                rockContext.SaveChanges();
            } );

            hfSiteId.Value = site.Id.ToString();
            BindTitleBar( site );
            DisplayDeleteAndPublishButton( true );
        }
    }

    /// <summary>
    /// Handles the Click event of the btnPublish control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnPublish_Click( object sender, EventArgs e )
    {
        mdConfirmPublish.Show();
    }

    /// <summary>
    /// Handles the Click event of the btnSaveLayout control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnSaveLayout_Click( object sender, EventArgs e )
    {
        if ( Page.IsValid )
        {
            var rockContext = new RockContext();
            LayoutService layoutService = new LayoutService( rockContext );
            Rock.Model.Layout layout;

            int layoutId = hfLayoutId.Value.IsNullOrWhiteSpace() ? 0 : int.Parse( hfLayoutId.Value );

            // If adding a new layout
            if ( layoutId.Equals( 0 ) )
            {
                layout = new Rock.Model.Layout { Id = 0 };
                layout.SiteId = hfSiteId.ValueAsInt();
            }
            else
            {
                // Load existing layout
                layout = layoutService.Get( layoutId );
            }

            layout.Name = tbLayoutName.Text;
            layout.Description = tbLayoutDescription.Text;
            layout.LayoutMobilePhone = cePhoneLayoutXaml.Text;
            layout.LayoutMobileTablet = ceTabletLayoutXaml.Text;
            layout.FileName = "Blank";

            if ( !layout.IsValid )
            {
                return;
            }

            if ( layout.Id.Equals( 0 ) )
            {
                layoutService.Add( layout );
            }

            rockContext.SaveChanges();
            hfLayoutId.Value = layout.Id.ToString();
            UpdateExistingLayouts( layout.SiteId );
            BuildLayoutMenue();
            BuildLayoutDisplay();
            DisplayApplicationTab( false );
        }
    }

    /// <summary>
    /// Handles the Clicked event of the btnCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnCancel_Clicked( object sender, EventArgs e )
    {
        ViewState["IsEdit"] = false;
        if ( hfSiteId.Value.AsIntegerOrNull() != null )
        {
            UpdateExistingLayouts( hfSiteId.Value.AsInteger() );
        }
        BuildLayoutMenue();
        BuildLayoutDisplay();
        ClearActiveTabs();
        SetApplicationTabActive();
        lbTabLayout.Enabled = false;
        lbTabPages.Enabled = false;
    }

    /// <summary>
    /// Handles the SaveClick event of the mdConfirmPublish control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void mdConfirmPublish_SaveClick( object sender, EventArgs e )
    {
        PublishPackages();
    }

    #endregion

    #region Layout Tab Events

    /// <summary>
    /// Handles the SelectedClick event of the LayoutItem control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void LayoutItem_SelectedClick( object sender, EventArgs e )
    {
        var selectedLayoutId = ( ( LinkButton ) sender ).CommandArgument.AsGuid();
        LoadSelectedLayout( selectedLayoutId );
    }
    /// <summary>
    /// Handles the Click event of the btnAddLayout control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnAddLayout_Click( object sender, EventArgs e )
    {
        ResetLayoutForAdd();
    }

    /// <summary>
    /// Handles the Click event of the btnDeleteLayout control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnDeleteLayout_Click( object sender, EventArgs e )
    {
        DeleteCurrentLayout();
    }

    /// <summary>
    /// Handles the DataBinding event of the LayoutItem control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void LayoutItem_DataBinding( object sender, EventArgs e )
    {
        var linkButton = sender as LinkButton;
        if ( linkButton != null )
        {
            var layoutItem = ( ( RepeaterItem ) linkButton.DataItemContainer ).DataItem as Rock.Model.Layout;
            linkButton.ID = string.Format( "layoutItem_{0}", layoutItem.Id );
            linkButton.Text = layoutItem.ToString();
            linkButton.CommandArgument = layoutItem.Guid.ToString();
        }
    }

    #endregion

    #region Page Tab Events

    /// <summary>
    /// Handles the DataBinding event of the PageLink control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void PageLink_DataBinding( object sender, EventArgs e )
    {
        var linkButton = sender as LinkButton;
        if ( linkButton != null )
        {
            var pageItem = ( ( RepeaterItem ) linkButton.DataItemContainer ).DataItem as Rock.Model.Page;
            linkButton.ID = string.Format( "pageItem+{0}", pageItem.Id );
            linkButton.Text = pageItem.PageTitle;
            linkButton.CommandArgument = pageItem.Guid.ToString();
        }
    }

    /// <summary>
    /// Handles the Click event of the lbtnAddPage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnAddPage_Click( object sender, EventArgs e )
    {
        hfPageId.Value = "0";
        ResetPageTabForAdd();
    }

    /// <summary>
    /// Handles the Click event of the lbtnSavePage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnSavePage_Click( object sender, EventArgs e )
    {
        nbPageLayoutRequired.Visible = false;

        var selectedlayout = ddlPageLayout.SelectedValue.AsInteger();
        if ( selectedlayout == 0 )
        {
            nbPageLayoutRequired.Visible = true;
            return;
        }

        if ( Page.IsValid )
        {
            Rock.Model.Page page;
            var rockContext = new RockContext();
            var pageService = new PageService( rockContext );
            int pageId = hfPageId.Value.AsInteger();

            if ( pageId == 0 )
            {
                page = new Rock.Model.Page();
                page.ParentPageId = null;

                var lastPage = this.ExistingPages != null ? this.ExistingPages.Last() : null;
                if ( lastPage == null )
                {
                    page.Order = 0;
                }
                else
                {
                    page.Order = lastPage.Order + 1;
                }
                pageService.Add( page );
            }
            else
            {
                page = pageService.Get( pageId );
            }

            page.LayoutId = selectedlayout;
            page.PageTitle = tbPageName.Text;
            page.BrowserTitle = page.PageTitle;
            if ( cbDisplayInNavigation.Checked )
            {
                page.DisplayInNavWhen = DisplayInNavWhen.WhenAllowed;
            }
            else
            {
                page.DisplayInNavWhen =  DisplayInNavWhen.Never;
            }
            
            page.InternalName = tbPageName.Text;
            page.LayoutId = selectedlayout;

            if ( page.IsValid )
            {
                rockContext.SaveChanges();
                hfPageId.Value = page.Id.ToString();
            }

            BuildPageMenu();
            ResetPageTabForAdd();
            SetSelectedFirstPage();

        }
    }

    /// <summary>
    /// Handles the Click event of the lbtnDeleteCurrentPage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void lbtnDeleteCurrentPage_Click( object sender, EventArgs e )
    {
        var pageId = hfPageId.Value.AsIntegerOrNull();
        if ( pageId.HasValue == false )
        {
            return;
        }

        var rockContext = new RockContext();
        var pageSerivce = new PageService( rockContext );
        var page = pageSerivce.Get((int) pageId );
        if ( page == null )
        {
            return;
        }

        string errorMessage = string.Empty;
        if ( !pageSerivce.CanDelete(page,out errorMessage) )
        {
            mdDeleteWarning.Show( errorMessage, ModalAlertType.Alert );
           return;
        }

        pageSerivce.Delete( page );
        rockContext.SaveChanges();
        BuildPageMenu();
     }

    /// <summary>
    /// Handles the Click event of the btnPageCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnPageCancel_Click( object sender, EventArgs e )
    {

    }

    /// <summary>
    /// Handles the Click event of the PageMenu control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void PageMenu_Click( object sender, EventArgs e )
    {
        var pageGuid = ( ( LinkButton ) sender ).CommandArgument.AsGuid();
        LoadSelectedPage( pageGuid );
    }

    #endregion

    #endregion

}
