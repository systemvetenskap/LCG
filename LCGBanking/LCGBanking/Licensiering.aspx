<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Licensiering.aspx.cs" Inherits="LCGBanking.Licensiering" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section>
        <h1>Licenseringstest</h1>
        <section id="test"> 



        <asp:Button ID="ButtonStart" runat="server" Text="Starta licenseringstest" Height="23px" OnClick="ButtonStart_Click" />
        <br /><br />

            

        <asp:Label ID="LabelQuestion" runat="server" Text="Label"></asp:Label>



        <br /><br />
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label> 
        <br />

        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
        <br />

        <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
        <br />

        
        </section> 
    </section>
  
</asp:Content>

