<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LCGBanking.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section id="main">
        <p>Behörig inloggning på JE Bankens licensieringsprov och kunskapstest krävs!</p>
        <br />
        <asp:Label ID="LabelAnv" runat="server" Text="Användarnamn"></asp:Label>
        <br />
        <asp:TextBox ID="TextBoxAnvId" runat="server" MaxLength="20" OnTextChanged="TextBoxAnvId_TextChanged"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="LabelLosen" runat="server" Text="Lösenord"></asp:Label>
        <br />
        <asp:TextBox ID="TextBoxLosen" runat="server" MaxLength="20" OnTextChanged="TextBoxAnvId_TextChanged" TextMode="Password"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="ButtonLogga" runat="server" Text="Logga in" OnClick="ButtonLogga_Click"></asp:Button>
        <br />
        <asp:Label ID="Msg" runat="server"></asp:Label>
        <br />
        <asp:Label ID="Welcome" runat="server"></asp:Label>
        </section>
        <br /><br />
</asp:Content>
