<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonProgramStepList.ascx.cs" Inherits="RockWeb.Blocks.Steps.PersonProgramStepList" %>

<asp:UpdatePanel ID="upContent" runat="server">
    <ContentTemplate>

        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title">
                    <i runat="server" class="fa fa-money"></i>
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
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
