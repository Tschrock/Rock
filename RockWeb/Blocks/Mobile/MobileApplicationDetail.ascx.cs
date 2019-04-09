using System;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rock;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

[DisplayName( "Mobile Application Detail" )]
[Category( "Mobile" )]
[Description( "Presents the details of mobile application detail." )]
public partial class Blocks_Mobile_MobileApplicationDetail : RockBlock, IDetailBlock
{

    private const string FLYOUT = "Flyout";
    private const string TABS = "Tabs";
    private const string BLANK = "Blank";
    protected void Page_Load( object sender, EventArgs e )
    {
        
    }

    protected override void OnLoad( EventArgs e )
    {
        base.OnLoad( e );
        if ( !IsPostBack )
        {
            if ( !TabSelected )
            {
                ShowDetail(0);

            }
        }
    }

    #region Properties
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

    public void ShowDetail( int itemId )
    {
        pnlApplicationDetails.Visible = true;
        var liApplication = this.FindControl( "liApplication" ) as HtmlGenericControl;
        liApplication.Attributes.Add( "class", "active" );
        InitializeRadioButtonLists();
    }

    /// <summary>
    /// Initializes the RadioButton lists.
    /// </summary>
    private void InitializeRadioButtonLists()
    {
        rblApplicationType.Items.AddRange(
            new ListItem[]
            {
                new ListItem { Text= FLYOUT ,Selected = false},
                new ListItem { Text = TABS, Selected = false },
                new ListItem { Text = BLANK, Selected = false },
            } );

        rblAndroidTabLocation.Items.AddRange( new ListItem[]
            {
                new ListItem { Text="Top" ,Selected = false},
                new ListItem { Text = "Bottom", Selected = false }
            } );
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
        var parent = tab.Parent as HtmlGenericControl;
        parent.Attributes.Add( "class", "active" );
        this.TabSelected = true;

        pnlApplicationDetails.Visible = tabSelected == "Application";
        pnlLayout.Visible = tabSelected == "Layout";
        pnlPages.Visible = tabSelected == "Pages";
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
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void rblApplicationType_SelectedIndexChanged( object sender, EventArgs e )
    {
        var list = sender as RockRadioButtonList;
        if ( list.SelectedValue == TABS )
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



    #endregion


    protected void btnSaveLayout_Click( object sender, EventArgs e )
    {

    }

    protected void btnLayoutCancel_Click( object sender, EventArgs e )
    {

    }
}