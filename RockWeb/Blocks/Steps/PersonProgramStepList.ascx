<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonProgramStepList.ascx.cs" Inherits="RockWeb.Blocks.Steps.PersonProgramStepList" %>

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
                    </asp:LinkButton><!--
                    --><asp:LinkButton ID="lbShowGrid" runat="server" class="btn btn-xs btn-square btn-default" OnClick="ShowGrid" AutoPostBack="true">
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
                                    <button runat="server" id="bAddStep" class="btn btn-sm btn-default" OnCommand="AddStep" CommandArgument='<%# Eval("StepTypeId") %>' onserverclick="bAddStep_ServerClick">
                                        <%# Eval("ButtonContents") %>
                                    </button>
                                </itemtemplate>
                            </asp:repeater>
                        </div><!-- col -->
                    </div><!-- row -->
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="grid grid-panel">
                                <Rock:GridFilter ID="gfGridFilter" runat="server">
                                    <Rock:RockTextBox ID="tbStepTypeName" runat="server" Label="Step Type Name"></Rock:RockTextBox>
                                    <Rock:RockTextBox ID="tbStepStatus" runat="server" Label="Step Status"></Rock:RockTextBox>
                                </Rock:GridFilter>
                                <Rock:Grid ID="gStepList" runat="server" RowItemText="Step" AllowSorting="true" OnRowSelected="gStepList_RowSelected">
                                    <Columns>
                                        <Rock:RockLiteralField HeaderText="Step Type" ID="lStepType" SortExpression="StepTypeName" OnDataBound="lStepType_DataBound" />
                                        <Rock:DateField DataField="CompletedDateTime" HeaderText="Completion Date" SortExpression="CompletedDateTime" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" />
                                        <Rock:RockLiteralField HeaderText="Summary" ID="lSummary" SortExpression="Summary" />
                                        <Rock:RockLiteralField HeaderText="Status" ID="lStepStatus" SortExpression="StepStatusName" OnDataBound="lStepStatus_DataBound" />
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
                                        <div class="card-info">
                                            <%# Eval( "RenderedLava" ) %>
                                        </div>
                                        <div class="step-card-hover">
                                            <asp:Panel ID="pnlStepRecords" runat="server">
                                                <asp:LinkButton runat="server" id="lbCardAddStep" OnCommand="AddStep" CommandArgument='<%# Eval("StepType.Id") %>' CssClass="card-add-step-button">
                                                    <span>
                                                    <i class="fa fa-plus-circle fa-2x"></i>
                                                    <br />
                                                    Add a Step
                                                    </span>
                                                </asp:LinkButton>
                                                <div class="step-records-table-container">
                                                    <table class="step-records-table">
                                                        <asp:repeater id="rSteps" runat="server">
                                                            <itemtemplate>
                                                                <tr>
                                                                    <td class="steps-status"><%# Eval("StatusHtml") %></td>
                                                                    <td>
                                                                        <asp:LinkButton runat="server" OnCommand="rSteps_Edit" CommandArgument='<%# Eval("StepId") %>' CssClass="btn-actions">
                                                                            <i class="fa fa-pencil"></i>
                                                                        </asp:LinkButton>
                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton runat="server" OnCommand="rSteps_Delete" CommandArgument='<%# Eval("StepId") %>' CssClass="btn-actions btn-delete">
                                                                            <i class="fa fa-times"></i>
                                                                        </asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </itemtemplate>
                                                        </asp:repeater>
                                                    </table>
                                                </div>
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
