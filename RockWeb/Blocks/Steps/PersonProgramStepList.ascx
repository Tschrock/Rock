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
                    <a href="#" class="btn btn-xs btn-square btn-default">
                        <i class='fa fa-money'></i>
                    </a>
                    <a href="#" class="btn btn-xs btn-square btn-default">
                        <i class='fa fa-money'></i>
                    </a>
                </div>
            </div>

            <div class="panel-body">
                <ul>
                    <asp:repeater id="rStepTypes" runat="server">
                        <itemtemplate>
                            <%# Eval( "RenderedLava" ) %>
                        </itemtemplate>
                    </asp:repeater>
                </ul>
            </div>

        </div>

    </ContentTemplate>
</asp:UpdatePanel>
