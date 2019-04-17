<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MobileApplicationDetail.ascx.cs" Inherits="Blocks_Mobile_MobileApplicationDetail" %>
<asp:UpdatePanel ID="upnlMobileApplicationDetail" runat="server">
    <ContentTemplate>
        <div class="panel panel-block">
            <div class="panel-heading panel-follow clearfix">
                <h1 class="panel-title"><i class="fa fa-desktop"></i>
                    <asp:Literal ID="lReadOnlyTitle" runat="server" /></h1>
                <div class="panel-labels">
                    <asp:Literal ID="ltVersion" runat="server" />
                </div>
            </div>
            <!-- Tabs -->
            <asp:Panel runat="server" ID="pnlTabs">
                <asp:HiddenField ID="hfTabSelected" runat="server" Value="false" />
                <div class="panel-body">
                    <ul class="nav nav-tabs">
                        <li runat="server" id="liApplication">
                            <asp:LinkButton ID="lbTabApplication" runat="server" OnClick="lbTab_SelectedClick">Application</asp:LinkButton></li>
                        <li runat="server" id="liLayout">
                            <asp:LinkButton ID="lbTabLayout" runat="server" OnClick="lbTab_SelectedClick">Layout</asp:LinkButton></li>
                        <li runat="server" id="liPages">
                            <asp:LinkButton ID="lbTabPages" runat="server" OnClick="lbTab_SelectedClick">Pages</asp:LinkButton></li>
                    </ul>
                </div>
            </asp:Panel>
            <!-- Application Tab Read Only View -->
            <asp:Panel ID="pnlApplicationDetails" runat="server" Visible="false">
                <asp:HiddenField ID="hfSiteId" runat="server" />
                <div class="panel-body">
                    <fieldset id="fieldsetApplicationViewDetails" runat="server">
                        <div class="row">
                            <div class="col-md-8">
                                <label class="control-label">Application Type</label>
                                <asp:Literal ID="ltApplicationType" runat="server" />
                                <br />
                                <label class="control-label">Layouts</label>
                                <asp:Literal ID="ltLayouts" runat="server" />
                            </div>
                            <div class="col-md-2">
                                <div class="photo">
                                    <asp:Literal ID="ltIconImage" runat="server" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="photo">
                                    <asp:Literal ID="ltMobileImage" runat="server" />
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <asp:Panel ID="pnlReadOnlyModeActions" runat="server" CssClass="actions">
                        <asp:LinkButton ID="btnEdit" runat="server" AccessKey="m" ToolTip="Alt+m" Text="Edit" CssClass="btn btn-primary" OnClick="lbtnEdit_Click" />
                        <asp:LinkButton ID="btnApplicationDelete" runat="server" Text="Delete" OnClick="lbtnApplicationDelete_Click" CssClass="btn btn-link" CausesValidation="false" />
                        <asp:LinkButton ID="btnPublish" Visible="false" runat="server" Text="Publish" CssClass="btn btn-secondary" OnClick="lbtnPublish_Click" />
                    </asp:Panel>
                </div>

                <Rock:ModalDialog ID="mdConfirmPublish" runat="server" Title="Please Confirm" SaveButtonText="Yes" OnSaveClick="mdConfirmPublish_SaveClick">
                    <Content>
                        "Are you wish to create a new version of the application."
                    </Content>
                </Rock:ModalDialog>
            </asp:Panel>
            <!-- Application Tab Edit Details -->
            <asp:Panel ID="pnlApplicationEditDetails" runat="server" Visible="false">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:DataTextBox ID="tbApplicationName" runat="server" Label="Application Name" SourceTypeName="Rock.Model.Site, Rock" PropertyName="Name" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockCheckBox ID="cbIsActive" runat="server" CssClass="js-isactivegroup" Text="Active" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <Rock:DataTextBox ID="tbDescription" runat="server" SourceTypeName="Rock.Model.Site, Rock" PropertyName="Description" TextMode="MultiLine" Rows="4" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:RockRadioButtonList ID="rblApplicationType" AutoPostBack="true" OnSelectedIndexChanged="rblApplicationType_SelectedIndexChanged"
                                runat="server"
                                Label="Application Type" RepeatDirection="Horizontal" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockRadioButtonList ID="rblAndroidTabLocation" runat="server"
                                Visible="false"
                                Label="Android Tab Location"
                                RepeatDirection="Horizontal"
                                Help="Determines where the Tab bar should be displayed on Android. iOS will always display at the bottom" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <Rock:CodeEditor ID="ceCssPreHtml" TextMode="MultiLine" Label="CSS Styles" runat="server" Help="CSS Styles to apply to UI elements." />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:ImageUploader ID="imgIcon"
                                runat="server"
                                Label="Icon"
                                Help="The icon to be used by the application when it's built. This will only be updated when a new application is deployed to the various stores." />
                        </div>
                        <div class="col-md-6">
                            <Rock:ImageUploader ID="imgPreviewThumbnail"
                                runat="server"
                                Label="Preview Thumbnail"
                                Help="Preview thumbnail to be used by Rock to distinguish applications." />
                        </div>
                    </div>
                    <div class="actions">
                        <Rock:BootstrapButton ID="btnSaveMobileDetails" runat="server" CssClass="btn btn-primary" Text="Save" CausesValidation="true" OnClick="lbtnSaveMobileDetails_Click" DataLoadingText="Saving..." />
                        <asp:LinkButton ID="lbtnCancelMobileDetails" runat="server" CssClass="btn btn-default btn-cancel" Text="Cancel" CausesValidation="false" OnClick="lbtnCancel_Clicked" />
                    </div>
                </div>
            </asp:Panel>
            <!-- Layout Tab View -->
            <asp:Panel ID="pnlLayout" runat="server" Visible="false">
                <div class="panel-body">
                    <asp:HiddenField ID="hfLayoutId" runat="server" />
                    <div class="col-md-2">
                        <asp:Repeater ID="rptLayoutMenu" runat="server">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server">
                                    <asp:LinkButton runat="server"
                                        OnDataBinding="LayoutItem_DataBinding"
                                        OnClick="LayoutItem_SelectedClick"
                                        CausesValidation="false" />
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>  
                            </FooterTemplate>
                        </asp:Repeater>
                        <div>
                            <asp:LinkButton ID="lbtnAddLayout" title="Add layout" runat="server" class="fa fa-plus-square-o" OnClick="lbtnAddLayout_Click" Visible="false" />
                        </div>
                    </div>
                    <div class="col-md-10">
                        <div class="row">
                            <div class="col-md-6">
                                <Rock:DataTextBox ID="tbLayoutName" runat="server" Label="Name" SourceTypeName="Rock.Model.Site, Rock" PropertyName="Name" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <Rock:DataTextBox ID="tbLayoutDescription" runat="server" SourceTypeName="Rock.Model.Group, Rock" PropertyName="Description" TextMode="MultiLine" Rows="4" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <Rock:CodeEditor ID="cePhoneLayoutXaml" TextMode="MultiLine" Label="Phone Layout XAML" runat="server" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <Rock:CodeEditor ID="ceTabletLayoutXaml" TextMode="MultiLine" Label="Tablet Layout XAML" runat="server" />
                            </div>
                        </div>
                    </div>
                    <div class="actions">
                        <Rock:BootstrapButton ID="lbtnSaveLayout" runat="server" CssClass="btn btn-primary" Text="Save" CausesValidation="true" OnClick="lbtnSaveLayout_Click" DataLoadingText="Saving..." />
                        <asp:LinkButton ID="lbtnDeleteCurrentLayout" runat="server" Text="Delete" OnClick="lbtnDeleteLayout_Click" CssClass="btn btn-link" CausesValidation="false" Visible="false" />
                    </div>
                </div>
            </asp:Panel>
            <!-- Page Tab View -->
            <asp:HiddenField ID="hfPageId" runat="server" />
            <asp:Panel ID="pnlPages" runat="server" Visible="false">
                <div class="panel-body">
                    <div class="col-md-2">
                        <asp:Repeater ID="rptPageMenue" runat="server">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server">
                                    <asp:LinkButton runat="server"
                                        OnDataBinding="PageLink_DataBinding"
                                        OnClick="PageMenu_Click"
                                        CausesValidation="false" />
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                        <div>
                            <div>
                                <asp:LinkButton ID="lbtnAddPage" title="Add page" runat="server" class="fa fa-plus-square-o" OnClick="lbtnAddPage_Click" Visible="false" />
                            </div>
                        </div>
                    </div>
                    <div class="col-md-10">
                        <asp:Panel runat="server" ID="pnlPageEdit" Visible="false">
                            <div class="panel-body">
                                <div class="row">
                                    <Rock:ModalAlert ID="mdDeleteWarning" runat="server" />
                                    <Rock:NotificationBox ID="nbPageLayoutRequired" runat="server" NotificationBoxType="Info" Title="Note" Visible="false" Text="A layout has not been selected." />
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <Rock:DataTextBox ID="tbPageName" runat="server" Label="Name" SourceTypeName="Rock.Model.Page,Rock" PropertyName="PageTitle" />
                                    </div>
                                    <div class="col-md-6">
                                        <Rock:RockCheckBox ID="cbDisplayInNavigation" Label="Display In Navigation" runat="server" Checked="false" CssClass="js-isactivegroup" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <Rock:RockDropDownList ID="ddlPageLayout" runat="server" Label="Layout" />
                                    </div>
                                </div>
                                <div class="actions">
                                    <Rock:BootstrapButton ID="btnSavePage" runat="server" CssClass="btn btn-primary" Text="Save" CausesValidation="true" OnClick="lbtnSavePage_Click" DataLoadingText="Saving..." />
                                    <asp:LinkButton ID="lbEditZones" runat="server" CssClass="btn btn-default" Text="Edit Zone" CausesValidation="false" OnClick="lbEditZones_Click" />
                                    <asp:LinkButton ID="lbtnDeleteCurrentPage" runat="server" Text="Delete" OnClick="lbtnDeleteCurrentPage_Click" CssClass="btn btn-link" CausesValidation="false" />
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlZoneEdit" Visible="false">
                            <div class="pnlBody">
                                <div class="row">
                                    <div class="col-md-6">
                                        <asp:Label runat="server" ID="lblPageTitle" class="control-label" />
                                    </div>
                                    <div class="col-md-6">
                                        <label>Display In Navigation</label>
                                        <br />
                                        <label id="lblDisplayInNavigationCheck" runat="server" class="fa fa-check" visible="false" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <fieldset>
                                            <span>Layout</span>
                                            </br>
                                            <asp:Label runat="server" ID="lblPageLayout" />
                                        </fieldset>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-10">
                                     <asp:Repeater ID="rptZones" runat="server">
                                         <ItemTemplate>
                                             <asp:Panel runat="server">
                                                 <div class="panel-body">
                                                  <div>Test</div>
                                                 </div>
                                             </asp:Panel>
                                         </ItemTemplate>
                                     </asp:Repeater>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <br />
                                        <asp:LinkButton ID="lbZoneEditPages" CssClass="btn btn-primary" Text="Edit" runat="server" OnClick="lbZoneEditPages_Click" />
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </asp:Panel>

        </div>
    </ContentTemplate>
</asp:UpdatePanel>
