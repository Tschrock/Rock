<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepEntry.ascx.cs" Inherits="RockWeb.Blocks.Steps.StepEntry" %>

<asp:UpdatePanel ID="pnlGatewayListUpdatePanel" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlDetails" CssClass="panel panel-block" runat="server">

            <div class="panel-heading">
                <h1 class="panel-title">
                    <asp:Literal ID="lStepTypeTitle" runat="server" />
                </h1>
            </div>

            <div class="panel-body">

                <Rock:NotificationBox ID="nbMessage" runat="server" NotificationBoxType="Warning" />

                <div id="pnlEditDetails" runat="server">

                    <asp:ValidationSummary ID="valGatewayDetail" runat="server" HeaderText="Please correct the following:" CssClass="alert alert-validation" />

                    <div class="row">
                        <div class="col-sm-6 col-md-3">
                            <Rock:DatePicker ID="rdpStartDate" runat="server" PropertyName="StartDate" />
                        </div>
                        <div class="col-sm-6 col-md-3">
                            <Rock:DatePicker ID="rdpEndDate" runat="server" PropertyName="EndDate" />
                        </div>
                        <div class="col-sm-12 col-md-6">
                            <Rock:StepStatusPicker ID="rsspStatus" runat="server" Label="Status" />                            
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12">
                            <Rock:AttributeValuesContainer ID="avcAttributes" runat="server" NumberOfColumns="2" />
                        </div>
                    </div>

                    <div class="actions">
                        <asp:LinkButton ID="btnSave" runat="server" AccessKey="s" ToolTip="Alt+s" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                        <asp:LinkButton ID="btnCancel" runat="server" AccessKey="c" ToolTip="Alt+c" Text="Cancel" CssClass="btn btn-link" CausesValidation="false" OnClick="btnCancel_Click" />
                    </div>

                </div>

            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
