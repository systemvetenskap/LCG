<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="LCGBanking.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="bannerpic">
        <img src="BILDER/bankjelogg1.jpg" style="height: 200px; width: 100%" />
    </div>
    <section id="main">
            <p id="fraga">Alla personer som är anställda vid JE-banken och som arbetar med värdepapper måste licensieras. 
                <br />Detta är viktigt för att upprätthålla branschens goda rykte. För att erhålla en licens måste man genomföra <br />ett licensieringstest.
            </p>
            <br /><br /><br /><br /><br />
            <p>Behörig inloggning på JE Bankens liceneseringstester och kunskapstester</p>
            <br />
            <asp:Label ID="LabelAnv" runat="server" Text="Användar-ID"></asp:Label>
            <br />
            <asp:TextBox ID="TextBoxAnvnamn" runat="server" MaxLength="10"></asp:TextBox>
            <br /><br />
            <asp:Button ID="ButtonLogga" runat="server" Text="Logga in"></asp:Button>
            </section>
            <br /><br />

</asp:Content>

