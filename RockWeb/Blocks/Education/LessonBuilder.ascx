<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LessonBuilder.ascx.cs" Inherits="RockWeb.Blocks.Education.LessonBuilder" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlLessonBuilder" runat="server">
            <div class="hidden" id="lesson_builder_asp_controls">
                <asp:HiddenField ID="hfWriteBackValue" runat="server" />
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" UseSubmitBehavior="false" OnClick="btnSave_Click" />
            </div>

            <asp:Literal ID="lOutput" runat="server"></asp:Literal>
        </asp:Panel>

        <!-- This will be removed after testing. -->
        <div id="debug_out">
            <asp:Literal ID="lDebug" runat="server"></asp:Literal>
        </div>


    </ContentTemplate>
</asp:UpdatePanel>
