﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepProgramDetail.ascx.cs" Inherits="RockWeb.Blocks.Steps.StepProgramDetail" %>

<script type="text/javascript">
    function clearActiveDialog() {
        $('#<%=hfActiveDialog.ClientID %>').val('');
    }
</script>

<asp:UpdatePanel ID="upStepProgram" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlDeleteConfirm" runat="server" CssClass="panel panel-body" Visible="false">
            <Rock:NotificationBox ID="nbDeleteConfirm" runat="server" NotificationBoxType="Warning" Text="Deleting a Step Program will delete all of the associated Step Types. Are you sure you want to delete this Step Program?" />
            <asp:LinkButton ID="btnDeleteConfirm" runat="server" Text="Confirm Delete" CssClass="btn btn-danger" OnClick="btnDeleteConfirm_Click" />
            <asp:LinkButton ID="btnDeleteCancel" runat="server" Text="Cancel" CssClass="btn btn-primary" OnClick="btnDeleteCancel_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlDetails" CssClass="panel panel-block" runat="server" Visible="false">
            <asp:HiddenField ID="hfStepProgramId" runat="server" />

            <div class="panel-heading">
                <h1 class="panel-title">
                    <asp:Literal ID="lIcon" runat="server" />
                    <asp:Literal ID="lReadOnlyTitle" runat="server" />
                </h1>
                <div class="panel-labels">
                    <Rock:HighlightLabel ID="hlCategory" runat="server" LabelType="Info" />
                    <Rock:HighlightLabel ID="hlInactive" runat="server" LabelType="Danger" Text="Inactive" />
                </div>

            </div>
            <Rock:PanelDrawer ID="pdAuditDetails" runat="server"></Rock:PanelDrawer>
            <div class="panel-body">
                <Rock:NotificationBox ID="nbEditModeMessage" runat="server" NotificationBoxType="Info" />
                <asp:ValidationSummary ID="valStepProgramDetail" runat="server" HeaderText="Please correct the following:" CssClass="alert alert-validation" />

                <div id="pnlViewDetails" runat="server">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Literal ID="lStepProgramDescription" runat="server"></asp:Literal>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-6">
                            <h3>Steps Activity Summary</h3>
                            <p class="small">Shows steps completed within the activity period.</p>
                        </div>
                        <div class="col-sm-6">
                            <div class="panel panel-body">
                                <div class="row">
                                    <div class="col-sm-8">
                                        <Rock:SlidingDateRangePicker ID="drpSlidingDateRange"
                                            runat="server"
                                            EnabledSlidingDateRangeTypes="Previous, Last, Current, DateRange"
                                            EnabledSlidingDateRangeUnits="Week, Month, Year"
                                            SlidingDateRangeMode="Current"
                                            TimeUnit="Year"
                                            CssClass="pull-right" />
                                    </div>
                                    <div class="col-sm-4">
                                        <span class="pull-right">
                                            <asp:LinkButton ID="btnRefreshChart" runat="server" CssClass="btn btn-primary" Style="vertical-align: bottom" ToolTip="Refresh Chart" OnClick="btnRefreshChart_Click"><i class="fa fa-refresh"></i> Update</asp:LinkButton>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <Rock:LineChart ID="lcSteps" runat="server" ChartHeight="280px" />
                        </div>
                    </div>


                    <div class="row">
                        <div class="col-md-6">
                            <asp:Literal ID="lblMainDetails" runat="server" />
                        </div>
                    </div>

                    <div class="actions">
                        <asp:LinkButton ID="btnEdit" runat="server" AccessKey="e" ToolTip="Alt+e" Text="Edit" CssClass="btn btn-primary" OnClick="btnEdit_Click" CausesValidation="false" />
                        <Rock:ModalAlert ID="mdDeleteWarning" runat="server" />
                        <asp:LinkButton ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-link" OnClick="btnDelete_Click" CausesValidation="false" />
                        <span class="pull-right">
                            <Rock:SecurityButton ID="btnSecurity" runat="server" class="btn btn-sm btn-security" />
                        </span>
                    </div>
                </div>

                <div id="pnlEditDetails" runat="server">
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:DataTextBox ID="tbName" runat="server" SourceTypeName="Rock.Model.StepProgram, Rock" PropertyName="Name" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockCheckBox ID="cbActive" runat="server" SourceTypeName="Rock.Model.StepProgram, Rock" PropertyName="IsActive" Label="Active" Checked="true" Text="Yes" />
                        </div>
                    </div>

                    <Rock:DataTextBox ID="tbDescription" runat="server" SourceTypeName="Rock.Model.StepProgram, Rock" PropertyName="Description" TextMode="MultiLine" Rows="4" />

                    <div class="row">
                        <div class="col-md-6">
                            <Rock:DataTextBox ID="tbIconCssClass" runat="server" SourceTypeName="Rock.Model.StepProgram, Rock" PropertyName="IconCssClass" ValidateRequestMode="Disabled" />
                            <Rock:CategoryPicker ID="cpCategory" runat="server" EntityTypeName="Rock.Model.StepProgram" Label="Category" Required="true" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockRadioButtonList ID="rblDefaultListView" runat="server" Label="Default List View" RepeatDirection="Horizontal" />
                        </div>
                    </div>

                    <Rock:PanelWidget ID="wpStatuses" runat="server" Title="Statuses">
                        <div class="grid">
                            <Rock:Grid ID="gStatuses" runat="server" AllowPaging="false" DisplayType="Light" RowItemText="Status" ShowConfirmDeleteDialog="false">
                                <Columns>
                                    <Rock:RockBoundField DataField="Name" HeaderText="Name" />
                                    <Rock:BoolField DataField="IsCompleteStatus" HeaderText="Completion?" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                    <Rock:EditField OnClick="gStatuses_Edit" />
                                    <Rock:DeleteField OnClick="gStatuses_Delete" />
                                </Columns>
                            </Rock:Grid>
                        </div>
                    </Rock:PanelWidget>

                    <Rock:PanelWidget ID="wpWorkflow" runat="server" Title="Workflows">
                        <div class="grid">
                            <Rock:Grid ID="gWorkflows" runat="server" AllowPaging="false" DisplayType="Light" RowItemText="Workflow" ShowConfirmDeleteDialog="false">
                                <Columns>
                                    <Rock:RockBoundField DataField="WorkflowType" HeaderText="Workflow Type" />
                                    <Rock:RockBoundField DataField="Trigger" HeaderText="Trigger" />
                                    <Rock:EditField OnClick="gWorkflows_Edit" />
                                    <Rock:DeleteField OnClick="gWorkflows_Delete" />
                                </Columns>
                            </Rock:Grid>
                        </div>
                    </Rock:PanelWidget>

                    <div class="actions">
                        <asp:LinkButton ID="btnSave" runat="server" AccessKey="s" ToolTip="Alt+s" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                        <asp:LinkButton ID="btnCancel" runat="server" AccessKey="c" ToolTip="Alt+c" Text="Cancel" CssClass="btn btn-link" CausesValidation="false" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <Rock:ModalAlert ID="mdCopy" runat="server" />

        <asp:HiddenField ID="hfActiveDialog" runat="server" />

        <Rock:ModalDialog ID="dlgStepStatuses" runat="server" ScrollbarEnabled="false" SaveButtonText="Add" OnSaveClick="btnAddStepStatus_Click" Title="Create Status" ValidationGroup="StepStatus">
            <Content>
                <asp:HiddenField ID="hfStepProgramAddStepStatusGuid" runat="server" />
                <div class="row">
                    <div class="col-md-6">
                        <Rock:DataTextBox ID="tbStepStatusName" SourceTypeName="Rock.Model.StepStatus, Rock" PropertyName="Name" Label="Name" runat="server" ValidationGroup="StepStatus" />
                    </div>
                    <div class="col-md-6">
                        <Rock:RockCheckBox ID="cbIsActive" runat="server" Label="Is Active" ValidationGroup="StepStatus" />
                        <Rock:RockCheckBox ID="cbIsCompleted"
                            runat="server"
                            Label="Is Completed"
                            Help="Does this status indicate that the step has been completed?"
                            ValidationGroup="StepStatus" />
                        <Rock:ColorPicker ID="cpStatus"
                            runat="server"
                            Label="Display Color"
                            Help="The color used to display a step having this status." />
                    </div>

                </div>
            </Content>
        </Rock:ModalDialog>

        <Rock:ModalDialog ID="dlgStepWorkflow" runat="server" Title="Select Workflow" OnSaveClick="dlgStepWorkflow_SaveClick" OnCancelScript="clearActiveDialog();" ValidationGroup="StepWorkflow">
            <Content>
                <asp:HiddenField ID="hfAddStepWorkflowGuid" runat="server" />
                <asp:ValidationSummary ID="valStepWorkflowSummary" runat="server" HeaderText="Please correct the following:" CssClass="alert alert-validation" ValidationGroup="StepWorkflow" />

                <div class="row">
                    <div class="col-md-6">
                        <Rock:RockDropDownList ID="ddlTriggerType" runat="server" Label="Launch Workflow When"
                            OnSelectedIndexChanged="ddlTriggerType_SelectedIndexChanged" AutoPostBack="true" Required="true" ValidationGroup="StepWorkflow" />
                    </div>
                    <div class="col-md-6">
                        <Rock:WorkflowTypePicker ID="wpWorkflowType" runat="server" Label="Workflow Type" Required="true" ValidationGroup="StepWorkflow" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <Rock:RockDropDownList ID="ddlPrimaryQualifier" runat="server" Visible="false" ValidationGroup="StepWorkflow" />
                        <Rock:RockDropDownList ID="ddlSecondaryQualifier" runat="server" Visible="false" ValidationGroup="StepWorkflow" />
                    </div>
                    <div class="col-md-6">
                    </div>
                </div>

            </Content>
        </Rock:ModalDialog>
    </ContentTemplate>
</asp:UpdatePanel>
