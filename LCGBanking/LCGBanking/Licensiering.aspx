<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Licensiering.aspx.cs" Inherits="LCGBanking.Licensiering" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section>        
        <h1>Tester</h1>
        <asp:Button ID="ButtonStart" runat="server" Text="Starta licenseringstest" Height="23px" OnClick="ButtonStart_Click" />
        <br /><br />
        
        <div id="fraga">
            <asp:Label ID="LabelQuestion" runat="server" Text="Label"></asp:Label>
        </div>

        <div id="svar">
            <asp:RadioButton ID="Svar1" runat="server" />
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label> <br />
            <asp:RadioButton ID="Svar2" runat="server" />
            <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label> <br />
            <asp:RadioButton ID="Svar3" runat="server" />
            <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label> <br />                   
        </div>

        <div id="knappar">
            <asp:Button ID="ButtonPrevious" runat="server" Text="Föregående fråga" OnClick="ButtonPrevious_Click" />
            <asp:Button ID="ButtonNext" runat="server" Text="Nästa fråga" OnClick="ButtonNext_Click" />
            <asp:Button ID="ButtonSparaProv" runat="server" OnClick="ButtonSparaProv_Click" Text="Spara prov" />
        </div>
    </section>  
</asp:Content>

