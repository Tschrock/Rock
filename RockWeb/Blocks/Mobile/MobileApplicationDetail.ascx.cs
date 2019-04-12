using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Rock;
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
    private int _depth = 0;
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
    private List<Rock.Model.Layout> ExistingLayouts { get; set; }

    #endregion

    #region Methods

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
    /// Enables the layout tab.
    /// </summary>
    /// <param name="enabled">if set to <c>true</c> [enabled].</param>
    private void EnableLayoutTab( bool enabled )
    {
        if ( !enabled )
        {
            this.liLayout.Attributes.Add( "class", "disabled" );
            this.tabLayout.Attributes.Add( "class", "disabled" );
            return;
        }

        this.liLayout.Attributes.Remove( "class" );
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
    /// Displays the pages tab.
    /// </summary>
    private void DisplayPagesTab()
    {
        pnlPages.Visible = true;
        pnlLayout.Visible = false;
        pnlApplicationDetails.Visible = false;
        pnlApplicationEditDetails.Visible = false;
    }

    /// <summary>
    /// Updates the existing layouts.
    /// </summary>
    /// <param name="siteId">The site identifier.</param>
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

        bool readOnly = false;

        if ( layout != null )
        {
            hfSiteId.Value = layout.SiteId.ToString();
            hfLayoutId.Value = layout.Id.ToString();
        }

        LoadSelectedLayout( layoutId );
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

            LoadSelectedLayout( ( int ) layoutId );

            // If we have at least one show add button
            DisplayLayoutAddButton( true );
            btnDeleteCurrentLayout.Visible = true;
            btnPublish.Visible = true;
        }
        else
        {
            // This is either the first one or all have been deleted
            DisplayLayoutAddButton( false );
            btnDeleteCurrentLayout.Visible = false;
            btnPublish.Visible = false;
        }
    }

    private void DisplayLayoutAddButton( bool isVisible )
    {
        // Display Add button if we at least have one
        var btnAddLayout = this.FindControl( "aAddLayout" );
        if ( btnAddLayout != null )
        {
            btnAddLayout.Visible = isVisible;
        }
    }

    /// <summary>
    /// Displays the application tab.
    /// </summary>
    private void DisplayApplicationTab()
    {
        pnlApplicationDetails.Visible = true;
        pnlApplicationEditDetails.Visible = false;
        pnlLayout.Visible = false;
        pnlPages.Visible = false;
        EnableLayoutTab( false );
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

        //var liPages = this.FindControl( "liPages" ) as HtmlGenericControl;
        //liPages.Attributes.Remove( "class" );
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
        if ( this.ExistingLayouts.Count > 0 )
        {
            var firstLayout = this.ExistingLayouts.First();
            LoadSelectedLayout( firstLayout.Id );
        }
        else
        {
            ResetLayoutForAdd();
        }
    }

    #endregion

    #region Events

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
    /// Handles the SelectedClick event of the Tab control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Tab_SelectedClick( object sender, EventArgs e )
    {
        var tab = sender as HtmlAnchor;
        var tabSelected = tab.InnerText;
        switch ( tabSelected )
        {
            case "Application":
                ClearActiveTabs();
                DisplayApplicationTab();
                break;
            case "Layout":
                if ( !this.pnlApplicationEditDetails.Visible )
                {
                    return;
                }
                ClearActiveTabs();
                DisplayLayoutTab( hfLayoutId.Value.AsIntegerOrNull(), hfSiteId.Value.AsInteger() );
                break;
            case "Pages":
                if ( !this.pnlApplicationEditDetails.Visible )
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

    private void SetApplicationTabActive()
    {
        Tab_SelectedClick( this.tabApplication, null );
    }

    /// <summary>
    /// Handles the SelectedClick event of the LayoutItem control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void LayoutItem_SelectedClick( object sender, EventArgs e )
    {
        var selectedLayoutId = ( ( LinkButton ) sender ).CommandArgument.AsIntegerOrNull();
        if ( selectedLayoutId.HasValue )
        {
            LoadSelectedLayout( selectedLayoutId );
        }
    }

    /// <summary>
    /// Loads the selected layout.
    /// </summary>
    /// <param name="selectedLayoutId">The selected layout identifier.</param>
    private void LoadSelectedLayout( int? selectedLayoutId )
    {
        if ( selectedLayoutId != null )
        {
            var layoutService = new LayoutService( new RockContext() );
            var selectedLayout = layoutService.Get( ( int ) selectedLayoutId );
            if ( selectedLayout != null )
            {
                this.tbLayoutName.Text = selectedLayout.Name;
                this.tbLayoutDescription.Text = selectedLayout.Description;
                this.cePhoneLayoutXaml.Text = selectedLayout.LayoutMobilePhone;
                this.ceTabletLayoutXaml.Text = selectedLayout.LayoutMobileTablet;
                this.hfLayoutId.Value = selectedLayout.Id.ToString();
            }
        }
    }

    /// <summary>
    /// Handles the Click event of the btnEdit control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnEdit_Click( object sender, EventArgs e )
    {
        pnlApplicationDetails.Visible = false;
        pnlApplicationEditDetails.Visible = true;
        if ( hfSiteId.Value.AsIntegerOrNull() != null && hfSiteId.Value.AsInteger() > 0 )
        {
            EnableLayoutTab( true );
        }
    }

    /// <summary>
    /// Handles the Click event of the btnSaveMobileDetails control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnSaveMobileDetails_Click( object sender, EventArgs e )
    {
        Site site;

        if ( Page.IsValid )
        {
            var rockContext = new RockContext();
            PageService pageService = new PageService( rockContext );
            SiteService siteService = new SiteService( rockContext );
            SiteDomainService siteDomainService = new SiteDomainService( rockContext );

            bool newApplication = false;

            int siteId = hfSiteId.Value.AsInteger();
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

        SetApplicationTabActive();
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
    /// Handles the Click event of the btnPublish control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnPublish_Click( object sender, EventArgs e )
    {
        var siteId = hfSiteId.Value.AsIntegerOrNull();
        if ( siteId == null && siteId == 0 )
        {
            return;
        }

        List<Rock.Model.Layout> mobilePackageLayoutInfo = new List<Rock.Model.Layout>();
        List<Rock.Model.Layout> tabletPackageLayoutInfo = new List<Rock.Model.Layout>();

        var rockContext = new RockContext();
        var siteToPublish = new SiteService( rockContext ).Get( ( int ) siteId );
        if ( siteToPublish != null )
        {
            var existingLayouts = new LayoutService( rockContext ).GetBySiteId( ( int ) siteId ).ToList();
            this.ExistingLayouts = existingLayouts;


            foreach ( var layout in existingLayouts )
            {
                if ( layout.LayoutMobilePhone.IsNotNullOrWhiteSpace() )
                {
                    mobilePackageLayoutInfo.Add( layout );
                }
                if ( layout.LayoutMobileTablet.IsNotNullOrWhiteSpace() )
                {
                    tabletPackageLayoutInfo.Add( layout );
                }
            }
        }

        var mobilePhonePackage = new UpdatePackage();
        var mobileTabletPackage = new UpdatePackage();

        var pages = new PageService( rockContext ).GetBySiteId( siteId ).Select( p => new PageInfo { Page = p, Block = p.Blocks } ).ToList();
        var rootPages = pages.Where( p => p.Page.ParentPageId == null ).Select( p => p ).ToList();

        foreach ( var rootPage in rootPages )
        {
            _depth = 0;
            rootPage.Depth = _depth;
            _pageWithDepth.Add( rootPage );
            AssignPageDepth( rootPage.Page,0);
        }

        var latestVersionDate = RockDateTime.Now;
        // Unix time stamp UTC - epoc-time of 1970-01-01.
        Int32 unixTimestamp = ( Int32 ) ( latestVersionDate.ToUniversalTime().Subtract( new DateTime( 1970, 1, 1 ) ) ).TotalSeconds;
        siteToPublish.LatestVersionDateTime = latestVersionDate;

        var appSetting = this.GetAdditionalSettingFromSite( siteToPublish.AdditionalSettings );

        mobilePhonePackage.ApplicationVersionId = unixTimestamp;
        if ( appSetting != null )
        {
            mobilePhonePackage.ApplicationType = ( ShellType ) appSetting.ShellType;
            mobileTabletPackage.ApplicationVersionId = unixTimestamp;

            if ( appSetting.CssStyle.IsNotNullOrWhiteSpace() )
            {
                mobilePhonePackage.CssStyles = appSetting.CssStyle;
            }

            if ( appSetting.ShellType == ShellType.Tabbed )
            {
                mobilePhonePackage.TabsOnBottomOnAndroid = appSetting.TabLocation == TabLocation.Bottom;
            }
        }

        foreach ( var mobilePackageInfo in mobilePackageLayoutInfo )
        {
            var mobileLayout = new Rock.Mobile.Common.Layout { LayoutGuid = mobilePackageInfo.Guid, LayoutXaml = mobilePackageInfo.LayoutMobilePhone, Name = mobilePackageInfo.Name };
            mobilePhonePackage.Layouts.Add( mobileLayout );
        }

        foreach ( var tabletPackageInfo in tabletPackageLayoutInfo )
        {
            var tabletLayout = new Rock.Mobile.Common.Layout { LayoutGuid = tabletPackageInfo.Guid, LayoutXaml = tabletPackageInfo.LayoutMobilePhone, Name = tabletPackageInfo.Name };
            mobilePhonePackage.Layouts.Add( tabletLayout );
        }
    }

    private void AssignPageDepth(Rock.Model.Page currentPage,int currentLevel)
    {
        foreach ( var child in currentPage.Pages)
        {
            currentLevel += 1;
            _pageWithDepth.Add( new PageInfo { Page = child,Depth = currentLevel} );
            if ( child.Pages.Count > 0)
            {
                AssignPageDepth( child, currentLevel);
            }
        }

        new PageInfo { Page = currentPage, Depth = currentLevel };
    }

    /// <summary>
    /// Handles the Click event of the btnSaveLayout control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnSaveLayout_Click( object sender, EventArgs e )
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
            DisplayApplicationTab();
        }
    }

    /// <summary>
    /// Handles the Clicked event of the btnCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnCancel_Clicked( object sender, EventArgs e )
    {
        if ( hfSiteId.Value.AsIntegerOrNull() != null )
        {
            UpdateExistingLayouts( hfSiteId.Value.AsInteger() );
        }

        BuildLayoutMenue();
        BuildLayoutDisplay();
        SetApplicationTabActive();
    }

    /// <summary>
    /// Handles the Click event of the btnApplicationDelete control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnApplicationDelete_Click( object sender, EventArgs e )
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
    /// Handles the Click event of the btnAddLayout control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnAddLayout_Click( object sender, EventArgs e )
    {
        ResetLayoutForAdd();
    }

    /// <summary>
    /// Handles the Click event of the btnDeleteLayout control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnDeleteLayout_Click( object sender, EventArgs e )
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
            linkButton.CommandArgument = layoutItem.Id.ToString();
        }
    }

    public class PageInfo
    {
        public Rock.Model.Page Page { get; set; }
        public ICollection<Rock.Model.Block> Block { get; set; }
        public int Depth { get; set; }
    }

    #endregion
}