<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonProgramStepList.ascx.cs" Inherits="RockWeb.Blocks.Steps.PersonProgramStepList" %>

<style>
    .step-card {
        border: 1px solid #d4d4d4;
        padding: 5px;
        height: 235px;
    }

    .step-card .badge {
        background-color: #23a5c5;
    }
</style>

<asp:UpdatePanel ID="upContent" runat="server">
    <ContentTemplate>

        <Rock:NotificationBox ID="nbNotificationBox" runat="server" Visible="false" />

        <div class="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title">
                    <i runat="server" Id="iIcon"></i>
                    <asp:Literal runat="server" ID="lStepProgramName" />
                </h1>

                <div class="panel-labels">
                    <asp:LinkButton runat="server" class="btn btn-xs btn-square btn-default" OnClick="ShowCards">
                        <i class='fa fa-th'></i>
                    </asp:LinkButton>
                    <asp:LinkButton runat="server" class="btn btn-xs btn-square btn-default" OnClick="ShowGrid">
                        <i class='fa fa-list'></i>
                    </asp:LinkButton>
                </div>
            </div>

            <div class="panel-body">

                <asp:HiddenField runat="server" ID="hfIsCardView" Value="true" ClientIDMode="Static" />

                <div class="row" runat="server" id="divGridView">
                    <div class="col-xs-12">
                        <div class="grid grid-panel">
                            <Rock:GridFilter ID="gfGridFilter" runat="server">
                                <Rock:DateRangePicker ID="drpDateRangePicker" runat="server" Label="Date Range" />
                            </Rock:GridFilter>

                            <Rock:Grid ID="gStepList" runat="server" RowItemText="Step" AllowSorting="true">
                                <Columns>
                                    <Rock:RockLiteralField HeaderText="Step Type" ID="lStepType" SortExpression="StepTypeName" OnDataBound="lStepType_DataBound" />
                                    <Rock:DateField DataField="CompletedDateTime" HeaderText="Completion Date" SortExpression="CompletedDateTime" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" />
                                    <Rock:RockLiteralField HeaderText="Status" ID="lStepStatus" SortExpression="Status" OnDataBound="lStepStatus_DataBound" />
                                </Columns>
                            </Rock:Grid>
                        </div>
                    </div>
                </div>

                <div class="row" runat="server" id="divCardView">
                    <asp:repeater id="rStepTypes" runat="server">
                        <itemtemplate>
                            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3 text-center">
                                <div class="step-card">
                                    <%# Eval( "RenderedLava" ) %>
                                </div>
                            </div>
                        </itemtemplate>
                    </asp:repeater>
                </div>

            </div>

        </div>

    </ContentTemplate>
</asp:UpdatePanel>
