<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LCGBanking.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section id="main">
        <p>Behörig inloggning på JE Bankens licensieringsprov och kunskapstest krävs!</p>
        <br /><br />
        <asp:Label ID="LabelAnv" runat="server" Text="Användarnamn"></asp:Label>
        <br />
        <asp:TextBox ID="TextBoxAnvId" runat="server" MaxLength="20" Height="30px" Width="200px" BorderColor="#003366" BorderStyle="Solid" Font-Bold="False" Font-Size="16px" Font-Strikeout="False" Wrap="False"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="LabelLosen" runat="server" Text="Lösenord"></asp:Label>
        <br />
        <asp:TextBox ID="TextBoxLosen" runat="server" MaxLength="20" TextMode="Password" Height="30px" Width="200px" BorderColor="#003366" BorderStyle="Solid" Font-Bold="False" Font-Size="16px" Wrap="False"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="ButtonLogga" runat="server" Text="Logga in" OnClick="ButtonLogga_Click" CssClass="roundCorner" Height="35px" Width="125px"></asp:Button>
        <br />
        <asp:Label ID="Msg" runat="server" CssClass="Msg"></asp:Label>
        <br />
        </section>
        <br /><br />
</asp:Content>
