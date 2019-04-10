using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
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

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load( object sender, EventArgs e )
    {

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

        if ( !siteId.Equals( 0 ) )
        {
            site = new SiteService( new RockContext() ).Get( siteId );
            BindTitleBar( site );
        }

        if ( site == null )
        {
            site = new Site { Id = 0 ,SiteType = SiteType.Mobile};
            site.SiteDomains = new List<SiteDomain>();
        }

        hfSiteId.Value = site.Id.ToString();

        tbApplicationName.ReadOnly = site.IsSystem;
        tbApplicationName.Text = site.Name;

        tbDescription.ReadOnly = site.IsSystem;
        tbDescription.Text = site.Description;

        cbIsActive.Checked = site.IsActive;

        imgIcon.BinaryFileId = site.SiteLogoBinaryFileId;
        ImgPreviewThumbnail.BinaryFileId = site.ThumbnailFileId;
    }

    private void BindTitleBar( Site site )
    {
       
        this.lReadOnlyTitle.Text = site.Name;
        this.ltVersion.Text = string.Format( "<span class='label label-info'>Latest Version{0}</span>", ( ( DateTime ) site.LatestVersionDateTime ).ToString( @"MM/dd/yyyy hh:mm tt" ) );
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

        rblAndroidTabLocation.Items.AddRange( new ListItem[]
            {
                new ListItem { Text="Top" ,Selected = false},
                new ListItem { Text = "Bottom", Selected = false }
            } );
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
    /// Displays the layout tab.
    /// </summary>
    private void DisplayLayoutTab( int? layoutId, int? siteId )
    {
        var layoutService = new LayoutService( new RockContext() );

        if ( !siteId.HasValue )
        {
            siteId = hfSiteId.Value.AsInteger();
        }
        var existingLayouts = layoutService.GetBySiteId( ( int ) siteId );

        BuildLayoutMenue( existingLayouts );

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

        //TODO : Layout edit mode read only based on Permissions?

        //if ( !IsUserAuthorized( Authorization.EDIT ) )
        //{
        //    readOnly = true;
        //    nbEditModeMessage.Text = EditModeMessage.ReadOnlyEditActionNotAllowed( Rock.Model.Layout.FriendlyTypeName );
        //}

        pnlLayout.Visible = true;
        pnlApplicationDetails.Visible = false;
        pnlApplicationEditDetails.Visible = false;
        pnlPages.Visible = false;
    }

    private void BuildLayoutMenue( IQueryable<Rock.Model.Layout> existingLayouts )
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine( "<ul id='ulLayouts' class='nav nav-tabs'>" );
        int id = 0;
        foreach ( var layout in existingLayouts )
        {
            sb.AppendLine( string.Format( "<li runat='server' id='li_{0}'>", id ) );
            sb.AppendLine( string.Format( "<a href='#' runat='server' onserverclick='LayoutItem_SelectedClick'>{0}</a></li>", layout.Name ) );
            sb.AppendLine( "</li>" );
        }

        sb.AppendLine( "</ul>" );
        ltLayoutMenue.Text = sb.ToString();
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
        ClearActiveTabs();

        var tab = sender as HtmlAnchor;
        var tabSelected = tab.InnerText;
        switch ( tabSelected )
        {
            case "Application":
                DisplayApplicationTab();
                break;
            case "Layout":
                DisplayLayoutTab(null, hfSiteId.Value.AsInteger() );
                break;
            case "Pages":
                DisplayPagesTab();
                break;
            default:
                break;
        }
        var parent = tab.Parent as HtmlGenericControl;
        parent.Attributes.Add( "class", "active" );
        this.TabSelected = true;
    }

    protected void LayoutItem_SelectedClick( object sender, EventArgs e )
    {

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
                site = new Rock.Model.Site() {SiteType = SiteType.Mobile};
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
            if ( site.ThumbnailFileId != ImgPreviewThumbnail.BinaryFileId )
            {
                existingThumbnailId = site.ThumbnailFileId;
                site.ThumbnailFileId = ImgPreviewThumbnail.BinaryFileId;
            }

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
        }

        DisplayApplicationTab();
    }

    /// <summary>
    /// Handles the Click event of the btnCancelMobileDetails control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnCancelMobileDetails_Click( object sender, EventArgs e )
    {
        pnlApplicationDetails.Visible = true;
        pnlApplicationEditDetails.Visible = false;
    }

    /// <summary>
    /// Handles the Click event of the btnPublish control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnPublish_Click( object sender, EventArgs e )
    {

    }

    protected void btnSaveLayout_Click( object sender, EventArgs e )
    {
        if ( Page.IsValid )
        {
            var rockContext = new RockContext();
            LayoutService layoutService = new LayoutService( rockContext );
            Rock.Model.Layout layout;

            int layoutId = hfLayoutId.Value.IsNullOrWhiteSpace() ? 0: int.Parse( hfLayoutId.Value );

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
        }
    }

    protected void btnLayoutCancel_Click( object sender, EventArgs e )
    {

    }

    #endregion



}