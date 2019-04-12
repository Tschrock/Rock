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
                        <li runat="server" id="liApplication"><a href="#" id="tabApplication" runat="server" onserverclick="Tab_SelectedClick">Application</a></li>
                        <li runat="server" id="liLayout" class="disabled" ><a href="#" id="tabLayout" runat="server"  onserverclick="Tab_SelectedClick">Layout</a></li>
                        <li runat="server" id="liPages" class="disabled"><a href="#" id="tabPages"  runat="server" onserverclick="Tab_SelectedClick">Pages</a></li>
                    </ul>
                </div>
            </asp:Panel>
            <!-- Application Tab Read Only View -->
            <asp:Panel ID="pnlApplicationDetails" runat="server" Visible="false">
                <asp:HiddenField ID="hfSiteId" runat="server" />
                <fieldset id="fieldsetApplicationViewDetails" runat="server">
                    <div class="panel-body">
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
                        <asp:Panel ID="pnlReadOnlyModeActions" runat="server" CssClass="actions">
                            <asp:LinkButton ID="btnEdit" runat="server" AccessKey="m" ToolTip="Alt+m" Text="Edit" CssClass="btn btn-primary" OnClick="btnEdit_Click" />
                            <asp:LinkButton ID="btnApplicationDelete" runat="server" Text="Delete" OnClick="btnApplicationDelete_Click" CssClass="btn btn-link" CausesValidation="false" />
                            <asp:LinkButton ID="btnPublish" Visible="false" runat="server" Text="Publish" CssClass="btn btn-secondary" OnClick="btnPublish_Click" />
                        </asp:Panel>
                    </div>
                </fieldset>
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
                        <Rock:BootstrapButton ID="btnSaveMobileDetails" runat="server" CssClass="btn btn-primary" Text="Save" CausesValidation="true" OnClick="btnSaveMobileDetails_Click" DataLoadingText="Saving..." />
                        <asp:LinkButton ID="btnCancelMobileDetails" runat="server" CssClass="btn btn-default btn-cancel" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Clicked" />
                    </div>
                </div>
            </asp:Panel>
            <!-- Layout Tab View -->
            <asp:Panel ID="pnlLayout" runat="server" Visible="false">
                <div class="panel-body">
                    <asp:HiddenField ID="hfLayoutId" runat="server" />
                    <div class="col-md-1">
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
                        <div><a href="#" id="aAddLayout" title="Add layout" runat="server" class="fa fa-plus-square-o" onserverclick="btnAddLayout_Click" visible="false" /></div> 
                    </div>
                    <div class="col-md-11">
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
                        <Rock:BootstrapButton ID="btnSaveLayout" runat="server" CssClass="btn btn-primary" Text="Save" CausesValidation="true" OnClick="btnSaveLayout_Click" DataLoadingText="Saving..." />
                        <asp:LinkButton ID="btnLayoutCancel" runat="server" CssClass="btn btn-default btn-cancel" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Clicked" />
                        <asp:LinkButton ID="btnDeleteCurrentLayout" runat="server" Text="Delete" OnClick="btnDeleteLayout_Click" CssClass="btn btn-link" CausesValidation="false" Visible="false" />
                    </div>
                </div>
            </asp:Panel>
            <!-- Page Tab View -->
            <asp:Panel ID="pnlPages" runat="server" Visible="false">
                <div class="row">
                    <div class="col-md-6">
                        <div>Pages</div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
