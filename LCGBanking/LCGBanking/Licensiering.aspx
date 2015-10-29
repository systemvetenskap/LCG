<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Licensiering.aspx.cs" Inherits="LCGBanking.Licensiering" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section>
    <section id="main">
        <h1>Tester</h1>
        <asp:Button ID="ButtonStart" runat="server" Text="Starta licenseringstest" Height="23px" OnClick="ButtonStart_Click" />
        <br /><br />
        
        <div id="fraga">
            <asp:Label ID="LabelKategori" runat="server" Text="Label"></asp:Label>
            <br /><br />
            <h2><asp:Label ID="LabelQuestion" runat="server" Text="Label"></asp:Label></h2>
            <asp:Label ID="LabelInfo" runat="server" Text="Label"></asp:Label>
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

