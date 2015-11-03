<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Licensiering.aspx.cs" Inherits="LCGBanking.Licensiering" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section>
        <asp:Label ID="Welcome" runat="server"></asp:Label>
    <section id="main">
        <nav id="QuestionNavbar">
            <asp:Repeater ID="RepeaterQuestNav" runat="server">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButtonQuestNav" CssClass='<%# Eval("value") %>' Text='<%# Eval("key") %>' runat="server" OnClick="LinkButtonQuestNav_Click"></asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>
        </nav>
        <h1 id="h1-Licensiering">Tester</h1>
        <br /><br />
        <asp:Button ID="ButtonStart" runat="server" Text="Starta licenseringstest" Height="23px" OnClick="ButtonStart_Click" />
        <br /><br />
        
        <div id="fraga">
            <h4><asp:Label ID="LabelKategori" runat="server"></asp:Label></h4>
            <br /><br />
            <asp:Label ID="LabelQuestion" runat="server"></asp:Label>
            <p class="p-mindretext"><asp:Label ID="LabelInfo" runat="server"></asp:Label></p>
        </div>
        <br />
        <div id="svar">
            <asp:Panel ID="PanelSvar" runat="server"></asp:Panel>                
        </div>

        <br /><br/ />

        <div id="knappar">
            <asp:Button ID="ButtonPrevious" runat="server" Text="Föregående fråga" OnClick="ButtonPrevious_Click" />
            <asp:Button ID="ButtonNext" runat="server" Text="Nästa fråga" OnClick="ButtonNext_Click" />
            <asp:Button ID="ButtonSparaProv" runat="server" OnClick="ButtonSparaProv_Click" Text="Spara prov" />
        </div>
        </section>
    </section>  
</asp:Content>

