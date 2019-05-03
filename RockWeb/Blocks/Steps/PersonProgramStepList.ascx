<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonProgramStepList.ascx.cs" Inherits="RockWeb.Blocks.Steps.PersonProgramStepList" %>

<style>
    .step-card {
        border: 1px solid #d4d4d4;
        padding: 5px;
        height: 235px;
        overflow: hidden;
        position: relative;
    }

    .step-card .badge {
        background-color: #23a5c5;
    }

    .step-card > .step-card-hover {
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
        opacity: 0;
        transition-duration: 0.25s;
        position: absolute;
        background-color: #fff;
    }

    .step-card:hover > .step-card-hover {
        opacity: 1;
    }

    .add-step-buttons + .row {
        margin-top: 25px;
    }

    .step-records-table {
        width: 100%;
        bottom: 0;
        position: absolute;
    }

    .step-records-table td {
        border: 1px solid #d4d4d4;
        vertical-align: central;
        width: 20%;
    }

    .step-records-table td:first-of-type {
        border-left: none;
        width: 60%;
    }

    .step-records-table td:last-of-type {
        border-right: none;
    }

    .step-records-table tr:last-of-type td {
        border-bottom: none;
    }

    .step-records-table td:not(:first-of-type):hover {
        background-color: #d4d4d4;
        color: #fff;
        cursor: pointer;
    }

    .prereq-list-info {
        padding: 20px 20px 0 20px;
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
                    <asp:LinkButton ID="lbShowCards" runat="server" class="btn btn-xs btn-square btn-default" OnClick="ShowCards" AutoPostBack="true">
                        <i class="fa fa-th"></i>
                    </asp:LinkButton>
                    <asp:LinkButton ID="lbShowGrid" runat="server" class="btn btn-xs btn-square btn-default" OnClick="ShowGrid" AutoPostBack="true">
                        <i class="fa fa-list"></i>
                    </asp:LinkButton>
                </div>
            </div><!-- panel heading -->

            <div class="panel-body">

                <asp:HiddenField runat="server" ID="hfIsCardView" Value="true" ClientIDMode="Static" />

                <asp:Panel runat="server" id="pnlGridView">
                    <div class="row add-step-buttons">
                        <div class="col-xs-12">
                            <asp:repeater id="rAddStepButtons" runat="server" OnItemDataBound="rAddStepButtons_ItemDataBound">
                                <itemtemplate>
                                    <asp:LinkButton runat="server" id="lbAddStep" class="btn btn-sm btn-default" OnCommand="AddStep" CommandArgument='<%# Eval("StepTypeId") %>'>
                                        <i class="fa fa-plus"></i>
                                        &nbsp;
                                        <%# Eval("ButtonContents") %>
                                    </asp:LinkButton>
                                </itemtemplate>
                            </asp:repeater>
                        </div><!-- col -->
                    </div><!-- row -->
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="grid grid-panel">
                                <Rock:GridFilter ID="gfGridFilter" runat="server">
                                    <Rock:DateRangePicker ID="drpDateRangePicker" runat="server" Label="Date Range" />
                                </Rock:GridFilter>

                                <Rock:Grid ID="gStepList" runat="server" RowItemText="Step" AllowSorting="true">
                                    <Columns>
                                        <Rock:RockLiteralField HeaderText="Step Type" ID="lStepType" SortExpression="StepTypeName" OnDataBound="lStepType_DataBound" />
                                        <Rock:DateField DataField="CompletedDateTime" HeaderText="Completion Date" SortExpression="CompletedDateTime" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" />
                                        <Rock:RockLiteralField HeaderText="Summary" ID="lSummary" SortExpression="Summary" />
                                        <Rock:RockLiteralField HeaderText="Status" ID="lStepStatus" SortExpression="Status" OnDataBound="lStepStatus_DataBound" />
                                        <Rock:DeleteField OnClick="gStepList_Delete" />
                                    </Columns>
                                </Rock:Grid>
                            </div><!-- .grid -->
                        </div><!-- col -->
                    </div><!-- row -->
                </asp:Panel><!-- pnlGridView -->

                <asp:Panel runat="server" id="pnlCardView">
                    <div class="row">
                        <asp:repeater id="rStepTypeCards" runat="server" OnItemDataBound="rStepTypeCards_ItemDataBound">
                            <itemtemplate>
                                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3 text-center">
                                    <div class="step-card">
                                        <%# Eval( "RenderedLava" ) %>
                                        <div class="step-card-hover">
                                            <asp:Panel ID="pnlStepRecords" runat="server">
                                                <table class="step-records-table">
                                                    <asp:repeater id="rSteps" runat="server">
                                                        <itemtemplate>
                                                            <tr>
                                                                <td><%# Eval("StatusHtml") %></td>
                                                                <td><i class="fa fa-pencil"></i></td>
                                                                <td>
                                                                    <asp:LinkButton runat="server" OnCommand="rSteps_Delete" CommandArgument='<%# Eval("StepId") %>'>
                                                                        <i class="fa fa-times"></i>
                                                                    </asp:LinkButton>
                                                                </td>
                                                            </tr>                                                        
                                                        </itemtemplate>
                                                    </asp:repeater>
                                                </table>
                                            </asp:Panel>
                                            <asp:Panel ID="pnlPrereqs" runat="server">
                                                <p class="prereq-list-info">This step requires the following prerequisite steps:</p>
                                                <ul class="list-unstyled">
                                                    <asp:repeater id="rPrereqs" runat="server">
                                                        <itemtemplate>
                                                            <li><%# Eval("Name") %></li>                                                     
                                                        </itemtemplate>
                                                    </asp:repeater>
                                                </ul>
                                            </asp:Panel>
                                        </div>
                                    </div>                                
                                </div><!-- col -->
                            </itemtemplate>
                        </asp:repeater>
                    </div><!-- row -->
                </asp:Panel><!-- pnlCardView -->

            </div><!-- .panel-body -->

        </div><!-- .panel -->

    </ContentTemplate>
</asp:UpdatePanel>
